using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLib
{
    public class IniFiles
    {
        public string inipath;

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, byte[] retVal, int size, string filePath);

        public IniFiles(string INIPath)
        {
            
            inipath = INIPath;
            
        }

        public IniFiles() { }

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
            WritePrivateProfileString(null, null, null, inipath);
        }


        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }

        /// <summary>
        /// 设置为当前应用的数据库配置
        /// </summary>
        public List<string> GetSelectedSqlConfig()
        {
            List<string> section = ReadSections();
            for (int i = 0; i < section.Count; i++)
            {
                if (IniReadValue(section[i], "Checked") == "1")
                {
                    return ReadValues(section[i]);
                }
            }
            return null;
        }

    }

}
