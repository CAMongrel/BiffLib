using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiffLib
{
    struct KeyFileHeader
    {
        public string Signature;
        public string Version;
        public int FileCount;
        public int FileTableOffset;
        public int KeyCount;
        public int KeyTableOffset;
        public int BuildYear;
        public int BuildDay;
    }

    struct KeyTableEntry
    {

    }

    class BiffFileEntry
    {
        public string BiffName;
        public int Size;

        public Dictionary<int, string> ResourceNames;

        public BiffFileEntry()
        {
            ResourceNames = new Dictionary<int, string>();
        }

        public override string ToString()
        {
            return BiffName;
        }
    }

    class KeyFile
    {
        private KeyFileHeader Header;
        private BiffFileEntry[] FileEntries;

        internal KeyFile(string filename)
        {
            Header = new KeyFileHeader();
            Header.Signature = "UNKW";
            Header.Version = "";

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                if (reader.BaseStream.Length < 8)
                {
                    throw new Exception("File is not a KEY V1.1 file (file is too short)");
                }

                // Read Header
                Header.Signature = new string(reader.ReadChars(4));
                Header.Version = new string(reader.ReadChars(4));

                if (Header.Signature != "BIFF" && Header.Version != "V1.1")
                {
                    throw new Exception("File is not a KEY V1.1 file (according to header)");
                }

                Header.FileCount = reader.ReadInt32();
                Header.KeyCount = reader.ReadInt32();
                reader.ReadInt32(); // NULL
                Header.FileTableOffset = reader.ReadInt32();
                Header.KeyTableOffset = reader.ReadInt32();
                Header.BuildYear = reader.ReadInt32();
                Header.BuildDay = reader.ReadInt32();

                // Read file table
                FileEntries = new BiffFileEntry[Header.FileCount];
                reader.BaseStream.Seek(Header.FileTableOffset, SeekOrigin.Begin);
                for (int i = 0; i < Header.FileCount; i++)
                {
                    FileEntries[i] = new BiffFileEntry();
                    FileEntries[i].Size = reader.ReadInt32();
                    int offset = reader.ReadInt32();
                    int size = reader.ReadInt32();
                    long position = reader.BaseStream.Position;
                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    FileEntries[i].BiffName = Encoding.UTF8.GetString(reader.ReadBytes(size));
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                }

                // Read resource names
                reader.BaseStream.Seek(Header.KeyTableOffset, SeekOrigin.Begin);
                for (int i = 0; i < Header.KeyCount; i++)
                {
                    string resName = Encoding.UTF8.GetString(reader.ReadBytes(16)).Replace("\0", "");
                    ResourceType type = (ResourceType)reader.ReadInt16();
                    int id = reader.ReadInt32();
                    int flags = reader.ReadInt32();

                    int bifIndex = (int)((flags & 0xFFF00000) >> 20);
                    FileEntries[bifIndex].ResourceNames.Add(id, resName);
                }
            }            
        }

        public BiffFileEntry GetBiffFileEntry(string bifname)
        {
            bifname = bifname.ToLowerInvariant();

            for (int i = 0; i < FileEntries.Length; i++)
            {
                if (FileEntries[i].BiffName.ToLowerInvariant() == bifname)
                    return FileEntries[i];
            }

            return null;
        }
    }
}
