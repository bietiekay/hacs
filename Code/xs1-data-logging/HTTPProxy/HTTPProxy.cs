using System;

namespace xs1_data_logging
{
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

		public bool isThisAProxyURL(String URL)
		{
			return false;
		}
	}
}

