using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLib
{
    public class MyIni
    {
        private string inipath;
        public Dictionary<string, Dictionary<string, string>> iniInfo = new Dictionary<string, Dictionary<string, string>>();

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, byte[] retVal, int size, string filePath);

        public MyIni(string INIPath)
        {
            
            inipath = INIPath;

        }

        public MyIni() { }


        public Dictionary<string, Dictionary<string, string>> InitInfo()
        {
            List<string> sectionList = ReadSections();

            foreach (string section in sectionList)
            {
                List<string> keyList = ReadKeys(section);
                List<string> valueList = ReadValues(section);
                Dictionary<string,string> dic = new Dictionary<string,string>();
                foreach (string key in keyList)
                {
                    dic.Add(key, valueList[keyList.IndexOf(key)]);
                }
                
                iniInfo.Add(section, dic);
            }

            return iniInfo;
        }

        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            GetPrivateProfileString(Section, Key, "", temp, 500, inipath);
            return temp.ToString();
        }

        /// <summary>
        /// 读取全部section
        /// </summary>
        /// <param name="iniFilename"></param>
        /// <returns></returns>
        public List<string> ReadSections()
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, inipath);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }

        /// <summary>
        /// 读取section下全部key
        /// </summary>
        /// <param name="SectionName"></param>
        /// <returns></returns>
        public List<string> ReadKeys(string Section)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(Section, null, null, buf, buf.Length, inipath);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }

        /// <summary>
        /// 读取全部value
        /// </summary>
        /// <param name="Section"></param>
        /// <returns></returns>
        public List<string> ReadValues(string Section)
        {
            List<string> result = new List<string>();
            List<string> keys = ReadKeys(Section);

            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(IniReadValue(Section, keys[i]));
            }

            return result;
        }

        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, inipath);
        }

        /// <summary>
        /// 根据名称删除
        /// </summary>
        /// <param name="section"></param>
        public void DeleteSection(string section)
        {
            WritePrivateProfileString(section, null, null, inipath);
        }

        /// <summary>
        /// 删除全部
        /// </summary>
        public void DeleteAllSection()
        {
            if (ExistINIFile())
            {
                File.Delete(inipath);
            }
        }


        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }

    }

}
