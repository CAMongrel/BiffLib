using BiffLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unbiff
{
    class Program
    {
        private static void PrintHelp()
        {
            Console.WriteLine("unbif - BIFF file reader");
            Console.WriteLine("Copyright (c) 2015 by Henning Thoele");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("\tunbif <command> <parameters> <biff_file> [key_file]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("\tl: List entries in BIFF file");
            Console.WriteLine("\tx: Extract entry from BIFF file");
            Console.WriteLine("\td: Dump all entries from BIFF file");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("\t-i <int>: Used with 'x'. Specify resource ID to extract");
            Console.WriteLine("\t-o <string>: Used with 'x' and 'd'. Specify output folder");
            Console.WriteLine();
            Console.WriteLine("\tbiff_file: The .bif file to open");
            Console.WriteLine("\tkey_file: Optional key file which references the bif file and provides filename information. If no key file is given, the bif file export produces filename based on the ID.");
        }

        private static void HandleListCommand(string[] args)
        {
            string filename = args[1];
            if (filename.StartsWith("-"))
            {
                PrintHelp();
                Console.WriteLine();
                Console.WriteLine("Error: List command does not accept any parameters");
                return;
            }

            string keyfile = null;
            if (args.Length == 3)
                keyfile = args[2];

            BiffFile file = new BiffFile(filename, keyfile);
            ResourceTableEntry[] table = file.GetResourceTableCopy();
            for (int i = 0; i < table.Length; i++)
            {
                Console.WriteLine("{0}\t:\t{1}\t({2} bytes)", table[i].ID, table[i].Name, table[i].Size);
            }
            Console.WriteLine("{0} entries in the file", table.Length);
        }

        private static void HandleExtractCommand(string[] args)
        {
            string iParam = null;
            string oParam = null;

            int fnStart = 1;
            if (args[1].StartsWith("-i"))
            {
                iParam = args[2];
                fnStart += 2;
            } else if (args[1].StartsWith("-o"))
            {
                oParam = args[2];
                fnStart += 2;
            }
            if (args[3].StartsWith("-i"))
            {
                iParam = args[4];
                fnStart += 2;
            } else if (args[3].StartsWith("-o"))
            {
                oParam = args[4];
                fnStart += 2;
            }

            if (oParam == null)
                oParam = Directory.GetCurrentDirectory();

            string filename = args[fnStart];

            string keyfile = null;
            if (args.Length >= fnStart + 2)
                keyfile = args[fnStart + 1];

            int id = -1;
            if (int.TryParse(iParam, out id) == false)
            {
                PrintHelp();
                Console.WriteLine();
                Console.WriteLine("Error: Invalid value for param 'i': Must be an integer");
                return;
            }

            BiffFile file = new BiffFile(filename, keyfile);
            ResourceEntry entry = file[id];
            if (entry == null)
            {
                PrintHelp();
                Console.WriteLine();
                Console.WriteLine("Error: ID '{0}' does not exist in BIFF file", id.ToString());
                return;
            }

            File.WriteAllBytes(Path.Combine(oParam, entry.Name), entry.Data);
            Console.WriteLine("{0} bytes written to '{1}'", entry.Data.Length, Path.Combine(oParam, entry.Name));
        }

        private static void HandleDumpCommand(string[] args)
        {
            string oParam = null;

            int fnStart = 1;
            if (args[1].StartsWith("-o"))
            {
                oParam = args[2];
                fnStart += 2;
            }

            if (oParam == null)
                oParam = Directory.GetCurrentDirectory();

            string filename = args[fnStart];

            string keyfile = null;
            if (args.Length >= fnStart + 2)
                keyfile = args[fnStart + 1];

            BiffFile file = new BiffFile(filename, keyfile);
            file.ShouldCache = false;
            ResourceTableEntry[] table = file.GetResourceTableCopy();
            for (int i = 0; i < table.Length; i++)
            {
                ResourceEntry entry = file[table[i].ID];
                if (entry == null)
                {
                    Console.WriteLine("Skipping file {0} due to the entry being invalid", i);
                    continue;
                }

                Console.WriteLine("Writing file {0} of {1}", i, table.Length);
                File.WriteAllBytes(Path.Combine(oParam, entry.Name), entry.Data);
            }

            Console.WriteLine("{0} files written to '{1}'", table.Length, oParam);
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintHelp();
                return;
            }

            string command = args[0];
            
            switch (command)
            {
                case "l":
                    HandleListCommand(args);
                    break;

                case "x":
                    HandleExtractCommand(args);
                    break;

                case "d":
                    HandleDumpCommand(args);
                    break;
            }
        }
    }
}
