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
    public partial class AboutUsForm : Form
    {
        public AboutUsForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ret_btn4_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void AboutUsForm_Load(object sender, EventArgs e)
        {

        }

        private void AboutUsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
