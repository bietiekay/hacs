using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xs1_data_logging
{
    public class ProxyElement
    {
		public String ActivationURL;
		public String OutgoingMappingURL;
	}

    public class HTTPProxyConfiguration
    {
        public static List<ProxyElement> ProxyElements = new List<ProxyElement>();

        public static void ReadConfiguration(String Configfilename)
        {
            if (File.Exists(Configfilename))
            {
                // get all lines from the 
                String[] ProxyConfigFileContent = File.ReadAllLines(Configfilename);
                Int32 LineNumber = 0;

                foreach (String LineElement in ProxyConfigFileContent)
                {
                    String[] TokenizedLine = LineElement.Split(new char[1] { ' ' });
                    LineNumber++;

                    if (!LineElement.StartsWith("#"))
                    {

                        ProxyElement NewElement = new ProxyElement();

                        if (TokenizedLine.Length == 2)
                        {
                            NewElement.ActivationURL = TokenizedLine[0];
							NewElement.OutgoingMappingURL = TokenizedLine[1];
                            
                            ProxyElements.Add(NewElement);
                        }
                        else
                            throw (new Exception("HTTPProxy Configuration File - Error in line " + LineNumber));
                    }
                }
            }
            else
            {
                throw (new Exception("HTTPProxy Configuration File not found!"));
            }
        }

    }
}
