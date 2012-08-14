using System;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace xs1_data_logging
{
	public class ProxyResponse
	{
		public HttpWebResponse response;
		public String Content;
	}

	/// <summary>
	/// this class is a very small and simple transparent HTTP proxy. It's taking a request, mapping it to an HTTP adress
	/// then retrieving from that URL and outputting it back.
	/// </summary>
	public class HTTPProxy
	{
        private ConsoleOutputLogger ConsoleOutputLogger;

		public HTTPProxy (ConsoleOutputLogger Logger)
		{
            ConsoleOutputLogger = Logger;
		}

		/// <summary>
		/// this checks if the given URL is contains an activation URL.
		/// </summary>
		public bool isThisAProxyURL(String URL)
		{
			// we get an url in here and we do a look up in our configuration table if we need to react
			foreach(ProxyElement element in HTTPProxyConfiguration.ProxyElements)
			{
				if (URL.StartsWith(element.ActivationURL))
				{
					// obviously we found a hit
					return true;
				}
			}
			return false;
		}

		public ProxyResponse Proxy(String URL)
		{
			ProxyResponse proxy_Response = new ProxyResponse();

			ProxyElement p_element = null;

			// we need to find out what we need to replace
			foreach(ProxyElement element in HTTPProxyConfiguration.ProxyElements)
			{
				if (URL.StartsWith(element.ActivationURL))
				{
					p_element = element;
					// obviously we found a hit
					break;
				}
			}

            ConsoleOutputLogger.WriteLine("Proxy: " + URL+" - "+URL.Replace(p_element.ActivationURL,p_element.OutgoingMappingURL));

			if (p_element != null)
			{
				WebRequest wrGetURL = WebRequest.Create(URL.Replace(p_element.ActivationURL,p_element.OutgoingMappingURL));

	            HttpWebResponse response = (HttpWebResponse)wrGetURL.GetResponse();
	            if (response.StatusCode != HttpStatusCode.OK)
	            {
					proxy_Response.response = response;
					proxy_Response.Content = "Something wicked happened.";

					return proxy_Response;
	            }

				proxy_Response.response = response;
	            // we will read data via the response stream
	            proxy_Response.Content = new StreamReader(response.GetResponseStream()).ReadToEnd();

				return proxy_Response;
			}
			return null;
		}
	}
}

