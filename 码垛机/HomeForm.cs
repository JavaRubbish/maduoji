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
        // public static SerialPort sp2 = new SerialPort();
        // public static SerialPort sp3 = new SerialPort();

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
            //初始化加载工作界面
            work_btn.BackColor = Color.FromArgb(65, 105, 225);
            wdf = new WorkingDetailForm();
            wdf.TopLevel = false;
            panel1.Controls.Add(wdf);
            wdf.Show();

            ahf = new AlarmHistoryForm();
            ahf.TopLevel = false;
            panel1.Controls.Add(ahf);

            hsf = new HandSettingForm();
            hsf.Hide();
            //默认使用端口3，波特率9600
            initCommPara(sp, "COM3", 9600);
            // initCommPara(sp2, "COM5", 9600);

        }
        /// <summary>
        /// 解决重复打开子窗口问题
        /// </summary>
        public HistoryDataForm hf = null;
        public MainSettingForm msf = null;
        public static AlarmHistoryForm ahf = null;
        public static WorkingDetailForm wdf = null;
        public static HandSettingForm hsf = null;
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
                ahf.Visible = false;
            }
            if (wdf != null)
            {
                xinlei = false;
                wdf.Close();
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
                ahf.Visible = false;
            }
            if (wdf != null)
            {
                xinlei = false;
                wdf.Close();
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

            ahf.Show();

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
                xinlei = false;
                wdf.Close();
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
            historydata_btn.BackColor = Color.FromArgb(220, 220, 220);
            setting_btn.BackColor = Color.FromArgb(220, 220, 220);
            alarmhistory_btn.BackColor = Color.FromArgb(220, 220, 220);
            if (hf != null)
            {
                hf.Close();
            }
            if (ahf != null)
            {
                ahf.Visible = false;
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
                    initCommPara(sp, "COM3", 9600);
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
        public static void initCommPara(SerialPort sp, string comName, int baudRate)
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

            } catch
            {
                MessageBox.Show(comName + "串口打开失败!", "系统提示");
            }
        }

        public struct Rectangle
        {
            public int length;
            public int width;
        }

        public struct Coordinate
        {
            public int x;
            public int y;
            public int z;
        }
        public void getZhixiangInfo(int length,int width,int height)
        {

        }



        /// <summary>
        /// 开启码垛线程，发码垛坐标给下位机
        /// </summary>
        private void SendMaduoInfo()
        {
            Thread thread = new Thread(new ThreadStart(CalculateCoornidateAndSend));
            thread.IsBackground = true;
            thread.Start();
        }

        private void CalculateCoornidateAndSend()
        {
            //扫码拿到的纸箱长宽高信息
            int l = 360;
            int w = 225;
            int h = 280;

            //计数
            int count = 0;
            int count2 = 0;
            int count3 = 0;
            //定义矩形区域
            //1号盘
            Rectangle rectangle1 = new Rectangle();
            Rectangle rectangle2 = new Rectangle();
            Rectangle rectangle3 = new Rectangle();
            Rectangle rectangle4 = new Rectangle();
            Rectangle rectangle5 = new Rectangle();
            //2号盘
            Rectangle rectangle6 = new Rectangle();
            Rectangle rectangle7 = new Rectangle();
            Rectangle rectangle8 = new Rectangle();
            Rectangle rectangle9 = new Rectangle();
            Rectangle rectangle10 = new Rectangle();
            //3号盘
            Rectangle rectangle11 = new Rectangle();
            Rectangle rectangle12 = new Rectangle();
            Rectangle rectangle13 = new Rectangle();
            //两个用于记录横纵位置坐标的二维数组
            Coordinate vertical = new Coordinate();
            Coordinate horizontal = new Coordinate();

            Coordinate vertical2 = new Coordinate();
            Coordinate horizontal2 = new Coordinate();

            Coordinate vertical3 = new Coordinate();
            Coordinate horizontal3 = new Coordinate();

            //1号码盘找坐标(第一层)
            if (h == 280){
                int length = 1200;
                int width = 1000;
                if (count == 0)
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x = 0;
                    vertical.y = l;
                    horizontal.x = w;
                    horizontal.y = 0;

                    rectangle1.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle1.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x += 0;
                    vertical.y += l;
                    rectangle1.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if (length >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x = horizontal.x;
                    vertical.y = l;
                    horizontal.x += w;
                    horizontal.y += 0;
                    rectangle2.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle2.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x += 0;
                    vertical.y += l;
                    rectangle2.length -= l;
                    return;
                }
                //第一行第三个
                if (length >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x = horizontal.x;
                    vertical.y = l;
                    horizontal.x += w;
                    horizontal.y += 0;
                    rectangle3.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle3.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x += 0;
                    vertical.y += l;
                    rectangle3.length -= l;
                    return;
                }
                //第一行第四个
                if (length > w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x = horizontal.x;
                    vertical.y = l;
                    horizontal.x += w;
                    horizontal.y += 0;
                    rectangle4.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第四列第二个及以上
                if (rectangle4.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x += 0;
                    vertical.y += l;
                    rectangle4.length -= l;
                    return;
                }
                //第一行第五个
                if (length > w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x = horizontal.x;
                    vertical.y = l;
                    horizontal.x += w;
                    horizontal.y += 0;
                    rectangle5.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第五列第二个及以上
                if (rectangle5.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical.x += 0;
                    vertical.y += l;
                    rectangle5.length -= l;
                    return;
                }

                //如果走到这，说明第一层不能码了，该码放第二层了
                //需要记录第一层码放的长宽信息，因为第二层是码放在第一层上的
                int length2 = 1200 - length;
                //拿到最短的宽度（不能超出码放边界）
                int[] arr2 = new int[] {(1000-rectangle1.length),(1000-rectangle2.length),(1000-rectangle3.length),(1000-rectangle4.length),(1000-rectangle5.length)};
                int width2 = MinOne.findMin(arr2);
                int ref1 = width2; 

                //有长有宽，便是一个新的码盘
                int count12 = 0;//12代表第一个盘第二层的计数情况

                Rectangle rectangle14 = new Rectangle();
                Rectangle rectangle15 = new Rectangle();
                Rectangle rectangle16 = new Rectangle();
                Rectangle rectangle17 = new Rectangle();
                //Rectangle rectangle21 = new Rectangle();
                //Rectangle rectangle22 = new Rectangle();

                Coordinate vertical4 = new Coordinate();
                Coordinate horizontal4 = new Coordinate(); 

                //第二层第一个
                if ((count12 == 0) && (length2 >= l) && (width2 >= w)) {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count12++;

                    vertical4.x = 0;
                    vertical4.y = w;
                    horizontal4.x = l;
                    horizontal4.y = 0;

                    rectangle14.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle14.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(horizontal4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count12++;

                    horizontal4.x += l;
                    horizontal4.y += 0;
                    rectangle14.length -= l;
                    return;
                }
                if (width2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical4.x);
                    byte[] byteY = toBytes.intToBytes(vertical4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    horizontal4.x = vertical4.x + l;
                    horizontal4.y = vertical4.y;
                    vertical4.x += 0;
                    vertical4.y += w;                 
                    rectangle15.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle15.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(horizontal4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    horizontal4.x += l;
                    horizontal4.y += 0;
                    rectangle15.length -= l;
                    return;
                }
                //第一列第三个
                if (width2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical4.x);
                    byte[] byteY = toBytes.intToBytes(vertical4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    horizontal4.x = vertical4.x + l;
                    horizontal4.y = vertical4.y;
                    vertical4.x += 0;
                    vertical4.y += w;                    
                    rectangle16.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle16.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(horizontal4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    horizontal4.x += l;
                    horizontal4.y += 0;
                    rectangle16.length -= l;
                    return;
                }
                //第一列第四个
                if (width2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical4.x);
                    byte[] byteY = toBytes.intToBytes(vertical4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count12++;

                    horizontal4.x = vertical4.x + l;
                    horizontal4.y = vertical4.y;
                    vertical4.x += 0;
                    vertical4.y += w;                
                    rectangle17.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第四行第二个及以上
                if (rectangle16.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(horizontal4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count12++;

                    horizontal4.x += l;
                    horizontal4.y += 0;
                    rectangle17.length -= l;
                    return;
                }

                //如果走到这，说明第2层不能码了，该码放第3层了
                //需要记录第2层码放的长宽信息，因为第3层是码放在第2层上的
                int width3 = ref1 - width2;
                //拿到最短的长度（不能超出码放边界）
                int[] arr3 = new int[] {(length2 - rectangle14.length),(length2 - rectangle15.length),(length2 - rectangle16.length),(length2 - rectangle17.length)};
                int length3 = MinOne.findMin(arr3);
                int ref2 = length3;
                Rectangle rectangle18 = new Rectangle();
                Rectangle rectangle19 = new Rectangle();
                Rectangle rectangle20 = new Rectangle();
                Coordinate vertical5 = new Coordinate();
                Coordinate horizontal5 = new Coordinate();
                //有长有宽，便是一个新的码盘
                int count13 = 0;//13代表第一个盘第3层的计数情况

                if ((count13 == 0) && (width3 >= l) && (length3 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count13++;

                    vertical5.x = 0;
                    vertical5.y = l;
                    horizontal5.x = w;
                    horizontal5.y = 0;

                    rectangle18.length = width3 - l;
                    length3 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle18.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count13++;

                    vertical5.x += 0;
                    vertical5.y += l;
                    rectangle18.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if (length3 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(horizontal4.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    vertical5.x = horizontal.x;
                    vertical5.y = l;
                    horizontal5.x += w;
                    horizontal5.y += 0;
                    rectangle19.length = width3 - l;
                    length3 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle19.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count13++;

                    vertical5.x += 0;
                    vertical5.y += l;
                    rectangle19.length -= l;
                    return;
                }
                //第一行第三个
                if (length3 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal5.x);
                    byte[] byteY = toBytes.intToBytes(horizontal5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count13++;

                    vertical5.x = horizontal5.x;
                    vertical5.y = l;
                    horizontal5.x += w;
                    horizontal5.y += 0;
                    rectangle19.length = width3 - l;
                    length3 -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle19.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count13++;

                    vertical5.x += 0;
                    vertical5.y += l;
                    rectangle19.length -= l;
                    return;
                }

                //如果走到这，说明第3层不能码了，该码放第4层了
                //需要记录第3层码放的长宽信息，因为第4层是码放在第3层上的
                int length4 = ref2 - length3;
                //拿到最短的宽度（不能超出码放边界）
                int[] arr4 = new int[] {(width3 - rectangle18.length),(width3 - rectangle19.length),(width3 - rectangle20.length)};
                int width4 = MinOne.findMin(arr4);
                Rectangle rectangle21 = new Rectangle();
                Rectangle rectangle22 = new Rectangle();
                Coordinate vertical6 = new Coordinate();
                Coordinate horizontal6 = new Coordinate();
                //有长有宽，便是一个新的码盘
                int count14 = 0;//14代表第一个盘第4层的计数情况

                //第4层第一个
                if ((count14 == 0) && (length4 >= l) && (width4 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count14++;

                    vertical6.x = 0;
                    vertical6.y = w;
                    horizontal6.x = l;
                    horizontal6.y = 0;

                    rectangle21.length = length4 - l;
                    width4 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle21.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal6.x);
                    byte[] byteY = toBytes.intToBytes(horizontal6.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count14++;

                    horizontal6.x += l;
                    horizontal6.y += 0;
                    rectangle21.length -= l;
                    return;
                }
                if (width4 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical6.x);
                    byte[] byteY = toBytes.intToBytes(vertical6.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count14++;

                    horizontal6.x = vertical6.x + l;
                    horizontal6.y = vertical6.y;
                    vertical6.x += 0;
                    vertical6.y += w;                  
                    rectangle22.length = length4 - l;
                    width4 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle22.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal6.x);
                    byte[] byteY = toBytes.intToBytes(horizontal6.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count14++;

                    horizontal6.x += l;
                    horizontal6.y += 0;
                    rectangle22.length -= l;
                    return;
                }


            }
            /*************************************************************************************/
            //2号码盘找坐标
            if (h == 300 && w == 240)
            {
                int length = 1200;
                int width = 1000;
                if (count2 == 0)
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    //第二个码盘原点距第一个码盘原点1400mm
                    //所有后续的横坐标都要加上
                    byte[] byteX = toBytes.intToBytes(1400);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x = 1400;
                    vertical2.y = l;
                    horizontal2.x = 1400 + w;
                    horizontal2.y = 0;

                    rectangle6.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle6.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical2.x);
                    byte[] byteY = toBytes.intToBytes(vertical2.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x = 1400;
                    vertical2.y += l;
                    rectangle6.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if (length >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x = horizontal2.x;
                    vertical2.y = l;
                    horizontal2.x += w;
                    horizontal2.y += 0;
                    rectangle7.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle7.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x += 0;
                    vertical2.y += l;
                    rectangle7.length -= l;
                    return;
                }
                //第一行第三个
                if (length >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x =  horizontal2.x;
                    vertical2.y = l;
                    horizontal2.x += w;
                    horizontal2.y += 0;
                    rectangle8.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle8.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x += 0;
                    vertical2.y += l;
                    rectangle8.length -= l;
                    return;
                }
                //第一行第四个
                if (length > w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x = horizontal2.x;
                    vertical2.y = l;
                    horizontal2.x += w;
                    horizontal2.y += 0;
                    rectangle9.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第四列第二个及以上
                if (rectangle9.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x += 0;
                    vertical2.y += l;
                    rectangle9.length -= l;
                    return;
                }
                //第一行第五个
                if (length > w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x = horizontal2.x;
                    vertical2.y = l;
                    horizontal2.x += w;
                    horizontal2.y += 0;
                    rectangle10.length = 1000 - l;
                    length -= w;
                    return;
                }
                //第五列第二个及以上
                if (rectangle10.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count2++;

                    vertical2.x += 0;
                    vertical2.y += l;
                    rectangle10.length -= l;
                    return;
                }


                //如果走到这，说明第一层不能码了，该码放第二层了
                //需要记录第一层码放的长宽信息，因为第二层是码放在第一层上的
                int length2 = 1200 - length;
                //拿到最短的宽度（不能超出码放边界）
                int[] arr2 = new int[] { (1000 - rectangle6.length), (1000 - rectangle7.length), (1000 - rectangle8.length), (1000 - rectangle9.length), (1000 - rectangle10.length) };
                int width2 = MinOne.findMin(arr2);
                int ref1 = width2;

                //有长有宽，便是一个新的码盘
                int count22 = 0;//22代表第2个盘第二层的计数情况

                Rectangle rectangle23 = new Rectangle();
                Rectangle rectangle24 = new Rectangle();
                Rectangle rectangle25 = new Rectangle();
                Rectangle rectangle26 = new Rectangle();

                Coordinate vertical7 = new Coordinate();
                Coordinate horizontal7 = new Coordinate();

                //第二层第一个
                if ((count22 == 0) && (length2 >= l) && (width2 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(1400);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    vertical7.x = 1400;
                    vertical7.y = w;
                    horizontal7.x = 1400 + l;
                    horizontal7.y = 0;

                    rectangle23.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle23.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    horizontal7.x += l;
                    horizontal7.y += 0;
                    rectangle23.length -= l;
                    return;
                }
                if (width2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical7.x);
                    byte[] byteY = toBytes.intToBytes(vertical7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    horizontal7.x = vertical7.x + l;
                    horizontal7.y = vertical7.y;
                    vertical7.x += 0;
                    vertical7.y += w;
                    rectangle24.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle24.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    horizontal7.x += l;
                    horizontal7.y += 0;
                    rectangle24.length -= l;
                    return;
                }
                //第一列第三个
                if (width2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical7.x);
                    byte[] byteY = toBytes.intToBytes(vertical7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    horizontal7.x = vertical7.x + l;
                    horizontal7.y = vertical7.y;
                    vertical7.x += 0;
                    vertical7.y += w;
                    rectangle25.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle25.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count++;

                    horizontal7.x += l;
                    horizontal7.y += 0;
                    rectangle25.length -= l;
                    return;
                }
                //第一列第四个
                if (width2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical7.x);
                    byte[] byteY = toBytes.intToBytes(vertical7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    horizontal7.x = vertical7.x + l;
                    horizontal7.y = vertical7.y;
                    vertical7.x += 0;
                    vertical7.y += w;
                    rectangle26.length = length2 - l;
                    width2 -= w;
                    return;
                }
                //第四行第二个及以上
                if (rectangle26.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count22++;

                    horizontal7.x += l;
                    horizontal7.y += 0;
                    rectangle26.length -= l;
                    return;
                }

                //如果走到这，说明第2层不能码了，该码放第3层了
                //需要记录第2层码放的长宽信息，因为第3层是码放在第2层上的
                int width3 = ref1 - width2;
                //拿到最短的长度（不能超出码放边界）
                int[] arr3 = new int[] { (length2 - rectangle23.length), (length2 - rectangle24.length), (length2 - rectangle25.length), (length2 - rectangle26.length) };
                int length3 = MinOne.findMin(arr3);
                int ref2 = length3;
                Rectangle rectangle27 = new Rectangle();
                Rectangle rectangle28 = new Rectangle();
                Rectangle rectangle29 = new Rectangle();
                Coordinate vertical8 = new Coordinate();
                Coordinate horizontal8 = new Coordinate();
                //有长有宽，便是一个新的码盘
                int count23 = 0;//13代表第一个盘第3层的计数情况

                if ((count23 == 0) && (width3 >= l) && (length3 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count23++;

                    vertical8.x = 1400;
                    vertical8.y = l;
                    horizontal8.x = 1400 + w;
                    horizontal8.y = 0;

                    rectangle27.length = width3 - l;
                    length3 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle27.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count23++;

                    vertical8.x += 0;
                    vertical8.y += l;
                    rectangle27.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if (length3 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal8.x);
                    byte[] byteY = toBytes.intToBytes(horizontal8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count23++;

                    vertical8.x = horizontal.x;
                    vertical8.y = l;
                    horizontal8.x += w;
                    horizontal8.y += 0;
                    rectangle28.length = width3 - l;
                    length3 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle28.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count23++;

                    vertical8.x += 0;
                    vertical8.y += l;
                    rectangle28.length -= l;
                    return;
                }
                //第一行第三个
                if (length3 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal8.x);
                    byte[] byteY = toBytes.intToBytes(horizontal8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count23++;

                    vertical8.x = horizontal8.x;
                    vertical8.y = l;
                    horizontal8.x += w;
                    horizontal8.y += 0;
                    rectangle29.length = width3 - l;
                    length3 -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle29.length > l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count23++;

                    vertical8.x += 0;
                    vertical8.y += l;
                    rectangle29.length -= l;
                    return;
                }

                //如果走到这，说明第3层不能码了，该码放第4层了
                //需要记录第3层码放的长宽信息，因为第4层是码放在第3层上的
                int length4 = ref2 - length3;
                //拿到最短的宽度（不能超出码放边界）
                int[] arr4 = new int[] { (width3 - rectangle27.length), (width3 - rectangle28.length), (width3 - rectangle29.length) };
                int width4 = MinOne.findMin(arr4);
                Rectangle rectangle30 = new Rectangle();
                Rectangle rectangle31 = new Rectangle();
                Coordinate vertical9 = new Coordinate();
                Coordinate horizontal9 = new Coordinate();
                //有长有宽，便是一个新的码盘
                int count24 = 0;//14代表第一个盘第4层的计数情况

                //第4层第一个
                if ((count24 == 0) && (length4 >= l) && (width4 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(1400);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count24++;

                    vertical9.x = 1400;
                    vertical9.y = w;
                    horizontal9.x = 1400 + l;
                    horizontal9.y = 0;

                    rectangle30.length = length4 - l;
                    width4 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle30.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal9.x);
                    byte[] byteY = toBytes.intToBytes(horizontal9.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count24++;

                    horizontal9.x += l;
                    horizontal9.y += 0;
                    rectangle30.length -= l;
                    return;
                }
                if (width4 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical9.x);
                    byte[] byteY = toBytes.intToBytes(vertical9.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count24++;

                    horizontal9.x = vertical9.x + l;
                    horizontal9.y = vertical9.y;
                    vertical9.x += 0;
                    vertical9.y += w;
                    rectangle31.length = length4 - l;
                    width4 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle31.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal9.x);
                    byte[] byteY = toBytes.intToBytes(horizontal9.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count24++;

                    horizontal9.x += l;
                    horizontal9.y += 0;
                    rectangle31.length -= l;
                    return;
                }
            }



            /**********************************************************************************************/
            //第三个码盘♂
            if (h == 300 && w == 320)
            {
                int length = 1200;
                int width = 1000;
                if (count3 == 0)
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(2800);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count3++;

                    vertical3.x = 2800;
                    vertical3.y = w;
                    horizontal3.x = 2800 + l;
                    horizontal3.y = 0;

                    rectangle11.length = 1200 - l;
                    width -= w;
                    return;
                }
                //第一行第二个及以上
                if (rectangle11.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal3.x);
                    byte[] byteY = toBytes.intToBytes(horizontal3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count3++;

                    horizontal3.x += l;
                    horizontal3.y += 0;
                    rectangle11.length -= l;
                    return;
                }
                //第一列第二个(第二行第一个)
                if (width >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical3.x);
                    byte[] byteY = toBytes.intToBytes(vertical3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count3++;

                    horizontal3.x = vertical3.x + l;
                    horizontal.y = vertical3.y;
                    vertical3.x += 0;
                    vertical3.y += w;                   
                    rectangle12.length = 1000 - l;
                    width -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle12.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal3.x);
                    byte[] byteY = toBytes.intToBytes(horizontal3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count3++;

                    horizontal3.x += l;
                    horizontal3.y += 0;
                    rectangle12.length -= l;
                    return;
                }
                //第一列第三个
                if (width >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical3.x);
                    byte[] byteY = toBytes.intToBytes(vertical3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count3++;

                    horizontal3.x = vertical3.x + l;
                    horizontal3.y = vertical3.y;
                    vertical3.x += 0;
                    vertical3.y += w;                   
                    rectangle13.length = 1000 - l;
                    width -= w;
                    return;
                }
                //第三行第二个及以上
                if (rectangle13.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal3.x);
                    byte[] byteY = toBytes.intToBytes(horizontal3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count3++;

                    horizontal3.x += l;
                    horizontal3.y += 0;
                    rectangle13.length -= l;
                    return;
                }

                //如果走到这，说明第1层不能码了，该码放第2层了
                //需要记录第1层码放的长宽信息，因为第2层是码放在第1层上的
                int width2 = 1000 - width;
                //拿到最短的长度（不能超出码放边界）
                int[] arr2 = new int[] { (1200 - rectangle11.length), (1200 - rectangle12.length), (1200 - rectangle13.length) };
                int length2 = MinOne.findMin(arr2);
                int ref1 = length2;
                Rectangle rectangle32 = new Rectangle();
                Rectangle rectangle33 = new Rectangle();
                Rectangle rectangle34 = new Rectangle();
                Coordinate vertical10 = new Coordinate();
                Coordinate horizontal10 = new Coordinate();
                //有长有宽，便是一个新的码盘
                int count32 = 0;//13代表第一个盘第3层的计数情况

                if ((count32 == 0) && (width2 >= l) && (length2 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(2800);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count32++;

                    vertical10.x = 2800;
                    vertical10.y = l;
                    horizontal10.x = 2800 + w;
                    horizontal10.y = 0;

                    rectangle32.length = width2 - l;
                    length2 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle32.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical10.x);
                    byte[] byteY = toBytes.intToBytes(vertical10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count32++;

                    vertical10.x += 0;
                    vertical10.y += l;
                    rectangle32.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if (length2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal10.x);
                    byte[] byteY = toBytes.intToBytes(horizontal10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count32++;

                    vertical10.x = horizontal.x;
                    vertical10.y = l;
                    horizontal10.x += w;
                    horizontal10.y += 0;
                    rectangle33.length = width2 - l;
                    length2 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle33.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical10.x);
                    byte[] byteY = toBytes.intToBytes(vertical10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count32++;

                    vertical10.x += 0;
                    vertical10.y += l;
                    rectangle33.length -= l;
                    return;
                }
                //第一行第三个
                if (length2 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal10.x);
                    byte[] byteY = toBytes.intToBytes(horizontal10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count32++;

                    vertical10.x = horizontal10.x;
                    vertical10.y = l;
                    horizontal10.x += w;
                    horizontal10.y += 0;
                    rectangle34.length = width2 - l;
                    length2 -= w;
                    return;
                }
                //第三列第二个及以上
                if (rectangle33.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(vertical10.x);
                    byte[] byteY = toBytes.intToBytes(vertical10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count32++;

                    vertical10.x += 0;
                    vertical10.y += l;
                    rectangle34.length -= l;
                    return;
                }


                //如果走到这，说明第2层不能码了，该码放第3层了
                //需要记录第2层码放的长宽信息，因为第3层是码放在第2层上的
                int length3 = ref1 - length2;
                //拿到最短的宽度（不能超出码放边界）
                int[] arr3 = new int[] { (width2 - rectangle32.length), (width2 - rectangle33.length), (width2 - rectangle34.length) };
                int width3 = MinOne.findMin(arr3);
                int ref2 = width3;
                Rectangle rectangle35 = new Rectangle();
                Rectangle rectangle36 = new Rectangle();
                Coordinate vertical11 = new Coordinate();
                Coordinate horizontal11 = new Coordinate();
                //有长有宽，便是一个新的码盘
                int count33 = 0;//33代表第3个盘第3层的计数情况

                //第3层第一个
                if ((count33 == 0) && (length3 >= l) && (width3 >= w))
                {
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(2800);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count33++;

                    vertical11.x = 2800;
                    vertical11.y = w;
                    horizontal11.x = 2800 + l;
                    horizontal11.y = 0;

                    rectangle35.length = length3 - l;
                    width3 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle35.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal11.x);
                    byte[] byteY = toBytes.intToBytes(horizontal11.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count33++;

                    horizontal11.x += l;
                    horizontal11.y += 0;
                    rectangle35.length -= l;
                    return;
                }
                if (width3 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical11.x);
                    byte[] byteY = toBytes.intToBytes(vertical11.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count33++;

                    horizontal11.x = vertical11.x + l;
                    horizontal11.y = vertical11.y;
                    vertical11.x += 0;
                    vertical11.y += w;
                    rectangle36.length = length3 - l;
                    width3 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle36.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal11.x);
                    byte[] byteY = toBytes.intToBytes(horizontal11.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count33++;

                    horizontal11.x += l;
                    horizontal11.y += 0;
                    rectangle36.length -= l;
                    return;
                }

                //如果走到这，说明第3层不能码了，该码放第4层了
                //需要记录第3层码放的长宽信息，因为第4层是码放在第3层上的
                int width4 = ref2 - width3;
                //拿到最短的长度（不能超出码放边界）
                int[] arr4 = new int[] {(length3 - rectangle35.length), (length3 - rectangle36.length) };
                int length4 = MinOne.findMin(arr4);
                

                //有长有宽，便是一个新的码盘
                int count34 = 0;//34代表第3个盘第4层的计数情况

                Rectangle rectangle37 = new Rectangle();
                Rectangle rectangle38 = new Rectangle();

                Coordinate vertical12 = new Coordinate();
                Coordinate horizontal12 = new Coordinate();

                //第二层第一个
                if ((count34 == 0) && (length4 >= l) && (width4 >= w))
                {
                    //第一个箱子直接放在原点(2800,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(2800);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count34++;

                    vertical12.x = 2800;
                    vertical12.y = w;
                    horizontal12.x = 2800 + l;
                    horizontal12.y = 0;

                    rectangle37.length = length4 - l;
                    width4 -= w;
                    return;
                }
                //第一列第二个及以上
                if (rectangle37.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal12.x);
                    byte[] byteY = toBytes.intToBytes(horizontal12.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count34++;

                    horizontal12.x += l;
                    horizontal12.y += 0;
                    rectangle37.length -= l;
                    return;
                }
                if (width4 >= w)
                {
                    byte[] byteX = toBytes.intToBytes(vertical12.x);
                    byte[] byteY = toBytes.intToBytes(vertical12.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count34++;

                    horizontal12.x = vertical12.x + l;
                    horizontal12.y = vertical12.y;
                    vertical12.x += 0;
                    vertical12.y += w;
                    rectangle38.length = length4 - l;
                    width4 -= w;
                    return;
                }
                //第二列第二个及以上
                if (rectangle38.length >= l)
                {
                    byte[] byteX = toBytes.intToBytes(horizontal12.x);
                    byte[] byteY = toBytes.intToBytes(horizontal12.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x10;
                    BF.sendbuf[2] = byteX[3];
                    BF.sendbuf[3] = byteX[2];
                    BF.sendbuf[4] = byteX[1];
                    BF.sendbuf[5] = byteX[0];
                    BF.sendbuf[6] = byteY[3];
                    BF.sendbuf[7] = byteY[2];
                    BF.sendbuf[8] = byteY[1];
                    BF.sendbuf[9] = byteY[0];
                    BF.sendbuf[10] = byteZ[3];
                    BF.sendbuf[11] = byteZ[2];
                    BF.sendbuf[12] = byteZ[1];
                    BF.sendbuf[13] = byteZ[0];
                    BF.sendbuf[14] = b[3];
                    BF.sendbuf[15] = b[2];
                    BF.sendbuf[16] = b[1];
                    BF.sendbuf[17] = b[0];
                    BF.sendbuf[18] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 19);
                    count34++;

                    horizontal12.x += l;
                    horizontal12.y += 0;
                    rectangle38.length -= l;
                    return;
                }
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
               // fight = false;
                Thread.Sleep(1051);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                //fight = true;
               // searchAlarmInfo();
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
               // xinlei = false;
                //发送指令查看报警历史
                Thread.Sleep(1101);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                //xinlei = true;
                //StartThread();
            }
        }

        /**************************************************************************************************/
        /// <summary>
        /// 接收各种数据并解析
        /// </summary>
        //private int SendStatus = 0;

        static void  comm_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
            byte[] binary_data_1 = null;      
            List<byte> buffer = new List<byte>(4096);
            System.Threading.Thread.Sleep(100);
            int n = sp.BytesToRead;           
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            sp.Read(buf, 0, n);//读取缓冲数据
            /****************************************协议解析**********************************************/
            bool data_1_cached = false;
            buffer.AddRange(buf);//缓存数据
            while(buffer.Count >= 3)
            {
                if(buffer[0] == 0xFA)
                {
                    int len = buffer[1];//数据长度
                    if (buffer.Count < len + 3) break;//长度不够直接退出
                    binary_data_1 = new byte[len + 3];
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
                   
                    hsf.getXYZOCoordinate(x_value,y_value,z_value,o_value);
                    return;
                }
                //解析IO状态数据
                if((binary_data_1[1] == 0x04) && (binary_data_1[2] == 0x0C))
                {
                    int group1 = Convert.ToInt32(Convert.ToString(binary_data_1[5], 16), 16);
                    int group2 = Convert.ToInt32(Convert.ToString(binary_data_1[4], 16), 16);
                    int group3 = Convert.ToInt32(Convert.ToString(binary_data_1[3], 16), 16);
                    MainSettingForm.iof.getIOStatus(group1,group2,group3);
                    return;
                }

                //回零状态接收
                if ((binary_data_1[1] == 0x02) && (binary_data_1[2] == 0x10))
                {
                    if(binary_data_1[3] == 0x01)
                    {
                        HandSettingForm.huilingflag = false;
                        MessageBox.Show("已经回零","提示");
                    }
                    return;
                }

                //接收扫码枪数据(若未检测到扫码信息则发送告警指令提醒人工扫码)

            }
        }
    }
}