using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.ListBox;

namespace NetaTools
{
    class Service
    {
        public static void GetAllDirectoryAndFiles(DirectoryInfo info, ref List<FileSystemInfo> infoList, List<string> filterExt)
        {
            //var infos = info.GetFileSystemInfos().Where(x => !(x is FileInfo && !filterExt.Contains(Path.GetExtension(x.FullName.ToLower()))));
            var infos = info.GetFileSystemInfos();
            infoList.AddRange(infos);

            foreach (var item in infos.Where(x => x is DirectoryInfo))
            {
                GetAllDirectoryAndFiles(item as DirectoryInfo, ref infoList, filterExt);
            }
        }

        public static void DistinctListBox(ObjectCollection items)
        {
            List<string> temp = new List<string>();
            foreach (var item in items)
            {
                temp.Add((string)item);
            }

            items.Clear();
            items.AddRange(temp.Distinct().OrderBy(x => x).ToArray());

        }

        public static void DoReplaceFiles(string file, string oldText, string newText)
        {
            int i = 0;
            var eType = FileEncoding.GetType(file);
            var lines = File.ReadAllLines(file, eType);
            StreamWriter sw = new StreamWriter(file, false, eType);
            while (i < lines.Length)
            {
                sw.WriteLine(lines[i].Replace(oldText, newText));
                i++;
            }
            sw.Close();
        }

        public static string DoReplaceFileName(string fullName, string oldText, string newText)
        {
            string destFileName = null;
            if (File.Exists(fullName))
            {
                var file = new FileInfo(fullName);
                destFileName = file.Directory + "\\" + file.Name.Replace(oldText, newText);

                file.MoveTo(destFileName);
            }

            return destFileName;
        }
    }


    /// <summary>
    /// C#获取文本文件的编码，自动区分GB2312和UTF8
    /// </summary>
    public static class FileEncoding
    {

        /// <summary>
          /// C#根据字节数据byte[]前2位判断文本文件的Encoding编码格式
          /// </summary>
          /// <param name="bs"></param>
          /// <returns></returns>
        public static System.Text.Encoding GetType(byte[] bs)
        {
            Encoding result = System.Text.Encoding.Default;

            using (System.IO.MemoryStream fs = new MemoryStream(bs))
            {
                using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
                {
                    Byte[] buffer = br.ReadBytes(2);

                    if (buffer[0] >= 0xEF)
                    {
                        if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                        {
                            result = System.Text.Encoding.UTF8;
                        }
                        else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                        {
                            result = System.Text.Encoding.BigEndianUnicode;
                        }
                        else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                        {
                            result = System.Text.Encoding.Unicode;
                        }
                        else
                        {
                            result = System.Text.Encoding.Default;
                        }
                    }
                    else
                    {
                        result = System.Text.Encoding.Default;
                    }
                    br.Close();
                    br.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
            }

            return result;
        }


        /// <summary>
          /// 获取文件编码格式
          /// </summary>
          /// <param name="file"></param>
          /// <returns></returns>
        public static System.Text.Encoding GetType(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
                byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
                byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
                Encoding reVal = Encoding.Default;
                using (BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default))
                {
                    int.TryParse(fs.Length.ToString(), out int i);
                    byte[] ss = r.ReadBytes(i);
                    if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                    {
                        reVal = Encoding.UTF8;
                    }
                    else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                    {
                        reVal = Encoding.BigEndianUnicode;
                    }
                    else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                    {
                        reVal = Encoding.Unicode;
                    }
                    r.Close();
                }
                fs.Close();
                fs.Dispose();
                return reVal;
            }
        }

        /// <summary>
          /// 判断是否是不带 BOM 的 UTF8 格式
          /// </summary>
          /// <param name=“data“></param>
          /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}
