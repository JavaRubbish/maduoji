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

namespace 码垛机
{
    public partial class IOSettingForm : Form
    {
        public IOSettingForm()
        {
            InitializeComponent();

            getStatus();
        }

        private void IOSettingForm_Load(object sender, EventArgs e)
        {
            // this.FormBorderStyle = FormBorderStyle.None;
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(5, 5, 50, 50);
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
            this.button18.Region = new Region(myPath);
            this.button19.Region = new Region(myPath);
            this.button20.Region = new Region(myPath);
            this.button21.Region = new Region(myPath);
            this.button22.Region = new Region(myPath);
            this.button23.Region = new Region(myPath);
            this.button24.Region = new Region(myPath);
            this.button25.Region = new Region(myPath);
            this.button26.Region = new Region(myPath);
            this.button27.Region = new Region(myPath);
            this.button28.Region = new Region(myPath);
            this.button29.Region = new Region(myPath);
            this.button30.Region = new Region(myPath);
            this.button31.Region = new Region(myPath);
            this.button32.Region = new Region(myPath);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ret_btn1_Click(object sender, EventArgs e)
        {
            saveStatus();

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

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x0C;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);
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

    }
}
