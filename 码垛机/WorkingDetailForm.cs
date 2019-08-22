using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 码垛机.HomeForm;

namespace 码垛机
{
    public partial class WorkingDetailForm : Form
    {
        public WorkingDetailForm()
        {
            InitializeComponent();
        }

        private void WorkingDetailForm_Load(object sender, EventArgs e)
        {
        }

                 

        public void SetCoordinate(int x,int z,int y,int o)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.label14.Text = x.ToString();
            this.label18.Text = z.ToString();
            this.label19.Text = y.ToString();
            this.label20.Text = o.ToString();
        }

        private void clr_btn1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = progressBar2.Value = progressBar3.Value = progressBar4.Value = 0;
        }

        private void clr_btn3_Click(object sender, EventArgs e)
        {
            progressBar5.Value = progressBar6.Value = progressBar7.Value = progressBar8.Value = 0;
        }

        private void clr_btn2_Click(object sender, EventArgs e)
        {
            progressBar9.Value = progressBar10.Value = progressBar11.Value = progressBar12.Value = 0;
        }

        private void clr_btn4_Click(object sender, EventArgs e)
        {
            HomeForm.xinlei = false;
            HomeForm.fight = false;
            label22.Text = 0.ToString();
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x0D;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);
            HomeForm.xinlei = true;
            HomeForm.fight = true;
        }


        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void str_btn_Click(object sender, EventArgs e)
        {
            HomeForm.xinlei = false;
            HomeForm.fight = false;
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x0D;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);
            HomeForm.xinlei = true;
            HomeForm.fight = true;
        }
            
        private void pause_btn_Click(object sender, EventArgs e)
        {
            HomeForm.xinlei = false;
            HomeForm.fight = false;
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x0D;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);
            HomeForm.xinlei = true;
            HomeForm.fight = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HomeForm.xinlei = false;
            HomeForm.fight = false;
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x0D;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);
            HomeForm.xinlei = true;
            HomeForm.fight = true;
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
    }
}
