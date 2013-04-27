/*
* h.a.c.s (home automation control server) - http://github.com/bietiekay/hacs
* Copyright (C) 2010 Daniel Kirstenpfad
*
* hacs is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* hacs is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with hacs. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xs1_backup_tool
{
    class XS1BackupTool
    {
        #region HelpMessage
        public static void HelpMessage()
        {
            Console.WriteLine("This tool backups and restores the configuration of an EzControl XS1 device.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("      xs1-backup-tool <IP or Name of EzControl XS1> <Username> <Password> <options>");
            Console.WriteLine();
            Console.WriteLine("Backup");
            Console.WriteLine(" -b <output-filename>");
            Console.WriteLine("Restore");
            Console.WriteLine(" -r <input-filename>");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine();
            Console.WriteLine("xs1-backup-tool 192.168.1.242 Administrator password -b backup.txt");
            Console.WriteLine("creates a backup of the XS1 device with IP 192.168.1.242.");
            Console.WriteLine();
            Console.WriteLine("xs1-backup-tool 192.168.1.242 Administrator password -r backup.txt");
            Console.WriteLine("restores a backup from backup.txt to the XS1 device.");
            Console.WriteLine();
        }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("EzControl XS1 Backup Tool 0.1 - part of the h.a.c.s toolkit");
            Console.WriteLine("(C) 2010-2011 Daniel Kirstenpfad - http://technology-ninja.com");
            Console.WriteLine();

            #region Syntax Handling
            
            // check for enough parameters
            if (args.Length > 4)
            {
                switch (args[3])
                {
                    case "-b":
                        // backup
                        if (BackupAndRestore.backup(args[0], args[1], args[2], args[4]))
                        {
                            Console.WriteLine("Backup successful!");
                        }
                        else
                        {
                            Console.WriteLine("Backup unsuccessful!");
                        }
                        break;
                    case "-r":
                        // ask before restore
                        Console.WriteLine("Restoring a configuration will overwrite the current settings of");
                        Console.WriteLine("the EzControl XS1 device!");
                        Console.WriteLine();
                        Console.WriteLine("This is an untested functionality - it's not supposed to work right now!");
                        Console.WriteLine();
                        Console.WriteLine("Do you really want to restore the configuration from a backup?");
                        Console.Write("Type YES and press Enter: ");
                        String userinput = Console.ReadLine();
                        if (userinput == "YES")
                        {
                            // restore
                            if (BackupAndRestore.restore(args[0], args[1], args[2], args[4]))
                            {
                                Console.WriteLine("Restore successful!");
                            }
                            else 
                            {
                                Console.WriteLine("Restore unsuccessful!");
                            }
                        }
                        else
                            Console.Write("Restore abortet!");
                        break;
                    default:
                        HelpMessage();
                        return;
                }
            }
            else
            {
                HelpMessage();
                return;
            }
            #endregion

        }
    }
}
