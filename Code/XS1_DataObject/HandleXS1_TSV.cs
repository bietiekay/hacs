using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hacs.xs1;

namespace hacs.xs1
{
    /*
     * Sample Data:
     *      0       1       2   3   4   5   6   7   8       9   10  11  12  13
     * UTC Time     YYYY    MM  DD  DDD HH  MM  SS  TC      T   N   Name    VT  Value
     * 1292690185	2010	12	18	Sat	17	36	25	+0100	A	2	Sofalampe	switch	100.0
     * 1292690251	2010	12	18	Sat	17	37	31	+0100	S	2	Keller_	hygrometer	47.1
     * 1292690251	2010	12	18	Sat	17	37	31	+0100	S	1	Keller	temperature	14.8
     * */

    public static class HandleXS1_TSV
    {
        /// <summary>
        /// this method converts a logged DataLine from the EzControl XS1 into a useable DataObject
        /// </summary>
        /// <param name="DataLine">a tab separated line from the XS1</param>
        public static XS1_DataObject HandleValue(String DataLine)
        {
            try
            {
            String[] SplittedLine = DataLine.Split(new char[1] { '\t' });

            if (SplittedLine.Length != 14)
                return null;

            XS1_DataObject newObject = new XS1_DataObject();

            newObject.Name = SplittedLine[11];
            newObject.Timecode = new DateTime(Convert.ToInt32(SplittedLine[1]), Convert.ToInt32(SplittedLine[2]), Convert.ToInt32(SplittedLine[3]), Convert.ToInt32(SplittedLine[5]), Convert.ToInt32(SplittedLine[6]), Convert.ToInt32(SplittedLine[7]));
            
            switch (SplittedLine[9])
            {
                case "A":
                    newObject.Type = ObjectTypes.Actor;
                    break;
                case "S":
                    newObject.Type = ObjectTypes.Sensor;
                    break;
                default:
                    newObject.Type = ObjectTypes.Unknown;
                    break;
            }

            newObject.TypeName = SplittedLine[12];
            newObject.Value = Convert.ToDouble(SplittedLine[13],System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            newObject.XS1ObjectID = Convert.ToInt32(SplittedLine[10]);

            return newObject;
            }
            catch(Exception)
            {
            
            }

            return null;
        }
    }
}
