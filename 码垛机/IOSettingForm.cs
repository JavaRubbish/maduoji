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
            myPath.AddEllipse(0, 10, 50, 50);
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

        private void button3_Click(object sender, EventArgs e)
        {

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
            OUT_btn.BackColor = Color.FromArgb(237,236,236);
            panelIN.Visible = true;
            panelOUT.Visible = false;
        }

        private void OUT_btn_Click(object sender, EventArgs e)
        {
            IN_btn.BackColor = Color.FromArgb(237, 236, 236);
            OUT_btn.BackColor = Color.FromArgb(118, 153, 223);
            panelIN.Visible = false;
            panelOUT.Visible = true;
        }
    }
}
