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
    public partial class MainSettingForm : Form
    {
        public MainSettingForm()
        {
            InitializeComponent();
            
        }

        public static UserSettingForm usf = null;
        public static RuanxianweiForm rxf = null;
        public HandSettingForm hsf = null;
        public IOSettingForm iof = null;
        public AboutUsForm auf = null;
        private void userset_btn_Click(object sender, EventArgs e)
        {
            usf = new UserSettingForm();
            this.Parent.FindForm().Hide();
            this.Hide();
            if(usf.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }
        }

        private void ruanxianweiset_btn_Click(object sender, EventArgs e)
        {
            
            rxf = new RuanxianweiForm();
            this.Parent.FindForm().Hide();
            this.Hide();
            if (rxf.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }
        }

        private void handset_btn_Click(object sender, EventArgs e)
        {
            hsf = new HandSettingForm();
            this.Parent.FindForm().Hide();
            this.Hide();
            if (hsf.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }
        }

        private void ioset_btn_Click(object sender, EventArgs e)
        {
            iof = new IOSettingForm();
            this.Parent.FindForm().Hide();
            this.Hide();

            if (iof.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }

           
        }

        private void aboutus_btn_Click(object sender, EventArgs e)
        {
            auf = new AboutUsForm();
            this.Parent.FindForm().Hide();
            this.Hide();

            if (auf.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }

        }
        //private void closingWindow()
        //{
        //    if(usf != null || (!usf.IsDisposed))
        //    {
        //        this.Close();
        //    }
        //}
    }
}
