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
    public partial class UserSettingForm : Form
    {

        public UserSettingForm()
        {
            InitializeComponent();

            AutoScale(this);
            //加载上次保存的用户设置
            comboBox1.Text = INIhelp.GetValue("当前用户");
            textBox1.Text = INIhelp.GetValue("背光时间");
            dateTimePicker2.Text = INIhelp.GetValue("时钟设置");
            textBox2.Text = INIhelp.GetValue("修改密码");
            comboBox2.Text = INIhelp.GetValue("蜂鸣器");
            textBox3.Text = INIhelp.GetValue("修改加密锁");
        }

        public static void AutoScale(Form frm)
        {
            frm.Tag = frm.Width.ToString() + "," + frm.Height.ToString();
            frm.SizeChanged += new EventHandler(frm_SizeChanged);
        }

        static void frm_SizeChanged(object sender, EventArgs e)
        {
            string[] tmp = ((Form)sender).Tag.ToString().Split(',');
            float width = (float)((Form)sender).Width / (float)Convert.ToInt16(tmp[0]);
            float heigth = (float)((Form)sender).Height / (float)Convert.ToInt16(tmp[1]);

            ((Form)sender).Tag = ((Form)sender).Width.ToString() + "," + ((Form)sender).Height;

            foreach (Control control in ((Form)sender).Controls)
            {
                control.Scale(new SizeF(width, heigth));
                //control.Font = new Font(control.Font.FontFamily,12F * width,control.Font.Style);
                // control.Font = new System.Drawing.Font("黑体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            }
        }


        public void setTextBox1Text(string str)
        {
            CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = str;
        }

        private void ret_btn3_Click(object sender, EventArgs e)
        {
            //点击返回保存用户设置
            INIhelp.SetValue("当前用户", comboBox1.Text);
            INIhelp.SetValue("背光时间", textBox1.Text);
            INIhelp.SetValue("时钟设置", dateTimePicker2.Text);
            INIhelp.SetValue("修改密码", textBox2.Text);
            INIhelp.SetValue("蜂鸣器", comboBox2.Text);
            INIhelp.SetValue("修改加密锁", textBox3.Text);

            if (INIhelp.GetValue("当前用户") != "管理员权限")
            {
                MainSettingForm.flag = true;
            }
            else
            {
                MainSettingForm.flag = false;
            }

            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void UserSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
       
    }
}
