using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 码垛机.HomeForm;

namespace 码垛机
{
    public partial class RuanxianweiForm : Form
    {
        public RuanxianweiForm()
        {
            InitializeComponent();
            //加载软限位设置(读上次保存)
            textBox1.Text = INIhelp.GetValue("X轴上限位");
            textBox4.Text = INIhelp.GetValue("X轴下限位");
            comboBox2.Text = INIhelp.GetValue("X轴开关");
            textBox2.Text = INIhelp.GetValue("Z轴上限位");
            textBox5.Text = INIhelp.GetValue("Z轴下限位");
            comboBox1.Text = INIhelp.GetValue("Z轴开关");
            textBox8.Text = INIhelp.GetValue("Y轴上限位");
            textBox6.Text = INIhelp.GetValue("Y轴下限位");
            comboBox3.Text = INIhelp.GetValue("Y轴开关");
            textBox9.Text = INIhelp.GetValue("O轴上限位");
            textBox7.Text = INIhelp.GetValue("O轴下限位");
            comboBox4.Text = INIhelp.GetValue("O轴开关");
        }

  
        /// <summary>
        /// 保存设置(下发指令并且设置保存)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //统统往下丢指令
            //X轴上限
            string str1 = textBox1.Text;
            if (str1 == "")
            {
                return;
            }
            int id1 = Convert.ToInt32(str1);
            byte[] iByte1 = toBytes.intToBytes(id1);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = iByte1[0];
            BF.sendbuf[6] = iByte1[1];
            BF.sendbuf[7] = iByte1[2];
            BF.sendbuf[8] = iByte1[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //X轴下限
            string str2 = textBox4.Text;
            if (str2 == "")
            {
                return;
            }
            int id2 = Convert.ToInt32(str2);
            byte[] iByte2 = toBytes.intToBytes(id2);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x01;
            BF.sendbuf[5] = iByte2[0];
            BF.sendbuf[6] = iByte2[1];
            BF.sendbuf[7] = iByte2[2];
            BF.sendbuf[8] = iByte2[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //X轴开关
            if (comboBox2.Text.Equals("开"))
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x01;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x01;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }


            //Z轴上限

            string str3 = textBox2.Text;
            if (str3 == "")
            {
                return;
            }
            int id3 = Convert.ToInt32(str3);
            byte[] iByte3 = toBytes.intToBytes(id3);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = iByte3[0];
            BF.sendbuf[6] = iByte3[1];
            BF.sendbuf[7] = iByte3[2];
            BF.sendbuf[8] = iByte3[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //Z轴下限
            string str4 = textBox5.Text;
            if (str4 == "")
            {
                return;
            }
            int id4 = Convert.ToInt32(str4);
            byte[] iByte4 = toBytes.intToBytes(id4);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x03;
            BF.sendbuf[5] = iByte4[0];
            BF.sendbuf[6] = iByte4[1];
            BF.sendbuf[7] = iByte4[2];
            BF.sendbuf[8] = iByte4[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //Z轴开关
            if (comboBox1.Text.Equals("开"))
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x03;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x03;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }


            //Y轴上限
            string str5 = textBox8.Text;
            if (str5 == "")
            {
                return;
            }
            int id5 = Convert.ToInt32(str5);
            byte[] iByte5 = toBytes.intToBytes(id5);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = iByte5[0];
            BF.sendbuf[6] = iByte5[1];
            BF.sendbuf[7] = iByte5[2];
            BF.sendbuf[8] = iByte5[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //Y轴下限
            string str6 = textBox6.Text;
            if (str6 == "")
            {
                return;
            }
            int id6 = Convert.ToInt32(str6);
            byte[] iByte6 = toBytes.intToBytes(id6);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x02;
            BF.sendbuf[5] = iByte6[0];
            BF.sendbuf[6] = iByte6[1];
            BF.sendbuf[7] = iByte6[2];
            BF.sendbuf[8] = iByte6[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //Y轴开关
            if (comboBox3.Text.Equals("开"))
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x02;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x02;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }

            //O轴上限
            string str7 = textBox9.Text;
            if(str7 == "")
            {
                return;
            }
            int id7 = Convert.ToInt32(str7);
            byte[] iByte7 = toBytes.intToBytes(id7);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x02;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = iByte7[0];
            BF.sendbuf[6] = iByte7[1];
            BF.sendbuf[7] = iByte7[2];
            BF.sendbuf[8] = iByte7[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //O轴下限
            string str8 = textBox7.Text;
            if(str8 == "")
            {
                return;
            }
            int id8 = Convert.ToInt32(str8);
            byte[] iByte8 = toBytes.intToBytes(id8);//4位
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x04;
            BF.sendbuf[5] = iByte8[0];
            BF.sendbuf[6] = iByte8[1];
            BF.sendbuf[7] = iByte8[2];
            BF.sendbuf[8] = iByte8[3];
            BF.sendbuf[9] = 0xF5;
            SendMenuCommand(BF.sendbuf, 10);

            //O轴开关
            if (comboBox4.Text.Equals("开"))
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x04;
                BF.sendbuf[5] = 0x01;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }
            else
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x04;
                BF.sendbuf[2] = 0x0B;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0x04;
                BF.sendbuf[5] = 0x02;
                BF.sendbuf[6] = 0xF5;
                SendMenuCommand(BF.sendbuf, 7);
            }



            //更新到配置文件里
            INIhelp.SetValue("X轴上限位",textBox1.Text);
            INIhelp.SetValue("X轴下限位", textBox4.Text);
            INIhelp.SetValue("X轴开关", comboBox2.Text);
            INIhelp.SetValue("Z轴上限位", textBox2.Text);
            INIhelp.SetValue("Z轴下限位", textBox5.Text);
            INIhelp.SetValue("Z轴开关", comboBox1.Text);
            INIhelp.SetValue("Y轴上限位", textBox8.Text);
            INIhelp.SetValue("Y轴下限位", textBox6.Text);
            INIhelp.SetValue("Y轴开关", comboBox3.Text);
            INIhelp.SetValue("O轴上限位", textBox9.Text);
            INIhelp.SetValue("O轴下限位", textBox7.Text);
            INIhelp.SetValue("O轴开关", comboBox4.Text);

            ////关闭窗口
            //this.Close();
            //this.DialogResult = DialogResult.OK;
        }

        private void ret_btn2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void RuanxianweiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
