using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 码垛机
{
    public partial class HomeForm : Form
    {

        public static SerialPort sp = new SerialPort();
        private StringBuilder builder = new StringBuilder();

        public static class BF
        {
            public static byte[] sendbuf = new byte[50];
        }



        public HomeForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //默认使用端口一，波特率9600
            initCommPara("COM1", 9600);
   
            //初始化加载工作界面
            work_btn.BackColor = Color.FromArgb(65, 105, 225);
            wdf = new WorkingDetailForm();
            wdf.TopLevel = false;
            panel1.Controls.Add(wdf);
            wdf.Show();
        }
        /// <summary>
        /// 解决重复打开子窗口问题
        /// </summary>
        public HistoryDataForm hf = null;
        public MainSettingForm msf = null;
        public static AlarmHistoryForm ahf = null;
        public WorkingDetailForm wdf = null;
       // public UserSettingForm usf = null;

        /// <summary>
        /// 历史数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historydata_btn_Click(object sender, EventArgs e)
        {
            this.Text = "历史数据";
            work_btn.BackColor = Color.FromArgb(220, 220, 220);
            historydata_btn.BackColor = Color.FromArgb(65, 105, 225);
            setting_btn.BackColor = Color.FromArgb(220, 220, 220);
            alarmhistory_btn.BackColor = Color.FromArgb(220, 220, 220);

            if (msf != null)
            {
                msf.Close();
            }
            if (ahf != null)
            {
                ahf.Close();
            }
            if (wdf != null)
            {
                wdf.Close();
            }
            //判断该子窗口是否已经打开过
            if (hf == null || hf.IsDisposed)
            {
                //this.IsMdiContainer = true;
                hf = new HistoryDataForm();
                hf.TopLevel = false;//设置为非顶级窗口
                //hf.FormBorderStyle = FormBorderStyle.None;//窗体为非边框样式
                panel1.Controls.Add(hf);
                hf.Show();
            }
            else
            {

            }
        }
        /// <summary>
        /// 设置调试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setting_btn_Click(object sender, EventArgs e)
        {
            this.Text = "设置调试";

            work_btn.BackColor = Color.FromArgb(220, 220, 220);
            historydata_btn.BackColor = Color.FromArgb(220, 220, 220);
            setting_btn.BackColor = Color.FromArgb(65, 105, 225);
            alarmhistory_btn.BackColor = Color.FromArgb(220, 220, 220);

            if (hf != null)
            {
                hf.Close();
            }
            if (ahf != null)
            {
                ahf.Close();
            }
            if (wdf != null)
            {
                wdf.Close();
            }

            if (msf == null || msf.IsDisposed)
            {

                //this.IsMdiContainer = true;
             
                msf = new MainSettingForm();
                msf.TopLevel = false;
                //msf.FormBorderStyle = FormBorderStyle.None;
                panel1.Controls.Add(msf);
                msf.Show();
            }
            else
            {

            }
        }
        /// <summary>
        /// 报警历史
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alarmhistory_btn_Click(object sender, EventArgs e)
        {
            this.Text = "报警历史";

            //发送指令查看报警历史
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x02;
            BF.sendbuf[2] = 0x0E;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0xF5;
            SendMenuCommand(BF.sendbuf, 5);

            work_btn.BackColor = Color.FromArgb(220, 220, 220);
            historydata_btn.BackColor = Color.FromArgb(220, 220, 220);
            setting_btn.BackColor = Color.FromArgb(220, 220, 220);
            alarmhistory_btn.BackColor = Color.FromArgb(65, 105, 225);

            if (hf != null)
            {
                hf.Close();
            }
            if (msf != null)
            {
                msf.Close();
            }
            if (wdf != null)
            {
                wdf.Close();
            }

            if (ahf == null || ahf.IsDisposed)
            {

                //this.IsMdiContainer = true;

                ahf = new AlarmHistoryForm();
                ahf.TopLevel = false;
                //msf.FormBorderStyle = FormBorderStyle.None;
                panel1.Controls.Add(ahf);
                ahf.Show();
            }
            else
            {

            }
        }
        /// <summary>
        /// 工作界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void work_btn_Click(object sender, EventArgs e)
        {
            this.Text = "工作界面";

            work_btn.BackColor = Color.FromArgb(65, 105, 225);
            historydata_btn.BackColor = Color.FromArgb(220,220,220);
            setting_btn.BackColor = Color.FromArgb(220, 220, 220);
            alarmhistory_btn.BackColor = Color.FromArgb(220, 220, 220);
            if (hf != null)
            {
                hf.Close();
            }
            if (ahf != null)
            {
                ahf.Close();
            }
            if (msf != null)
            {
                msf.Close();
            }

            if (wdf == null || wdf.IsDisposed)
            {

                //this.IsMdiContainer = true;

                wdf = new WorkingDetailForm();
                wdf.TopLevel = false;
                //msf.FormBorderStyle = FormBorderStyle.None;
                panel1.Controls.Add(wdf);
                wdf.Show();
            }
            else
            {

            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>
        /// 发送命令
        /// 解释：跨窗体调用，所以写成了静态方法，然后方法体内调用到的方法
        /// 同样要写成了静态方法  > 目前没找到好的解决办法！
        /// </summary>
        /// <param name="command"></param>
        /// <param name="len"></param>
        public static void SendMenuCommand(Byte[] command, int len)
        {
            try
            {
                    if (!sp.IsOpen)
                    {
                        initCommPara("COM1",9600);
                    }
                    sp.Write(command, 0, len);               
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("串口无法打开");
            }

        }

        /// <summary>
        /// 串口初始化
        /// </summary>
        /// <param name="comName"></param>
        /// <param name="baudRate"></param>
        public static void initCommPara(string comName,int baudRate)
        {
            try
            {
                //
                sp.PortName = comName;
                sp.BaudRate = baudRate;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.DataReceived += new SerialDataReceivedEventHandler(comm_DataReceived);
                sp.Open();

            }catch
            {
                MessageBox.Show("串口打开失败!", "系统提示");
            }
        }

        /// <summary>
        /// 接收数据并解析
        /// </summary>
        //private int SendStatus = 0;
      
        static void  comm_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
            byte[] binary_data_1 = new byte[50];      
            List<byte> buffer = new List<byte>(512);

            int n = sp.BytesToRead;
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            sp.Read(buf, 0, buf.Length);//读取缓冲数据
            /****************************************协议解析**********************************************/
            bool data_1_cached = false;
            buffer.AddRange(buf);//缓存数据
            while(buffer.Count >= 3)
            {
                if(buffer[0] == 0xFA)
                {
                    int len = buffer[1];//数据长度
                    if (buffer.Count < len + 3) break;//长度不够直接退出
                    buffer.CopyTo(0,binary_data_1,0,len + 3);//复制一条完整数据到具体的数据缓存
                    data_1_cached = true;
                    buffer.RemoveRange(0,len + 3);//正确分析一条数据，从缓存中移除数据

                }
                else
                {
                    buffer.RemoveAt(0);
                }
            }
            //分析数据(暂时测试用)
            if (data_1_cached)
            {
                string data1;
                int data2;
                //报警信息的处理
                //第三个数据用于判断是否是报警信息数据
                if(binary_data_1[2] == 0xAF)
                {
                    
                    string desc;
                    //将16进制字符串转为10进制整型数
                    data2 =Convert.ToInt32(binary_data_1[3].ToString("X2"),16);
                    if((data2 > 0) && (data2 <= 100)){
                         desc = "电机故障";
                    }
                    else if((data2 > 100) && (data2 <= 200))
                    {
                         desc = "机械手故障";
                    }
                    else
                    {
                        desc = "电路故障"; 
                    }
                    ahf.AddAlarmDataListViewItem(data2,desc);
                   return;
                }

                //普通数据的处理
                 data1 = binary_data_1[2].ToString("X2") + " " + binary_data_1[3].ToString("X2") + " " + binary_data_1[4].ToString("X2")
                    + " " + binary_data_1[5].ToString("X2") + " " + binary_data_1[6].ToString("X2") + " " + binary_data_1[7].ToString("X2");
                //  this.Invoke((EventHandler)(delegate{ }));
                MainSettingForm.usf.setTextBox1Text(data1);                
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}