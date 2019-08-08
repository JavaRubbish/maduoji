using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 码垛机
{
    /// <summary>
    /// Config配置文件，用于初始化加载数据
    /// </summary>
    class INIhelp
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        //ini文件名称
        private static string inifilename = "Config.ini";
        //获取ini文件路径
        private static string inifilepath = Directory.GetCurrentDirectory() + "\\" + inifilename;

        public static string GetValue(string key)
        {
            StringBuilder s = new StringBuilder(1024);
            GetPrivateProfileString("CONFIG", key, "", s, 1024, inifilepath);
            return s.ToString();
        }


        public static void SetValue(string key, string value)
        {
            try
            {
                WritePrivateProfileString("CONFIG", key, value, inifilepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
