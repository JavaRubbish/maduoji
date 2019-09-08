using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 码垛机
{
    public partial class SplashForm : Form
    {
        public static Boolean flag = false;
        public SplashForm()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 加载等待时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        int a = 3;
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if(a > 0)
            {
                a--;
                loadtimelabel.Text = a.ToString();

            }
            else
            {
                flag = true;
                this.Close();
            }
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
