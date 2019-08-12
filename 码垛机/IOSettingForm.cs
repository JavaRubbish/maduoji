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

            //System.Drawing.Drawing2D.GraphicsPath path1 = new System.Drawing.Drawing2D.GraphicsPath();
            //path1.AddEllipse(this.button5.ClientRectangle);
            //this.button5.Region = new Region(path1);
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
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void IOSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void panelOUT_Paint(object sender, PaintEventArgs e)
        {

        }

        private void IN_btn_Click(object sender, EventArgs e)
        {
            IN_btn.BackColor = Color.FromArgb(118, 153, 223);
            OUT_btn.BackColor = Color.FromArgb(237, 236, 236);
            panelIN.Visible = true;
            panelOUT.Visible = false;
        }

        private void OUT_btn_Click(object sender, EventArgs e)
        {
            IN_btn.BackColor = Color.FromArgb(237, 236, 236);
            OUT_btn.BackColor = Color.FromArgb(118, 153, 223);
            panelOUT.Visible = true;
            panelIN.Visible = false;
        }

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

        private void button31_Click(object sender, EventArgs e)
        {

        }

        private void button29_Click(object sender, EventArgs e)
        {

        }
    }
}
