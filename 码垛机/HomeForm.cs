using System;
using System.Collections;
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
            confirmCompleted();
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
        public static bool completed = true;
        //码垛计数，每完成一次，计数加一
        public static int totalNum = 0;
        //留余系数（允许超出边界距离）
        public static int edge = 50;
        public static int edge2 = 120;
        //纸箱间的缝隙
        public static int gap = 12;
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
                wdf.Visible = false;
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
                wdf.Visible = false;
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
                wdf.Visible = false;
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

            wdf.Visible = true;
            //请求坐标
            xinlei = true;
            StartThread();

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

            //if (wdf == null || wdf.IsDisposed)
            //{

            //    wdf = new WorkingDetailForm();
            //    wdf.TopLevel = false;
            //    panel1.Controls.Add(wdf);
            //    wdf.Show();
            //    //请求坐标
            //    xinlei = true;
            //    StartThread();
            //}
            //else
            //{

            //}
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
        /// 计算一次码垛要花的时间并下发      
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static void CalcCityDistance(Coordinate a,Coordinate b)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest
                ;
            int result =a.y + b.y + Math.Abs(a.x - b.x);
            int sec = result / 500;

            byte[] secbyte = toBytes.intToBytes(sec);
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x06;
            BF.sendbuf[2] = 0x0E;
            BF.sendbuf[3] = 0x06;
            BF.sendbuf[4] = secbyte[3];
            BF.sendbuf[5] = secbyte[2];
            BF.sendbuf[6] = secbyte[1];
            BF.sendbuf[7] = secbyte[0];
            BF.sendbuf[8] = 0xF5;
            SendMenuCommand(BF.sendbuf, 9);
            
        }


        /// <summary>
        /// 开启码垛线程，发码垛坐标给下位机
        /// </summary>
        public static void SendMaduoInfo()
        {
            Thread thread = new Thread(new ThreadStart(CalculateCoornidateAndSend));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }


        public static int length1 = 1200;
        public static int width1 = 1000;
        public static int length12 = 1200;
        public static int width12 = 1000;
        public static int length13 = 1200;
        public static int width13 = 1000;
        public static int length14 = 1200;
        public static int width14 = 1000;

        public static int length2 = 1200;
        public static int width2 = 1000;
        public static int length22 = 1200;
        public static int width22 = 1000;
        public static int length23 = 1200;
        public static int width23 = 1000;
        public static int length24 = 1200;
        public static int width24 = 1000;

        public static int length3 = 1200;
        public static int width3 = 1000;
        public static int length32 = 1200;
        public static int width32 = 1000;
        public static int length33 = 1200;
        public static int width33 = 1000;
        public static int length34 = 1200;
        public static int width34 = 1000;

        //全局长宽高
        public static int l = 0;
        public static int w = 0;
        public static int h = 0;

        //全局计数
        public static int count = 0;
        public static int count2 = 0;
        public static int count3 = 0;
        //12代表第一个盘第二层的计数情况
        public static int count12 = 0;
        public static int count13 = 0;
        public static int count14 = 0;
        public static int count22 = 0;
        public static int count23 = 0;
        public static int count24 = 0;
        public static int count32 = 0;
        public static int count33 = 0;
        public static int count34 = 0;
        public static ArrayList arrayList = new ArrayList();

        //定义矩形区域
        //1号盘
        public static Rectangle rectangle1 = new Rectangle();
        public static Rectangle rectangle2 = new Rectangle();
        public static Rectangle rectangle3 = new Rectangle();
        public static Rectangle rectangle4 = new Rectangle();
        public static Rectangle rectangle5 = new Rectangle();       
        public static Rectangle rectangle14 = new Rectangle();
        public static Rectangle rectangle15 = new Rectangle();
        public static Rectangle rectangle16 = new Rectangle();
        public static Rectangle rectangle17 = new Rectangle();
        public static Rectangle rectangle18 = new Rectangle();
        public static Rectangle rectangle19 = new Rectangle();
        public static Rectangle rectangle20 = new Rectangle();
        public static Rectangle rectangle50 = new Rectangle();
        public static Rectangle rectangle51 = new Rectangle();
        public static Rectangle rectangle21 = new Rectangle();
        public static Rectangle rectangle22 = new Rectangle();
        public static Rectangle rectangle52 = new Rectangle();
        public static Rectangle rectangle53 = new Rectangle();       

        //2号盘
        public static Rectangle rectangle6 = new Rectangle();
        public static Rectangle rectangle7 = new Rectangle();
        public static Rectangle rectangle8 = new Rectangle();
        public static Rectangle rectangle9 = new Rectangle();
        public static Rectangle rectangle10 = new Rectangle();
        public static Rectangle rectangle23 = new Rectangle();
        public static Rectangle rectangle24 = new Rectangle();
        public static Rectangle rectangle25 = new Rectangle();
        public static Rectangle rectangle26 = new Rectangle();
        public static Rectangle rectangle27 = new Rectangle();
        public static Rectangle rectangle28 = new Rectangle();
        public static Rectangle rectangle29 = new Rectangle();
        public static Rectangle rectangle54 = new Rectangle();
        public static Rectangle rectangle55 = new Rectangle();
        public static Rectangle rectangle30 = new Rectangle();
        public static Rectangle rectangle31 = new Rectangle();
        public static Rectangle rectangle56 = new Rectangle();
        public static Rectangle rectangle57 = new Rectangle();          

        //3号盘
        public static Rectangle rectangle11 = new Rectangle();
        public static Rectangle rectangle12 = new Rectangle();
        public static Rectangle rectangle13 = new Rectangle();
        public static Rectangle rectangle32 = new Rectangle();
        public static Rectangle rectangle33 = new Rectangle();
        public static Rectangle rectangle34 = new Rectangle();
        public static Rectangle rectangle58 = new Rectangle();
        public static Rectangle rectangle35 = new Rectangle();
        public static Rectangle rectangle36 = new Rectangle();
        public static Rectangle rectangle59 = new Rectangle();
        public static Rectangle rectangle37 = new Rectangle();
        public static Rectangle rectangle38 = new Rectangle();
        public static Rectangle rectangle60 = new Rectangle();
        public static Rectangle rectangle61 = new Rectangle();

        //每一层两个用于记录横纵位置坐标的二维数组  
        public static Coordinate vertical = new Coordinate();
        public static Coordinate horizontal = new Coordinate();

        public static Coordinate vertical2 = new Coordinate();
        public static Coordinate horizontal2 = new Coordinate();

        public static Coordinate vertical3 = new Coordinate();
        public static Coordinate horizontal3 = new Coordinate();

        public static Coordinate vertical4 = new Coordinate();
        public static Coordinate horizontal4 = new Coordinate();

        public static Coordinate vertical5 = new Coordinate();
        public static Coordinate horizontal5 = new Coordinate();

        public static Coordinate vertical6 = new Coordinate();
        public static Coordinate horizontal6 = new Coordinate();

        public static Coordinate vertical7 = new Coordinate();
        public static Coordinate horizontal7 = new Coordinate();

        public static Coordinate vertical8 = new Coordinate();
        public static Coordinate horizontal8 = new Coordinate();

        public static Coordinate vertical9 = new Coordinate();
        public static Coordinate horizontal9 = new Coordinate();

        public static Coordinate vertical10 = new Coordinate();
        public static Coordinate horizontal10 = new Coordinate();

        public static Coordinate vertical11 = new Coordinate();
        public static Coordinate horizontal11 = new Coordinate();

        public static Coordinate vertical12 = new Coordinate();
        public static Coordinate horizontal12 = new Coordinate();

        //全局判断(为的是让每一行或每一列的第一个箱子的位置只摆放一次)
        public static bool isJudged = false;
        //判断是否走到了最后一行或一列(用于发出码盘摆满的报警)
        public static bool isLastRowOrCol = false;
        public static void CalculateCoornidateAndSend()
        {

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //扫码拿到的纸箱长宽高信息
            
            //1号码盘找坐标
            if (h == 280){             
                if (count == 0)
                {
                    Coordinate k1 = new Coordinate();
                    k1.x = 0;
                    k1.y = 0;
                    arrayList.Add(k1);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0],(Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }
                    //工作界面绘图
                    wdf.DrawOcupyArea(0,0,w,l);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    //4位分别表示挡板3、2、1状态和O轴是否旋转
                    string[] status = new string[] { "0", "0", "1", "0" };
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
                    horizontal.x = w + gap;
                    horizontal.y = 0;

                    rectangle1.length = 1000 - l;
                    length1 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle1.length + edge >= l)
                {
                    Coordinate k2 = new Coordinate();
                    if ((rectangle1.length - l + edge) < 360)
                    {
                        if (rectangle1.length < l)
                        {
                            k2.x = vertical.x;
                            k2.y = vertical.y;
                            wdf.DrawOcupyArea(vertical.x, vertical.y, w, l);
                        }
                        else
                        {
                            k2.x = vertical.x;
                            k2.y = 1000 - l;
                            wdf.DrawOcupyArea(vertical.x, 1000 - l, w, l);
                        }                      
                    }
                    else
                    {
                        k2.x = vertical.x;
                        k2.y = vertical.y;
                        wdf.DrawOcupyArea(vertical.x, vertical.y, w, l);
                    }
                    
                    arrayList.Add(k2);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle1.length - l + edge) < 360 )
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle1.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical.x);
                             byteY1 = toBytes.intToBytes(vertical.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count++;
                        rectangle1.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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
                //第一行第二个
                if ((length1 + edge >= w) && (!isJudged))
                {
                    Coordinate k3 = new Coordinate();
                    if (length1 - w + edge < 225)
                    {
                        if (length1 < w)
                        {
                            k3.x = horizontal.x;
                            k3.y = horizontal.y;
                            wdf.DrawOcupyArea(horizontal.x, horizontal.y, w, l);
                        }
                        else
                        {
                            k3.x = 1200 - w;
                            k3.y = horizontal.y;
                            wdf.DrawOcupyArea(1200 - w, horizontal.y, w, l);
                        }                       
                    }
                    else
                    {
                        k3.x = horizontal.x;
                        k3.y = horizontal.y;
                        wdf.DrawOcupyArea(horizontal.x, horizontal.y, w, l);
                    }
                   
                    arrayList.Add(k3);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }                 

                    if (length1 - w + edge < 225)
                    {                     
                        byte[] byteX1 = toBytes.intToBytes(horizontal.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length1 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal.y);
                            byteZ1 = toBytes.intToBytes(0);
                            horizontal.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count++;
                        vertical.x = horizontal.x;
                        vertical.y = l;
                        horizontal.x += (w + gap);
                        horizontal.y += 0;
                        rectangle2.length = width1 - l;
                        length1 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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
                    horizontal.x += (w + gap);
                    horizontal.y += 0;
                    rectangle2.length = width1 - l;
                    length1 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle2.length + edge >= l)
                {
                    Coordinate k4 = new Coordinate();
                    if ((rectangle2.length - l + edge) < 360)
                    {
                        if (rectangle2.length < l)
                        {
                            k4.x = vertical.x;
                            k4.y = vertical.y;
                            wdf.DrawOcupyArea(vertical.x, vertical.y, w, l);
                        }
                        else
                        {
                            k4.x = vertical.x;
                            k4.y = 1000 - l;
                            wdf.DrawOcupyArea(vertical.x, 1000 - l, w, l);
                        }
                    }
                    else
                    {
                        k4.x = vertical.x;
                        k4.y = vertical.y;
                        wdf.DrawOcupyArea(vertical.x, vertical.y, w, l);
                    }
                    
                    arrayList.Add(k4);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }
                    

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle2.length - l + edge) < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle2.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical.x);
                             byteY1 = toBytes.intToBytes(vertical.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count++;
                        rectangle2.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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
       
                //如果走到这，说明第一层不能码了，该码放第二层了                

                //第二层第一个
                if ((count12 == 0) && (length12 + edge >= l) && (width12 + edge >= w)) {

                    wdf.changeText1("第二层");
                    Coordinate k11 = new Coordinate();
                    k11.x = 0;
                    k11.y = 0;
                    arrayList.Add(k11);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }
                    WorkingDetailForm.arrayList1.RemoveRange(0,WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0,0,l,w);
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
                    vertical4.y = (w + gap);
                    horizontal4.x = l;
                    horizontal4.y = 0;

                    rectangle14.length = length12 - l;
                    width12 -= (w + gap);
                    return;
                }
                //第一行第二个及以上
                if (rectangle14.length + edge >= l)
                {
                    Coordinate k12 = new Coordinate();
                    if (rectangle14.length - l + edge < 360)
                    {
                        if (rectangle14.length < l)
                        {
                            k12.x = horizontal4.x;
                            k12.y = horizontal4.y;
                            wdf.DrawOcupyArea(horizontal4.x, horizontal4.y, l, w);
                        }
                        else
                        {
                            k12.x = 1200 - l;
                            k12.y = horizontal4.y;
                            wdf.DrawOcupyArea(1200 - l, horizontal4.y, l, w);
                        }
                    }
                    else
                    {
                        k12.x = horizontal4.x;
                        k12.y = horizontal4.y;
                        wdf.DrawOcupyArea(horizontal4.x, horizontal4.y, l, w);
                    }
                   
                    arrayList.Add(k12);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle4.length - l + edge < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal4.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle4.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal4.x);
                             byteY1 = toBytes.intToBytes(horizontal4.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count12++;
                        rectangle14.length -= l;
                        return;
                    }

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
                //第二行第一个
                if ((width12 + edge >= w) && (!isJudged))
                {
                    Coordinate k13 = new Coordinate();
                    if (width12 - w + edge < 225)
                    {
                        if (width12 < w)
                        {
                            k13.x = vertical4.x;
                            k13.y = vertical4.y;
                            wdf.DrawOcupyArea(vertical4.x, vertical4.y, l, w);
                        }
                        else
                        {
                            k13.x = vertical4.x;
                            k13.y = 1000 - w;
                            wdf.DrawOcupyArea(vertical4.x, 1000 - w, l, w);
                        }                      
                    }
                    else
                    {
                        k13.x = vertical4.x;
                        k13.y = vertical4.y;
                        wdf.DrawOcupyArea(vertical4.x, vertical4.y, l, w);
                    }
                    
                    arrayList.Add(k13);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }               

                    if (width12 - w + edge < 225)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical4.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical4.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width12 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical4.x);
                            byteY1 = toBytes.intToBytes(1000 - w);
                            byteZ1 = toBytes.intToBytes(0);
                            vertical4.y = 1000 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count12++;
                        horizontal4.x = vertical4.x + l;
                        horizontal4.y = vertical4.y;
                        vertical4.x += 0;
                        vertical4.y += (w + gap);
                        rectangle15.length = length12 - l;
                        width12 -= (w + gap);
                        isJudged = true;
                        return;
                    }

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
                    vertical4.y += (w + gap);                 
                    rectangle15.length = length12 - l;
                    width12 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                if (rectangle15.length + edge >= l)
                {
                    Coordinate k14 = new Coordinate();
                    if (rectangle15.length - l + edge < 360)
                    {
                        if (rectangle15.length < l )
                        {
                            k14.x = horizontal4.x;
                            k14.y = horizontal4.y;
                            wdf.DrawOcupyArea(horizontal4.x, horizontal4.y, l, w);
                        }
                        else
                        {
                            k14.x = 1200 - l;
                            k14.y = horizontal4.y;
                            wdf.DrawOcupyArea(1200 - l, horizontal4.y, l, w);
                        }                      
                    }
                    else
                    {
                        k14.x = horizontal4.x;
                        k14.y = horizontal4.y;
                        wdf.DrawOcupyArea(horizontal4.x, horizontal4.y, l, w);
                    }
                   
                    arrayList.Add(k14);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle15.length - l + edge < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal4.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle15.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal4.x);
                             byteY1 = toBytes.intToBytes(horizontal4.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count12++;
                        rectangle15.length -= l;
                        isJudged = false;
                        return;
                    }

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
                    rectangle15.length -= l;
                    return;
                }     

                //如果走到这，说明第2层不能码了，该码放第3层了             

                if ((count13 == 0) && (width13 + edge >= l) && (length13 + edge >= w))
                {

                    wdf.changeText1("第三层");
                    Coordinate k19 = new Coordinate();
                    k19.x = 0;
                    k19.y = 0;
                    arrayList.Add(k19);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0,0,w,l);
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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
                    horizontal5.x = (w + gap);
                    horizontal5.y = 0;

                    rectangle18.length = width13 - l;
                    length13 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle18.length + edge >= l)
                {
                    Coordinate k20 = new Coordinate();
                    if ((rectangle18.length - l + edge) < 360)
                    {
                        if (rectangle18.length < l)
                        {
                            k20.x = vertical5.x;
                            k20.y = vertical5.y;
                            wdf.DrawOcupyArea(vertical5.x, vertical5.y, w, l);
                        }
                        else
                        {
                            k20.x = vertical5.x;
                            k20.y = 1000 - l;
                            wdf.DrawOcupyArea(vertical5.x, 1000 - l, w, l);
                        }
                    }
                    else
                    {
                        k20.x = vertical5.x;
                        k20.y = vertical5.y;
                        wdf.DrawOcupyArea(vertical5.x, vertical5.y, w, l);
                    }
                    
                    arrayList.Add(k20);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle18.length - l + edge) < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical5.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle18.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical5.x);
                             byteY1 = toBytes.intToBytes(vertical5.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count13++;
                        rectangle18.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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
                if ((length13 + edge >= w) && (!isJudged))
                {
                    Coordinate k21 = new Coordinate();
                    if (length13 - w + edge < 225)
                    {
                        if (length13 < w)
                        {
                            k21.x = horizontal5.x;
                            k21.y = horizontal5.y;
                            wdf.DrawOcupyArea(horizontal5.x, horizontal5.y, w, l);
                        }
                        else
                        {
                            k21.x = 1200 - w;
                            k21.y = horizontal5.y;
                            wdf.DrawOcupyArea(1200 - w, horizontal5.y, w, l);
                        }                   
                    }
                    else
                    {
                        k21.x = horizontal5.x;
                        k21.y = horizontal5.y;
                        wdf.DrawOcupyArea(horizontal5.x, horizontal5.y, w, l);
                    }
                    
                    arrayList.Add(k21);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }                   


                    if (length13 - w + edge < 225)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal5.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal5.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length13 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal5.y);
                            byteZ1 = toBytes.intToBytes(0);
                            horizontal5.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count13++;
                        rectangle19.length = width13 - l;
                        length13 -= (w + gap);
                        vertical5.x = horizontal5.x;
                        vertical5.y = l;
                        horizontal5.x += (w + gap);
                        horizontal5.y += 0;                      
                        isJudged = true;
                        return;
                    }
                    byte[] byteX = toBytes.intToBytes(horizontal5.x);
                    byte[] byteY = toBytes.intToBytes(horizontal5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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
                    horizontal5.x += (w + gap);
                    horizontal5.y += 0;
                    rectangle19.length = width13 - l;
                    length13 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle19.length + edge >= l)
                {
                    Coordinate k22 = new Coordinate();
                    if ((rectangle19.length - l + edge) < 360)
                    {
                        if (rectangle19.length < l)
                        {
                            k22.x = vertical5.x;
                            k22.y = vertical5.y;
                            wdf.DrawOcupyArea(vertical5.x, vertical5.y, w, l);
                        }
                        else
                        {
                            k22.x = vertical5.x;
                            k22.y = 1000 - l;
                            wdf.DrawOcupyArea(vertical5.x, 1000 - l, w, l);
                        }                       
                    }
                    else
                    {
                        k22.x = vertical5.x;
                        k22.y = vertical5.y;
                        wdf.DrawOcupyArea(vertical5.x, vertical5.y, w, l);
                    }
                    
                    arrayList.Add(k22);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle19.length - l + edge) < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical5.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle19.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical5.x);
                             byteY1 = toBytes.intToBytes(vertical5.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count13++;
                        rectangle19.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "0", "1", "0" };
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

                //第4层第一个
                if ((count14 == 0) && (length14 + edge >= l) && (width14 + edge >= w))
                {
                    wdf.changeText1("第四层");
                    Coordinate k29 = new Coordinate();
                    k29.x = 0;
                    k29.y = 0;
                    arrayList.Add(k29);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0,0,l,w);

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
                    vertical6.y = (w + gap);
                    horizontal6.x = l;
                    horizontal6.y = 0;

                    rectangle21.length = length14 - l;
                    width14 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle21.length >= l)
                {
                    Coordinate k30 = new Coordinate();
                    if (rectangle21.length - l + edge < 360)
                    {
                        if (rectangle21.length < l)
                        {
                            k30.x = horizontal6.x;
                            k30.y = horizontal6.y;
                            wdf.DrawOcupyArea(horizontal6.x, horizontal6.y, l, w);
                        }
                        else
                        {
                            k30.x = 1200 - l;
                            k30.y = horizontal6.y;
                            wdf.DrawOcupyArea(1200 - l, horizontal6.y, l, w);
                        }                      
                    }
                    else
                    {
                        k30.x = horizontal6.x;
                        k30.y = horizontal6.y;
                        wdf.DrawOcupyArea(horizontal6.x, horizontal6.y, l, w);
                    }
                    
                    arrayList.Add(k30);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle21.length - l + edge < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal6.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle21.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal6.x);
                             byteY1 = toBytes.intToBytes(horizontal6.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count14++;
                        rectangle21.length -= l;
                        return;
                    }

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
                //第一列第二个
                if ((width14 + edge>= w) && (!isJudged))
                {
                    Coordinate k31 = new Coordinate();
                    if (width14 - w + edge < 225)
                    {
                        if (width14 < w)
                        {
                            k31.x = vertical6.x;
                            k31.y = vertical6.y;
                            wdf.DrawOcupyArea(vertical6.x, vertical6.y, l, w);
                        }
                        else
                        {
                            k31.x = vertical6.x;
                            k31.y = 1000 - w;
                            wdf.DrawOcupyArea(vertical6.x, 1000 - w, l, w);
                        }
                    }
                    else
                    {
                        k31.x = vertical6.x;
                        k31.y = vertical6.y;
                        wdf.DrawOcupyArea(vertical6.x, vertical6.y, l, w);
                    }
                   
                    arrayList.Add(k31);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }               

                    if (width14 - w + edge < 225)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical6.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical6.y);                        
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width14 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical6.x);
                            byteY1 = toBytes.intToBytes(1000 - w);
                            byteZ1 = toBytes.intToBytes(0);
                            vertical6.y = 1000 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count14++;
                        rectangle22.length = length14 - l;
                        horizontal6.x = vertical6.x + l;
                        horizontal6.y = vertical6.y;
                        vertical6.x += 0;
                        vertical6.y += (w + gap);
                        isJudged = true;
                        isLastRowOrCol = true;
                        return;
                    }

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
                    vertical6.y += (w + gap);                  
                    rectangle22.length = length14 - l;
                    width14 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                if (rectangle22.length + edge >= l)
                {
                    Coordinate k32 = new Coordinate();
                    if (rectangle22.length - l + edge < 360)
                    {
                        if (rectangle22.length < l)
                        {
                            k32.x = horizontal6.x;
                            k32.y = horizontal6.y;
                            wdf.DrawOcupyArea(horizontal6.x, horizontal6.y, l, w);
                        }
                        else
                        {
                            k32.x = 1200 - l;
                            k32.y = horizontal6.y;
                            wdf.DrawOcupyArea(1200 - l, horizontal6.y, l, w);
                        }                     
                    }
                    else
                    {
                        k32.x = horizontal6.x;
                        k32.y = horizontal6.y;
                        wdf.DrawOcupyArea(horizontal6.x, horizontal6.y, l, w);
                    }
                   
                    arrayList.Add(k32);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle22.length - l + edge < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal6.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle22.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal6.x);
                             byteY1 = toBytes.intToBytes(horizontal6.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count14++;
                        rectangle22.length -= l;
                        isJudged = false;
                        if (isLastRowOrCol)
                        {
                            finish_1();
                            isLastRowOrCol = false;
                        }
                        return;
                    }

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
                if (count2 == 0)
                {
                    Coordinate k37 = new Coordinate();
                    k37.x = 0;
                    k37.y = 1500;
                    arrayList.Add(k37);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    wdf.DrawOcupyArea2(0, 0, w, l);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    //第二个码盘原点距第一个码盘原点1500mm
                    //所有后续的横坐标都要加上
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(1500);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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

                    vertical2.x = 0;
                    vertical2.y = l + 1500;
                    horizontal2.x = w + gap;
                    horizontal2.y = 1500;

                    rectangle6.length = 1000 - l;
                    length2 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle6.length + edge >= l)
                {
                    Coordinate k38 = new Coordinate();
                    if ((rectangle6.length - l + edge) < 390)
                    {
                        if (rectangle6.length < l)
                        {
                            k38.x = vertical2.x;
                            k38.y = vertical2.y;
                            wdf.DrawOcupyArea2(vertical2.x, vertical2.y - 1500, w, l);
                        }
                        else
                        {
                            k38.x = vertical2.x;
                            k38.y = 1500 + 1000 - l;
                            wdf.DrawOcupyArea2(vertical2.x, 1000 - l, w, l);
                        }                        
                        
                    }
                    else
                    {
                        k38.x = vertical2.x;
                        k38.y = vertical2.y;
                        wdf.DrawOcupyArea2(vertical2.x, vertical2.y - 1500, w, l);
                    }
                 
                    arrayList.Add(k38);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle6.length - l + edge) < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical2.x);
                        byte[] byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle6.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical2.x);
                             byteY1 = toBytes.intToBytes(vertical2.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count2++;
                        rectangle6.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical2.x);
                    byte[] byteY = toBytes.intToBytes(vertical2.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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

                    vertical2.x = 0;
                    vertical2.y += l;
                    rectangle6.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length2 + edge >= w) && (!isJudged))
                {
                    Coordinate k39 = new Coordinate();
                    if (length2 - w + edge < 240)
                    {
                        if (length2 < w)
                        {
                            k39.x = horizontal2.x;
                            k39.y = horizontal2.y;
                            wdf.DrawOcupyArea2(horizontal2.x, horizontal2.y - 1500, w, l);
                        }
                        else
                        {
                            k39.x = 1200 - w;
                            k39.y = horizontal2.y;
                            wdf.DrawOcupyArea2(1200 - w, horizontal2.y - 1500, w, l);
                        }                       
                    }
                    else
                    {
                        k39.x = horizontal2.x;
                        k39.y = horizontal2.y;
                        wdf.DrawOcupyArea2(horizontal2.x, horizontal2.y - 1500, w, l);
                    }
                    
                    arrayList.Add(k39);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }


                    if (length2 - w + edge < 240)
                    {                                            
                        byte[] byteX1 = toBytes.intToBytes(1200 - w);
                        byte[] byteY1 = toBytes.intToBytes(horizontal2.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length2 < w)
                        {
                             byteX1 = toBytes.intToBytes(horizontal2.x);
                             byteY1 = toBytes.intToBytes(horizontal2.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count2++;
                        vertical2.x = horizontal2.x;
                        vertical2.y = 1500 + l;
                        horizontal2.x += (w + gap);
                        horizontal2.y += 0;
                        rectangle7.length = width2 - l;
                        length2 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal2.x);
                    byte[] byteY = toBytes.intToBytes(horizontal2.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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
                    vertical2.y = 1500 + l;
                    horizontal2.x += (w + gap);
                    horizontal2.y += 0;
                    rectangle7.length = width2 - l;
                    length2 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle7.length + edge >= l)
                {
                    Coordinate k40 = new Coordinate();
                    if ((rectangle7.length - l + edge) < 390)
                    {
                        if (rectangle7.length < l)
                        {
                            k40.x = vertical2.x;
                            k40.y = vertical2.y;
                            wdf.DrawOcupyArea2(vertical2.x, vertical2.y - 1500, w, l);
                        }
                        else
                        {
                            k40.x = vertical2.x;
                            k40.y = 1500 + 1000 - l;
                            wdf.DrawOcupyArea2(vertical2.x, 1000 - l, w, l);
                        }                       
                    }
                    else
                    {
                        k40.x = vertical2.x;
                        k40.y = vertical2.y;
                        wdf.DrawOcupyArea2(vertical2.x, vertical2.y - 1500, w, l);
                    }
                   
                    arrayList.Add(k40);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle7.length - l + edge) < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical2.x);
                        byte[] byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle7.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical2.x);
                             byteY1 = toBytes.intToBytes(vertical2.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count2++;
                        rectangle7.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical2.x);
                    byte[] byteY = toBytes.intToBytes(vertical2.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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


                //如果走到这，说明第一层不能码了，该码放第二层了

                //第二层第一个
                if ((count22 == 0) && (length22 + edge >= l) && (width22 + edge >= w))
                {
                    wdf.changeText2("第二层");
                    Coordinate k47 = new Coordinate();
                    k47.x = 0;
                    k47.y = 1500;
                    arrayList.Add(k47);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0, 0, l, w);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(1500);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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

                    vertical7.x = 0;
                    vertical7.y = 1500 + (w + gap);
                    horizontal7.x = l;
                    horizontal7.y = 1500;

                    rectangle23.length = length22 - l;
                    width22 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle23.length + edge >= l)
                {
                    Coordinate k48 = new Coordinate();
                    if (rectangle23.length - l + edge < 390)
                    {
                        if (rectangle23.length < l)
                        {
                            k48.x = horizontal7.x;
                            k48.y = horizontal7.y;
                            wdf.DrawOcupyArea2(horizontal7.x, horizontal7.y - 1500, l, w);
                        }
                        else
                        {
                            k48.x = 1200 - l;
                            k48.y = horizontal7.y;
                            wdf.DrawOcupyArea2(1200 - l, horizontal7.y - 1500, l, w);
                        }                      
                    }
                    else
                    {
                        k48.x = horizontal7.x;
                        k48.y = horizontal7.y;
                        wdf.DrawOcupyArea2(horizontal7.x, horizontal7.y - 1500, l, w);
                    }
                    
                    arrayList.Add(k48);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle23.length - l + edge < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal7.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle23.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal7.x);
                             byteY1 = toBytes.intToBytes(horizontal7.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count22++;
                        rectangle23.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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
                //第一行第二个
                if ((width22 + edge >= w) && (!isJudged))
                {
                    Coordinate k49 = new Coordinate();
                    if (width22 - w + edge < 240)
                    {
                        if (width22 < w)
                        {
                            k49.x = vertical7.x;
                            k49.y = vertical7.y;
                            wdf.DrawOcupyArea2(vertical7.x, vertical7.y - 1500, l, w);
                        }
                        else
                        {
                            k49.x = vertical7.x;
                            k49.y = 1500 + 1000 - w;
                            wdf.DrawOcupyArea2(vertical7.x, 1000 - w, l, w);
                        }                      
                    }
                    else
                    {
                        k49.x = vertical7.x;
                        k49.y = vertical7.y;
                        wdf.DrawOcupyArea2(vertical7.x, vertical7.y - 1500, l, w);
                    }
                    
                    arrayList.Add(k49);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }                    

                    if (width22 - w + edge < 240)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical7.x);
                        byte[] byteY1 = toBytes.intToBytes(1500 + 1000 - w);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width22 < w)
                        {
                             byteX1 = toBytes.intToBytes(vertical7.x);
                             byteY1 = toBytes.intToBytes(vertical7.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count22++;
                        horizontal7.x = vertical7.x + l;
                        horizontal7.y = vertical7.y;
                        vertical7.x += 0;
                        vertical7.y += (w + gap);
                        rectangle24.length = length22 - l;
                        width22 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical7.x);
                    byte[] byteY = toBytes.intToBytes(vertical7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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
                    vertical7.y += (w + gap);
                    rectangle24.length = length22 - l;
                    width22 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle24.length + edge >= l)
                {
                    Coordinate k50 = new Coordinate();
                    if (rectangle24.length - l + edge < 390)
                    {
                        if (rectangle24.length < l)
                        {
                            k50.x = horizontal7.x;
                            k50.y = horizontal7.y;
                            wdf.DrawOcupyArea2(horizontal7.x, horizontal7.y - 1500, l, w);
                        }
                        else
                        {
                            k50.x = 1200 - l;
                            k50.y = horizontal7.y;
                            wdf.DrawOcupyArea2(1200 - l, horizontal7.y - 1500, l, w);
                        }                       
                    }
                    else
                    {
                        k50.x = horizontal7.x;
                        k50.y = horizontal7.y;
                        wdf.DrawOcupyArea2(horizontal7.x, horizontal7.y - 1500, l, w);
                    }
                   
                    arrayList.Add(k50);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle24.length - l + edge < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal7.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle24.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal7.x);
                             byteY1 = toBytes.intToBytes(horizontal7.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count22++;
                        rectangle24.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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

                //如果走到这，说明第2层不能码了，该码放第3层了

                if ((count23 == 0) && (width23 + edge >= l) && (length23 + edge >= w))
                {
                    wdf.changeText2("第三层");
                    Coordinate k55 = new Coordinate();
                    k55.x = 0;
                    k55.y = 1500;
                    arrayList.Add(k55);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0,0,w,l);

                    //第一个箱子直接放在原点(0,1500,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(1500);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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

                    vertical8.x = 0;
                    vertical8.y = 1500 + l;
                    horizontal8.x = w + gap;
                    horizontal8.y = 1500;

                    rectangle27.length = width23 - l;
                    length23 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle27.length + edge >= l)
                {
                    Coordinate k56 = new Coordinate();
                    if ((rectangle27.length - l + edge) < 390)
                    {
                        if (rectangle27.length < l)
                        {
                            k56.x = vertical8.x;
                            k56.y = vertical8.y;
                            wdf.DrawOcupyArea2(vertical8.x, vertical8.y - 1500, w, l);
                        }
                        else
                        {
                            k56.x = vertical8.x;
                            k56.y = 1500 + 1000 - l;
                            wdf.DrawOcupyArea2(vertical8.x,1000 - l, w, l);
                        }                    
                    }
                    else
                    {
                        k56.x = vertical8.x;
                        k56.y = vertical8.y;
                        wdf.DrawOcupyArea2(vertical8.x, vertical8.y - 1500, w, l);
                    }
                   
                    arrayList.Add(k56);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle27.length - l + edge) < 390)
                    {

                        byte[] byteX1 = toBytes.intToBytes(vertical8.x);
                        byte[] byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle27.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical8.x);
                             byteY1 = toBytes.intToBytes(vertical8.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count23++;
                        rectangle27.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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
                if ((length23 + edge >= w) && (!isJudged))
                {
                    Coordinate k57 = new Coordinate();
                    if (length23 - w + edge < 240)
                    {
                        if (length23 < w)
                        {
                            k57.x = horizontal8.x;
                            k57.y = horizontal8.y;
                            wdf.DrawOcupyArea2(horizontal8.x, horizontal8.y - 1500, w, l);
                        }
                        else
                        {
                            k57.x = 1200 - w;
                            k57.y = horizontal8.y;
                            wdf.DrawOcupyArea2(1200 - w, horizontal8.y - 1500, w, l);
                        }                      
                    }
                    else
                    {
                        k57.x = horizontal8.x;
                        k57.y = horizontal8.y;
                        wdf.DrawOcupyArea2(horizontal8.x, horizontal8.y - 1500, w, l);
                    }
                    
                    arrayList.Add(k57);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }                   
                   
                    if (length23 - w + edge < 240)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - w);
                        byte[] byteY1 = toBytes.intToBytes(horizontal8.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length23 < w)
                        {
                             byteX1 = toBytes.intToBytes(horizontal8.x);
                             byteY1 = toBytes.intToBytes(horizontal8.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count23++;
                        vertical8.x = horizontal8.x;
                        vertical8.y = 1500 + l;
                        horizontal8.x += (w + gap);
                        horizontal8.y += 0;
                        rectangle28.length = width23 - l;
                        length23 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal8.x);
                    byte[] byteY = toBytes.intToBytes(horizontal8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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
                    vertical8.y = 1500 + l;
                    horizontal8.x += (w + gap);
                    horizontal8.y += 0;
                    rectangle28.length = width23 - l;
                    length23 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle28.length + edge >= l)
                {
                    Coordinate k58 = new Coordinate();
                    if ((rectangle28.length - l + edge) < 390)
                    {
                        if (rectangle28.length < l)
                        {
                            k58.x = vertical8.x;
                            k58.y = vertical8.y;
                            wdf.DrawOcupyArea2(vertical8.x, vertical8.y - 1500, w, l);
                        }
                        else
                        {
                            k58.x = vertical8.x;
                            k58.y = 1500 + 1000 - l;
                            wdf.DrawOcupyArea2(vertical8.x, 1000 - l, w, l);
                        }                      
                    }
                    else
                    {
                        k58.x = vertical8.x;
                        k58.y = vertical8.y;
                        wdf.DrawOcupyArea2(vertical8.x, vertical8.y - 1500, w, l);
                    }
                  
                    arrayList.Add(k58);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle28.length - l + edge) < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical8.x);
                        byte[] byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle28.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical8.x);
                             byteY1 = toBytes.intToBytes(vertical8.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count23++;
                        rectangle28.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "0" };
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
           

                //如果走到这，说明第3层不能码了，该码放第4层了

                //第4层第一个
                if ((count24 == 0) && (length24 + edge >= l) && (width24 + edge >= w))
                {
                    wdf.changeText2("第四层");
                    Coordinate k65 = new Coordinate();
                    k65.x = 0;
                    k65.y = 1500;
                    arrayList.Add(k65);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0,0,l,w);

                    //第一个箱子直接放在原点(0,1500,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(1400);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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

                    vertical9.x = 0;
                    vertical9.y = 1500 + w + gap;
                    horizontal9.x = l;
                    horizontal9.y = 1500;

                    rectangle30.length = length24 - l;
                    width24 -= (w + gap);
                    return;
                }
                //第一行第二个及以上
                if (rectangle30.length + edge >= l)
                {
                    Coordinate k66 = new Coordinate();
                    if (rectangle30.length - l + edge < 390)
                    {
                        if (rectangle30.length < l)
                        {
                            k66.x = horizontal9.x;
                            k66.y = horizontal9.y;
                            wdf.DrawOcupyArea2(horizontal9.x, horizontal9.y - 1500, l, w);
                        }
                        else
                        {
                            k66.x = 1200 - l;
                            k66.y = horizontal9.y;
                            wdf.DrawOcupyArea2(1200 - l, horizontal9.y - 1500, l, w);
                        }                        
                    }
                    else
                    {
                        k66.x = horizontal9.x;
                        k66.y = horizontal9.y;
                        wdf.DrawOcupyArea2(horizontal9.x, horizontal9.y - 1500, l, w);
                    }
                
                    arrayList.Add(k66);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle30.length - l + edge < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal9.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle30.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal9.x);
                             byteY1 = toBytes.intToBytes(horizontal9.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count24++;
                        rectangle30.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal9.x);
                    byte[] byteY = toBytes.intToBytes(horizontal9.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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
                //第一列第二个
                if ((width24 + edge >= w) && (!isJudged))
                {
                    Coordinate k67 = new Coordinate();
                    if (width24 - w + edge < 240)
                    {
                        if (width24 < w)
                        {
                            k67.x = vertical9.x;
                            k67.y = vertical9.y;
                            wdf.DrawOcupyArea2(vertical9.x, vertical9.y - 1500, l, w);
                        }
                        else
                        {
                            k67.x = vertical9.x;
                            k67.y = 1500 + 1000 - w;
                            wdf.DrawOcupyArea2(vertical9.x, 1000 - w, l, w);
                        }                   
                    }
                    else
                    {
                        k67.x = vertical9.x;
                        k67.y = vertical9.y;
                        wdf.DrawOcupyArea2(vertical9.x, vertical9.y - 1500, l, w);
                    }
                   
                    arrayList.Add(k67);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }
                    

                    if (width24 - w + edge < 240)
                    {                       
                        byte[] byteX1 = toBytes.intToBytes(vertical9.x);
                        byte[] byteY1 = toBytes.intToBytes(1500 + 1000 - w);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width24 < w)
                        {
                             byteX1 = toBytes.intToBytes(vertical9.x);
                             byteY1 = toBytes.intToBytes(vertical9.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count24++;
                        horizontal9.x = vertical9.x + l;
                        horizontal9.y = vertical9.y;
                        vertical9.x += 0;
                        vertical9.y += (w + gap);
                        rectangle31.length = length24 - l;
                        width24 -= (w + gap);
                        isJudged = true;
                        isLastRowOrCol = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical9.x);
                    byte[] byteY = toBytes.intToBytes(vertical9.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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
                    vertical9.y += (w + gap);
                    rectangle31.length = length24 - l;
                    width24 -= (w + gap);
                    isJudged = true;                   
                    return;
                }
                //第二行第二个及以上
                if (rectangle31.length + edge >= l)
                {
                    Coordinate k68 = new Coordinate();
                    if (rectangle31.length - l + edge < 390)
                    {
                        if (rectangle31.length < l)
                        {
                            k68.x = horizontal9.x;
                            k68.y = horizontal9.y;
                            wdf.DrawOcupyArea2(horizontal9.x, horizontal9.y - 1500, l, w);
                        }
                        else
                        {
                            k68.x = 1200 - l;
                            k68.y = horizontal9.y;
                            wdf.DrawOcupyArea2(1200 - l, horizontal9.y - 1500, l, w);
                        }
                    }
                    else
                    {
                        k68.x = horizontal9.x;
                        k68.y = horizontal9.y;
                        wdf.DrawOcupyArea2(horizontal9.x, horizontal9.y - 1500, l, w);
                    }
                    
                    arrayList.Add(k68);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle31.length - l + edge < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal9.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle31.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal9.x);
                             byteY1 = toBytes.intToBytes(horizontal9.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count24++;
                        rectangle31.length -= l;
                        isJudged = false;
                        if (isLastRowOrCol)
                        {
                            finish_2();
                            isLastRowOrCol = false;
                        }
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal9.x);
                    byte[] byteY = toBytes.intToBytes(horizontal9.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "0", "1" };
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
                if (count3 == 0)
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = 0;
                    k73.y = 3000;
                    arrayList.Add(k73);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }
                    wdf.DrawOcupyArea3(0, 0, l, w);

                    //第一个箱子直接放在原点(0,3000,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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

                    vertical3.x = 0;
                    vertical3.y = 3000 + (w + gap);
                    horizontal3.x = l;
                    horizontal3.y = 3000;

                    rectangle11.length = 1200 - l;
                    width3 -= (w + gap);
                    return;
                }
                //第一行第二个及以上
                if (rectangle11.length + edge2 >= l)
                {
                    Coordinate k74 = new Coordinate();
                    if (rectangle11.length - l + edge2 < 550)
                    {
                        if (rectangle11.length < l)
                        {
                            k74.x = horizontal3.x;
                            k74.y = horizontal3.y;
                            wdf.DrawOcupyArea3(horizontal3.x, horizontal3.y - 3000, l, w);
                        }
                        else
                        {
                            k74.x = 1200 - l;
                            k74.y = horizontal3.y;
                            wdf.DrawOcupyArea3(1200 - l, horizontal3.y - 3000, l, w);
                        }                    
                    }
                    else
                    {
                        k74.x = horizontal3.x;
                        k74.y = horizontal3.y;
                        wdf.DrawOcupyArea3(horizontal3.x, horizontal3.y - 3000, l, w);
                    }

                    arrayList.Add(k74);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle11.length - l + edge2 < 550)
                    {

                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal3.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle11.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal3.x);
                             byteY1 = toBytes.intToBytes(horizontal3.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }                                             
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count3++;
                        rectangle11.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal3.x);
                    byte[] byteY = toBytes.intToBytes(horizontal3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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
                //第一列第二个
                if ((width3 + edge2 >= w) && (!isJudged))
                {
                    Coordinate k75 = new Coordinate();
                    if (width3 - w + edge2 < 320)
                    {
                        k75.x = vertical3.x;
                        k75.y = 3000 + 1000 - w;
                        wdf.DrawOcupyArea3(vertical3.x, 1000 - w, l, w);
                    }
                    else
                    {
                        k75.x = vertical3.x;
                        k75.y = vertical3.y;
                        wdf.DrawOcupyArea3(vertical3.x, vertical3.y - 3000, l, w);
                    }

                    arrayList.Add(k75);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }


                    if (width3 - w + edge2 < 320)
                    {
                        vertical3.y =3000 + 1000 - w;
                        byte[] byteX1 = toBytes.intToBytes(vertical3.x);
                        byte[] byteY1 = toBytes.intToBytes(4000 - w);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count3++;
                        horizontal3.x = vertical3.x + l;
                        horizontal3.y = vertical3.y;
                        vertical3.x += 0;
                        vertical3.y += (w + gap);
                        rectangle12.length = 1200 - l;
                        width3 -= (w + gap);
                        if (rectangle12.length + edge2 < l)
                        {
                            return;
                        }
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical3.x);
                    byte[] byteY = toBytes.intToBytes(vertical3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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
                    vertical3.y += (w + gap);
                    rectangle12.length = 1200 - l;
                    width3 -= (w + gap);
                    //可能后面码放不下，则重复执行每列或每行第一个纸箱的码放
                    if (rectangle12.length + edge2 < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle12.length + edge2 >= l)
                {
                    Coordinate k76 = new Coordinate();
                    if (rectangle12.length - l + edge2 < 550)
                    {
                        if (rectangle12.length < l)
                        {
                            k76.x = horizontal3.x;
                            k76.y = horizontal3.y;
                            wdf.DrawOcupyArea3(horizontal3.x, horizontal3.y - 3000, l, w);
                        }
                        else
                        {
                            k76.x = 1200 - l;
                            k76.y = horizontal3.y;
                            wdf.DrawOcupyArea3(1200 - l, horizontal3.y - 3000, l, w);
                        }                      
                    }
                    else
                    {
                        k76.x = horizontal3.x;
                        k76.y = horizontal3.y;
                        wdf.DrawOcupyArea3(horizontal3.x, horizontal3.y - 3000, l, w);
                    }

                    arrayList.Add(k76);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle12.length - l + edge2 < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal3.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);

                        if (rectangle12.length < l)
                        {
                            byteX1 = toBytes.intToBytes(horizontal3.x);
                            byteY1 = toBytes.intToBytes(horizontal3.y);
                            byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count3++;
                        rectangle12.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal3.x);
                    byte[] byteY = toBytes.intToBytes(horizontal3.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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

                //如果走到这，说明第1层不能码了，该码放第2层了                         

                if ((count32 == 0) && (width32 + edge2 >= l) && (length32 + edge2 >= w))
                {
                    wdf.changeText3("第二层");
                    Coordinate k79 = new Coordinate();
                    k79.x = 0;
                    k79.y = 3000;
                    arrayList.Add(k79);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    wdf.DrawOcupyArea3(0, 0, w, l);

                    //第一个箱子直接放在原点(0,3000,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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

                    vertical10.x = 0;
                    vertical10.y = 3000 + l;
                    horizontal10.x = w + gap;
                    horizontal10.y = 3000;

                    rectangle32.length = width32 - l;
                    length32 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle32.length + edge2 >= l)
                {
                    Coordinate k80 = new Coordinate();
                    if ((rectangle32.length - l + 120) < 550)
                    {
                        if (rectangle32.length < l)
                        {
                            k80.x = vertical10.x;
                            k80.y = vertical10.y;
                            wdf.DrawOcupyArea3(vertical10.x, vertical10.y - 3000, w, l);
                        }
                        else
                        {
                            k80.x = vertical10.x;
                            k80.y = 4000 - l;
                            wdf.DrawOcupyArea3(vertical10.x, 1000 - l, w, l);
                        }                     
                    }
                    else
                    {
                        k80.x = vertical10.x;
                        k80.y = vertical10.y;
                        wdf.DrawOcupyArea3(vertical10.x, vertical10.y - 3000, w, l);
                    }

                    arrayList.Add(k80);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle32.length - l + edge2) < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical10.x);
                        byte[] byteY1 = toBytes.intToBytes(4000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);

                        if (rectangle32.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical10.x);
                             byteY1 = toBytes.intToBytes(vertical10.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }

                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count32++;
                        rectangle32.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical10.x);
                    byte[] byteY = toBytes.intToBytes(vertical10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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
                if ((length32 + edge2 >= w) && (!isJudged))
                {
                    Coordinate k81 = new Coordinate();
                    if ((length32 - w + edge2) < 320)
                    {
                        if (length32 < w)
                        {
                            k81.x = horizontal10.x;
                            k81.y = horizontal10.y;
                            wdf.DrawOcupyArea3(horizontal10.x, horizontal10.y - 3000, w, l);
                        }
                        else
                        {
                            k81.x = 1200 - w;
                            k81.y = horizontal10.y;
                            wdf.DrawOcupyArea3(1200 - w, horizontal10.y - 3000, w, l);
                        }                       
                    }
                    else
                    {
                        k81.x = horizontal10.x;
                        k81.y = horizontal10.y;
                        wdf.DrawOcupyArea3(horizontal10.x, horizontal10.y - 3000, w, l);
                    }

                    arrayList.Add(k81);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }


                    if ((length32 - w + edge2) < 320)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - w);
                        byte[] byteY1 = toBytes.intToBytes(horizontal10.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length32 < w)
                        {
                             byteX1 = toBytes.intToBytes(horizontal10.x);
                             byteY1 = toBytes.intToBytes(horizontal10.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                       
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count32++;
                        vertical10.x = horizontal10.x;
                        vertical10.y = 3000 + l;
                        horizontal10.x += (w + gap);
                        horizontal10.y += 0;
                        rectangle33.length = width32 - l;
                        length32 -= (w + gap);
                        if (rectangle33.length + edge < l)
                        {
                            return;
                        }
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal10.x);
                    byte[] byteY = toBytes.intToBytes(horizontal10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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
                    vertical10.y = 3000 + l;
                    horizontal10.x += (w + gap);
                    horizontal10.y += 0;
                    rectangle33.length = width32 - l;
                    length32 -= (w + gap);
                    if (rectangle33.length + edge2 < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                if (rectangle33.length + edge2 >= l)
                {
                    Coordinate k82 = new Coordinate();
                    if ((rectangle33.length - l + edge2) < 550)
                    {
                        if (rectangle33.length < l)
                        {
                            k82.x = vertical10.x;
                            k82.y = vertical10.y;
                            wdf.DrawOcupyArea3(vertical10.x, vertical10.y - 3000, w, l);
                        }
                        else
                        {
                            k82.x = vertical10.x;
                            k82.y = 4000 - l;
                            wdf.DrawOcupyArea3(vertical10.x, 1000 - l, w, l);
                        }                      
                    }
                    else
                    {
                        k82.x = vertical10.x;
                        k82.y = vertical10.y;
                        wdf.DrawOcupyArea3(vertical10.x, vertical10.y - 3000, w, l);
                    }

                    arrayList.Add(k82);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle33.length - l + edge2) < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical10.x);
                        byte[] byteY1 = toBytes.intToBytes(4000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle33.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical10.x);
                             byteY1 = toBytes.intToBytes(vertical10.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count32++;
                        rectangle33.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical10.x);
                    byte[] byteY = toBytes.intToBytes(vertical10.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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

                //如果走到这，说明第2层不能码了，该码放第3层了         

                //第3层第一个
                if ((count33 == 0) && (length33 + edge2 >= l) && (width33 + edge2 >= w))
                {
                    wdf.changeText3("第三层");
                    Coordinate k87 = new Coordinate();
                    k87.x = 0;
                    k87.y = 3000;
                    arrayList.Add(k87);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    wdf.DrawOcupyArea3(0, 0, l, w);

                    //第一个箱子直接放在原点(0,3000,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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

                    vertical11.x = 0;
                    vertical11.y = 3000 + w + gap;
                    horizontal11.x = l;
                    horizontal11.y = 3000;

                    rectangle35.length = length33 - l;
                    width33 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle35.length + edge2 >= l)
                {
                    Coordinate k88 = new Coordinate();
                    if (rectangle35.length - l + edge2 < 550)
                    {
                        if (rectangle35.length < l)
                        {
                            k88.x = horizontal11.x;
                            k88.y = horizontal11.y;
                            wdf.DrawOcupyArea3(horizontal11.x, horizontal11.y - 3000, l, w);
                        }
                        else
                        {
                            k88.x = 1200 - l;
                            k88.y = horizontal11.y;
                            wdf.DrawOcupyArea3(1200 - l, horizontal11.y - 3000, l, w);
                        }
                        
                    }
                    else
                    {
                        k88.x = horizontal11.x;
                        k88.y = horizontal11.y;
                        wdf.DrawOcupyArea3(horizontal11.x - 2800, horizontal11.y - 3000, l, w);
                    }

                    arrayList.Add(k88);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle35.length - l + edge2 < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(2800 + 1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal11.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle35.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal11.x);
                             byteY1 = toBytes.intToBytes(horizontal11.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count33++;
                        rectangle35.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal11.x);
                    byte[] byteY = toBytes.intToBytes(horizontal11.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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
                //第一列第二个
                if ((width33 + edge >= w) && (!isJudged))
                {
                    Coordinate k89 = new Coordinate();
                    if (width33 - w + edge < 320)
                    {
                        k89.x = vertical11.x;
                        k89.y = 4000 - w;
                        wdf.DrawOcupyArea3(vertical11.x, 1000 - w, l, w);
                    }
                    else
                    {
                        k89.x = vertical11.x;
                        k89.y = vertical11.y;
                        wdf.DrawOcupyArea3(vertical11.x, vertical11.y - 3000, l, w);
                    }

                    arrayList.Add(k89);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (width33 - w + edge < 320)
                    {
                        vertical11.y = 4000 - w;
                        byte[] byteX1 = toBytes.intToBytes(vertical11.x);
                        byte[] byteY1 = toBytes.intToBytes(4000 - w);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count33++;
                        horizontal11.x = vertical11.x + l;
                        horizontal11.y = vertical11.y;
                        vertical11.x += 0;
                        vertical11.y += (w + gap);
                        rectangle36.length = length33 - l;
                        width33 -= (w + gap);
                        if (rectangle36.length + edge < l)
                        {
                            return;
                        }
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical11.x);
                    byte[] byteY = toBytes.intToBytes(vertical11.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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
                    vertical11.y += (w + gap);
                    rectangle36.length = length33 - l;
                    width33 -= (w + gap);
                    if (rectangle36.length + edge < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                if (rectangle36.length + edge >= l)
                {
                    Coordinate k90 = new Coordinate();
                    if (rectangle36.length - l + edge < 550)
                    {
                        if (rectangle36.length < l)
                        {
                            k90.x = horizontal11.x;
                            k90.y = horizontal11.y;
                            wdf.DrawOcupyArea3(horizontal11.x, horizontal11.y - 3000, l, w);
                        }
                        else
                        {
                            k90.x = 1200 - l;
                            k90.y = horizontal11.y;
                            wdf.DrawOcupyArea3(1200 - l, horizontal11.y - 3000, l, w);
                        }                       
                    }
                    else
                    {
                        k90.x = horizontal11.x;
                        k90.y = horizontal11.y;
                        wdf.DrawOcupyArea3(horizontal11.x, horizontal11.y - 3000, l, w);
                    }

                    arrayList.Add(k90);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle36.length - l + edge < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal11.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle36.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal11.x);
                             byteY1 = toBytes.intToBytes(horizontal11.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count33++;
                        rectangle36.length -= l;
                        isJudged = false;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal11.x);
                    byte[] byteY = toBytes.intToBytes(horizontal11.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "1" };
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

                //第4层第一个
                if ((count34 == 0) && (length34 + edge2 >= l) && (width34 + edge2 >= w))
                {
                    wdf.changeText3("第四层");
                    Coordinate k93 = new Coordinate();
                    k93.x = 0;
                    k93.y = 3000;
                    arrayList.Add(k93);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    wdf.DrawOcupyArea3(0, 0, w, l);

                    //第一个箱子直接放在原点(2800,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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

                    vertical12.x = 0;
                    vertical12.y = 3000 + l;
                    horizontal12.x = w + gap;
                    horizontal12.y = 3000;

                    rectangle37.length = width34 - l;
                    length34 -= (w + gap);
                    return;
                }
                //第一列第二个及以上
                if (rectangle37.length + edge2 >= l)
                {
                    Coordinate k94 = new Coordinate();
                    if ((rectangle37.length - l + edge2) < 550)
                    {
                        if (rectangle37.length < l)
                        {
                            k94.x = vertical12.x;
                            k94.y = vertical12.y;
                            wdf.DrawOcupyArea3(vertical12.x, vertical12.y - 3000, w, l);
                        }
                        else
                        {
                            k94.x = vertical12.x;
                            k94.y = 4000 - l;
                            wdf.DrawOcupyArea3(vertical12.x, 1000 - l, w, l);
                        }                      
                    }
                    else
                    {
                        k94.x = vertical12.x;
                        k94.y = vertical12.y;
                        wdf.DrawOcupyArea3(vertical12.x, vertical12.y - 3000, w, l);
                    }

                    arrayList.Add(k94);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle37.length - l + edge) < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical12.x);
                        byte[] byteY1 = toBytes.intToBytes(4000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle37.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical12.x);
                             byteY1 = toBytes.intToBytes(vertical12.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count34++;
                        rectangle37.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal12.x);
                    byte[] byteY = toBytes.intToBytes(horizontal12.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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

                    vertical12.x += 0;
                    vertical12.y += l;
                    rectangle37.length -= l;
                    return;
                }
                //第一行第二个
                if ((length34 + edge2 >= w) && (!isJudged))
                {
                    Coordinate k95 = new Coordinate();
                    if ((length34 - w + edge2) < 320)
                    {
                        if (length34 < w)
                        {
                            k95.x = horizontal12.x;
                            k95.y = horizontal12.y;
                            wdf.DrawOcupyArea3(horizontal12.x, horizontal12.y - 3000, w, l);
                        }
                        else
                        {
                            k95.x = 1200 - w;
                            k95.y = horizontal12.y;
                            wdf.DrawOcupyArea3(1200 - w, horizontal12.y - 3000, w, l);
                        }                     
                    }
                    else
                    {
                        k95.x = horizontal12.x;
                        k95.y = horizontal12.y;
                        wdf.DrawOcupyArea3(horizontal12.x, horizontal12.y - 3000, w, l);
                    }

                    arrayList.Add(k95);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((length34 - w + edge2) < 320)
                    {                       
                        byte[] byteX1 = toBytes.intToBytes(1200 - w);
                        byte[] byteY1 = toBytes.intToBytes(horizontal12.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length34 < w)
                        {
                             byteX1 = toBytes.intToBytes(horizontal12.x);
                             byteY1 = toBytes.intToBytes(horizontal12.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count34++;
                        vertical12.x = horizontal12.x;
                        vertical12.y = 3000 + l;
                        horizontal12.x += (w + gap);
                        horizontal12.y += 0;
                        rectangle38.length = width34 - l;
                        length34 -= (w + gap);
                        if (rectangle38.length + edge2 < l)
                        {
                            finish_3();
                            return;
                        }
                        isJudged = true;
                        isLastRowOrCol = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal12.x);
                    byte[] byteY = toBytes.intToBytes(horizontal12.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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

                    vertical12.x = horizontal12.x;
                    vertical12.y = 3000 + l;
                    horizontal12.x += (w + gap);
                    horizontal12.y += 0;
                    rectangle38.length = width34 - l;
                    length34 -= (w + gap);
                    if (rectangle38.length + edge2 < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;

                }
                //第二列第二个及以上
                if (rectangle38.length + edge2 >= l)
                {
                    Coordinate k96 = new Coordinate();
                    if ((rectangle38.length - l + edge2) < 550)
                    {
                        if (rectangle38.length < l)
                        {
                            k96.x = vertical12.x;
                            k96.y = vertical12.y;
                            wdf.DrawOcupyArea3(vertical12.x, vertical12.y - 3000, w, l);
                        }
                        else
                        {
                            k96.x = vertical12.x;
                            k96.y = 4000 - l;
                            wdf.DrawOcupyArea3(vertical12.x, 1000 - l, w, l);
                        }
                        
                    }
                    else
                    {
                        k96.x = vertical12.x;
                        k96.y = vertical12.y;
                        wdf.DrawOcupyArea3(vertical12.x, vertical12.y - 3000, w, l);
                    }

                    arrayList.Add(k96);
                    if (arrayList.Count == 2)
                    {
                        CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle38.length - l + edge2) < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical12.x);
                        byte[] byteY1 = toBytes.intToBytes(4000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle38.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical12.x);
                             byteY1 = toBytes.intToBytes(vertical12.y);
                             byteZ1 = toBytes.intToBytes(0);
                        }
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = byteX1[3];
                        BF.sendbuf[3] = byteX1[2];
                        BF.sendbuf[4] = byteX1[1];
                        BF.sendbuf[5] = byteX1[0];
                        BF.sendbuf[6] = byteY1[3];
                        BF.sendbuf[7] = byteY1[2];
                        BF.sendbuf[8] = byteY1[1];
                        BF.sendbuf[9] = byteY1[0];
                        BF.sendbuf[10] = byteZ1[3];
                        BF.sendbuf[11] = byteZ1[2];
                        BF.sendbuf[12] = byteZ1[1];
                        BF.sendbuf[13] = byteZ1[0];
                        BF.sendbuf[14] = b1[3];
                        BF.sendbuf[15] = b1[2];
                        BF.sendbuf[16] = b1[1];
                        BF.sendbuf[17] = b1[0];
                        BF.sendbuf[18] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 19);
                        count34++;
                        rectangle38.length -= l;
                        isJudged = false;
                        if (isLastRowOrCol)
                        {
                            finish_3();
                        }
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical12.x);
                    byte[] byteY = toBytes.intToBytes(vertical12.y);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "0", "0" };
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

                    vertical12.x += 0;
                    vertical12.y += l;
                    rectangle38.length -= l;
                    return;
                }
            }    
           
        }       

        /// <summary>
        /// 码盘摆满
        /// </summary>
        public static void finish_1()
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x04;
            BF.sendbuf[2] = 0x0C;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x09;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = 0xF5;
            SendMenuCommand(BF.sendbuf, 7);
            MessageBox.Show("码盘1码垛完成,请移走!", "警告");
        }

        public static void finish_2()
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x04;
            BF.sendbuf[2] = 0x0C;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x0A;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = 0xF5;
            SendMenuCommand(BF.sendbuf, 7);
            MessageBox.Show("码盘2码垛完成,请移走!", "警告");
        }

        public static void finish_3()
        {
            BF.sendbuf[0] = 0xFA;
            BF.sendbuf[1] = 0x04;
            BF.sendbuf[2] = 0x0C;
            BF.sendbuf[3] = 0x01;
            BF.sendbuf[4] = 0x0B;
            BF.sendbuf[5] = 0x01;
            BF.sendbuf[6] = 0xF5;
            SendMenuCommand(BF.sendbuf, 7);
            MessageBox.Show("码盘3码垛完成,请移走!", "警告");
        }


        /// <summary>
        /// 开启一个线程，每隔一段时间请求坐标
        /// </summary>
        public static void StartThread()
        {
            Thread thread = new Thread(new ThreadStart(GetCoordinate));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
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


        /// <summary>
        /// 开启一个线程，时刻查询有无报警信息
        /// </summary>
        public static void searchAlarmInfo()
        {
            Thread thread = new Thread(new ThreadStart(requestAlrInfo));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
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

        /// <summary>
        /// 开启一个线程，查询是否码垛完成
        /// </summary>
        public static void confirmCompleted()
        {
            Thread thread = new Thread(new ThreadStart(requestStatus));
            thread.Priority = ThreadPriority.BelowNormal;
            thread.IsBackground = true;
            thread.Start();
        }
        public static void requestStatus()
        {
            while (completed)
            {
                // xinlei = false;
                //发送指令查看码垛是否完成
                Thread.Sleep(1131);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x05;
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

                //下发指令的应答
                if (binary_data_1[2] == 0x0D)
                {
                    if(binary_data_1[3] == 0x01)
                    {
                        WorkingDetailForm.isReceived1 = true;
                    }
                    if(binary_data_1[3] == 0x02)
                    {
                        WorkingDetailForm.isReceived2 = true;
                    }
                    if (binary_data_1[3] == 0x03)
                    {
                        WorkingDetailForm.isReceived3 = true;
                    }
                    if (binary_data_1[3] == 0x04)
                    {
                        WorkingDetailForm.isReceived4 = true;
                    }
                }



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

                //模拟收到的纸箱数据
                if((binary_data_1[1] == 0x0D) && (binary_data_1[2] == 0XAA))
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

                    l = x_value;
                    w = y_value;
                    h = z_value;

                    xinlei = false;
                    fight = false;
                    completed = false;
                    SendMaduoInfo();                   
                    xinlei = true;
                    fight = true;
                    completed = true;
                }

                //码垛完成则收到数据
                if ((binary_data_1[1] == 0x02) && (binary_data_1[2] == 0x0F))
                {
                    wdf.GetTotalNum(++totalNum);
                    return;
                }

                //接收扫码枪数据(若未检测到扫码信息则发送告警指令提醒人工扫码)

            }
        }
    }
}