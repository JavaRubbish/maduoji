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
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);
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

        public void AutoScale(Form frm)
        {
            frm.Tag = frm.Width.ToString() + "," + frm.Height.ToString();
            frm.SizeChanged += new EventHandler(frm_SizeChanged);
        }

        void frm_SizeChanged(object sender, EventArgs e)
        {
            string[] tmp = ((Form)sender).Tag.ToString().Split(',');
            float width = (float)((Form)sender).Width / (float)Convert.ToInt16(tmp[0]);
            float heigth = (float)((Form)sender).Height / (float)Convert.ToInt16(tmp[1]);

            ((Form)sender).Tag = ((Form)sender).Width.ToString() + "," + ((Form)sender).Height;

            this.X_btn.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.Y_btn.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.Z_btn.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.O_btn.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.ret_btn4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button3.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button5.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button6.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button7.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button8.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button9.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button10.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button11.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button12.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button13.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button14.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button15.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button16.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button17.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button18.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button19.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button20.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button21.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button22.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button23.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button24.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button25.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button26.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button27.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button28.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button29.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button30.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button31.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button32.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button33.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button34.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button35.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button36.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button37.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button38.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button39.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button40.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button41.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button42.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button43.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button44.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button45.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button46.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button47.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button48.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

            this.label1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label3.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label5.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label6.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label7.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label8.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label9.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label10.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label11.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label12.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label13.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label14.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label15.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label16.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label17.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label18.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label19.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label20.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label21.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label22.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label23.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label24.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

            this.textBox1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox3.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox5.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox6.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox7.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox8.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox9.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox10.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox11.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134)); this.textBox1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox12.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox13.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox14.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox15.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox16.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

            foreach (Control control in ((Form)sender).Controls)
            {
                control.Scale(new SizeF(width, heigth));

            }
        }

        /// <summary>
        /// 启用双缓存减少界面闪烁
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
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
            this.WindowState = FormWindowState.Maximized;
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
