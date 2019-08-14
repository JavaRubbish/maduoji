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

            //加载上次保存的用户设置
            comboBox1.Text = INIhelp.GetValue("当前用户");
            textBox1.Text = INIhelp.GetValue("背光时间");
            dateTimePicker2.Text = INIhelp.GetValue("时钟设置");
            textBox2.Text = INIhelp.GetValue("修改密码");
            comboBox2.Text = INIhelp.GetValue("蜂鸣器");
            textBox3.Text = INIhelp.GetValue("修改加密锁");
        }

        public void setTextBox1Text(string str)
        {
            CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = str;
        }

        private void ret_btn3_Click(object sender, EventArgs e)
        {
            //点击返回保存用户设置
            INIhelp.SetValue("当前用户",comboBox1.Text);
            INIhelp.SetValue("背光时间",textBox1.Text);
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
