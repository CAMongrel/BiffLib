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
            Console.WriteLine("\tunbif <command> <parameters> <biff_file>");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("\tl: List entries in BIFF file");
            Console.WriteLine("\tx: Extract entry from BIFF file");
            Console.WriteLine("\td: Dump all entries from BIFF file");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("\t-i <int>: Used with 'x'. Specify resource ID to extract");
            Console.WriteLine("\t-o <string>: Used with 'x' and 'd'. Specify output folder");
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

            BiffFile file = new BiffFile(filename);
            ResourceTableEntry[] table = file.GetResourceTableCopy();
            for (int i = 0; i < table.Length; i++)
            {
                Console.WriteLine("{0}\t:\t{1}", table[i].ID, table[i].Type);
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
