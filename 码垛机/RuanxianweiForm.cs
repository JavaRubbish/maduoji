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
    public partial class RuanxianweiForm : Form
    {
        public RuanxianweiForm()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);
            InitializeComponent();
            AutoScale(this);
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

            this.button1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.ret_btn2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

            this.软限位设定.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label3.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label5.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label7.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label8.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

            this.textBox1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox5.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox6.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox7.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox8.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.textBox9.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

            this.comboBox1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.comboBox2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.comboBox3.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.comboBox4.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
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

            //X轴下限
            string str2 = textBox4.Text;
            if (str2 == "")
            {
                return;
            }
            int id2 = Convert.ToInt32(str2);
            byte[] iByte2 = toBytes.intToBytes(id2);//4位

            //Y轴上限
            string str5 = textBox8.Text;
            if (str5 == "")
            {
                return;
            }
            int id5 = Convert.ToInt32(str5);
            byte[] iByte5 = toBytes.intToBytes(id5);//4位

            //Y轴下限
            string str6 = textBox6.Text;
            if (str6 == "")
            {
                return;
            }
            int id6 = Convert.ToInt32(str6);
            byte[] iByte6 = toBytes.intToBytes(id6);//4位

            //Z轴上限

            string str3 = textBox2.Text;
            if (str3 == "")
            {
                return;
            }
            int id3 = Convert.ToInt32(str3);
            byte[] iByte3 = toBytes.intToBytes(id3);//4位

            //Z轴下限
            string str4 = textBox5.Text;
            if (str4 == "")
            {
                return;
            }
            int id4 = Convert.ToInt32(str4);
            byte[] iByte4 = toBytes.intToBytes(id4);//4位

            //O轴上限
            string str7 = textBox9.Text;
            if (str7 == "")
            {
                return;
            }
            int id7 = Convert.ToInt32(str7);
            byte[] iByte7 = toBytes.intToBytes(id7);//4位


            //O轴下限
            string str8 = textBox7.Text;
            if (str8 == "")
            {
                return;
            }
            int id8 = Convert.ToInt32(str8);
            byte[] iByte8 = toBytes.intToBytes(id8);//4位


            

            //将四轴的开关状态写进一个字节发送出去
            string[] status = new string[4];
            try
            {
                if (comboBox2.Text.Equals("开"))
                {
                    status[0] = "1";
                }
                if (comboBox2.Text.Equals("关"))
                {
                    status[0] = "0";
                }
                if (comboBox1.Text.Equals("开"))
                {
                    status[1] = "1";
                }
                if (comboBox1.Text.Equals("关"))
                {
                    status[1] = "0";
                }
                if (comboBox3.Text.Equals("开"))
                {
                    status[2] = "1";
                }
                if (comboBox3.Text.Equals("关"))
                {
                    status[2] = "0";
                }
                if (comboBox4.Text.Equals("开"))
                {
                    status[3] = "1";
                }
                if (comboBox4.Text.Equals("关"))
                {
                    status[3] = "0";
                }
            }
            catch
            {

            }
            string status2 = string.Join("",status);
            int a = Convert.ToInt32(status2, 2);
            byte[] b = toBytes.intToBytes(a);

            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x07;
            BF.sendbuf[2] = 0x0B;
            BF.sendbuf[3] = iByte1[3];
            BF.sendbuf[4] = iByte1[2];
            BF.sendbuf[5] = iByte1[1];
            BF.sendbuf[6] = iByte1[0];
            BF.sendbuf[7] = iByte2[3];
            BF.sendbuf[8] = iByte2[2];
            BF.sendbuf[9] = iByte2[1];
            BF.sendbuf[10] = iByte2[0];
            BF.sendbuf[11] = iByte5[3];
            BF.sendbuf[12] = iByte5[2];
            BF.sendbuf[13] = iByte5[1];
            BF.sendbuf[14] = iByte5[0];
            BF.sendbuf[15] = iByte6[3];
            BF.sendbuf[16] = iByte6[2];
            BF.sendbuf[17] = iByte6[1];
            BF.sendbuf[18] = iByte6[0];
            BF.sendbuf[19] = iByte3[3];
            BF.sendbuf[20] = iByte3[2];
            BF.sendbuf[21] = iByte3[1];
            BF.sendbuf[22] = iByte3[0];
            BF.sendbuf[23] = iByte4[3];
            BF.sendbuf[24] = iByte4[2];
            BF.sendbuf[25] = iByte4[1];
            BF.sendbuf[26] = iByte4[0];
            BF.sendbuf[27] = iByte7[3];
            BF.sendbuf[28] = iByte7[2];
            BF.sendbuf[29] = iByte7[1];
            BF.sendbuf[30] = iByte7[0];
            BF.sendbuf[31] = iByte8[3];
            BF.sendbuf[32] = iByte8[2];
            BF.sendbuf[33] = iByte8[1];
            BF.sendbuf[34] = iByte8[0];
            BF.sendbuf[35] = b[0];
            BF.sendbuf[36] = 0xF5;
            SendMenuCommand(BF.sendbuf, 37);

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

        private void RuanxianweiForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
