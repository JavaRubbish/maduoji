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
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);
            InitializeComponent();
            AutoScale(this);

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

            this.userset_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.handset_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.ruanxianweiset_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.ioset_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.Upan_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.aboutus_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
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

        private void MainSettingForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
