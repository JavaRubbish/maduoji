using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 码垛机
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SplashForm sp = new SplashForm();
            sp.ShowDialog();
            if (SplashForm.flag == true)
            {
                Application.Run(new HomeForm());
            }
                       
        }

      

    }
}
