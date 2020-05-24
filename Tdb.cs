using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeriousTDB
{
    class Tdb
    {

        public static List<Tuple<string, int, int>> lstTdb;

        public static List<string> lstExtra;

        public static bool IsLittleEndian;

        public static bool LoadTdbFromFile(string file)
        {
            int pos = 8, i = 0;
            try
            {
                byte[] filecontent = File.ReadAllBytes(file);
                IsLittleEndian = (filecontent[4] != 0);
                lstTdb = new List<Tuple<string, int, int>>();
                lstExtra = new List<string>();
                int extracount = ReadInt32(filecontent, 4);
                int stringCount = ReadInt32(filecontent, 0);

                do
                {
                    int v1 = ReadInt32(filecontent, pos);
                    int v2 = ReadInt32(filecontent, pos + 4);
                    int v3 = ReadInt32(filecontent, pos + 8);
                    string str = ReadString(filecontent, v1);
                    lstTdb.Add(new Tuple<string, int, int>(str, v2, v3));
                    i++;
                    pos += 12;
                } while (i < stringCount);

                while (extracount > 0)
                {
                    int strptr = ReadInt32(filecontent, pos);
                    lstExtra.Add(ReadString(filecontent, strptr));
                    pos += 4;
                    extracount--;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0:D1}\n{1:X8}\n{2}", i, pos, ex.ToString()), "LoadTdbFromFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static String ReadString(byte[] tdb, int loc)
        {
            MemoryStream ms = new MemoryStream();
            while (tdb[loc] != 0)
            {
                ms.WriteByte(tdb[loc]);
                loc++;
            }
            return Encoding.Default.GetString(ms.ToArray());
        }

        public static int ReadInt32(byte[] tdb, int loc)
        {
            if (IsLittleEndian)
                return BitConverter.ToInt32(tdb, loc);
            else
                return BitConverter.ToInt32(new byte[] { tdb[loc + 3], tdb[loc + 2], tdb[loc + 1], tdb[loc] }, 0);
        }

        //public static void WriteInt32(byte[] tdb, int loc, int value)
        //{
        //    byte[] valbytes = BitConverter.GetBytes(value);
        //    if (!IsLittleEndian)
        //        Array.Reverse(valbytes);
        //    tdb[loc] = valbytes[0];
        //    tdb[loc + 1] = valbytes[1];
        //    tdb[loc + 2] = valbytes[2];
        //    tdb[loc + 3] = valbytes[3];
        //}

        public static bool SaveTdbToFile(string file)
        {
            int pos = 8, i = 0;
            try
            {
                MemoryStream ms = new MemoryStream();
                InsertInt32(ms, lstTdb.Count);
                InsertInt32(ms, lstExtra.Count);
                InsertBytes(ms, 0, 12 * lstTdb.Count);
                foreach (var e in lstExtra)
                    InsertInt32(ms, 0);
                
                for (i = 0; i < lstTdb.Count; i++)
                {
                    int pointertostr = (int)ms.Position;
                    WriteInt32(ms, pos, pointertostr);
                    WriteInt32(ms, pos+4, lstTdb[i].Item2);
                    WriteInt32(ms, pos+8, lstTdb[i].Item3);
                    InsertString(ms, lstTdb[i].Item1);
                    pos += 12;
                }

                int a = (int)(ms.Position % 4);
                InsertBytes(ms, 0, 4 - a);

                foreach (var e in lstExtra)
                {
                    WriteInt32(ms, pos, (int)ms.Position);
                    InsertString(ms, e);
                    a = (e.Length + 1) % 16;
                    InsertBytes(ms, 0, 16 - a);
                    pos += 4;
                }
                File.WriteAllBytes(file, ms.ToArray());
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("{0:D1}\n{1:X8}\n{2}", i, pos, ex.ToString()), "SaveTdbToFile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void WriteInt32(MemoryStream ms, int loc, int value)
        {
            ms.Position = loc;
            byte[] valbytes = BitConverter.GetBytes(value);
            if (!IsLittleEndian)
                Array.Reverse(valbytes);
            ms.Write(valbytes, 0, 4);
            ms.Seek(0, SeekOrigin.End);
        }

        public static void InsertInt32(MemoryStream ms, int value)
        {
            byte[] valbytes = BitConverter.GetBytes(value);
            if (!IsLittleEndian)
                Array.Reverse(valbytes);
            ms.Write(valbytes, 0, 4);
        }

        public static void InsertString(MemoryStream ms, string str)
        {
            if(str.Length > 0)
            { 
                byte[] strbytes = Encoding.Default.GetBytes(str);
                ms.Write(strbytes, 0, strbytes.Length);
            }
            ms.WriteByte(0);
        }

        public static void InsertBytes(MemoryStream ms, byte value, int count)
        {
            while (count > 0)
            {
                ms.WriteByte(value);
                count--;
            }
        }

    }
}
