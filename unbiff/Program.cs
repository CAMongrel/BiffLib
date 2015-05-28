using BiffLib;
using System;
using System.Collections.Generic;
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
                    break;

                case "d":
                    break;
            }
        }
    }
}
