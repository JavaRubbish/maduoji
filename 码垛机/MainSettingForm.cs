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
        public static bool flag = false;
        public MainSettingForm()
        {
            InitializeComponent();


            HomeForm.xinlei = false;
            if(INIhelp.GetValue("当前用户") != "管理员权限")
            {
                flag = true;
            }
            else
            {
                flag = false;
            }           
        }

       
        /// <summary>
        /// 打开设置界面，根据权限设置是否禁用
        /// </summary>
        public static UserSettingForm usf = null;
        public static RuanxianweiForm rxf = null;
        //public static HandSettingForm hsf = null;
        public static IOSettingForm iof = null;
        public AboutUsForm auf = null;
        public USBForm usbf = null;

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
            if(flag)
            {
                MessageBox.Show("没有权限","警告");
                return;
            }            
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
            if (flag)
            {
                MessageBox.Show("没有权限", "警告");
                return;
            }

            //hsf = new HandSettingForm();
            //HomeForm.hsf.Show();
            this.Parent.FindForm().Hide();
            this.Hide();
            if (HomeForm.hsf.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }
        }

        private void ioset_btn_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                MessageBox.Show("没有权限", "警告");
                return;
            }

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

        private void Upan_btn_Click(object sender, EventArgs e)
        {
            usbf = new USBForm();
            this.Parent.FindForm().Hide();
            this.Hide();

            if (usbf.ShowDialog() == DialogResult.OK)
            {
                this.Parent.FindForm().Show();
                this.Show();
            }
        }
    }
}
