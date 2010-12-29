using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading;
using System.Web;
using System.Net;
using xs1_data_logging;
using xs1_data_logging.JSONHandlers;
using sones.storage;
using hacs.xs1;
using System.Collections.Specialized;

namespace HTTP
{
	#region HttpProcessor
	/// <summary>
	/// Implements a Handler for each HTTP Client Request
	/// </summary>
	public class HttpProcessor
	{
		#region Variables'r'us
		private static int threads = 0;
		private Socket s;
		public NetworkStream ns;
		private StreamReader sr;
		private StreamWriter sw;
		private string method;
		private string url;
		private string original_url;
		private string querystring;
		private string protocol;
		private Hashtable headers;
		private string request;
		private bool keepAlive = false;
		private int numRequests = 0;
		private byte[] bytes = new byte[20480];
		private FileInfo docRootFile;
		private String HTTPServer_DocumentRoot;
		private JSONData JSON_Data;
		#endregion

		#region Constructor
		/// <summary>
		/// Each HTTP processor object handles one client.  If Keep-Alive is enabled then this
		/// object will be reused for subsequent requests until the client breaks keep-alive.
		/// This usually happens when it times out.  Because this could easily lead to a DoS
		/// attack, we keep track of the number of open processors and only allow 100 to be
		/// persistent active at any one time.  Additionally, we do not allow more than 500
		/// outstanding requests.
		/// </summary>
		/// <param name="docRoot">Root-Directory of the HTTP Server</param>
		/// <param name="s">the Socket to work with</param>
		/// <param name="webserver">the "master" HttpServer Object of this Client</param>
		public HttpProcessor(Socket s, String HTTP_DocumentRoot, TinyOnDiskStorage Storage)
		{
			this.s = s;
			HTTPServer_DocumentRoot = HTTP_DocumentRoot;
			JSON_Data = new JSONData(Storage);
			docRootFile = new FileInfo(HTTPServer_DocumentRoot);
			headers = new Hashtable();
		}
		#endregion

		#region processor starting point
		/// <summary>
		/// This is the main method of each thread of HTTP processing.  We pass this method
		/// to the thread constructor when starting a new connection.
		/// </summary>
		public void process()
		{
			try
			{
				// Increment the number of current connections
				Interlocked.Increment(ref threads);
				// Bundle up our sockets nice and tight in various streams
				ns = new NetworkStream(s, FileAccess.ReadWrite);
				// It looks like these streams buffer
				sr = new StreamReader(ns);
				sw = new StreamWriter(ns);
				// Parse the request, if that succeeds, read the headers, if that
				// succeeds, then write the given URL to the stream, if possible.
				while (parseRequest())
				{
					if (readHeaders())
					{
						// This makes sure we don't have too many persistent connections and also
						// checks to see if the client can maintain keep-alive, if so then we will
						// keep this http processor around to process again.
						if (threads <= 100 && "Keep-Alive".Equals(headers["Connection"]))
						{
							keepAlive = true;
						}
						// Copy the file to the socket
						writeURL();
						// If keep alive is not active then we want to close down the streams
						// and shutdown the socket
						if (!keepAlive)
						{
							ns.Close();
							try
							{
								s.Shutdown(SocketShutdown.Both);
							}
							catch (Exception) { }
							break;
						}
					}
				}
			}
			finally
			{
				// Always decrement the number of connections
				Interlocked.Decrement(ref threads);
			}
		}
		#endregion

		#region Validationchecks
		/// <summary>
		/// parses the Request and determines if it's actually a valid one or not
		/// </summary>
		/// <returns>is valid request(true) or not(false)</returns>
		public bool parseRequest()
		{
			// The number of requests handled by this persistent connection
			numRequests++;
			// Here is where we ensure that we are not overloaded
			if (threads > 500)
			{
				writeError(502, "Server temporarily overloaded");
				return false;
			}
			// FIXME: This could conceivably used to DoS us if we never finish reading the
			// line and they never hang up.  We could set the socket options to limit
			// the amount of time before reading a request.
			try
			{
				request = null;
				request = sr.ReadLine();
			}
			catch (IOException)
			{
			}
			// If the request line is null, then the other end has hung up on us.  A well
			// behaved client will do this after 15-60 seconds of inactivity.
			if (request == null)
			{
				return false;
			}
			// HTTP request lines are of the form:
			// [METHOD] [Encoded URL] HTTP/1.?
			string[] tokens = request.Split(new char[] { ' ' });
			if (tokens.Length != 3)
			{
				writeError(400, "Bad request");
				return false;
			}
			// We currently only handle GET requests
			method = tokens[0];
			if (!method.Equals("GET"))
			{
				writeError(501, method + " not implemented");
				return false;
			}
			url = tokens[1];
			// Only accept valid urls
			if (!url.StartsWith("/"))
			{
				writeError(400, "Bad URL");
				return false;
			}
			// Decode all encoded parts of the URL using the built in URI processing class			
			// this is buggy
			/*int i = 0;
			while((i = url.IndexOf("%", i)) != -1)
			{
				url = url.Substring(0, i) + Uri.HexUnescape(url, ref i) + url.Substring(i);
			}*/

			// this works
			original_url = url;
			url = HttpUtility.UrlDecode(url, Encoding.UTF8);

			// Lets just make sure we are using HTTP, thats about all I care about
			protocol = tokens[2];
			if (!protocol.StartsWith("HTTP/"))
			{
				writeError(400, "Bad protocol: " + protocol);
			}
			return true;
		}

		/// <summary>
		/// Reads the headers of the request and determines if they are valid or not
		/// </summary>
		/// <returns>is valid request(true) or not(false)</returns>
		public bool readHeaders()
		{
			string line;
			string name = null;
			// The headers end with either a socket close (!) or an empty line
			while ((line = sr.ReadLine()) != null && line != "")
			{
				// If the value begins with a space or a hard tab then this
				// is an extension of the value of the previous header and
				// should be appended
				if (name != null && Char.IsWhiteSpace(line[0]))
				{
					headers[name] += line;
					continue;
				}
				// Headers consist of [NAME]: [VALUE] + possible extension lines
				int firstColon = line.IndexOf(":");
				if (firstColon != -1)
				{
					name = line.Substring(0, firstColon);
					String value = line.Substring(firstColon + 1).Trim();
					//if (internalSettings.WebServer.verboseLogging) ConsoleOutputLogger.WriteLine(name + ": " + value);
					headers[name] = value;
				}
				else
				{
					writeError(400, "Bad header: " + line);
					return false;
				}
			}
			return line != null;
		}
		#endregion

		#region HTML OK Message Generator
		String ForwardToIndexHTML(String Message)
		{
			String Output = "";

			Output = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head><title>" + Message + "</title><meta http-equiv=\"refresh\" content=\"5; URL=/index.html\"></head><body><a href=\"/index.html\">" + Message + "</a></body></html>";

			return Output;
		}
		String ForwardToLastPage(String Message)
		{
			String Output = "";

			Output = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head><title>" + Message + "</title><meta http-equiv=\"refresh\" content=\"5; URL=\"javascript:history.back(2)\"></head><body><a href=\"/index.html\">" + Message + "</a></body></html>";

			return Output;
		}
		String ForwardToPage(String Message, String URL)
		{
			String Output = "";

			Output = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head><title>" + Message + "</title><meta http-equiv=\"refresh\" content=\"5; URL=" + URL + "\"></head><body><a href=\"" + URL + "\">" + Message + "</a></body></html>";

			return Output;
		}
		#endregion

		#region GetFileExtension
		/// <summary>
		/// extracts the file extension from a filename
		/// </summary>
		/// <param name="filename">the complete filename or path+filename</param>
		/// <returns>extension as string</returns>
		public string getFileExtension(string filename)
		{
			int position = filename.LastIndexOf('.');

			if (position != -1)
			{
				string output = filename.Remove(0, position);
				return output;
			}
			else
				return "";
		}
		#endregion

		#region Request URL handling
		/// <summary>
		/// We need to make sure that the url that we are trying to treat as a file
		/// lies below the document root of the http server so that people can't grab
		/// random files off your computer while this is running.
		/// </summary>
		public void writeURL()
		{
			try
			{
				// first check if the request is actually authenticated

				IPEndPoint AC_endpoint = (IPEndPoint)s.RemoteEndPoint;

				//if (!HTTPAuthProcessor.AllowedToAccessThisServer(AC_endpoint.Address))
				//{
				//    // now give the user a 403 and break...
				//    writeForbidden();
				//    ns.Flush();
				//    return;
				//}

				querystring = "";
				url = original_url;

				if (url.StartsWith("/data/"))
				{
					#region data request
					// remove the /data/ stuff
					url = url.Remove(0, 6);

					// set this to true when implementing and reaching a new method
					bool method_found = false;

					#region Sensor Data
					if (url.StartsWith("sensor"))
					{
						method_found = true;
						url = url.Remove(0,6);
						
    					NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);
                            
                        String ObjectTypeName = "";
                        String ObjectName = "";

                        foreach (String Key in nvcollection.AllKeys)
                        {
                            if (Key.ToUpper() == "NAME")
                                ObjectName = nvcollection[Key];
                            if (Key.ToUpper() == "TYPE")
                                ObjectTypeName = nvcollection[Key];
                        }

                        if (ObjectTypeName == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }
                        if (ObjectName == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }

                        String Output = JSON_Data.GenerateDataJSONOutput(ObjectTypes.Sensor, ObjectTypeName, ObjectName);

                        int left = new UTF8Encoding().GetByteCount(Output);
                        //writeSuccess(left, "application/json");
                        writeSuccess(left, "text/html");
                        byte[] buffer = new UTF8Encoding().GetBytes(Output);
                        ns.Write(buffer, 0, left);
                        ns.Flush();
                        return;
					}
					#endregion

					#region Actor Data
					if (url.StartsWith("actor"))
					{
						method_found = true;
						url = url.Remove(0,5);
					}
					#endregion

					if (!method_found)
					{
						// nothing to do...
						writeError(404, "No Method found");
					}
					#endregion
				}
				else
				{
					#region File request (everything else...)

					#region default page
					if (url == "/")
					{
						url = "/index.html";
					}
					#endregion

					// check if we have some querystring parameters
					if (url.Contains("?"))
					{
						// yes, remove everything after the ? from the url but save it to querystring
						querystring = url.Substring(url.IndexOf('?') + 1);
						url = url.Remove(url.IndexOf('?'));
					}

					// Replace the forward slashes with back-slashes to make a file name
					string filename = url.Replace('/', Path.DirectorySeparatorChar); //you have different path separators in unix and windows
					try
					{
						// Construct a filename from the doc root and the filename
						FileInfo file = new FileInfo(docRootFile + filename);
						// Make sure they aren't trying in funny business by checking that the
						// resulting canonical name of the file has the doc root as a subset.
						filename = file.FullName;
						if (!filename.StartsWith(docRootFile.FullName))
						{
							writeForbidden();
						}
						else
						{
							FileStream fs = null;
							BufferedStream bs = null;
							long bytesSent = 0;
							bool resumed = false;

							try
							{
								if (filename.EndsWith(".log"))
								{
									// now give the user a 403 and break...
									writeForbidden();
									ns.Flush();
								}
								else
									if (filename.EndsWith(".html") | (filename.EndsWith(".htm")))
									{
										// 
										String Output = File.ReadAllText(filename);

										int left = new UTF8Encoding().GetByteCount(Output);
										writeSuccess(left, "text/html");
										byte[] buffer = new UTF8Encoding().GetBytes(Output);
										ns.Write(buffer, 0, left);
										ns.Flush();
									}
									else
									{
										// Open the file for binary transfer
										fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

										long left = file.Length;
										bool isThisARecordingRecording = false;

										#region different mime-type-handling
										switch (getFileExtension(filename))
										{
											case ".css":
												writeSuccess(left, "text/css");
												break;
											case ".gif":
												writeSuccess(left, "image/gif");
												break;
											case ".png":
												writeSuccess(left, "image/png");
												break;
											case ".jpg":
												writeSuccess(left, "image/jpeg");
												break;
											case ".jpeg":
												writeSuccess(left, "image/jpeg");
												break;
											case ".ico":
												writeSuccess(left, "image/ico");
												break;
											default:
												// Write the content length and the success header to the stream; it's binary...so treat it as binary
												writeSuccess(left, "application/octet-stream");
												break;
										}
										#endregion

										// Copy the contents of the file to the stream, ensure that we never write
										// more than the content length we specified.  Just in case the file somehow
										// changes out from under us, although I don't know if that is possible.
										bs = new BufferedStream(fs);
										left = file.Length;

										// for performance reasons...
										int read;
										while (left > 0 && (read = bs.Read(bytes, 0, (int)Math.Min(left, bytes.Length))) != 0)
										{
											ns.Write(bytes, 0, read);
											bytesSent = bytesSent + read;
											left -= read;
										}
										ns.Flush();
										bs.Close();
										fs.Close();
									}
							}
							catch (Exception e)
							{
								ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] " + e.Message);
								try
								{
									writeFailure();
								}
								catch (Exception)
								{
									ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] connection lost to client");
								}
								if (bs != null) bs.Close();
								if (bs != null) fs.Close();
							}

						}
					}
					catch (Exception e)
					{
						ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] " + e.Message);
						writeFailure();
					}
					#endregion
				}
			}
			catch (Exception e)
			{
				ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] " + e.Message);
				writeFailure();
			}
		}
		#endregion

		#region Simple HTTP Responses+Codes
		/**
		 * These write out the various HTTP responses that are possible with this
		 * very simple web server.
		 * */
		public void writeSuccess(long length, string mimetype)
		{
			writeResult(200, "OK", length, mimetype);
		}

		public void writeSuccess(long length)
		{
			writeResult(200, "OK", length);
		}

		public void writeFailure()
		{
			writeError(404, "File not found");
		}

		public void writeForbidden()
		{
			writeError(403, "Forbidden");
		}

		public void writeError(int status, string message)
		{
			try
			{
				string output = "<h1>HTTP/1.0 " + status + " " + message + "</h1>";
				writeResult(status, message, (long)output.Length);
				ConsoleOutputLogger.WriteLine("[HTTP] Error " + status);
				sw.Write(output);
				sw.Flush();
			}
			catch (Exception e)
			{
				ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] " + e.Message);
			}
		}

		public void writeResult(int status, string message, long length)
		{
			writeResult(status, message, length, "text/html");
		}


		public void writeResult(int status, string message, long length, string mimetype)
		{
			try
			{
				//ConsoleOutputLogger.WriteLine(request + " " + status + " " + numRequests);
				sw.Write("HTTP/1.0 " + status + " " + message + "\r\n");
				sw.Write("Content-Type: " + mimetype + "\r\n");
				sw.Write("Content-Length: " + length + "\r\n");
				if (keepAlive)
				{
					sw.Write("Connection: Keep-Alive\r\n");
				}
				else
				{
					sw.Write("Connection: close\r\n");
				}
				sw.Write("\r\n");
				sw.Flush();
			}
			catch (Exception e)
			{
				ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] " + e.Message);
			}
		}
		#endregion
	}
	#endregion
}
