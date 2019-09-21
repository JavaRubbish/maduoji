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
    public partial class HandSettingForm : Form
    {
        //回零标志位
        public static bool huilingflag = true;
        //确认停止下发标志位
        public static bool stoped = false;
        public HandSettingForm()
        {
            InitializeComponent();
            AutoScale(this);
            //读取上次保存配置
            textBox3.Text = INIhelp.GetValue("X轴连动速度");
            textBox4.Text = INIhelp.GetValue("X轴寸动步进");
            textBox8.Text = INIhelp.GetValue("Z轴连动速度");
            textBox7.Text = INIhelp.GetValue("Z轴寸动步进");
            textBox12.Text = INIhelp.GetValue("Y轴连动速度");
            textBox11.Text = INIhelp.GetValue("Y轴寸动步进");
            textBox16.Text = INIhelp.GetValue("O轴连动速度");
            textBox15.Text = INIhelp.GetValue("O轴寸动步进");

            HomeForm.xinlei = true;
            HomeForm.StartThread();
        }

        public static void AutoScale(Form frm)
        {
            frm.Tag = frm.Width.ToString() + "," + frm.Height.ToString();
            frm.SizeChanged += new EventHandler(frm_SizeChanged);
        }

        static void frm_SizeChanged(object sender, EventArgs e)
        {
            string[] tmp = ((Form)sender).Tag.ToString().Split(',');
            float width = (float)((Form)sender).Width / (float)Convert.ToInt16(tmp[0]);
            float heigth = (float)((Form)sender).Height / (float)Convert.ToInt16(tmp[1]);

            ((Form)sender).Tag = ((Form)sender).Width.ToString() + "," + ((Form)sender).Height;

            foreach (Control control in ((Form)sender).Controls)
            {
                control.Scale(new SizeF(width, heigth));

            }
        }

        public void getXYZOCoordinate(int a,int b,int c,int d)
        {
            CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = a.ToString();    
            textBox10.Text = b.ToString();
            textBox6.Text = c.ToString();
            textBox14.Text = d.ToString();

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

      

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// X轴回原点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
            textBox1.Text = "0";
            textBox2.Text = "0";

            //是否已经回原点查询
            lock (locker)
            {
                while (huilingflag)
                {                 
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x04;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(1000);                   
                }
                huilingflag = true;
            }
        }

        private void ret_btn4_Click(object sender, EventArgs e)
        {
            INIhelp.SetValue("X轴连动速度",textBox3.Text);
            INIhelp.SetValue("X轴寸动步进",textBox4.Text);
            INIhelp.SetValue("Z轴连动速度",textBox8.Text);
            INIhelp.SetValue("Z轴寸动步进",textBox7.Text);
            INIhelp.SetValue("Y轴连动速度",textBox12.Text);
            INIhelp.SetValue("Y轴寸动步进",textBox11.Text);
            INIhelp.SetValue("O轴连动速度",textBox16.Text);
            INIhelp.SetValue("O轴寸动步进",textBox15.Text);

            HomeForm.xinlei = false;
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void HandSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void X_btn_Click(object sender, EventArgs e)
        {
            X_btn.BackColor = Color.SteelBlue;
            Y_btn.BackColor = Color.WhiteSmoke;
            Z_btn.BackColor = Color.WhiteSmoke;
            O_btn.BackColor = Color.WhiteSmoke;
            panelX.Visible = true;
            panelZ.Visible = false;
            panelY.Visible = false;
            panelO.Visible = false;
        }

        private void Z_btn_Click(object sender, EventArgs e)
        {
            X_btn.BackColor = Color.WhiteSmoke;
            Y_btn.BackColor = Color.WhiteSmoke;
            Z_btn.BackColor = Color.SteelBlue;
            O_btn.BackColor = Color.WhiteSmoke;
            panelX.Visible = false;
            panelZ.Visible = true;
            panelY.Visible = false;
            panelO.Visible = false;
        }

        private void Y_btn_Click(object sender, EventArgs e)
        {
            X_btn.BackColor = Color.WhiteSmoke;
            Y_btn.BackColor = Color.SteelBlue;
            Z_btn.BackColor = Color.WhiteSmoke;
            O_btn.BackColor = Color.WhiteSmoke;
            panelX.Visible = false;
            panelZ.Visible = false;
            panelY.Visible = true;
            panelO.Visible = false;
        }

        private void O_btn_Click(object sender, EventArgs e)
        {
            X_btn.BackColor = Color.WhiteSmoke;
            Y_btn.BackColor = Color.WhiteSmoke;
            Z_btn.BackColor = Color.WhiteSmoke;
            O_btn.BackColor = Color.SteelBlue;
            panelX.Visible = false;
            panelZ.Visible = false;
            panelY.Visible = false;
            panelO.Visible = true;
        }

        /// <summary>
        /// O轴回零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button36_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
            textBox13.Text = "0";
            textBox14.Text = "0";

            lock (locker)
            {
                //是否已经回原点查询
                while (huilingflag)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x04;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(1000);
                }
                huilingflag = true;
            }            
        }

        /// <summary>
        /// X轴连动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            //调试
            string str = textBox3.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf,11);
        }

        /// <summary>
        /// X轴连动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            string str = textBox3.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }

        /// <summary>
        /// X轴寸动左移???给一个脉冲？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            string str = textBox4.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }

        /// <summary>
        /// X轴寸动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            string str = textBox4.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }

        private void panelX_Paint(object sender, PaintEventArgs e)
        {

        }
        /// <summary>
        /// O轴连动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button40_Click(object sender, EventArgs e)
        {
            string str = textBox16.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// O轴连动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button39_Click(object sender, EventArgs e)
        {
            string str = textBox16.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// O轴寸动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button38_Click(object sender, EventArgs e)
        {
            string str = textBox15.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// O轴寸动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button37_Click(object sender, EventArgs e)
        {
            string str = textBox15.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }

        private void panelO_Paint(object sender, PaintEventArgs e)
        {

        }
        /// <summary>
        /// Y轴连动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button30_Click(object sender, EventArgs e)
        {
            string str = textBox12.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Y轴连动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button29_Click(object sender, EventArgs e)
        {
            string str = textBox12.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Y轴寸动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button28_Click(object sender, EventArgs e)
        {
            string str = textBox11.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Y轴寸动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button27_Click(object sender, EventArgs e)
        {
            string str = textBox11.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Z轴连动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {
            string str = textBox8.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Z轴连动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {
            string str = textBox8.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x03;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Z轴寸动左移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {
            string str = textBox7.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }

      
        /// <summary>
        /// Z轴寸动右移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            string str = textBox7.Text;
            int id = Convert.ToInt32(str);
            byte[] iByte = toBytes.intToBytes(id);//4位

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x08;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = 0x02;
            BF.sendbuf[6] = iByte[0];
            BF.sendbuf[7] = iByte[1];
            BF.sendbuf[8] = iByte[2];
            BF.sendbuf[9] = iByte[3];
            BF.sendbuf[10] = 0xF5;
            SendMenuCommand(BF.sendbuf, 11);
        }
        /// <summary>
        /// Y轴回原点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button26_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
            textBox9.Text = "0";
            textBox10.Text = "0";

            lock (locker)
            {
                //是否已经回原点查询
                while (huilingflag)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x04;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(1000);
                }
                huilingflag = true;
            }          
        }
        /// <summary>
        /// Z轴回原点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
            textBox5.Text = "0";
            textBox6.Text = "0";

            lock (locker)
            {
                //是否已经回原点查询
                while (huilingflag)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x04;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(1000);
                }
                huilingflag = true;
            }          
        }


        private void button15_Click(object sender, EventArgs e)
        {
            string str1 = textBox1.Text;
            if (str1 == "")
            {
                return;
            }
            int id1 = Convert.ToInt32(str1);
            byte[] iByte1 = toBytes.intToBytes(id1);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = iByte1[3];
            BF.sendbuf[6] = iByte1[2];
            BF.sendbuf[7] = iByte1[1];
            BF.sendbuf[8] = iByte1[0];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox3.Text);
            if(n <= 100)
            {
                textBox3.Text = (n + 1).ToString();
            }           
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox3.Text);
            if(n > 0)
            {
                textBox3.Text = (n - 1).ToString();
            }            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox4.Text);
            if (n <= 100)
            {
                textBox4.Text = (n + 1).ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox4.Text);
            if (n > 0)
            {
                textBox4.Text = (n - 1).ToString();
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            if (textBox16.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox16.Text);
            if (n <= 100)
            {
                textBox16.Text = (n + 1).ToString();
            }
        }

        private void button44_Click(object sender, EventArgs e)
        {
            if (textBox16.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox16.Text);
            if (n > 0)
            {
                textBox16.Text = (n - 1).ToString();
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            if (textBox15.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox15.Text);
            if (n <= 100)
            {
                textBox15.Text = (n + 1).ToString();
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            if (textBox15.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox15.Text);
            if (n > 0)
            {
                textBox15.Text = (n - 1).ToString();
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            if (textBox12.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox12.Text);
            if (n <= 100)
            {
                textBox12.Text = (n + 1).ToString();
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (textBox12.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox12.Text);
            if (n > 0)
            {
                textBox12.Text = (n - 1).ToString();
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (textBox11.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox11.Text);
            if (n <= 100)
            {
                textBox11.Text = (n + 1).ToString();
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (textBox11.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox11.Text);
            if (n > 0)
            {
                textBox11.Text = (n - 1).ToString();
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (textBox8.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox8.Text);
            if (n <= 100)
            {
                textBox8.Text = (n + 1).ToString();
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (textBox8.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox8.Text);
            if (n > 0)
            {
                textBox8.Text = (n - 1).ToString();
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox7.Text);
            if (n <= 100)
            {
                textBox7.Text = (n + 1).ToString();
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if(textBox7.Text == "")
            {
                return;
            }
            int n = Convert.ToInt32(textBox7.Text);
            if (n > 0)
            {
                textBox7.Text = (n - 1).ToString();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string str6 = textBox6.Text;
            if (str6 == "")
            {
                return;
            }
            int id6 = Convert.ToInt32(str6);
            byte[] iByte6 = toBytes.intToBytes(id6);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = iByte6[3];
            BF.sendbuf[6] = iByte6[2];
            BF.sendbuf[7] = iByte6[1];
            BF.sendbuf[8] = iByte6[0];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

        }

        private void button35_Click(object sender, EventArgs e)
        {
            string str14 = textBox14.Text;
            if (str14 == "")
            {
                return;
            }
            int id14 = Convert.ToInt32(str14);
            byte[] iByte14 = toBytes.intToBytes(id14);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = iByte14[3];
            BF.sendbuf[6] = iByte14[2];
            BF.sendbuf[7] = iByte14[1];
            BF.sendbuf[8] = iByte14[0];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            string str10 = textBox10.Text;
            if (str10 == "")
            {
                return;
            }
            int id10 = Convert.ToInt32(str10);
            byte[] iByte10 = toBytes.intToBytes(id10);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = iByte10[3];
            BF.sendbuf[6] = iByte10[2];
            BF.sendbuf[7] = iByte10[1];
            BF.sendbuf[8] = iByte10[0];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);
        }

        private void HandSettingForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void panelZ_Paint(object sender, PaintEventArgs e)
        {

        }

        //X轴停止
        private void button1_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!stoped)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x03;
                    BF.sendbuf[2] = 0x0A;
                    BF.sendbuf[3] = 0x05;
                    BF.sendbuf[4] = 0x03;
                    BF.sendbuf[5] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 6);
                    Thread.Sleep(300);
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x01;
            BF.sendbuf[3] = 0x04;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);
        }

        //O轴停止
        private void button3_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!stoped)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x03;
                    BF.sendbuf[2] = 0x0A;
                    BF.sendbuf[3] = 0x05;
                    BF.sendbuf[4] = 0x03;
                    BF.sendbuf[5] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 6);
                    Thread.Sleep(300);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x05;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
        }

        //Y轴停止
        private void button45_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!stoped)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x03;
                    BF.sendbuf[2] = 0x0A;
                    BF.sendbuf[3] = 0x05;
                    BF.sendbuf[4] = 0x03;
                    BF.sendbuf[5] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 6);
                    Thread.Sleep(300);
                }
            }
        }

        private void button46_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x05;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
        }

        //Z轴停止
        private void button47_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!stoped)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x03;
                    BF.sendbuf[2] = 0x0A;
                    BF.sendbuf[3] = 0x05;
                    BF.sendbuf[4] = 0x03;
                    BF.sendbuf[5] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 6);
                    Thread.Sleep(300);
                }
            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x03;
            BF.sendbuf[2] = 0x0A;
            BF.sendbuf[3] = 0x05;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = 0xF5;
            SendMenuCommand(BF.sendbuf, 6);
        }
    }
}
