using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 码垛机.WebServiceWriteSapRq;

namespace 码垛机
{
    public partial class PrintForm : Form
    {

        //失败重发标志位
        public static bool resend = false;


        public static List<string> serialNo;
        public static List<string> orderNo;
        public static List<string> materialNo;

        //public static string[] serialNo;
        //public static string[] orderNo;
        //public static string[] materialNo;

        public static int countdown = 300;
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


        //插入条码到表格
        public void InsertIntoTable(string str1, string str2)
        {
            CheckForIllegalCrossThreadCalls = false;
            DateTime dt = DateTime.Now;
            //写进日文件夹
            string fname1 = "todayUSBdata\\" + dt.ToString("yyyy-MM-dd") + "his.txt";                 
           
            int index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value = str1;
            this.dataGridView1.Rows[index].Cells[1].Value = str2;
            WriteToDisk2(fname1);

        }

        //把数据写进项目文件夹
        public void WriteToDisk2(string fname)
        {

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, false, System.Text.Encoding.GetEncoding("gb2312"));
            try
            {
                int len = 0;
                string line = "";
                string temp = "";
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    temp = (string)dataGridView1.Columns[i].HeaderCell.Value;
                    len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length; //考虑中英文的情况
                    temp = temp.PadRight(len, ' ');
                    line += temp;
                }
                sw.WriteLine(line);
                line = "";
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        temp = (string)dataGridView1.Rows[i].Cells[j].Value;
                        len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length;
                        temp = temp.PadRight(len, ' ');
                        line += temp;
                    }
                    sw.WriteLine(line);
                    line = "";
                }
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        public void ReadFromFile(string filepath)
        {
            //先清空每一行数据，否则多个日期的数据都会写到一个表里
            dataGridView1.Rows.Clear();
            //string fname = "C:\\Users\\John\\source\\repos\\码垛机\\码垛机\\bin\\Debug\\alarmhis.txt";
            if (!System.IO.File.Exists(filepath))
            {
                //label4.Text = totalNum().ToString();
                return;
            }
            StreamReader sr = new StreamReader(filepath, Encoding.Default);
            string vline;
            while ((vline = sr.ReadLine()) != null)
            {
                string[] vitems = vline.Split(new string[] { " ", "时间", "条码"}, StringSplitOptions.RemoveEmptyEntries);

                if (vitems.Length <= 0)
                {
                    continue;
                }
                for (int i = 0; i < vitems.Length / 2; i++)
                {
                    int index = this.dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = vitems[2 * i];
                    dataGridView1.Rows[index].Cells[1].Value = vitems[2 * i + 1];
                }
               // label4.Text = totalNum().ToString();
            }
            sr.Close();//读完一定要关闭流，否则会和上面的写进程冲突

        }

        /*********************************************************************************/
        //插入条码到表格
        public void InsertIntoTable2(string str1, string str2,string str3)
        {
            CheckForIllegalCrossThreadCalls = false;
            //写进日文件夹
            string fname1 = "toDB\\sapdata.txt";

            int index = this.dataGridView2.Rows.Add();
            this.dataGridView2.Rows[index].Cells[0].Value = str1;
            this.dataGridView2.Rows[index].Cells[1].Value = str2;
            this.dataGridView2.Rows[index].Cells[2].Value = str3;
            WriteToDisk3(fname1);

        }

        //把数据写进项目文件夹
        public void WriteToDisk3(string fname)
        {

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, false, System.Text.Encoding.GetEncoding("gb2312"));
            try
            {
                int len = 0;
                string line = "";
                string temp = "";
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    temp = (string)dataGridView2.Columns[i].HeaderCell.Value;
                    len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length; //考虑中英文的情况
                    temp = temp.PadRight(len, ' ');
                    line += temp;
                }
                sw.WriteLine(line);
                line = "";
                for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < dataGridView2.Columns.Count; j++)
                    {
                        temp = (string)dataGridView2.Rows[i].Cells[j].Value;
                        len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length;
                        temp = temp.PadRight(len, ' ');
                        line += temp;
                    }
                    sw.WriteLine(line);
                    line = "";
                }
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        public void ReadFromFile2(string filepath)
        {
            dataGridView2.Rows.Clear();
            if (!System.IO.File.Exists(filepath))
            {               
                return;
            }
            StreamReader sr = new StreamReader(filepath, Encoding.Default);
            string vline;
            while ((vline = sr.ReadLine()) != null)
            {
                string[] vitems = vline.Split(new string[] { " ", "序列号", "订单号","物料编号" }, StringSplitOptions.RemoveEmptyEntries);

                if (vitems.Length <= 0)
                {
                    continue;
                }
                for (int i = 0; i < vitems.Length / 3; i++)
                {
                    int index = this.dataGridView2.Rows.Add();
                    dataGridView2.Rows[index].Cells[0].Value = vitems[3 * i];
                    dataGridView2.Rows[index].Cells[1].Value = vitems[3 * i + 1];
                    dataGridView2.Rows[index].Cells[2].Value = vitems[3 * i + 2];
                }
            }
            sr.Close();

        }


        /*******************************************************************************************************************/
        public void DivideDBDataAndSendToRq(List<string> str1, List<string> str2, List<string> str3)
        {
            string filename = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\toDB\\sapdata.txt";
            if (!System.IO.File.Exists(filename))
            {
                return;
            }

            str1.Clear();
            str2.Clear();
            str3.Clear();


            StreamReader sr = new StreamReader(filename, Encoding.Default);
            string vline;
            int strNum = 0;
            while ((vline = sr.ReadLine()) != null)
            {
                string[] vitems = vline.Split(new string[] { " ", "序列号", "订单号", "物料编号" }, StringSplitOptions.RemoveEmptyEntries);

                if (vitems.Length <= 0)
                {
                    continue;
                }
               
                for (int i = 0; i < vitems.Length / 3; i++)
                {
                    str1.Add(vitems[3 * i]);
                    str2.Add(vitems[3 * i + 1]);
                    str3.Add(vitems[3 * i + 2]);
                    //str1[strNum] = vitems[3 * i];
                    //str2[strNum] = vitems[3 * i + 1];
                    //str3[strNum] = vitems[3 * i + 2];
                }
                strNum++;  //tmny  20200305 修改测试数据
            }
            sr.Close();

            
            //调用写数据库接口
            SapRetInfo sri = WebServiceWriteSapRq.WritePackinfoToSap(str1, str2, str3);
            if(null == sri)
            {
                Logger.WriteLogs("Logs", "第一次调用", "传入空数组");
                return;
            }
            serialNo = str1;
            orderNo = str2;
            materialNo = str3;
            if (sri.Retmsg == "成功")
            {
                Logger.WriteLogs("Logs", "第一次调用", sri.Retmsg);
                countdown = 300;
                resend = false;
                //删除文件
                System.IO.File.WriteAllText("C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\toDB\\sapdata.txt", string.Empty);
            }
            else
            {
                Logger.WriteLogs("Logs", "第一次调用", sri.Retmsg);
                resend = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(resend)
            {
                countdown--;
                if (countdown == 0)
                {
                    SapRetInfo sri = WebServiceWriteSapRq.WritePackinfoToSap(serialNo, orderNo, materialNo);
                    if (null == sri)
                    {
                        Logger.WriteLogs("Logs", "第二次调用", "传入空数组");
                        return;
                    }
                    if (sri.Retmsg == "成功")
                    {
                        Logger.WriteLogs("Logs","第二次调用",sri.Retmsg);
                        countdown = 300;
                        resend = false;
                        //删除文件
                        System.IO.File.WriteAllText("C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\toDB\\sapdata.txt", string.Empty);
                    }
                    else
                    {
                        Logger.WriteLogs("Logs", "第二次调用", sri.Retmsg);
                        MessageBox.Show("请手动同步!");
                    } 
                }              
            }
        }
    }
}
