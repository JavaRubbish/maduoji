using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
            public static byte[] sendbuf = new byte[100];
        }

        public HomeForm()
        {
            InitializeComponent();
            StartThread();
            searchAlarmInfo();
            initialIOSetting();
        }

        private void initialIOSetting()
        {
            INIhelp.SetValue(GlobalV.sts3, "0");
            INIhelp.SetValue(GlobalV.sts4, "0");
            INIhelp.SetValue(GlobalV.sts5, "0");
            INIhelp.SetValue(GlobalV.sts6, "0");
            INIhelp.SetValue(GlobalV.sts7, "0");
            INIhelp.SetValue(GlobalV.sts8, "0");
            INIhelp.SetValue(GlobalV.sts9, "0");
            INIhelp.SetValue(GlobalV.sts10, "0");
            INIhelp.SetValue(GlobalV.sts11, "0");
            INIhelp.SetValue(GlobalV.sts12, "0");
            INIhelp.SetValue(GlobalV.sts13, "0");
            INIhelp.SetValue(GlobalV.sts14, "0");
            INIhelp.SetValue(GlobalV.sts15, "0");
            INIhelp.SetValue(GlobalV.sts16, "0");
            INIhelp.SetValue(GlobalV.sts17, "0");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //默认使用端口一，波特率9600
            initCommPara("COM3", 9600);
   

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
        public static WorkingDetailForm wdf = null;
        
        //标志位，用于只在工作界面才发送坐标请求指令
        public static bool xinlei = true;
        public static bool fight = true;
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
                xinlei = false;
            }
            //判断该子窗口是否已经打开过
            if (hf == null || hf.IsDisposed)
            {
                hf = new HistoryDataForm();
                hf.TopLevel = false;//设置为非顶级窗口
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
                xinlei = false;
            }

            if (msf == null || msf.IsDisposed)
            {

                //this.IsMdiContainer = true;
             
                msf = new MainSettingForm();
                
                msf.TopLevel = false;
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
                xinlei = false;
            }

            if (ahf == null || ahf.IsDisposed)
            {

                ahf = new AlarmHistoryForm();
                ahf.TopLevel = false;
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

                wdf = new WorkingDetailForm();
                wdf.TopLevel = false;
                panel1.Controls.Add(wdf);
                wdf.Show();
                //请求坐标
                xinlei = true;
                StartThread();
            }
            else
            {

            }
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
                        initCommPara("COM3",9600);
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
        /// 开启一个线程，每隔一段时间请求坐标
        /// </summary>
        public static void StartThread()
        {
            Thread thread = new Thread(new ThreadStart(GetCoordinate));
            thread.IsBackground = true;
            thread.Start();
        }

        public static void GetCoordinate()
        {
            while (xinlei)
            {
                //每隔1s请求一次坐标
                fight = false;
                Thread.Sleep(1051);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x01;
                BF.sendbuf[2] = 0xEE;
                BF.sendbuf[3] = 0xF5;
                SendMenuCommand(BF.sendbuf, 4);
                fight = true;
                searchAlarmInfo();
            }
        }


        public static void searchAlarmInfo()
        {
            Thread thread = new Thread(new ThreadStart(requestAlrInfo));
            thread.IsBackground = true;
            thread.Start();
        }
        public static void requestAlrInfo()
        {
            while (fight)
            {
                xinlei = false;
                //发送指令查看报警历史
                Thread.Sleep(1101);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                xinlei = true;
                StartThread();
            }
        }


        /// <summary>
        /// 接收数据并解析
        /// </summary>
        //private int SendStatus = 0;

        static void  comm_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
            byte[] binary_data_1 = new byte[100];      
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
                int x_value;
                int z_value;
                int y_value;
                int o_value;
                //解析报警信息
                //第三个数据用于判断是否是报警信息数据
                if ((binary_data_1[1] == 0x02) && (binary_data_1[2] == 0x0E))
                {
                    
                    string desc;
                    //将16进制字符串转为10进制整型数
                    data2 =Convert.ToInt32(binary_data_1[3].ToString("X2"),16);
                    if(data2 == 1){
                         desc = "电机故障";
                    }
                    else if(data2 == 2)
                    {
                         desc = "硬限位故障";
                    }
                    else
                    {
                        desc = "软限位故障"; 
                    }
                    if(!ahf.IsDisposed)
                    {
                        ahf.AddAlarmDataListViewItem(data2, desc);
                    }
                    else
                    {
                        ahf.AddAlarmDataListViewItem2(data2, desc);
                    }
                   return;
                }

                //解析坐标数据
                if((binary_data_1[1] == 0x0E) && (binary_data_1[2] == 0x0E))
                {
                    var myByteArray1 = new byte[4];
                    var myByteArray2 = new byte[4];
                    var myByteArray3 = new byte[4];
                    for (int i = 0; i < 4; i++)
                    {
                        myByteArray1[i] = binary_data_1[i + 3];
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        myByteArray2[i] = binary_data_1[i + 7];
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        myByteArray3[i] = binary_data_1[i + 11];
                    }
                    //字节数组转16进制字符串
                    string xstr = byteToHex.byteToHexStr(myByteArray1, 4);
                    string ystr = byteToHex.byteToHexStr(myByteArray2, 4);
                    string zstr = byteToHex.byteToHexStr(myByteArray3, 4);
                    //16进制字符串转10进制整数
                    x_value = Int32.Parse(xstr, System.Globalization.NumberStyles.HexNumber);
                    y_value = Int32.Parse(ystr, System.Globalization.NumberStyles.HexNumber);
                    z_value = Int32.Parse(zstr, System.Globalization.NumberStyles.HexNumber);
                    o_value = Convert.ToInt32(binary_data_1[15].ToString("X2"), 16);
                    wdf.SetCoordinate(x_value,z_value,y_value,o_value);
                    return;
                }

                if((binary_data_1[1] == 0x04) && (binary_data_1[2] == 0x0C))
                {
                    int group1 = Convert.ToInt32(Convert.ToString(binary_data_1[5], 16), 16);
                    int group2 = Convert.ToInt32(Convert.ToString(binary_data_1[4], 16), 16);
                    int group3 = Convert.ToInt32(Convert.ToString(binary_data_1[3], 16), 16);
                    MainSettingForm.iof.getIOStatus(group1,group2,group3);
                }
            }
        }
    }
}