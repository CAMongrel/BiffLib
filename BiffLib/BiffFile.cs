using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiffLib
{
    public enum ResourceType
    {
        res = 0x0000,       // Misc. GFF resources
        bmp = 0x0001,       // Microsoft Windows Bitmap
        mve = 0x0002,
        tga = 0x0003, 	//Targa Graphics Format
        wav = 0x0004, 	//Wave

        plt = 0x0006, 	//Bioware Packed Layer Texture
        ini = 0x0007, 	//Windows INI
        mp3 = 0x0008, 	//MP3
        mpg = 0x0009, 	//MPEG
        txt = 0x000A, 	//Text file
        xml = 0x000B,

        plh = 0x07D0,
        tex = 0x07D1,
        mdl = 0x07D2, 	//Model
        thg = 0x07D3,

        fnt = 0x07D5, 	//Font

        lua = 0x07D7, 	//Lua script source code ( http://www.lua.org/ )
        slt = 0x07D8,
        nss = 0x07D9, 	//NWScript source code
        ncs = 0x07DA, 	//NWScript bytecode
        mod = 0x07DB, 	//Module
        are = 0x07DC, 	//Area (GFF)
        set = 0x07DD, 	//Tileset (unused in KOTOR?)
        ifo = 0x07DE, 	//Module information
        bic = 0x07DF, 	//Character sheet (unused)
        wok = 0x07E0, 	// walk-mesh
        tda = 0x07E1, 	//2-dimensional array (2da)
        tlk = 0x07E2, 	//conversation file

        txi = 0x07E6, 	//Texture information
        git = 0x07E7, 	//Dynamic area information, game instance file, all area and objects that are scriptable
        bti = 0x07E8,
        uti = 0x07E9, 	//item blueprint
        btc = 0x07EA,
        utc = 0x07EB, 	//Creature blueprint

        dlg = 0x07ED, 	//Dialogue
        itp = 0x07EE, 	//tile blueprint pallet file
        btt = 0x07EF,
        utt = 0x07F0, 	//trigger blueprint
        dds = 0x07F1, 	//compressed texture file
        bts = 0x07F2,
        uts = 0x07F3, 	//sound blueprint
        ltr = 0x07F4, 	//letter combo probability info
        gff = 0x07F5, 	//Generic File Format
        fac = 0x07F6, 	//faction file
        bte = 0x07F7,
        ute = 0x07F8, 	//encounter blueprint
        btd = 0x07F9,
        utd = 0x07FA, 	//door blueprint
        btp = 0x07FB,
        utp = 0x07FC, 	//placeable object blueprint
        dft = 0x07FD, 	//default values file (text-ini)
        gic = 0x07FE, 	//game instance comments
        gui = 0x07FF, 	//GUI definition (GFF)
        css = 0x0800,
        ccs = 0x0801,
        btm = 0x0802,
        utm = 0x0803, 	//store merchant blueprint
        dwk = 0x0804, 	//door walkmesh
        pwk = 0x0805, 	//placeable object walkmesh
        btg = 0x0806,

        jrl = 0x0808, 	//Journal
        sav = 0x0809, 	//Saved game (ERF)
        utw = 0x080A, 	//waypoint blueprint
        fpc = 0x080B,   // 4pc
        ssf = 0x080C, 	//sound set file

        bik = 0x080F, 	//movie file (bik format)
        ndb = 0x0810,        //script debugger file
        ptm = 0x0811,        //plot manager/plot instance
        ptt = 0x0812,        //plot wizard blueprint
        ncm = 0x0813,
        mfx = 0x0814,
        mat = 0x0815,
        mdb = 0x0816,        //not the standard MDB, multiple file formats present despite same type
        say = 0x0817,
        ttf = 0x0818,        //standard .ttf font files
        ttc = 0x0819,
        cut = 0x081A,        //cutscene? (GFF)
        ka = 0x081B,         //karma file (XML)
        jpg = 0x081C,        //jpg image
        ico = 0x081D,        //standard windows .ico files
        ogg = 0x081E,        //ogg vorbis sound file
        spt = 0x081F,
        spw = 0x0820,
        wfx = 0x0821,        //woot effect class (XML)
        ugm = 0x0822,        // 2082 ??? [textures00.bif]
        qdb = 0x0823,        //quest database (GFF v3.38)
        qst = 0x0824,        //quest (GFF)
        npc = 0x0825,
        spn = 0x0826,
        utx = 0x0827,        //spawn point? (GFF)
        mmd = 0x0828,
        smm = 0x0829,
        uta = 0x082A,        //uta (GFF)
        mde = 0x082B,
        mdv = 0x082C,
        mda = 0x082D,
        mba = 0x082E,
        oct = 0x082F,
        bfx = 0x0830,
        pdb = 0x0831,
        TheWitcherSave = 0x0832,
        pvs = 0x0833,
        cfx = 0x0834,
        luc = 0x0835,        //compiled lua script

        prb = 0x0837,
        cam = 0x0838,
        vds = 0x0839,
        bin = 0x083A,
        wob = 0x083B,
        api = 0x083C,
        properties = 0x083D,
        png = 0x083E,

        big = 0x270B,

        erf = 0x270D, 	//Encapsulated Resource Format
        bif = 0x270E,
        key = 0x270F,
    }

    struct BiffFileHeader
    {
        public string Signature;
        public string Version;
        public int ResCount;
        public int ResTableOffset;
    }

    public class ResourceTableEntry
    {
        public int ID;
        public int Flags;  // BIF index is now in this value, (flags & 0xFFF00000) >> 20)
        public int Offset;
        public int Size;
        public ResourceType Type;
        public string Name;
    }

    public class ResourceEntry
    {
        public int ID;
        public string Name;
        public ResourceType Type;
        public byte[] Data;
    }

    public class BiffFile
    {
        private string Filename;
        private BiffFileHeader Header;
        private ResourceTableEntry[] Table;
        private BiffFileEntry biffEntry;

        private Dictionary<int, int> TableIDLookup;
        private Dictionary<int, ResourceEntry> CachedResources;

        public bool ShouldCache { get; set; } = true;

        /// <summary>
        /// Opens a biff file and reads the header and resource table information
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="keyfile">Can be null</param>
        public BiffFile(string filename, string keyfilename)
        {
            Filename = filename;

            TableIDLookup = new Dictionary<int, int>();
            CachedResources = new Dictionary<int, ResourceEntry>();
            Header = new BiffFileHeader();
            Header.Signature = "UNKW";
            Header.Version = "";
            Header.ResCount = 0;
            Header.ResTableOffset = 0;

            Table = null;

            KeyFile keyFile = new KeyFile(keyfilename);
            biffEntry = keyFile.GetBiffFileEntry(Path.GetFileName(filename));

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                if (reader.BaseStream.Length < 8)
                {
                    throw new Exception("File is not a BIFF V1.1 file (file is too short)");
                }

                // Read Header
                Header.Signature = new string(reader.ReadChars(4));
                Header.Version = new string(reader.ReadChars(4));

                if (Header.Signature != "BIFF" && Header.Version != "V1.1")
                {
                    throw new Exception("File is not a BIFF V1.1 file (according to header)");
                }

                Header.ResCount = reader.ReadInt32();
                reader.ReadInt32(); // NULL
                Header.ResTableOffset = reader.ReadInt32();

                // Read Resource Table
                Table = new ResourceTableEntry[Header.ResCount];

                reader.BaseStream.Seek(Header.ResTableOffset, SeekOrigin.Begin);
                for (int i = 0; i < Header.ResCount; i++)
                {
                    Table[i] = new ResourceTableEntry();
                    Table[i].ID = reader.ReadInt32();
                    Table[i].Flags = reader.ReadInt32();
                    Table[i].Offset = reader.ReadInt32();
                    Table[i].Size = reader.ReadInt32();
                    Table[i].Type = (ResourceType)reader.ReadInt16();
                    Table[i].Name = Table[i].ID.ToString();
                    if (biffEntry != null && biffEntry.ResourceNames.ContainsKey(Table[i].ID))
                    {
                        Table[i].Name = biffEntry.ResourceNames[Table[i].ID];
                    }
                    Table[i].Name += "." + Table[i].Type;
                    reader.ReadInt16(); // NULL

                    TableIDLookup.Add(Table[i].ID, i);
                }
            }            
        }

        /// <summary>
        /// Fully reads all resources into memory and caches them.
        /// 
        /// WARNING: Might take a long time and consume a lot of memory. Use with caution
        /// </summary>
        public void ReadAllResources()
        {
            if (Table == null)
                return;

            CachedResources.Clear();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(Filename)))
            {
                for (int i = 0; i < Table.Length; i++)
                {
                    ResourceEntry entry = new ResourceEntry();
                    entry.ID = Table[i].ID;
                    entry.Type = Table[i].Type;

                    reader.BaseStream.Seek(Table[i].Offset, SeekOrigin.Begin);
                    entry.Data = reader.ReadBytes(Table[i].Size);

                    entry.Name = Table[i].Name;

                    CachedResources.Add(Table[i].ID, entry);
                }
            }
        }

        /// <summary>
        /// Returns a deep copy of the resource table
        /// </summary>
        public ResourceTableEntry[] GetResourceTableCopy()
        {
            if (Table == null)
                return null;

            ResourceTableEntry[] result = new ResourceTableEntry[Table.Length];
            for (int i = 0; i < Table.Length; i++)
            {
                result[i] = new ResourceTableEntry();
                result[i].ID = Table[i].ID;
                result[i].Flags = Table[i].Flags;
                result[i].Offset = Table[i].Offset;
                result[i].Size = Table[i].Size;
                result[i].Type = Table[i].Type;
                result[i].Name = Table[i].Name;
            }
            return result;
        }

        /// <summary>
        /// Returns the number of resources in the BIFF file
        /// </summary>
        public int Count => Header.ResCount;

        /// <summary>
        /// Returns Resource entry as indexed by its ID (not the index in the file).
        /// 
        /// Returns a cached version if one exists.
        /// </summary>
        /// <param name="ID">Resource ID</param>
        /// <returns></returns>
        public ResourceEntry this[int ID]
        {
            get
            {
                if (CachedResources.ContainsKey(ID))
                    return CachedResources[ID];

                if (TableIDLookup.ContainsKey(ID) == false)
                    return null;

                using (BinaryReader reader = new BinaryReader(File.OpenRead(Filename)))
                {
                    ResourceTableEntry tableEntry = Table[TableIDLookup[ID]];

                    ResourceEntry entry = new ResourceEntry();
                    entry.ID = tableEntry.ID;
                    entry.Type = tableEntry.Type;

                    reader.BaseStream.Seek(tableEntry.Offset, SeekOrigin.Begin);
                    entry.Data = reader.ReadBytes(tableEntry.Size);

                    entry.Name = tableEntry.Name;

                    if (ShouldCache)
                        CachedResources.Add(tableEntry.ID, entry);

                    return entry;
                }                
            }
        }
    }
}
