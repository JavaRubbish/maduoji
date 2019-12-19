using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 码垛机
{
    public partial class PrintForm : Form
    {
        public PrintForm()
        {
            InitializeComponent();
        }

        //打印
        public  void print_list()
        {
            CheckForIllegalCrossThreadCalls = false;
            //实例化打印对象
            PrintDocument printDocument1 = new PrintDocument();
            //设置打印用的纸张,当设置为Custom的时候，可以自定义纸张的大小
            printDocument1.DefaultPageSettings.PaperSize = new PaperSize("Custum", 500, 500);
            //注册PrintPage事件，打印每一页时会触发该事件
            printDocument1.PrintPage += new PrintPageEventHandler(this.PrintDocument_PrintPage1);
            //开始打印
            printDocument1.Print();
        }

        public void PrintDocument_PrintPage1(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //设置打印内容及其字体，颜色和位置
            e.Graphics.DrawString(textBox1.Text, new Font(new FontFamily("黑体"), 24), System.Drawing.Brushes.Red, 50, 50);
        }

        public  void changeTextBoxText(ArrayList list)
        {
            CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = "";
            for (int i = 0;i < list.Count/3;i++)
            {
                textBox1.AppendText((string)list[3 * i]);
                textBox1.AppendText((string)list[3 * i + 1]);
                textBox1.AppendText((string)list[3 * i + 2]);
                textBox1.AppendText("\r\n");
            }
        }
    }
}
