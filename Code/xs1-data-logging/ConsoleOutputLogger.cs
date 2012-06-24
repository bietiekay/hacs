using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace xs1_data_logging
{
    /// <summary>
    /// This Class stores a number of Console Output Lines into a ring buffer
    /// </summary>
    public static class ConsoleOutputLogger
    {
        private static int Max_Number_Of_Entries = 500;
        private static LinkedList<String> LoggerList = new LinkedList<String>();
        public static bool verbose = false;
        public static bool writeLogfile = false;

        public static StreamWriter Logfile = null;

        public static void SetNumberOfMaxEntries(int Number)
        {
            // TODO: It would be nice to keep at least the Number of Lines we're setting
            lock (LoggerList)
            {
                LoggerList.Clear();
            }
            Max_Number_Of_Entries = Number;
        }

        public static int GetMaxNumberOfEntries()
        {
            return Max_Number_Of_Entries;
        }

        public static void LogToFile(String text)
        {
            if (Logfile == null)
            {
                try
                {
                    Logfile = new StreamWriter("data.log", true);
                    Logfile.AutoFlush = true;
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception in Logger: " + e.Message + " ## " + e.StackTrace);
                }
            }
            lock (Logfile)
            {
                Logfile.WriteLine(text);
            }
        }

        public static void WriteLine(String text)
        {
            DateTime TimeDate = DateTime.Now;

            text = TimeDate.ToShortDateString() + " - " + TimeDate.ToShortTimeString() + " " + text;

            // write it to the console
            if (verbose) Console.WriteLine(text);
            if (writeLogfile) LogToFile(text);

            lock (LoggerList)
            {
                if (LoggerList.Count == Max_Number_Of_Entries)
                {
                    LoggerList.RemoveFirst();
                }

                LoggerList.AddLast(text);
            }
        }

        public static void WriteLineToScreenOnly(String text)
        {
            DateTime TimeDate = DateTime.Now;

            text = TimeDate.ToShortDateString() + " - " + TimeDate.ToShortTimeString() + " " + text;
            // write it to the console
            if (verbose) Console.WriteLine(text);
        }

        public static String[] GetLoggedLines()
        {
            String[] Output = new String[Max_Number_Of_Entries];
            int Current_Position = 0;

            lock (LoggerList)
            {
                foreach (String line in LoggerList)
                {
                    Output[Current_Position] = line;
                    Current_Position++;
                }
            }
            return Output;
        }
    }
}
