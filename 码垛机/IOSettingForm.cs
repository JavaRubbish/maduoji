using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static 码垛机.HomeForm;
using System.Threading;

namespace 码垛机
{
    public partial class IOSettingForm : Form
    {
        //设置标志位，轮询判断IO输入灯的状态信息
        public static bool ioflag = true;
        public IOSettingForm()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);

            InitializeComponent();
            
            AutoScale(this);

            getStatus();
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

            this.ret_btn1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.IN_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.OUT_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
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

        private void IOSettingForm_Load(object sender, EventArgs e)
        {
            huayuan();
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 重绘画圆
        /// </summary>
        private void huayuan()
        {
            // this.FormBorderStyle = FormBorderStyle.None;
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(5, 5, 50, 50);
            GraphicsPath myPath2 = new GraphicsPath();
            myPath2.AddEllipse(4, 4, 40, 40);
            this.button1.Region = new Region(myPath2);
            this.button2.Region = new Region(myPath2);
            this.button3.Region = new Region(myPath);
            this.button4.Region = new Region(myPath);
            this.button5.Region = new Region(myPath);
            this.button6.Region = new Region(myPath);
            this.button7.Region = new Region(myPath);
            this.button8.Region = new Region(myPath);
            this.button9.Region = new Region(myPath);
            this.button10.Region = new Region(myPath);
            this.button11.Region = new Region(myPath);
            this.button12.Region = new Region(myPath);
            this.button13.Region = new Region(myPath);
            this.button14.Region = new Region(myPath);
            this.button15.Region = new Region(myPath);
            this.button16.Region = new Region(myPath);
            this.button17.Region = new Region(myPath);
            this.button18.Region = new Region(myPath2);
            this.button19.Region = new Region(myPath2);
            this.button20.Region = new Region(myPath2);
            this.button21.Region = new Region(myPath2);
            this.button22.Region = new Region(myPath2);
            this.button23.Region = new Region(myPath2);
            this.button24.Region = new Region(myPath2);
            this.button25.Region = new Region(myPath2);
            this.button26.Region = new Region(myPath2);
            this.button27.Region = new Region(myPath2);
            this.button28.Region = new Region(myPath2);
            this.button29.Region = new Region(myPath2);
            this.button30.Region = new Region(myPath2);
            this.button31.Region = new Region(myPath2);
            this.button32.Region = new Region(myPath2);
            this.button33.Region = new Region(myPath2);
            this.button34.Region = new Region(myPath2);
            this.button35.Region = new Region(myPath2);
            this.button36.Region = new Region(myPath2);
            this.button37.Region = new Region(myPath2);
            this.button38.Region = new Region(myPath2);
            this.button39.Region = new Region(myPath2);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ret_btn1_Click(object sender, EventArgs e)
        {
            saveStatus();
            ioflag = false;
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 保存指示灯状态，怎么遍历简化代码？
        /// </summary>

        private void saveStatus()
        {
            
            if(button3.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts3,"0");
            }
            if (button3.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts3, "1");
            }

            if (button4.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts4, "0");
           }
            if (button4.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts4, "1");
            }

            if (button5.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts5, "0");
            }
            if (button5.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts5, "1");
            }

            if (button6.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts6, "0");
            }
            if (button6.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts6, "1");
            }

            if (button7.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts7, "0");
            }
            if (button7.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts7, "1");
            }

            if (button8.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts8, "0");
            }
            if (button8.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts8, "1");
            }

            if (button9.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts9, "0");
            }
            if (button9.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts9, "1");
            }

            if (button10.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts10, "0");
            }
            if (button10.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts10, "1");
            }

            if (button11.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts11, "0");
            }
            if (button11.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts11, "1");
            }

            if (button12.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts12, "0");
            }
            if (button12.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts12, "1");
            }

            if (button13.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts13, "0");
            }
            if (button13.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts13, "1");
            }

            if (button14.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts14, "0");
            }
            if (button14.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts14, "1");
            }

            if (button15.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts15, "0");
            }
            if (button15.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts15, "1");
            }

            if (button16.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts16, "0");
            }
            if (button16.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts16, "1");
            }

            if (button17.BackColor == Color.Transparent)
            {
                INIhelp.SetValue(GlobalV.sts17, "0");
            }
            if (button17.BackColor == Color.Red)
            {
                INIhelp.SetValue(GlobalV.sts17, "1");
            }
        }

        /// <summary>
        /// 获取指示灯状态
        /// </summary>
        private void getStatus()
        {

            if (INIhelp.GetValue(GlobalV.sts3) == "1")
            {
                button3.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts3) == "0")
            {
                button3.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts4) == "1")
            {
                button4.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts4) == "0")
            {
                button4.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts5) == "1")
            {
                button5.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts5) == "0")
            {
                button5.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts6) == "1")
            {
               button6.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts6) == "0")
            {
               button6.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts7) == "1")
            {
               button7.BackColor = Color.Red;
             }
            if (INIhelp.GetValue(GlobalV.sts7) == "0")
            {
               button7.BackColor = Color.Transparent;
             }

            if (INIhelp.GetValue(GlobalV.sts8) == "1")
            {
               button8.BackColor = Color.Red;
             }
            if (INIhelp.GetValue(GlobalV.sts8) == "0")
            {
               button8.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts9) == "1")
            {
               button9.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts9) == "0")
            {
               button9.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts10) == "1")
            {
               button10.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts10) == "0")
            {
               button10.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts11) == "1")
            {
               button11.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts11) == "0")
            {
               button11.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts12) == "1")
            {
               button12.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts12) == "0")
            {
               button12.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts13) == "1")
            {
               button13.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts13) == "0")
            {
               button13.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts14) == "1")
            {
               button14.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts14) == "0")
            {
               button14.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts15) == "1")
            {
               button15.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts15) == "0")
            {
               button15.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts16) == "1")
            {
               button16.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts16) == "0")
            {
               button16.BackColor = Color.Transparent;
            }

            if (INIhelp.GetValue(GlobalV.sts17) == "1")
            {
               button17.BackColor = Color.Red;
            }
            if (INIhelp.GetValue(GlobalV.sts17) == "0")
            {
               button17.BackColor = Color.Transparent;
            }
        }
       
        private void IOSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void panelOUT_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>
        /// 一键获取输入点监控状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IN_btn_Click(object sender, EventArgs e)
        {
            IN_btn.BackColor = Color.FromArgb(118, 153, 223);
            OUT_btn.BackColor = Color.FromArgb(237, 236, 236);
            panelIN.Visible = true;
            panelOUT.Visible = false;

            Thread thread = new Thread(new ThreadStart(sendInputRequest));
            thread.IsBackground = true;
            thread.Start();

        }
        public static void sendInputRequest()
        {
            while (ioflag)
            {
                Thread.Sleep(1000);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x02;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
            }
           
        }

        public void getIOStatus(int a,int b,int c)
        {
                if ((a & 1) == 1) {
                    button2.BackColor = Color.Red;
                }
                if ((a & 1) == 0)
                {
                    button2.BackColor = Color.Transparent;
                }

                if (((a & (1 << 1)) >> 1) == 1)
                {
                    button1.BackColor = Color.Red;
                }
                if (((a & (1 << 1)) >> 1) == 0)
                {
                    button1.BackColor = Color.Transparent;
                }

                if (((a & (1 << 2)) >> 2) == 1)
                {
                    button18.BackColor = Color.Red;
                }
                if (((a & (1 << 2)) >> 2) == 0)
                {
                    button18.BackColor = Color.Transparent;
                }

                if (((a & (1 << 3)) >> 3) == 1)
                {
                    button19.BackColor = Color.Red;
                }
                if (((a & (1 << 3)) >> 3) == 0)
                {
                    button19.BackColor = Color.Transparent;
                }

                if (((a & (1 << 4)) >> 4) == 1)
                {
                    button20.BackColor = Color.Red;
                }
                if (((a & (1 << 4)) >> 4) == 0)
                {
                    button20.BackColor = Color.Transparent;
                }

                if (((a & (1 << 5)) >> 5) == 1)
                {
                    button21.BackColor = Color.Red;
                }
                if (((a & (1 << 5)) >> 5) == 0)
                {
                    button21.BackColor = Color.Transparent;
                }

                if (((a & (1 << 6)) >> 6) == 1)
                {
                    button22.BackColor = Color.Red;
                }
                if (((a & (1 << 6)) >> 6) == 0)
                {
                    button22.BackColor = Color.Transparent;
                }

                if (((a & (1 << 7)) >> 7) == 1)
                {
                    button23.BackColor = Color.Red;
                }
                if (((a & (1 << 7)) >> 7) == 0)
                {
                    button23.BackColor = Color.Transparent;
                }


                //******************************************
                if ((b & 1) == 1)
                {
                    button24.BackColor = Color.Red;
                }
                if ((b & 1) == 0)
                {
                    button24.BackColor = Color.Transparent;
                }

                if (((b & (1 << 1)) >> 1) == 1)
                {
                    button25.BackColor = Color.Red;
                }
                if (((b & (1 << 1)) >> 1) == 0)
                {
                    button25.BackColor = Color.Transparent;
                }

                if (((b & (1 << 2)) >> 2) == 1)
                {
                    button26.BackColor = Color.Red;
                }
                if (((b & (1 << 2)) >> 2) == 0)
                {
                    button26.BackColor = Color.Transparent;
                }

                if (((b & (1 << 3)) >> 3) == 1)
                {
                    button27.BackColor = Color.Red;
                }
                if (((b & (1 << 3)) >> 3) == 0)
                {
                    button27.BackColor = Color.Transparent;
                }

                if (((b & (1 << 4)) >> 4) == 1)
                {
                    button28.BackColor = Color.Red;
                }
                if (((b & (1 << 4)) >> 4) == 0)
                {
                    button28.BackColor = Color.Transparent;
                }

                if (((b & (1 << 5)) >> 5) == 1)
                {
                    button29.BackColor = Color.Red;
                }
                if (((b & (1 << 5)) >> 5) == 0)
                {
                    button29.BackColor = Color.Transparent;
                }

                if (((b & (1 << 6)) >> 6) == 1)
                {
                    button32.BackColor = Color.Red;
                }
                if (((b & (1 << 6)) >> 6) == 0)
                {
                    button32.BackColor = Color.Transparent;
                }

                if (((b & (1 << 7)) >> 7) == 1)
                {
                    button34.BackColor = Color.Red;
                }
                if (((b & (1 << 7)) >> 7) == 0)
                {
                    button34.BackColor = Color.Transparent;
                }

                //**************************************
                if ((c & 1) == 1)
                {
                    button36.BackColor = Color.Red;
                }
                if ((c & 1) == 0)
                {
                    button36.BackColor = Color.Transparent;
                }

                if (((c & (1 << 1)) >> 1) == 1)
                {
                    button38.BackColor = Color.Red;
                }
                if (((c & (1 << 1)) >> 1) == 0)
                {
                    button38.BackColor = Color.Transparent;
                }

                if (((c & (1 << 2)) >> 2) == 1)
                {
                    button30.BackColor = Color.Red;
                }
                if (((c & (1 << 2)) >> 2) == 0)
                {
                    button30.BackColor = Color.Transparent;
                }

                if (((c & (1 << 3)) >> 3) == 1)
                {
                    button31.BackColor = Color.Red;
                }
                if (((c & (1 << 3)) >> 3) == 0)
                {
                    button31.BackColor = Color.Transparent;
                }

                if (((c & (1 << 4)) >> 4) == 1)
                {
                    button33.BackColor = Color.Red;
                }
                if (((c & (1 << 4)) >> 4) == 0)
                {
                    button33.BackColor = Color.Transparent;
                }

                if (((c & (1 << 5)) >> 5) == 1)
                {
                    button35.BackColor = Color.Red;
                }
                if (((c & (1 << 5)) >> 5) == 0)
                {
                    button35.BackColor = Color.Transparent;
                }

                if (((c & (1 << 6)) >> 6) == 1)
                {
                    button37.BackColor = Color.Red;
                }
                if (((c & (1 << 6)) >> 6) == 0)
                {
                    button37.BackColor = Color.Transparent;
                }

                if (((c & (1 << 7)) >> 7) == 1)
                {
                    button39.BackColor = Color.Red;
                }
                if (((c & (1 << 7)) >> 7) == 0)
                {
                    button39.BackColor = Color.Transparent;
                }
        }

        private void OUT_btn_Click(object sender, EventArgs e)
        {
            IN_btn.BackColor = Color.FromArgb(237, 236, 236);
            OUT_btn.BackColor = Color.FromArgb(118, 153, 223);
            panelOUT.Visible = true;
            panelIN.Visible = false;
        }

        /// <summary>
        /// 下发指令设置各个IO输出灯的状态（红灯置位，灰色清零）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if(button3.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x00;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button3.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x00;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button3.BackColor = Color.Transparent;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x01;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button4.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x01;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button4.BackColor = Color.Transparent;
            }
        }
     
        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x02;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button5.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x02;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button5.BackColor = Color.Transparent;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x03;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button6.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x03;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button6.BackColor = Color.Transparent;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (button7.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x04;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button7.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x04;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button7.BackColor = Color.Transparent;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (button8.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x05;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button8.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x05;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button8.BackColor = Color.Transparent;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (button9.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x06;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button9.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x06;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button9.BackColor = Color.Transparent;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (button10.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x07;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button10.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x07;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button10.BackColor = Color.Transparent;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (button11.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x08;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button11.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x08;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button11.BackColor = Color.Transparent;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (button12.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x09;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button12.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x09;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button12.BackColor = Color.Transparent;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (button13.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0A;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button13.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0A;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button13.BackColor = Color.Transparent;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (button14.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0B;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button14.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0B;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button14.BackColor = Color.Transparent;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (button15.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0C;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button15.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0C;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button15.BackColor = Color.Transparent;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (button16.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0D;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button16.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0D;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button16.BackColor = Color.Transparent;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (button17.BackColor == Color.Transparent)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0E;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button17.BackColor = Color.Red;
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0C;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0x0E;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
                button17.BackColor = Color.Transparent;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void panel24_Paint(object sender, PaintEventArgs e)
        {
                    }

        private void panel21_Paint(object sender, PaintEventArgs e)
        {
                    }

        private void panel18_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel23_Paint(object sender, PaintEventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel24_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void panel21_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panelOUT_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel32_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel31_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
