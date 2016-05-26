﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.Web;
using System.Net;
using hacs;
using hacs.JSONHandlers;
using sones.storage;
using hacs.xs1;
using System.Collections.Specialized;
using hacs.xs1.configuration;
using hacs.set_state_actuator;

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
		//private string querystring;
		private string protocol;
		private Hashtable headers;
		private string request;
		private bool keepAlive = false;
		private int numRequests = 0;
		private byte[] bytes = new byte[20480];
		private FileInfo docRootFile;
		private String HTTPServer_DocumentRoot;
        private XS1Configuration XS1_Configuration;
		private JSONData JSON_Data;
        private NumericsJSONData NumericsJSON_Data;
		//private Geolocation LatitudeGeoLocation;
        private ConsoleOutputLogger ConsoleOutputLogger;
		private HTTPProxy internal_proxy;
		private MAXMonitoringThread ELVMAX;
		private TinyOnDiskStorage SensorDataStore;
		//private TinyOnDiskStorage LatitudeDataStore;
		private bool AuthorizationEnabled;
		private String Username;
		private String Password;
		private bool AuthenticatedSuccessfully;
        private String AuthDisabledForAdressesThatStartWith;
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
		public HttpProcessor(Socket s, String HTTP_DocumentRoot, TinyOnDiskStorage Storage, TinyOnDiskStorage LatitudeStorage, XS1Configuration _XS1_Configuration, ConsoleOutputLogger Logger, MAXMonitoringThread ELVMAXMonitoring, bool AuthEnabled, String Uname, String Pword, String StartAddrFilter)
		{
			this.s = s;
			HTTPServer_DocumentRoot = HTTP_DocumentRoot;
			JSON_Data = new JSONData(Storage,Logger);
            NumericsJSON_Data = new NumericsJSONData(Storage, Logger);
			docRootFile = new FileInfo(HTTPServer_DocumentRoot);
			headers = new Hashtable();
            XS1_Configuration = _XS1_Configuration;
            ConsoleOutputLogger = Logger;
			internal_proxy = new HTTPProxy(ConsoleOutputLogger,ELVMAXMonitoring);
			ELVMAX = ELVMAXMonitoring;
			SensorDataStore = Storage;
			//LatitudeDataStore = LatitudeStorage;
			//LatitudeGeoLocation = new Geolocation(LatitudeStorage,Logger);
			AuthorizationEnabled = AuthEnabled;
			Username = Uname;
			Password = Pword;
            AuthDisabledForAdressesThatStartWith = StartAddrFilter;
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

						AuthenticatedSuccessfully = true;
						#region Authentification
						if (AuthorizationEnabled)
						{
							AuthenticatedSuccessfully = false;
							if (headers.ContainsKey("Authorization"))
							{
								String encodedInputPW = (String)headers["Authorization"];
								// remove first 6 bytes..."Basic asfasfasfasfdsdf=="
								encodedInputPW = encodedInputPW.Remove(0,6);

								string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encodedInputPW));
								int position = decoded.IndexOf(':');
								if (position == -1)
								{
									AuthenticatedSuccessfully = false;
									writeError(401,"Authorization header not correct set");
								}

								string password = decoded.Substring(position + 1, decoded.Length - position - 1);
								string userName = decoded.Substring(0, position);

								if ((Username == userName) && (Password == password))
									AuthenticatedSuccessfully = true;
							}
							else
							{
								AuthenticatedSuccessfully = false;
							}
						}
						#endregion

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
                // set this to true when implementing and reaching a new method
                bool method_found = false;

				// first check if the request is actually authenticated
				IPEndPoint AC_endpoint = (IPEndPoint)s.RemoteEndPoint;
                ConsoleOutputLogger.WriteLine(AC_endpoint.Address.ToString() + " GET " + original_url);

				//if (!HTTPAuthProcessor.AllowedToAccessThisServer(AC_endpoint.Address))
				//{
				//    // now give the user a 403 and break...
				//    writeForbidden();
				//    ns.Flush();
				//    return;
				//}

				//querystring = "";
				url = original_url;

				#region Authentification
                if (AC_endpoint.Address.ToString().StartsWith(AuthDisabledForAdressesThatStartWith))
                    AuthenticatedSuccessfully = true;

				if (AuthorizationEnabled)
				{
					if (!AuthenticatedSuccessfully)
					{
						writeNotAuthorized("hacs");
					}
				}
				#endregion

				if (internal_proxy.isThisAProxyURL(url))
				{
					ProxyResponse proxy_response = internal_proxy.Proxy(url);

					if (proxy_response == null)
					{
						writeError(500, "Proxy Activation URL not found");
						return;
					}

					int left = new UTF8Encoding().GetByteCount(proxy_response.Content);
					writeSuccess(left, "text/html");
					byte[] buffer = new UTF8Encoding().GetBytes(proxy_response.Content);
					ns.Write(buffer, 0, left);
					ns.Flush();
                    return;
				}

                #region NUMERICS JSON data implementation
                if (url.ToUpper().StartsWith("/NUMERICS/"))
                {
                    #region data request
                    // remove the /data/ stuff
                    url = url.Remove(0, 10);

                    #region Sensor Data
                    if (url.ToUpper().StartsWith("SENSOR"))
                    {
                        method_found = true;
                        url = url.Remove(0, 6);

                        NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);

                        String ObjectTypeName = "";
                        String ObjectName = "";
                        String StartDate = "";
                        String EndDate = "";
                        Boolean JustLastEntry = false;
                        DateTime start = DateTime.Now;
                        DateTime end = DateTime.Now;
                        //ConsoleOutputLogger.WriteLineToScreenOnly("...");

                        foreach (String Key in nvcollection.AllKeys)
                        {
                            if (Key.ToUpper() == "NAME")
                                ObjectName = nvcollection[Key];
                            if (Key.ToUpper() == "TYPE")
                                ObjectTypeName = nvcollection[Key];
                            if (Key.ToUpper() == "START")
                                StartDate = nvcollection[Key];
                            if (Key.ToUpper() == "END")
                                EndDate = nvcollection[Key];
                            if (Key.ToUpper() == "LASTENTRY")
                                JustLastEntry = true;
                        }
                        //ConsoleOutputLogger.WriteLineToScreenOnly("...");

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
                        if (StartDate == "") // defaults
                        {
                            start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = StartDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                start = new DateTime(year, month, day);
                            }
                            else
                            {
                                start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }

                        if (EndDate == "")
                        {
                            end = DateTime.Now;
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = EndDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                end = new DateTime(year, month, day);
                            }
                            else
                            {
                                end = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }

                        //ConsoleOutputLogger.WriteLineToScreenOnly("...");
                        String Output;
                        if (!JustLastEntry)
                            Output = NumericsJSON_Data.GenerateDataNumericsJSONOutput(ObjectTypes.Sensor, ObjectTypeName, ObjectName, start, end);
                        else
                            Output = NumericsJSON_Data.GenerateDataNumericsJSONOutput_LastEntryOnly(ObjectTypes.Sensor, ObjectTypeName, ObjectName);

                        int left = new UTF8Encoding().GetByteCount(Output);
                        //writeSuccess(left, "application/json");
                        writeSuccess(left, "text/html");
                        byte[] buffer = new UTF8Encoding().GetBytes(Output);
                        ns.Write(buffer, 0, left);
                        ns.Flush();
                        return;
                    }
                    #endregion

                    #region Power Sensor Data
                    if (url.ToUpper().StartsWith("POWERSENSOR"))
                    {
                        method_found = true;
                        url = url.Remove(0, 11);

                        NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);

                        // TODO: ADD handling and calculation here
                        String ObjectName = "";
                        String StartDate = "";
                        String EndDate = "";
                        String OutputType = "";
                        DateTime start = DateTime.Now;
                        DateTime end = DateTime.Now;
                        PowerSensorOutputs Outputs = PowerSensorOutputs.HourkWh;

                        foreach (String Key in nvcollection.AllKeys)
                        {
                            if (Key.ToUpper() == "NAME")
                                ObjectName = nvcollection[Key];
                            if (Key.ToUpper() == "TYPE")
                                OutputType = nvcollection[Key];
                            if (Key.ToUpper() == "START")
                                StartDate = nvcollection[Key];
                            if (Key.ToUpper() == "END")
                                EndDate = nvcollection[Key];
                        }

                        if (ObjectName == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }
                        if (StartDate == "") // defaults
                        {
                            start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = StartDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                start = new DateTime(year, month, day);
                            }
                            else
                            {
                                start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }

                        if (EndDate == "")
                        {
                            end = DateTime.Now;
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = EndDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                end = new DateTime(year, month, day);
                            }
                            else
                            {
                                end = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }


                        if (OutputType.ToUpper() == "HOUR")
                            Outputs = PowerSensorOutputs.HourkWh;

                        if (OutputType.ToUpper() == "HOURPEAK")
                            Outputs = PowerSensorOutputs.HourPeakkWh;

                        if (OutputType.ToUpper() == "CALCKWH")
                            Outputs = PowerSensorOutputs.CalculatedkWhCounterTotal;

                        if (OutputType.ToUpper() == "CALCWEEKLYKWH")
                            Outputs = PowerSensorOutputs.CalculateWeeklykWh;

                        if (OutputType.ToUpper() == "CALCDAILYKWH")
                            Outputs = PowerSensorOutputs.CalculatedDailykWh;

                        if (OutputType.ToUpper() == "CALCHOURLYKWH")
                            Outputs = PowerSensorOutputs.CalculatedHourlykWh;

                        String Output = NumericsJSON_Data.GeneratePowerSensorNumericsJSONOutput(Outputs, ObjectName, start, end);

                        int left = new UTF8Encoding().GetByteCount(Output);
                        writeSuccess(left, "text/html");
                        byte[] buffer = new UTF8Encoding().GetBytes(Output);
                        ns.Write(buffer, 0, left);
                        ns.Flush();
                        return;
                    }
                    #endregion

                    #region Actor Data
                    if (url.ToUpper().StartsWith("ACTOR"))
                    {
                        method_found = true;
                        url = url.Remove(0, 5);

                        NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);
                        String ObjectName = "";
                        String OutputType = "";
                        ActorsStatusOutputTypes ActorOutputType = ActorsStatusOutputTypes.Binary;

                        foreach (String Key in nvcollection.AllKeys)
                        {
                            if (Key.ToUpper() == "NAME")
                                ObjectName = nvcollection[Key];
                            if (Key.ToUpper() == "OUTPUTTYPE")
                                OutputType = nvcollection[Key];
                        }

                        if (ObjectName == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }

                        if (OutputType != "")
                        {
                            if (OutputType.ToUpper() == "BINARY")
                                ActorOutputType = ActorsStatusOutputTypes.Binary;

                            if (OutputType.ToUpper() == "TRUEFALSE")
                                ActorOutputType = ActorsStatusOutputTypes.TrueFalse;

                            if (OutputType.ToUpper() == "ONOFF")
                                ActorOutputType = ActorsStatusOutputTypes.OnOff;
                        }

                        // now we should have a name we need to look up
                        //bool foundactor = false;

                        // get the XS1 Actuator List to find the ID and the Preset ID
                        XS1ActuatorList actuatorlist = XS1_Configuration.getXS1ActuatorList(hacs.Properties.Settings.Default.XS1, hacs.Properties.Settings.Default.Username, hacs.Properties.Settings.Default.Password);

                        foreach (XS1Actuator _actuator in actuatorlist.actuator)
                        {
                            if (_actuator.name.ToUpper() == ObjectName.ToUpper())
                            {
                                // we found one!
                                //foundactor = true;

                                // TODO: we need to output a JSON dataset here
                                //                                bool Status = false;
                                //
                                //                                if (_actuator.value == 0.0)
                                //                                    Status = false;
                                //                                else
                                //                                    Status = true;

                                String Output = NumericsJSON_Data.GenerateNumericsJSONDataActorStatus(ActorOutputType, _actuator.name);

                                int left = new UTF8Encoding().GetByteCount(Output);
                                writeSuccess(left, "text/html");
                                byte[] buffer = new UTF8Encoding().GetBytes(Output);
                                ns.Write(buffer, 0, left);
                                ns.Flush();
                                return;
                            }
                        }

                    }
                    #endregion

                    if (!method_found)
                    {
                        // nothing to do...
                        writeError(404, "No Method found");
                    }
                    #endregion
                }
                #endregion

                #region JSON data implementation
                if (url.ToUpper().StartsWith("/DATA/"))
				{
					#region data request
					// remove the /data/ stuff
					url = url.Remove(0, 6);

					#region Swimlane - All Sensors
					// this is a swimlane diagram containing for now containing all switches in the given household
					// TODO: do something more useful here
					if (url.ToUpper().StartsWith("SWIMLANE"))
					{
						String ObjectTypeName = "";
						String ObjectName = "";
						String StartDate = "";
						String EndDate = "";
						DateTime start = DateTime.Now;
						DateTime end = DateTime.Now;

						method_found = true;
						#region Querystring Handling
						url = url.Remove(0,8);
						NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);

						foreach (String Key in nvcollection.AllKeys)
						{
							if (Key.ToUpper() == "NAME")
								ObjectName = nvcollection[Key];
							if (Key.ToUpper() == "TYPE")
								ObjectTypeName = nvcollection[Key];
							if (Key.ToUpper() == "START")
								StartDate = nvcollection[Key];
							if (Key.ToUpper() == "END")
								EndDate = nvcollection[Key];
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
						if (StartDate == "") // defaults
						{
							start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
						}
						else
						{
							// parse the date and set it...
							// since we are only interested in the day, month and year it's necessary to only parse that
							// we expect the following format: day-month-year
							// for example: 12-01-2012 will be 12th of January 2012
							String[] Splitted = StartDate.Split(new char[1] { '-' });
							
							if (Splitted.Length == 3)
							{
								Int32 year = Convert.ToInt32(Splitted[2]);
								Int32 month = Convert.ToInt32(Splitted[1]);
								Int32 day = Convert.ToInt32(Splitted[0]);
								
								start = new DateTime(year, month, day);
							}
							else
							{
								start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
							}
						}
						
						if (EndDate == "")
						{
							end = DateTime.Now;
						}
						else
						{
							// parse the date and set it...
							// since we are only interested in the day, month and year it's necessary to only parse that
							// we expect the following format: day-month-year
							// for example: 12-01-2012 will be 12th of January 2012
							String[] Splitted = EndDate.Split(new char[1] { '-' });
							
							if (Splitted.Length == 3)
							{
								Int32 year = Convert.ToInt32(Splitted[2]);
								Int32 month = Convert.ToInt32(Splitted[1]);
								Int32 day = Convert.ToInt32(Splitted[0]);
								
								end = new DateTime(year, month, day);
							}
							else
							{
								end = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
							}
						}

						#endregion

						String Output = GenerateSwimlane.Generate(ELVMAX,SensorDataStore,ObjectName,ObjectTypeName,start,end);

						int left = new UTF8Encoding().GetByteCount(Output);
						//writeSuccess(left, "application/json");
						writeSuccess(left, "text/html");
						byte[] buffer = new UTF8Encoding().GetBytes(Output);
						ns.Write(buffer, 0, left);
						ns.Flush();
						return;

					}
					#endregion

					#region Sensor Data
					if (url.ToUpper().StartsWith("SENSOR"))
					{
						method_found = true;
						url = url.Remove(0,6);
						
						NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);
							
						String ObjectTypeName = "";
						String ObjectName = "";
                        String StartDate = "";
                        String EndDate = "";
                        Boolean JustLastEntry = false;
                        DateTime start = DateTime.Now;
                        DateTime end = DateTime.Now;
                        //ConsoleOutputLogger.WriteLineToScreenOnly("...");

						foreach (String Key in nvcollection.AllKeys)
						{
							if (Key.ToUpper() == "NAME")
								ObjectName = nvcollection[Key];
							if (Key.ToUpper() == "TYPE")
								ObjectTypeName = nvcollection[Key];
                            if (Key.ToUpper() == "START")
                                StartDate = nvcollection[Key];
                            if (Key.ToUpper() == "END")
                                EndDate = nvcollection[Key];
                            if (Key.ToUpper() == "LASTENTRY")
                                JustLastEntry = true;
						}
                        //ConsoleOutputLogger.WriteLineToScreenOnly("...");

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
                        if (StartDate == "") // defaults
                        {
                            start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = StartDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                start = new DateTime(year, month, day);
                            }
                            else
                            {
                                start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }

                        if (EndDate == "")
                        {
                            end = DateTime.Now;
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = EndDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                end = new DateTime(year, month, day);
                            }
                            else
                            {
                                end = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }

                        //ConsoleOutputLogger.WriteLineToScreenOnly("...");
                        String Output;
                        if (!JustLastEntry)
						    Output = JSON_Data.GenerateDataJSONOutput(ObjectTypes.Sensor, ObjectTypeName, ObjectName,start,end);
                        else
                            Output = JSON_Data.GenerateDataJSONOutput_LastEntryOnly(ObjectTypes.Sensor, ObjectTypeName, ObjectName);

						int left = new UTF8Encoding().GetByteCount(Output);
						//writeSuccess(left, "application/json");
						writeSuccess(left, "text/html");
						byte[] buffer = new UTF8Encoding().GetBytes(Output);
						ns.Write(buffer, 0, left);
						ns.Flush();
						return;
					}
					#endregion

					/*					#region Google Latitude
					if (hacs.Properties.Settings.Default.GoogleLatitudeEnabled)
					{
						if (url.ToUpper().StartsWith("GEOLOCATION"))
						{
							method_found = true;
							url = url.Remove(0, 11);
							
							NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);
							
							// TODO: ADD handling and calculation here
							String ObjectName = "";
							String StartDate = "";
							String EndDate = "";
							//String OutputType = "";
							DateTime start = DateTime.Now;
							DateTime end = DateTime.Now;
							Boolean JustLastEntry = false;

							#region Querystring handling
							foreach (String Key in nvcollection.AllKeys)
							{
								if (Key.ToUpper() == "NAME")
									ObjectName = nvcollection[Key];
								if (Key.ToUpper() == "START")
									StartDate = nvcollection[Key];
								if (Key.ToUpper() == "END")
									EndDate = nvcollection[Key];
								if (Key.ToUpper() == "LASTENTRY")
									JustLastEntry = true;
							}
							
							if (ObjectName == "")
							{
								writeError(404, "No Method found");
								return;
							}
							if (StartDate == "") // defaults
							{
								start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
							}
							else
							{
								// parse the date and set it...
								// since we are only interested in the day, month and year it's necessary to only parse that
								// we expect the following format: day-month-year
								// for example: 12-01-2012 will be 12th of January 2012
								String[] Splitted = StartDate.Split(new char[1] { '-' });
								
								if (Splitted.Length == 3)
								{
									Int32 year = Convert.ToInt32(Splitted[2]);
									Int32 month = Convert.ToInt32(Splitted[1]);
									Int32 day = Convert.ToInt32(Splitted[0]);
									
									start = new DateTime(year, month, day);
								}
								else
								{
									start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
								}
							}
							
							if (EndDate == "")
							{
								end = DateTime.Now;
							}
							else
							{
								// parse the date and set it...
								// since we are only interested in the day, month and year it's necessary to only parse that
								// we expect the following format: day-month-year
								// for example: 12-01-2012 will be 12th of January 2012
								String[] Splitted = EndDate.Split(new char[1] { '-' });
								
								if (Splitted.Length == 3)
								{
									Int32 year = Convert.ToInt32(Splitted[2]);
									Int32 month = Convert.ToInt32(Splitted[1]);
									Int32 day = Convert.ToInt32(Splitted[0]);
									
									end = new DateTime(year, month, day);
								}
								else
								{
									end = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
								}
							}
							#endregion

							String Output = "";

							if (JustLastEntry)
								Output = LatitudeGeoLocation.GenerateJSON_LastEntry(ObjectName);
							
							int left = new UTF8Encoding().GetByteCount(Output);
							writeSuccess(left, "text/html");
							byte[] buffer = new UTF8Encoding().GetBytes(Output);
							ns.Write(buffer, 0, left);
							ns.Flush();

							return;
						}
					}
					#endregion
*/
                    #region Power Sensor Data
                    if (url.ToUpper().StartsWith("POWERSENSOR"))
                    {
                        method_found = true;
                        url = url.Remove(0, 11);

                        NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);

                        // TODO: ADD handling and calculation here
                        String ObjectName = "";
                        String StartDate = "";
                        String EndDate = "";
                        String OutputType = "";
                        DateTime start = DateTime.Now;
                        DateTime end = DateTime.Now;
                        PowerSensorOutputs Outputs = PowerSensorOutputs.HourkWh;
                        
                        foreach (String Key in nvcollection.AllKeys)
                        {
                            if (Key.ToUpper() == "NAME")
                                ObjectName = nvcollection[Key];
                            if (Key.ToUpper() == "TYPE")
                                OutputType = nvcollection[Key];
                            if (Key.ToUpper() == "START")
                                StartDate = nvcollection[Key];
                            if (Key.ToUpper() == "END")
                                EndDate = nvcollection[Key];
                        }

                        if (ObjectName == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }
                        if (StartDate == "") // defaults
                        {
                            start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = StartDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                start = new DateTime(year, month, day);
                            }
                            else
                            {
                                start = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }

                        if (EndDate == "")
                        {
                            end = DateTime.Now;
                        }
                        else
                        {
                            // parse the date and set it...
                            // since we are only interested in the day, month and year it's necessary to only parse that
                            // we expect the following format: day-month-year
                            // for example: 12-01-2012 will be 12th of January 2012
                            String[] Splitted = EndDate.Split(new char[1] { '-' });

                            if (Splitted.Length == 3)
                            {
                                Int32 year = Convert.ToInt32(Splitted[2]);
                                Int32 month = Convert.ToInt32(Splitted[1]);
                                Int32 day = Convert.ToInt32(Splitted[0]);

                                end = new DateTime(year, month, day);
                            }
                            else
                            {
                                end = DateTime.Now - (new TimeSpan(hacs.Properties.Settings.Default.DefaultSensorOutputPeriod, 0, 0, 0));
                            }
                        }


                        if (OutputType.ToUpper() == "HOUR")
                            Outputs = PowerSensorOutputs.HourkWh;

                        if (OutputType.ToUpper() == "HOURPEAK")
                            Outputs = PowerSensorOutputs.HourPeakkWh;

                        if (OutputType.ToUpper() == "CALCKWH")
                            Outputs = PowerSensorOutputs.CalculatedkWhCounterTotal;

						if (OutputType.ToUpper() == "CALCWEEKLYKWH")
							Outputs = PowerSensorOutputs.CalculateWeeklykWh;

                        if (OutputType.ToUpper() == "CALCDAILYKWH")
                            Outputs = PowerSensorOutputs.CalculatedDailykWh;

                        if (OutputType.ToUpper() == "CALCHOURLYKWH")
                            Outputs = PowerSensorOutputs.CalculatedHourlykWh;

                        String Output = JSON_Data.GeneratePowerSensorJSONOutput(Outputs,ObjectName, start, end);

                        int left = new UTF8Encoding().GetByteCount(Output);
                        writeSuccess(left, "text/html");
                        byte[] buffer = new UTF8Encoding().GetBytes(Output);
                        ns.Write(buffer, 0, left);
                        ns.Flush();
                        return;
                    }
                    #endregion

					#region Actor Data
					if (url.ToUpper().StartsWith("ACTOR"))
					{
						method_found = true;
						url = url.Remove(0,5);
						
						NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);
						String ObjectName = "";
						String OutputType = "";
                        ActorsStatusOutputTypes ActorOutputType = ActorsStatusOutputTypes.Binary;

						foreach (String Key in nvcollection.AllKeys)
						{
							if (Key.ToUpper() == "NAME")
								ObjectName = nvcollection[Key];
							if (Key.ToUpper() == "OUTPUTTYPE")
								OutputType = nvcollection[Key];
						}

						if (ObjectName == "")
						{
							writeError(404, "No Method found");
							return;
						}

                        if (OutputType != "")
                        {
                            if (OutputType.ToUpper() == "BINARY")
                                ActorOutputType = ActorsStatusOutputTypes.Binary;

                            if (OutputType.ToUpper() == "TRUEFALSE")
                                ActorOutputType = ActorsStatusOutputTypes.TrueFalse;

                            if (OutputType.ToUpper() == "ONOFF")
                                ActorOutputType = ActorsStatusOutputTypes.OnOff;
                        }
						
						// now we should have a name we need to look up
						//bool foundactor = false;
						
						// get the XS1 Actuator List to find the ID and the Preset ID
                        XS1ActuatorList actuatorlist = XS1_Configuration.getXS1ActuatorList(hacs.Properties.Settings.Default.XS1,hacs.Properties.Settings.Default.Username,hacs.Properties.Settings.Default.Password);

						foreach (XS1Actuator _actuator in actuatorlist.actuator)
                        {
                            if (_actuator.name.ToUpper() == ObjectName.ToUpper())
                            {
								// we found one!
								//foundactor = true;
								
								// TODO: we need to output a JSON dataset here
//                                bool Status = false;
//
//                                if (_actuator.value == 0.0)
//                                    Status = false;
//                                else
//                                    Status = true;

                                String Output = JSON_Data.GenerateJSONDataActorStatus(ActorOutputType, _actuator.name);

                                int left = new UTF8Encoding().GetByteCount(Output);
                                writeSuccess(left, "text/html");
                                byte[] buffer = new UTF8Encoding().GetBytes(Output);
                                ns.Write(buffer, 0, left);
                                ns.Flush();
                                return;
							}
						}
						
					}
					#endregion

					if (!method_found)
					{
						// nothing to do...
						writeError(404, "No Method found");
					}
					#endregion
                }
                #endregion

                #region ACTOR switching requests
                if (url.ToUpper().StartsWith("/ACTOR/"))
				{
					#region actor switching request
                    // /actor/preset?name=[actor_name]&preset=[preset_function_name]
                    // /actor/direct?name=[actor_name]&value=[new_actor_value]

                    // remove the /actor/ stuff
                    url = url.Remove(0, 7);

                    #region Preset Mode
                    if (url.ToUpper().StartsWith("PRESET"))
                    {
                        method_found = true;
                        url = url.Remove(0, 6);

                        NameValueCollection nvcollection = HttpUtility.ParseQueryString(url);

                        String actorname = "";
                        String preset = "";

                        foreach (String Key in nvcollection.AllKeys)
                        {
                            if (Key.ToUpper() == "NAME")
                                actorname = nvcollection[Key];
                            if (Key.ToUpper() == "PRESET")
                                preset = nvcollection[Key];
                        }

                        Int32 foundActorID = 0;
                        Int32 foundPresetID = 0;

                        #region error handling
                        if (actorname == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }
                        if (preset == "")
                        {
                            writeError(404, "No Method found");
                            return;
                        }
                        #endregion

                        // get the XS1 Actuator List to find the ID and the Preset ID
                        XS1ActuatorList actuatorlist = XS1_Configuration.getXS1ActuatorList(hacs.Properties.Settings.Default.XS1,hacs.Properties.Settings.Default.Username,hacs.Properties.Settings.Default.Password);

                        bool foundatleastoneactuator = false;
                        //
                        foreach (XS1Actuator _actuator in actuatorlist.actuator)
                        {
                            if (_actuator.name.ToUpper() == actorname.ToUpper())
                            {
                                foundActorID = _actuator.id;

                                bool foundpreset = false;

                                foreach (actuator_function actorfunction in _actuator.function)
                                {
                                    foundPresetID++;

                                    if (actorfunction.type.ToUpper() == preset.ToUpper())
                                    {
                                        foundpreset = true;
                                        break;
                                    }
                                }

                                #region error handling
                                if (foundpreset)
                                {
                                    if (foundActorID != 0)
                                    {
                                        // so we obviously got the actor and the preset id... now lets do the call
                                        set_state_actuator ssa = new set_state_actuator();
                                        ssa.SetStateActuatorPreset(hacs.Properties.Settings.Default.XS1, hacs.Properties.Settings.Default.Username, hacs.Properties.Settings.Default.Password, foundActorID, foundPresetID);
                                        foundatleastoneactuator = true;
                                        break;
                                    }
                                }
                                #endregion
                            }
                        }
                        if (!foundatleastoneactuator)
                        {
                            writeError(404, "actor or function not found");
                            return;
                        }
                    }
                    else
                        if (url.ToUpper().StartsWith("DIRECT"))
                        {

                        }
                    #endregion

                    if (!method_found)
                    {
                        // nothing to do...
                        writeError(404, "No Method found");
                        return;
                    }
					#endregion
                }
                #endregion
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
						//querystring = url.Substring(url.IndexOf('?') + 1);
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
							//bool resumed = false;

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
										//bool isThisARecordingRecording = false;

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
								ConsoleOutputLogger.WriteLineToScreenOnly("[FEHLER@HTTP] " + e.Message);
								try
								{
									writeFailure();
								}
								catch (Exception)
								{
									ConsoleOutputLogger.WriteLineToScreenOnly("[FEHLER@HTTP] connection lost to client");
								}
								if (bs != null) bs.Close();
								if (bs != null) fs.Close();
							}

						}
					}
					catch (Exception e)
					{
						ConsoleOutputLogger.WriteLineToScreenOnly("[FEHLER@HTTP] " + e.Message);
						writeFailure();
					}
					#endregion
				}
			}
			catch (Exception e)
			{
				ConsoleOutputLogger.WriteLineToScreenOnly("[FEHLER@HTTP] " + e.Message+" ## "+e.StackTrace);
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

		public void writeNotAuthorized(String realm)
		{
			string auth_header = "WWW-Authenticate: Basic realm=\""+realm+"\"\n";
			string output = "HTTP/1.0 401 Not Authorized";

			try
			{
				sw.Write("HTTP/1.0 401 Not Authorized\r\n");
				sw.Write("Content-Type: text/html\r\n");
				sw.Write("Content-Length: " + output.Length + "\r\n");
				sw.Write("WWW-Authenticate: Basic realm=\""+realm+"\"\r\n");
				sw.Write("Connection: close\r\n");
				sw.Write("\r\n");
				sw.Write(output);
				sw.Flush();
			}
			catch (Exception e)
			{
				ConsoleOutputLogger.WriteLine("[FEHLER@HTTP] " + e.Message);
			}
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
