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
        public static SerialPort sp2 = new SerialPort();
        // public static SerialPort sp3 = new SerialPort();

        public static class BF
        {
            public static byte[] sendbuf = new byte[100];
        }

        public HomeForm()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);
            InitializeComponent();
            AutoScale(this);
           //  StartThread();
           // searchAlarmInfo();
           // confirmCompleted();
            initialIOSetting();
        }

        public void AutoScale(Form frm)
        {
            frm.Tag = frm.Width.ToString() + "," + frm.Height.ToString();
            frm.SizeChanged += new EventHandler(frm_SizeChanged);
        }

        //动态的长宽缩放比例
        public static float change_l = 1;
        public static float change_w = 1;

        void frm_SizeChanged(object sender, EventArgs e)
        {
            string[] tmp = ((Form)sender).Tag.ToString().Split(',');
            float width = (float)((Form)sender).Width / (float)Convert.ToInt16(tmp[0]);
            float heigth = (float)((Form)sender).Height / (float)Convert.ToInt16(tmp[1]);

            change_l = heigth;
            change_w = width;

            ((Form)sender).Tag = ((Form)sender).Width.ToString() + "," + ((Form)sender).Height;

            this.work_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.historydata_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.setting_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.alarmhistory_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.SysTime.Font = new System.Drawing.Font("黑体", 10.5F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
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
            this.WindowState = FormWindowState.Maximized;
            //初始化加载工作界面
            work_btn.BackColor = Color.FromArgb(65, 105, 225);
            wdf = new WorkingDetailForm();
            wdf.TopLevel = false;
            panel1.Controls.Add(wdf);
            wdf.Show();

            ahf = new AlarmHistoryForm();
            ahf.TopLevel = false;
            panel1.Controls.Add(ahf);

            hf = new HistoryDataForm();
            hf.TopLevel = false;
            panel1.Controls.Add(hf);
            DateTime dt = DateTime.Now;
            string fname = "C:\\码垛机2.0\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            hf.ReadFromFile(fname);


            msf = new MainSettingForm();
            msf.TopLevel = false;
            panel1.Controls.Add(msf);

            hsf = new HandSettingForm();
            hsf.Hide();


            //如果串口被占用，则先关闭
            if (sp.IsOpen)
            {
                sp.Close();
            }
            //默认使用端口3，波特率9600
            initCommPara(sp, "COM6", 57600);
            initCommPara2(sp2, "COM3", 9600);

            timer2.Interval = 1000;
            timer2.Start();

        }

     
        /// <summary>
        /// 解决重复打开子窗口问题
        /// </summary>
        public static HistoryDataForm hf = null;
        public static MainSettingForm msf = null;
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
        public static int edge = 120;
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
            hf.Show();
            HistoryDataForm.dec = -1;
            HistoryDataForm.inc = 1;
            DateTime dt = DateTime.Now;
            string fname = "C:\\码垛机2.0\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            hf.ReadFromFile(fname);


            work_btn.BackColor = Color.FromArgb(220, 220, 220);
            historydata_btn.BackColor = Color.FromArgb(65, 105, 225);
            setting_btn.BackColor = Color.FromArgb(220, 220, 220);
            alarmhistory_btn.BackColor = Color.FromArgb(220, 220, 220);

            if (msf != null)
            {
                msf.Visible = false;
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
            
        }
        /// <summary>
        /// 设置调试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setting_btn_Click(object sender, EventArgs e)
        {
            this.Text = "设置调试";
            msf.Show();

            work_btn.BackColor = Color.FromArgb(220, 220, 220);
            historydata_btn.BackColor = Color.FromArgb(220, 220, 220);
            setting_btn.BackColor = Color.FromArgb(65, 105, 225);
            alarmhistory_btn.BackColor = Color.FromArgb(220, 220, 220);

            if (hf != null)
            {
                hf.Visible = false;
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
                hf.Visible = false;
            }
            if (msf != null)
            {
                msf.Visible = false;
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
                hf.Visible = false;
            }
            if (ahf != null)
            {
                ahf.Visible = false;
            }
            if (msf != null)
            {
                msf.Visible = false;
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
                    initCommPara(sp, "COM6", 57600);
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


        /// <summary>
        /// 接收扫码枪的数据
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="comName"></param>
        /// <param name="baudRate"></param>
        public static void initCommPara2(SerialPort sp, string comName, int baudRate)
        {
            try
            {
                //
                sp.PortName = comName;
                sp.BaudRate = baudRate;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.DataReceived += new SerialDataReceivedEventHandler(comm_DataReceived2);
                sp.Open();

            }
            catch
            {
                  MessageBox.Show(comName + "扫码串口打开失败!", "系统提示");
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

        public struct Guige
        {
            public int l;
            public int w;
            public int h;
            public int num;
        }
        public static Guige g1 = new Guige();
        
        
        public static ArrayList usbList = new ArrayList();


        ////计算挡板下落时间        
        //public static void DropTime()
        //{
        //    Thread.CurrentThread.Priority = ThreadPriority.Normal;
        //    //int sec = (int)larrayList[0] * 100 / 500;
        //    byte[] secbyte = toBytes.intToBytes(100);//有问题
        //    BF.sendbuf[0] = 0xFA;
        //    BF.sendbuf[1] = 0x06;
        //    BF.sendbuf[2] = 0x0E;
        //    BF.sendbuf[3] = 0x06;
        //    BF.sendbuf[4] = secbyte[3];
        //    BF.sendbuf[5] = secbyte[2];
        //    BF.sendbuf[6] = secbyte[1];
        //    BF.sendbuf[7] = secbyte[0];
        //    BF.sendbuf[8] = 0xF5;
        //    SendMenuCommand(BF.sendbuf, 9);
        //    larrayList.RemoveAt(0);
        //}




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
        //存放扫码得到的纸箱长度，以计算挡板下放时间
        public static ArrayList larrayList = new ArrayList();
        //缓存扫到的条码信息
        public static ArrayList bufarrayList = new ArrayList();
        //定义用于存放坐标的数组
        public static ArrayList cdarrayList = new ArrayList();
        public static ArrayList cdarrayList2 = new ArrayList();
        public static ArrayList cdarrayList3 = new ArrayList();
        public static ArrayList cdarrayList4= new ArrayList();
        public static ArrayList cdarrayList5 = new ArrayList();
        public static ArrayList cdarrayList6 = new ArrayList();
        public static ArrayList cdarrayList7 = new ArrayList();
        public static ArrayList cdarrayList8 = new ArrayList();
        public static ArrayList cdarrayList9 = new ArrayList();
        public static ArrayList cdarrayList10 = new ArrayList();
        public static ArrayList cdarrayList11 = new ArrayList();
        public static ArrayList cdarrayList12 = new ArrayList();
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
        //判断是否已存在此规格，用于存储规格数量拷贝进U盘
        public static bool exist = false;

        public static bool sendflag = true;

        //判断是否扫到码
        public static bool saodao = false;
        public static bool timeout = true;


        public static Coordinate ZARA = new Coordinate();
        public static void CalculateCoornidateAndSend()
        {

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //扫码拿到的纸箱长宽高信息
            
            //1号码盘找坐标
            if (h == 280){             
                if (count == 0)
                {
                    wdf.changeText1("第一层");
                    if(cdarrayList4.Count > 0)
                    {
                        cdarrayList4.RemoveRange(0,cdarrayList4.Count);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count++;

                    vertical.x += 0;
                    vertical.y += l;
                    rectangle1.length -= l;
                    return;
                }
                //第一行第二个
                if ((length1 + edge >= w))
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
                    //是否走到了最后一列                
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
                            //让这一列上面的元素与其对齐
                            horizontal.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count++;
                        k3.x = horizontal.x;
                        k3.y = l;
                        ZARA.x = k3.x;
                        ZARA.y = k3.y;
                        // horizontal.x += (w + gap);
                        // horizontal.y += 0;
                        rectangle2.length = width1 - l;
                        cdarrayList.Add(ZARA.x);
                        cdarrayList.Add(ZARA.y);
                        cdarrayList.Add(rectangle2.length);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count++;

                    k3.x = horizontal.x;
                    k3.y = l;
                    ZARA.x = k3.x;
                    ZARA.y = k3.y;
                    horizontal.x += (w + gap);
                    horizontal.y += 0;
                    rectangle2.length = width1 - l;
                    cdarrayList.Add(ZARA.x);
                    cdarrayList.Add(ZARA.y);
                    cdarrayList.Add(rectangle2.length);
                    length1 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0;i < cdarrayList.Count/3;i++)
                {
                    if ((int)cdarrayList[3*i+2] + edge >= l)
                    {
                        Coordinate k4 = new Coordinate();
                        if (((int)cdarrayList[3 * i + 2] - l + edge) < 360)
                        {
                            if ((int)cdarrayList[3 * i + 2] < l)
                            {
                                k4.x = (int)cdarrayList[3*i];
                                k4.y = (int)cdarrayList[3*i+1];
                                wdf.DrawOcupyArea((int)cdarrayList[3 * i], (int)cdarrayList[3 * i + 1], w, l);
                            }
                            else
                            {
                                k4.x = (int)cdarrayList[3 * i];
                                k4.y = 1000 - l;
                                wdf.DrawOcupyArea((int)cdarrayList[3 * i], 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k4.x = (int)cdarrayList[3 * i];
                            k4.y = (int)cdarrayList[3 * i + 1];
                            wdf.DrawOcupyArea((int)cdarrayList[3 * i], (int)cdarrayList[3 * i + 1], w, l);
                        }

                        //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                        if (((int)cdarrayList[3 * i + 2] - l + edge) < 360)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes(1000 - l);
                            byte[] byteZ1 = toBytes.intToBytes(0);
                            if ((int)cdarrayList[3 * i + 2] < l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList[3 * i]);
                                byteY1 = toBytes.intToBytes((int)cdarrayList[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(0);
                            }
                            string[] status1 = new string[] { "0", "0", "1", "0" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count++;
                            cdarrayList[3 * i + 2] = (int)cdarrayList[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count++;
                        cdarrayList[3 * i] = (int)cdarrayList[3*i] + 0;
                        cdarrayList[3 * i + 1] = (int)cdarrayList[3 * i + 1] + l;
                        cdarrayList[3 * i + 2] = (int)cdarrayList[3 * i + 2] - l;
                        return;
                    }
                }
             
       
                //如果走到这，说明第一层不能码了，该码放第二层了                

                //第二层第一个
                if ((count12 == 0) && (length12 + edge >= l) && (width12 + edge >= w)) {

                    wdf.changeText1("第二层");
                    //清空缓存数组
                    cdarrayList.RemoveRange(0,cdarrayList.Count);

                    Coordinate k11 = new Coordinate();
                    k11.x = 0;
                    k11.y = 0;                 
                    WorkingDetailForm.arrayList1.RemoveRange(0,WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0,0,l,w);
                    //第一个箱子直接放在原点(0,0,0) 
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(280);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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

                    if (rectangle14.length - l + edge < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal4.y);
                        byte[] byteZ1 = toBytes.intToBytes(280);
                        if (rectangle14.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal4.x);
                             byteY1 = toBytes.intToBytes(horizontal4.y);
                             byteZ1 = toBytes.intToBytes(280);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count12++;
                        rectangle14.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(horizontal4.y);
                    byte[] byteZ = toBytes.intToBytes(280);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count12++;

                    horizontal4.x += l;
                    horizontal4.y += 0;
                    rectangle14.length -= l;
                    return;
                }
                //第二行第一个
                if ((width12 + edge >= w))
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
                    
                    if (width12 - w + edge < 225)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical4.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical4.y);
                        byte[] byteZ1 = toBytes.intToBytes(280);
                        if (width12 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical4.x);
                            byteY1 = toBytes.intToBytes(1000 - w);
                            byteZ1 = toBytes.intToBytes(280);
                            vertical4.y = 1000 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count12++;
                        k13.x = vertical4.x + l;
                        k13.y = vertical4.y;
                        ZARA.x = k13.x;
                        ZARA.y = k13.y;
                        //vertical4.x += 0;
                        //vertical4.y += (w + gap);
                        rectangle15.length = length12 - l;
                        cdarrayList2.Add(ZARA.x);
                        cdarrayList2.Add(ZARA.y);
                        cdarrayList2.Add(rectangle15.length);
                        width12 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical4.x);
                    byte[] byteY = toBytes.intToBytes(vertical4.y);
                    byte[] byteZ = toBytes.intToBytes(280);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count12++;

                    k13.x = vertical4.x + l;
                    k13.y = vertical4.y;
                    ZARA.x = k13.x;
                    ZARA.y = k13.y;
                    vertical4.x += 0;
                    vertical4.y += (w + gap);                 
                    rectangle15.length = length12 - l;
                    cdarrayList2.Add(ZARA.x);
                    cdarrayList2.Add(ZARA.y);
                    cdarrayList2.Add(rectangle15.length);
                    width12 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                for(int i = 0;i < cdarrayList2.Count / 3; i++)
                {
                    if ((int)cdarrayList2[3*i+2] + edge >= l)
                    {
                        Coordinate k14 = new Coordinate();
                        if ((int)cdarrayList2[3 * i + 2] - l + edge < 360)
                        {
                            if ((int)cdarrayList2[3 * i + 2] < l)
                            {
                                k14.x = (int)cdarrayList2[3*i];
                                k14.y = (int)cdarrayList2[3*i+1];
                                wdf.DrawOcupyArea((int)cdarrayList2[3 * i], (int)cdarrayList2[3 * i + 1], l, w);
                            }
                            else
                            {
                                k14.x = 1200 - l;
                                k14.y = (int)cdarrayList2[3*i+1];
                                wdf.DrawOcupyArea(1200 - l, (int)cdarrayList2[3 * i + 1], l, w);
                            }
                        }
                        else
                        {
                            k14.x = (int)cdarrayList2[3 * i];
                            k14.y = (int)cdarrayList2[3 * i + 1];
                            wdf.DrawOcupyArea((int)cdarrayList2[3 * i], (int)cdarrayList2[3 * i + 1], l, w);
                        }

                        if ((int)cdarrayList2[3 * i + 2] - l + edge < 360)
                        {
                            byte[] byteX1 = toBytes.intToBytes(1200 - l);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList2[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(280);
                            if ((int)cdarrayList2[3 * i + 2] < l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList2[3 * i]);
                                byteY1 = toBytes.intToBytes((int)cdarrayList2[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(280);
                            }
                            string[] status1 = new string[] { "0", "0", "1", "1" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count12++;
                            cdarrayList2[3 * i + 2] = (int)cdarrayList2[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList2[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList2[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(280);
                        string[] status = new string[] { "0", "0", "1", "1" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count12++;

                        cdarrayList2[3 * i] = (int)cdarrayList2[3*i] + l;
                        cdarrayList2[3 * i + 1] = (int)cdarrayList2[3 * i + 1] + 0;
                        cdarrayList2[3 * i + 2] = (int)cdarrayList2[3 * i + 2] - l;
                        return;
                    }
                }                   

                //如果走到这，说明第2层不能码了，该码放第3层了             

                if ((count13 == 0) && (width13 + edge >= l) && (length13 + edge >= w))
                {

                    wdf.changeText1("第三层");
                    cdarrayList2.RemoveRange(0, cdarrayList2.Count);
                    Coordinate k19 = new Coordinate();
                    k19.x = 0;
                    k19.y = 0;
                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0,0,w,l);
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(560);
                    string[] status = new string[] { "0", "0", "1", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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

                    if ((rectangle18.length - l + edge) < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical5.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - l);
                        byte[] byteZ1 = toBytes.intToBytes(560);
                        if (rectangle18.length < l)
                        {
                             byteX1 = toBytes.intToBytes(vertical5.x);
                             byteY1 = toBytes.intToBytes(vertical5.y);
                             byteZ1 = toBytes.intToBytes(560);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count13++;
                        rectangle18.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y);
                    byte[] byteZ = toBytes.intToBytes(560);
                    string[] status = new string[] { "0", "0", "1", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count13++;

                    vertical5.x += 0;
                    vertical5.y += l;
                    rectangle18.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length13 + edge >= w))
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

                    if (length13 - w + edge < 225)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal5.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal5.y);
                        byte[] byteZ1 = toBytes.intToBytes(560);
                        if (length13 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal5.y);
                            byteZ1 = toBytes.intToBytes(560);
                            horizontal5.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count13++;
                        rectangle19.length = width13 - l;
                        length13 -= (w + gap);
                        k21.x = horizontal5.x;
                        k21.y = l;
                        ZARA.x = k21.x;
                        ZARA.y = k21.y;
                        cdarrayList3.Add(ZARA.x);
                        cdarrayList3.Add(ZARA.y);
                        cdarrayList3.Add(rectangle19.length);
                        //horizontal5.x += (w + gap);
                        //horizontal5.y += 0;                      
                        isJudged = true;
                        return;
                    }
                    byte[] byteX = toBytes.intToBytes(horizontal5.x);
                    byte[] byteY = toBytes.intToBytes(horizontal5.y);
                    byte[] byteZ = toBytes.intToBytes(560);
                    string[] status = new string[] { "0", "0", "1", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count13++;

                    k21.x = horizontal5.x;
                    k21.y = l;
                    ZARA.x = k21.x;
                    ZARA.y = k21.y;
                    horizontal5.x += (w + gap);
                    horizontal5.y += 0;
                    rectangle19.length = width13 - l;
                    cdarrayList3.Add(ZARA.x);
                    cdarrayList3.Add(ZARA.y);
                    cdarrayList3.Add(rectangle19.length);
                    length13 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for(int i = 0;i < cdarrayList3.Count / 3; i++)
                {
                    if ((int)cdarrayList3[3*i+2] + edge >= l)
                    {
                        Coordinate k22 = new Coordinate();
                        if (((int)cdarrayList3[3 * i + 2] - l + edge) < 360)
                        {
                            if ((int)cdarrayList3[3 * i + 2] < l)
                            {
                                k22.x = (int)cdarrayList3[3 * i];
                                k22.y = (int)cdarrayList3[3 * i + 1];
                                wdf.DrawOcupyArea((int)cdarrayList3[3 * i], (int)cdarrayList3[3 * i + 1], w, l);
                            }
                            else
                            {
                                k22.x = (int)cdarrayList3[3 * i];
                                k22.y = 1000 - l;
                                wdf.DrawOcupyArea((int)cdarrayList3[3 * i], 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k22.x = (int)cdarrayList3[3 * i];
                            k22.y = (int)cdarrayList3[3 * i + 1];
                            wdf.DrawOcupyArea((int)cdarrayList3[3 * i], (int)cdarrayList3[3 * i + 1], w, l);
                        }

                        if (((int)cdarrayList3[3 * i + 2] - l + edge) < 360)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList3[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes(1000 - l);
                            byte[] byteZ1 = toBytes.intToBytes(560);
                            if ((int)cdarrayList3[3 * i + 2] < l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList3[3 * i]);
                                byteY1 = toBytes.intToBytes((int)cdarrayList3[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(560);
                            }
                            string[] status1 = new string[] { "0", "0", "1", "0" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count13++;
                            cdarrayList3[3 * i + 2] = (int)cdarrayList3[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes(vertical5.x);
                        byte[] byteY = toBytes.intToBytes(vertical5.y);
                        byte[] byteZ = toBytes.intToBytes(560);
                        string[] status = new string[] { "0", "0", "1", "0" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count13++;

                        cdarrayList3[3 * i] = (int)cdarrayList3[3 * i] + 0;
                        cdarrayList3[3 * i + 1] = (int)cdarrayList3[3 * i + 1] + l;
                        cdarrayList3[3 * i + 2] = (int)cdarrayList3[3 * i + 2] - l;
                        return;
                    }
                }                  

                //如果走到这，说明第3层不能码了，该码放第4层了

                //第4层第一个
                if ((count14 == 0) && (length14 + edge >= l) && (width14 + edge >= w))
                {
                    wdf.changeText1("第四层");
                    cdarrayList3.RemoveRange(0,cdarrayList3.Count);
                    Coordinate k29 = new Coordinate();
                    k29.x = 0;
                    k29.y = 0;
                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0,0,l,w);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(840);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count14++;

                    vertical6.x = 0;
                    vertical6.y = (w + gap);
                    horizontal6.x = l;
                    horizontal6.y = 0;

                    rectangle21.length = length14 - l;
                    width14 -= (w + gap);
                    nearfinish();
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
                             
                    if (rectangle21.length - l + edge < 360)
                    {
                        byte[] byteX1 = toBytes.intToBytes(1200 - l);
                        byte[] byteY1 = toBytes.intToBytes(horizontal6.y);
                        byte[] byteZ1 = toBytes.intToBytes(840);
                        if (rectangle21.length < l)
                        {
                             byteX1 = toBytes.intToBytes(horizontal6.x);
                             byteY1 = toBytes.intToBytes(horizontal6.y);
                             byteZ1 = toBytes.intToBytes(840);
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count14++;
                        rectangle21.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal6.x);
                    byte[] byteY = toBytes.intToBytes(horizontal6.y);
                    byte[] byteZ = toBytes.intToBytes(840);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count14++;

                    horizontal6.x += l;
                    horizontal6.y += 0;
                    rectangle21.length -= l;
                    return;
                }
                //第一列第二个
                if ((width14 + edge>= w))
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

                    if (width14 - w + edge < 225)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical6.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical6.y);                        
                        byte[] byteZ1 = toBytes.intToBytes(840);
                        if (width14 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical6.x);
                            byteY1 = toBytes.intToBytes(1000 - w);
                            byteZ1 = toBytes.intToBytes(840);
                            vertical6.y = 1000 - w;
                        }
                        string[] status1 = new string[] { "0", "0", "1", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count14++;
                        rectangle22.length = length14 - l;
                        k31.x = vertical6.x + l;
                        k31.y = vertical6.y;
                        ZARA.x = k31.x;
                        ZARA.y = k31.y;
                        //vertical6.x += 0;
                        //vertical6.y += (w + gap);
                        cdarrayList4.Add(ZARA.x);
                        cdarrayList4.Add(ZARA.y);
                        cdarrayList4.Add(rectangle22.length);
                        width14 -= (w + gap);
                        isJudged = true;
                       // isLastRowOrCol = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical6.x);
                    byte[] byteY = toBytes.intToBytes(vertical6.y);
                    byte[] byteZ = toBytes.intToBytes(840);
                    string[] status = new string[] { "0", "0", "1", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count14++;

                    k31.x = vertical6.x + l;
                    k31.y = vertical6.y;
                    ZARA.x = k31.x;
                    ZARA.y = k31.y;
                    vertical6.x += 0;
                    vertical6.y += (w + gap);                  
                    rectangle22.length = length14 - l;
                    cdarrayList4.Add(ZARA.x);
                    cdarrayList4.Add(ZARA.y);
                    cdarrayList4.Add(rectangle22.length);
                    width14 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                for(int i = 0;i < cdarrayList4.Count / 3; i++)
                {
                    if ((int)cdarrayList4[3 * i + 2] + edge >= l)
                    {
                        Coordinate k32 = new Coordinate();
                        if ((int)cdarrayList4[3 * i + 2] - l + edge < 360)
                        {
                            if ((int)cdarrayList4[3 * i + 2] < l)
                            {
                                k32.x = (int)cdarrayList4[3 * i];
                                k32.y = (int)cdarrayList4[3 * i + 1];
                                wdf.DrawOcupyArea((int)cdarrayList4[3 * i], (int)cdarrayList4[3 * i + 1], l, w);
                            }
                            else
                            {
                                k32.x = 1200 - l;   
                                k32.y = (int)cdarrayList4[3 * i + 1];
                                wdf.DrawOcupyArea(1200 - l, (int)cdarrayList4[3 * i + 1], l, w);
                            }
                        }
                        else
                        {
                            k32.x = (int)cdarrayList4[3 * i];
                            k32.y = (int)cdarrayList4[3 * i + 1];
                            wdf.DrawOcupyArea((int)cdarrayList4[3 * i], (int)cdarrayList4[3 * i + 1], l, w);
                        }

                        if ((int)cdarrayList4[3 * i + 2] - l + edge < 360)
                        {
                            byte[] byteX1 = toBytes.intToBytes(1200 - l);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList4[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(840);
                            if ((int)cdarrayList4[3 * i + 2] < l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList4[3 * i]);
                                byteY1 = toBytes.intToBytes((int)cdarrayList4[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(840);
                            }
                            string[] status1 = new string[] { "0", "0", "1", "1" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count14++;
                            cdarrayList4[3 * i + 2] = (int)cdarrayList4[3 * i + 2] - l;
                            isJudged = false;
                            if (i== (cdarrayList4.Count/3-1))
                            {
                                finish_1();
                                //isLastRowOrCol = false;
                                WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                            }
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList4[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList4[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(840);
                        string[] status = new string[] { "0", "0", "1", "1" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count14++;

                        cdarrayList4[3 * i] = (int)cdarrayList4[3 * i] + l;
                        cdarrayList4[3 * i + 1] = (int)cdarrayList4[3 * i + 1] + 0;
                        cdarrayList4[3 * i + 2] = (int)cdarrayList4[3 * i + 2] - l;
                        return;
                    }
                }
                finish_1();
                WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
            }          

            /*************************************************************************************/
            //2号码盘找坐标
            if (h == 300 && w == 240)
            {             
                if (count2 == 0)
                {
                    wdf.changeText2("第一层");
                    Coordinate k37 = new Coordinate();
                    k37.x = 0;
                    k37.y = 1500;      
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle6.length - l + edge) < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical2.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical2.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle6.length > l)
                        {
                             byteX1 = toBytes.intToBytes(vertical2.x);
                             byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                             byteZ1 = toBytes.intToBytes(0);
                            vertical2.y = 2500 - l;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count2++;

                    vertical2.x = 0;
                    vertical2.y += l;
                    rectangle6.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length2 + edge >= w))
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

                    if (length2 - w + edge < 240)
                    {                                            
                        byte[] byteX1 = toBytes.intToBytes(horizontal2.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal2.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (length2 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal2.y);
                            byteZ1 = toBytes.intToBytes(0);
                            horizontal2.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count2++;
                        k39.x = horizontal2.x;
                        k39.y = 1500 + l;
                        ZARA.x = k39.x;
                        ZARA.y = k39.y;
                        horizontal2.x += (w + gap);
                        horizontal2.y += 0;
                        rectangle7.length = width2 - l;
                        cdarrayList5.Add(ZARA.x);
                        cdarrayList5.Add(ZARA.y);
                        cdarrayList5.Add(rectangle7.length);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count2++;

                    k39.x = horizontal2.x;
                    k39.y = 1500 + l;
                    ZARA.x = k39.x;
                    ZARA.y = k39.y;
                    horizontal2.x += (w + gap);
                    horizontal2.y += 0;
                    rectangle7.length = width2 - l;
                    cdarrayList5.Add(ZARA.x);
                    cdarrayList5.Add(ZARA.y);
                    cdarrayList5.Add(rectangle7.length);
                    length2 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0;i<cdarrayList5.Count/3;i++)
                {
                    if ((int)cdarrayList5[3*i+2] + edge >= l)
                    {
                        Coordinate k40 = new Coordinate();
                        if (((int)cdarrayList5[3 * i + 2] - l + edge) < 390)
                        {
                            if ((int)cdarrayList5[3 * i + 2] < l)
                            {
                                k40.x = (int)cdarrayList5[3 * i];
                                k40.y = (int)cdarrayList5[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList5[3 * i], (int)cdarrayList5[3 * i + 1] - 1500, w, l);
                            }
                            else
                            {
                                k40.x = (int)cdarrayList5[3 * i];
                                k40.y = 1500 + 1000 - l;
                                wdf.DrawOcupyArea2((int)cdarrayList5[3 * i], 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k40.x = (int)cdarrayList5[3 * i];
                            k40.y = (int)cdarrayList5[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList5[3 * i], (int)cdarrayList5[3 * i + 1] - 1500, w, l);
                        }

                        //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                        if (((int)cdarrayList5[3 * i + 2] - l + edge) < 390)
                        {
                            byte[] byteX1 = toBytes.intToBytes(vertical2.x);
                            byte[] byteY1 = toBytes.intToBytes(vertical2.y);
                            byte[] byteZ1 = toBytes.intToBytes(0);
                            if ((int)cdarrayList5[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(vertical2.x);
                                byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                                byteZ1 = toBytes.intToBytes(0);
                                vertical2.y = 2500 - l;
                            }
                            string[] status1 = new string[] { "0", "1", "0", "0" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count2++;
                            cdarrayList5[3 * i + 2] = (int)cdarrayList5[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList5[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList5[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "0", "0" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count2++;

                        cdarrayList5[3 * i] = (int)cdarrayList5[3 * i] + 0;
                        cdarrayList5[3 * i + 1] = (int)cdarrayList5[3 * i + 1] + l;
                        cdarrayList5[3 * i + 2] = (int)cdarrayList5[3 * i + 2] - l;
                        return;
                    }
                }
                         


                //如果走到这，说明第一层不能码了，该码放第二层了

                //第二层第一个
                if ((count22 == 0) && (length22 + edge >= l) && (width22 + edge >= w))
                {
                    wdf.changeText2("第二层");
                    Coordinate k47 = new Coordinate();
                    k47.x = 0;
                    k47.y = 1500;
                    cdarrayList5.RemoveRange(0,cdarrayList5.Count);
                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0, 0, l, w);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(1500);
                    byte[] byteZ = toBytes.intToBytes(300);
                    string[] status = new string[] { "0", "1", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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
                       // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle23.length - l + edge < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal7.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal7.y);
                        byte[] byteZ1 = toBytes.intToBytes(300);
                        if (rectangle23.length > l)
                        {
                            byteX1 = toBytes.intToBytes(1200 - l);
                            byteY1 = toBytes.intToBytes(horizontal7.y);
                            byteZ1 = toBytes.intToBytes(300);
                            horizontal7.x = 1200 - l;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count22++;
                        rectangle23.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(horizontal7.y);
                    byte[] byteZ = toBytes.intToBytes(300);
                    string[] status = new string[] { "0", "1", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count22++;

                    horizontal7.x += l;
                    horizontal7.y += 0;
                    rectangle23.length -= l;
                    return;
                }
                //第一行第二个
                if ((width22 + edge >= w))
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
                      
                    if (width22 - w + edge < 240)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical7.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical7.y);
                        byte[] byteZ1 = toBytes.intToBytes(300);
                        if (width22 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical7.x);
                            byteY1 = toBytes.intToBytes(1500 + 1000 - w);
                            byteZ1 = toBytes.intToBytes(300);
                            vertical7.y = 2500 - w;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count22++;
                        k49.x = vertical7.x + l;
                        k49.y = vertical7.y;
                        ZARA.x = k49.x;
                        ZARA.y = k49.y;
                        vertical7.x += 0;
                        vertical7.y += (w + gap);
                        rectangle24.length = length22 - l;
                        cdarrayList6.Add(ZARA.x);
                        cdarrayList6.Add(ZARA.y);
                        cdarrayList6.Add(rectangle24.length);
                        width22 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical7.x);
                    byte[] byteY = toBytes.intToBytes(vertical7.y);
                    byte[] byteZ = toBytes.intToBytes(300);
                    string[] status = new string[] { "0", "1", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count22++;

                    k49.x = vertical7.x + l;
                    k49.y = vertical7.y;
                    ZARA.x = k49.x;
                    ZARA.y = k49.y;
                    vertical7.x += 0;
                    vertical7.y += (w + gap);
                    rectangle24.length = length22 - l;
                    cdarrayList6.Add(ZARA.x);
                    cdarrayList6.Add(ZARA.y);
                    cdarrayList6.Add(rectangle24.length);
                    width22 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i=0;i<cdarrayList6.Count/3;i++)
                {
                    if ((int)cdarrayList6[3*i+2] + edge >= l)
                    {
                        Coordinate k50 = new Coordinate();
                        if ((int)cdarrayList6[3 * i + 2] - l + edge < 390)
                        {
                            if ((int)cdarrayList6[3 * i + 2] < l)
                            {
                                k50.x = (int)cdarrayList6[3 * i];
                                k50.y = (int)cdarrayList6[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList6[3 * i], (int)cdarrayList6[3 * i + 1] - 1500, l, w);
                            }
                            else
                            {
                                k50.x = 1200 - l;
                                k50.y = (int)cdarrayList6[3 * i + 1];
                                wdf.DrawOcupyArea2(1200 - l, (int)cdarrayList6[3 * i + 1] - 1500, l, w);
                            }
                        }
                        else
                        {
                            k50.x = (int)cdarrayList6[3 * i];
                            k50.y = (int)cdarrayList6[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList6[3 * i], (int)cdarrayList6[3 * i + 1] - 1500, l, w);
                        }

                        if ((int)cdarrayList6[3 * i + 2] - l + edge < 390)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList6[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList6[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(300);
                            if ((int)cdarrayList6[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(1200 - l);
                                byteY1 = toBytes.intToBytes((int)cdarrayList6[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(300);
                                horizontal7.x = 1200 - l;
                            }
                            string[] status1 = new string[] { "0", "1", "0", "1" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count22++;
                            cdarrayList6[3 * i + 2] = (int)cdarrayList6[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList6[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList6[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(300);
                        string[] status = new string[] { "0", "1", "0", "1" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count22++;

                        cdarrayList6[3 * i] = (int)cdarrayList6[3 * i] + l;
                        cdarrayList6[3 * i + 1] = (int)cdarrayList6[3 * i + 1] + 0;
                        cdarrayList6[3 * i + 2] = (int)cdarrayList6[3 * i + 2] - l;
                        return;
                    }
                }
                

                //如果走到这，说明第2层不能码了，该码放第3层了

                if ((count23 == 0) && (width23 + edge >= l) && (length23 + edge >= w))
                {
                    wdf.changeText2("第三层");
                    cdarrayList6.RemoveRange(0,cdarrayList6.Count);
                    Coordinate k55 = new Coordinate();
                    k55.x = 0;
                    k55.y = 1500;
                    arrayList.Add(k55);
                    if (arrayList.Count == 2)
                    {
                       // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0,0,w,l);

                    //第一个箱子直接放在原点(0,1500,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(1500);
                    byte[] byteZ = toBytes.intToBytes(600);
                    string[] status = new string[] { "0", "1", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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
                       // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle27.length - l + edge) < 390)
                    {

                        byte[] byteX1 = toBytes.intToBytes(vertical8.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical8.y);
                        byte[] byteZ1 = toBytes.intToBytes(600);
                        if (rectangle27.length > l)
                        {
                             byteX1 = toBytes.intToBytes(vertical8.x);
                             byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                             byteZ1 = toBytes.intToBytes(600);
                            vertical8.y = 2500 - l;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count23++;
                        rectangle27.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y);
                    byte[] byteZ = toBytes.intToBytes(600);
                    string[] status = new string[] { "0", "1", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count23++;

                    vertical8.x += 0;
                    vertical8.y += l;
                    rectangle27.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length23 + edge >= w))
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
          
                    if (length23 - w + edge < 240)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal8.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal8.y);
                        byte[] byteZ1 = toBytes.intToBytes(600);
                        if (length23 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal8.y);
                            byteZ1 = toBytes.intToBytes(600);
                            horizontal8.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count23++;
                        k57.x = horizontal8.x;
                        k57.y = 1500 + l;
                        ZARA.x = k57.x;
                        ZARA.y = k57.y;
                        horizontal8.x += (w + gap);
                        horizontal8.y += 0;
                        rectangle28.length = width23 - l;
                        cdarrayList7.Add(ZARA.x);
                        cdarrayList7.Add(ZARA.y);
                        cdarrayList7.Add(rectangle28.length);
                        length23 -= (w + gap);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal8.x);
                    byte[] byteY = toBytes.intToBytes(horizontal8.y);
                    byte[] byteZ = toBytes.intToBytes(600);
                    string[] status = new string[] { "0", "1", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count23++;

                    k57.x = horizontal8.x;
                    k57.y = 1500 + l;
                    ZARA.x = k57.x;
                    ZARA.y = k57.y;
                    horizontal8.x += (w + gap);
                    horizontal8.y += 0;
                    rectangle28.length = width23 - l;
                    cdarrayList7.Add(ZARA.x);
                    cdarrayList7.Add(ZARA.y);
                    cdarrayList7.Add(rectangle28.length);
                    length23 -= (w + gap);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0;i < cdarrayList7.Count/3;i++)
                {
                    if ((int)cdarrayList7[3*i+2] + edge >= l)
                    {
                        Coordinate k58 = new Coordinate();
                        if (((int)cdarrayList7[3 * i + 2] - l + edge) < 390)
                        {
                            if ((int)cdarrayList7[3 * i + 2] < l)
                            {
                                k58.x = (int)cdarrayList7[3 * i];
                                k58.y = (int)cdarrayList7[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList7[3 * i], (int)cdarrayList7[3 * i + 1] - 1500, w, l);
                            }
                            else
                            {
                                k58.x = (int)cdarrayList7[3 * i];
                                k58.y = 1500 + 1000 - l;
                                wdf.DrawOcupyArea2((int)cdarrayList7[3 * i], 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k58.x = (int)cdarrayList7[3 * i];
                            k58.y = (int)cdarrayList7[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList7[3 * i], (int)cdarrayList7[3 * i + 1] - 1500, w, l);
                        }

                        //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                        if (((int)cdarrayList7[3 * i + 2] - l + edge) < 390)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList7[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList7[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(600);
                            if ((int)cdarrayList7[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList7[3 * i]);
                                byteY1 = toBytes.intToBytes(1500 + 1000 - l);
                                byteZ1 = toBytes.intToBytes(600);
                                vertical8.y = 2500 - l;
                            }
                            string[] status1 = new string[] { "0", "1", "0", "0" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count23++;
                            cdarrayList7[3 * i + 2] = (int)cdarrayList7[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes(vertical8.x);
                        byte[] byteY = toBytes.intToBytes(vertical8.y);
                        byte[] byteZ = toBytes.intToBytes(600);
                        string[] status = new string[] { "0", "1", "0", "0" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count23++;

                        cdarrayList7[3 * i] = (int)cdarrayList7[3 * i] + 0;
                        cdarrayList7[3 * i + 1] = (int)cdarrayList7[3 * i + 1] + l;
                        cdarrayList7[3 * i + 2] = (int)cdarrayList7[3 * i + 2] - l;
                        return;
                    }
                }
                         

                //如果走到这，说明第3层不能码了，该码放第4层了

                //第4层第一个
                if ((count24 == 0) && (length24 + edge >= l) && (width24 + edge >= w))
                {
                    wdf.changeText2("第四层");
                    cdarrayList7.RemoveRange(0,cdarrayList7.Count);
                    Coordinate k65 = new Coordinate();
                    k65.x = 0;
                    k65.y = 1500;         
                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0,0,l,w);

                    //第一个箱子直接放在原点(0,1500,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(1400);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(900);
                    string[] status = new string[] { "0", "1", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count24++;

                    vertical9.x = 0;
                    vertical9.y = 1500 + w + gap;
                    horizontal9.x = l;
                    horizontal9.y = 1500;

                    rectangle30.length = length24 - l;
                    width24 -= (w + gap);
                    nearfinish();
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
                
                    if (rectangle30.length - l + edge < 390)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal9.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal9.y);
                        byte[] byteZ1 = toBytes.intToBytes(900);
                        if (rectangle30.length > l)
                        {
                            byteX1 = toBytes.intToBytes(1200 - l);
                            byteY1 = toBytes.intToBytes(horizontal9.y);
                            byteZ1 = toBytes.intToBytes(900);
                            horizontal9.x = 1200 - l;
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count24++;
                        rectangle30.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal9.x);
                    byte[] byteY = toBytes.intToBytes(horizontal9.y);
                    byte[] byteZ = toBytes.intToBytes(900);
                    string[] status = new string[] { "0", "1", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count24++;

                    horizontal9.x += l;
                    horizontal9.y += 0;
                    rectangle30.length -= l;
                    return;
                }
                //第一列第二个
                if ((width24 + edge >= w))
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
                   
                    if (width24 - w + edge < 240)
                    {                       
                        byte[] byteX1 = toBytes.intToBytes(vertical9.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical9.y);
                        byte[] byteZ1 = toBytes.intToBytes(900);
                        if (width24 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical9.x);
                            byteY1 = toBytes.intToBytes(1500 + 1000 - w);
                            byteZ1 = toBytes.intToBytes(900);
                            vertical9.y = 2500 - w; 
                        }
                        string[] status1 = new string[] { "0", "1", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count24++;
                        k67.x = vertical9.x + l;
                        k67.y = vertical9.y;
                        ZARA.x = k67.x;
                        ZARA.y = k67.y;
                        vertical9.x += 0;
                        vertical9.y += (w + gap);
                        rectangle31.length = length24 - l;
                        cdarrayList8.Add(ZARA.x);
                        cdarrayList8.Add(ZARA.y);
                        cdarrayList8.Add(rectangle31.length);
                        width24 -= (w + gap);
                        isJudged = true;
                        isLastRowOrCol = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical9.x);
                    byte[] byteY = toBytes.intToBytes(vertical9.y);
                    byte[] byteZ = toBytes.intToBytes(900);
                    string[] status = new string[] { "0", "1", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count24++;

                    k67.x = vertical9.x + l;
                    k67.y = vertical9.y;
                    ZARA.x = k67.x;
                    ZARA.y = k67.y;
                    vertical9.x += 0;
                    vertical9.y += (w + gap);
                    rectangle31.length = length24 - l;
                    cdarrayList8.Add(ZARA.x);
                    cdarrayList8.Add(ZARA.y);
                    cdarrayList8.Add(rectangle31.length);
                    width24 -= (w + gap);
                    isJudged = true;                   
                    return;
                }
                //第二行第二个及以上
                for (int i = 0;i < cdarrayList8.Count/3;i++)
                {
                    if ((int)cdarrayList8[3*i+2] + edge >= l)
                    {
                        Coordinate k68 = new Coordinate();
                        if ((int)cdarrayList8[3 * i + 2] - l + edge < 390)
                        {
                            if ((int)cdarrayList8[3 * i + 2] < l)
                            {
                                k68.x = (int)cdarrayList8[3 * i];
                                k68.y = (int)cdarrayList8[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList8[3 * i], (int)cdarrayList8[3 * i + 1] - 1500, l, w);
                            }
                            else
                            {
                                k68.x = 1200 - l;
                                k68.y = (int)cdarrayList8[3 * i + 1];
                                wdf.DrawOcupyArea2(1200 - l, (int)cdarrayList8[3 * i + 1] - 1500, l, w);
                            }
                        }
                        else
                        {
                            k68.x = (int)cdarrayList8[3 * i];
                            k68.y = (int)cdarrayList8[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList8[3 * i], (int)cdarrayList8[3 * i + 1] - 1500, l, w);
                        }

                        if ((int)cdarrayList8[3 * i + 2] - l + edge < 390)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList8[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList8[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(900);
                            if ((int)cdarrayList8[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(1200 - l);
                                byteY1 = toBytes.intToBytes((int)cdarrayList8[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(900);
                                horizontal9.x = 1200 - l;
                            }
                            string[] status1 = new string[] { "0", "1", "0", "1" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count24++;
                            cdarrayList8[3 * i + 2] = (int)cdarrayList8[3 * i + 2] - l;
                            isJudged = false;
                            if (i == (cdarrayList8.Count/3 - 1))
                            {
                                finish_2();
                                WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                            }
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList8[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList8[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(900);
                        string[] status = new string[] { "0", "1", "0", "1" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count24++;

                        cdarrayList8[3 * i] = (int)cdarrayList8[3 * i] + l;
                        cdarrayList8[3 * i + 1] = (int)cdarrayList8[3 * i + 1] + 0;
                        cdarrayList8[3 * i + 2] = (int)cdarrayList8[3 * i + 2] - l;
                        return;
                    }
                }
                finish_2();
                WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
            }

            /**********************************************************************************************/
            //第三个码盘♂
            if (h == 300 && w == 320)
            {
                if (count3 == 0)
                {
                    wdf.changeText3("第一层");
                    Coordinate k73 = new Coordinate();
                    k73.x = 0;
                    k73.y = 3000;              
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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
                       // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle11.length - l + edge2 < 550)
                    {

                        byte[] byteX1 = toBytes.intToBytes(horizontal3.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal3.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle11.length > l)
                        {
                            byteX1 = toBytes.intToBytes(1200 - l);
                            byteY1 = toBytes.intToBytes(horizontal3.y);
                            byteZ1 = toBytes.intToBytes(0);
                            horizontal3.x = 1200 - l;
                        }                                             
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count3++;

                    horizontal3.x += l;
                    horizontal3.y += 0;
                    rectangle11.length -= l;
                    return;
                }
                //第一列第二个
                if ((width3 + edge2 >= w))
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

                    if (width3 - w + edge2 < 320)
                    {
                        
                        byte[] byteX1 = toBytes.intToBytes(vertical3.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical3.y);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width3 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical3.x);
                            byteY1 = toBytes.intToBytes(3000 + 1000 - w);
                            byteZ1 = toBytes.intToBytes(0);
                            vertical3.y =4000 - w;
                        }
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count3++;
                        k75.x = vertical3.x + l;
                        k75.y = vertical3.y;
                        ZARA.x = k75.x;
                        ZARA.y = k75.y;
                        vertical3.x += 0;
                        vertical3.y += (w + gap);
                        rectangle12.length = 1200 - l;
                        cdarrayList9.Add(ZARA.x);
                        cdarrayList9.Add(ZARA.y);
                        cdarrayList9.Add(rectangle12.length);
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
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count3++;

                    k75.x = vertical3.x + l;
                    k75.y = vertical3.y;
                    ZARA.x = k75.x;
                    ZARA.y = k75.y;
                    vertical3.x += 0;
                    vertical3.y += (w + gap);
                    rectangle12.length = 1200 - l;
                    cdarrayList9.Add(ZARA.x);
                    cdarrayList9.Add(ZARA.y);
                    cdarrayList9.Add(rectangle12.length);
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
                for (int i = 0;i < cdarrayList9.Count/3;i++)
                {
                    if ((int)cdarrayList9[3*i+2] + edge2 >= l)
                    {
                        Coordinate k76 = new Coordinate();
                        if ((int)cdarrayList9[3 * i + 2] - l + edge2 < 550)
                        {
                            if ((int)cdarrayList9[3 * i + 2] < l)
                            {
                                k76.x = (int)cdarrayList9[3 * i];
                                k76.y = (int)cdarrayList9[3 * i + 1];
                                wdf.DrawOcupyArea3((int)cdarrayList9[3 * i], (int)cdarrayList9[3 * i + 1] - 3000, l, w);
                            }
                            else
                            {
                                k76.x = 1200 - l;
                                k76.y = (int)cdarrayList9[3 * i + 1];
                                wdf.DrawOcupyArea3(1200 - l, (int)cdarrayList9[3 * i + 1] - 3000, l, w);
                            }
                        }
                        else
                        {
                            k76.x = (int)cdarrayList9[3 * i];
                            k76.y = (int)cdarrayList9[3 * i + 1];
                            wdf.DrawOcupyArea3((int)cdarrayList9[3 * i], (int)cdarrayList9[3 * i + 1] - 3000, l, w);
                        }

                        if ((int)cdarrayList9[3 * i + 2] - l + edge2 < 550)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList9[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList9[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(0);

                            if ((int)cdarrayList9[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(1200 - l);
                                byteY1 = toBytes.intToBytes((int)cdarrayList9[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(0);
                                horizontal3.x = 1200 - l;
                            }
                            string[] status1 = new string[] { "1", "0", "0", "1" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count3++;
                            cdarrayList9[3 * i + 2] = (int)cdarrayList9[3 * i + 2] - l;
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
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count3++;

                        cdarrayList9[3 * i] = (int)cdarrayList9[3 * i] + l;
                        cdarrayList9[3 * i + 1] = (int)cdarrayList9[3 * i + 1] + 0;
                        cdarrayList9[3 * i + 2] = (int)cdarrayList9[3 * i + 2] - l;
                        return;
                    }
                }
                

                //如果走到这，说明第1层不能码了，该码放第2层了                         

                if ((count32 == 0) && (width32 + edge2 >= l) && (length32 + edge2 >= w))
                {
                    wdf.changeText3("第二层");
                    cdarrayList9.RemoveRange(0,cdarrayList9.Count);
                    Coordinate k79 = new Coordinate();
                    k79.x = 0;
                    k79.y = 3000;
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    wdf.DrawOcupyArea3(0, 0, w, l);

                    //第一个箱子直接放在原点(0,3000,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(300);
                    string[] status = new string[] { "1", "0", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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
                       // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if ((rectangle32.length - l + edge2) < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical10.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical10.y);
                        byte[] byteZ1 = toBytes.intToBytes(300);

                        if (rectangle32.length > l)
                        {
                            byteX1 = toBytes.intToBytes(vertical10.x);
                            byteY1 = toBytes.intToBytes(3000 + 1000 - l);
                            byteZ1 = toBytes.intToBytes(300);
                            vertical10.y = 4000 - l;
                        }

                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count32++;
                        rectangle32.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical10.x);
                    byte[] byteY = toBytes.intToBytes(vertical10.y);
                    byte[] byteZ = toBytes.intToBytes(300);
                    string[] status = new string[] { "1", "0", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count32++;

                    vertical10.x += 0;
                    vertical10.y += l;
                    rectangle32.length -= l;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length32 + edge2 >= w))
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

                    if ((length32 - w + edge2) < 320)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal10.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal10.y);
                        byte[] byteZ1 = toBytes.intToBytes(300);
                        if (length32 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal10.y);
                            byteZ1 = toBytes.intToBytes(300);
                            horizontal10.x = 1200 - w;
                        }
                       
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x10;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count32++;
                        k81.x = horizontal10.x;
                        k81.y = 3000 + l;
                        ZARA.x = k81.x;
                        ZARA.y = k81.y;
                        horizontal10.x += (w + gap);
                        horizontal10.y += 0;
                        rectangle33.length = width32 - l;
                        cdarrayList10.Add(ZARA.x);
                        cdarrayList10.Add(ZARA.y);
                        cdarrayList10.Add(rectangle33.length);
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
                    byte[] byteZ = toBytes.intToBytes(300);
                    string[] status = new string[] { "1", "0", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count32++;

                    k81.x = horizontal10.x;
                    k81.y = 3000 + l;
                    ZARA.x = k81.x;
                    ZARA.y = k81.y;
                    horizontal10.x += (w + gap);
                    horizontal10.y += 0;
                    rectangle33.length = width32 - l;
                    cdarrayList10.Add(ZARA.x);
                    cdarrayList10.Add(ZARA.y);
                    cdarrayList10.Add(rectangle33.length);
                    length32 -= (w + gap);
                    if (rectangle33.length + edge2 < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0;i < cdarrayList10.Count/3;i++)
                {
                    if ((int)cdarrayList10[3*i+2] + edge2 >= l)
                    {
                        Coordinate k82 = new Coordinate();
                        if (((int)cdarrayList10[3 * i + 2] - l + edge2) < 550)
                        {
                            if ((int)cdarrayList10[3 * i + 2] < l)
                            {
                                k82.x = (int)cdarrayList10[3 * i];
                                k82.y = (int)cdarrayList10[3 * i + 1];
                                wdf.DrawOcupyArea3((int)cdarrayList10[3 * i], (int)cdarrayList10[3 * i + 1] - 3000, w, l);
                            }
                            else
                            {
                                k82.x = (int)cdarrayList10[3 * i];
                                k82.y = 4000 - l;
                                wdf.DrawOcupyArea3((int)cdarrayList10[3 * i], 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k82.x = (int)cdarrayList10[3 * i];
                            k82.y = (int)cdarrayList10[3 * i + 1];
                            wdf.DrawOcupyArea3((int)cdarrayList10[3 * i], (int)cdarrayList10[3 * i + 1] - 3000, w, l);
                        }

                        if (((int)cdarrayList10[3 * i + 2] - l + edge2) < 550)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList10[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList10[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(300);
                            if ((int)cdarrayList10[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList10[3 * i]);
                                byteY1 = toBytes.intToBytes(3000 + 1000 - l);
                                byteZ1 = toBytes.intToBytes(300);
                                vertical10.y = 4000 - l;
                            }
                            string[] status1 = new string[] { "1", "0", "0", "0" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count32++;
                            cdarrayList10[3 * i + 2] = (int)cdarrayList10[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList10[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList10[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(300);
                        string[] status = new string[] { "1", "0", "0", "0" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count32++;

                        cdarrayList10[3 * i] = (int)cdarrayList10[3 * i] + 0;
                        cdarrayList10[3 * i + 1] = (int)cdarrayList10[3 * i + 1] + l;
                        cdarrayList10[3 * i + 2] = (int)cdarrayList10[3 * i + 2] - l;
                        return;
                    }
                }               

                //如果走到这，说明第2层不能码了，该码放第3层了         

                //第3层第一个
                if ((count33 == 0) && (length33 + edge2 >= l) && (width33 + edge2 >= w))
                {
                    wdf.changeText3("第三层");
                    cdarrayList10.RemoveRange(0,cdarrayList10.Count);
                    Coordinate k87 = new Coordinate();
                    k87.x = 0;
                    k87.y = 3000;
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    wdf.DrawOcupyArea3(0, 0, l, w);

                    //第一个箱子直接放在原点(0,3000,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(600);
                    string[] status = new string[] { "1", "0", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
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

                    if (rectangle35.length - l + edge2 < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(horizontal11.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal11.y);
                        byte[] byteZ1 = toBytes.intToBytes(600);
                        if (rectangle35.length > l)
                        {
                            byteX1 = toBytes.intToBytes(1200 - l);
                            byteY1 = toBytes.intToBytes(horizontal11.y);
                            byteZ1 = toBytes.intToBytes(600);
                            horizontal11.x = 1200 - l;
                        }
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count33++;
                        rectangle35.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal11.x);
                    byte[] byteY = toBytes.intToBytes(horizontal11.y);
                    byte[] byteZ = toBytes.intToBytes(600);
                    string[] status = new string[] { "1", "0", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count33++;

                    horizontal11.x += l;
                    horizontal11.y += 0;
                    rectangle35.length -= l;
                    return;
                }
                //第一列第二个
                if ((width33 + edge >= w))
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

                    if (width33 - w + edge < 320)
                    {                       
                        byte[] byteX1 = toBytes.intToBytes(vertical11.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical11.y);
                        byte[] byteZ1 = toBytes.intToBytes(600);
                        if (width33 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical11.x);
                            byteY1 = toBytes.intToBytes(3000 + 1000 - w);
                            byteZ1 = toBytes.intToBytes(600);
                            vertical11.y = 4000 - w;
                        }
                        string[] status1 = new string[] { "1", "0", "0", "1" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count33++;
                        k89.x = vertical11.x + l;
                        k89.y = vertical11.y;
                        ZARA.x = k89.x;
                        ZARA.y = k89.y;
                        vertical11.x += 0;
                        vertical11.y += (w + gap);
                        rectangle36.length = length33 - l;
                        cdarrayList11.Add(ZARA.x);
                        cdarrayList11.Add(ZARA.y);
                        cdarrayList11.Add(rectangle36.length);
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
                    byte[] byteZ = toBytes.intToBytes(600);
                    string[] status = new string[] { "1", "0", "0", "1" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count33++;

                    k89.x = vertical11.x + l;
                    k89.y = vertical11.y;
                    ZARA.x = k89.x;
                    ZARA.y = k89.y;
                    vertical11.x += 0;
                    vertical11.y += (w + gap);
                    rectangle36.length = length33 - l;
                    cdarrayList11.Add(ZARA.x);
                    cdarrayList11.Add(ZARA.y);
                    cdarrayList11.Add(rectangle36.length);
                    width33 -= (w + gap);
                    if (rectangle36.length + edge < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                for (int i = 0;i<cdarrayList11.Count/3;i++)
                {
                    if ((int)cdarrayList11[3*i+2] + edge >= l)
                    {
                        Coordinate k90 = new Coordinate();
                        if ((int)cdarrayList11[3 * i + 2] - l + edge < 550)
                        {
                            if ((int)cdarrayList11[3 * i + 2] < l)
                            {
                                k90.x = (int)cdarrayList11[3 * i];
                                k90.y = (int)cdarrayList11[3 * i + 1];
                                wdf.DrawOcupyArea3((int)cdarrayList11[3 * i], (int)cdarrayList11[3 * i + 1] - 3000, l, w);
                            }
                            else
                            {
                                k90.x = 1200 - l;
                                k90.y = (int)cdarrayList11[3 * i + 1];
                                wdf.DrawOcupyArea3(1200 - l, (int)cdarrayList11[3 * i + 1] - 3000, l, w);
                            }
                        }
                        else
                        {
                            k90.x = (int)cdarrayList11[3 * i];
                            k90.y = (int)cdarrayList11[3 * i + 1];
                            wdf.DrawOcupyArea3((int)cdarrayList11[3 * i], (int)cdarrayList11[3 * i + 1] - 3000, l, w);
                        }

                        if ((int)cdarrayList11[3 * i + 2] - l + edge < 550)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList11[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList11[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(600);
                            if ((int)cdarrayList11[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(1200 - l);
                                byteY1 = toBytes.intToBytes((int)cdarrayList11[3 * i + 1]);
                                byteZ1 = toBytes.intToBytes(600);
                                horizontal11.x = 1200 - l;
                            }
                            string[] status1 = new string[] { "1", "0", "0", "1" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count33++;
                            cdarrayList11[3 * i + 2] = (int)cdarrayList11[3 * i + 2] - l;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList11[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList11[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(600);
                        string[] status = new string[] { "1", "0", "0", "1" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count33++;

                        cdarrayList11[3 * i] = (int)cdarrayList11[3 * i] + l;
                        cdarrayList11[3 * i + 1] = (int)cdarrayList11[3 * i + 1] + 0;
                        cdarrayList11[3 * i + 2] = (int)cdarrayList11[3 * i + 2] - l;
                        return;
                    }
                }
                


                //如果走到这，说明第3层不能码了，该码放第4层了            

                //第4层第一个
                if ((count34 == 0) && (length34 + edge2 >= l) && (width34 + edge2 >= w))
                {
                    wdf.changeText3("第四层");
                    cdarrayList11.RemoveRange(0,cdarrayList11.Count);
                    Coordinate k93 = new Coordinate();
                    k93.x = 0;
                    k93.y = 3000;   

                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    wdf.DrawOcupyArea3(0, 0, w, l);

                    //第一个箱子直接放在原点(2800,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(3000);
                    byte[] byteZ = toBytes.intToBytes(900);
                    string[] status = new string[] { "1", "0", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count34++;

                    vertical12.x = 0;
                    vertical12.y = 3000 + l;
                    horizontal12.x = w + gap;
                    horizontal12.y = 3000;

                    rectangle37.length = width34 - l;
                    length34 -= (w + gap);
                    nearfinish();
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
     
                    if ((rectangle37.length - l + edge) < 550)
                    {
                        byte[] byteX1 = toBytes.intToBytes(vertical12.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical12.y);
                        byte[] byteZ1 = toBytes.intToBytes(900);
                        if (rectangle37.length > l)
                        {
                            byteX1 = toBytes.intToBytes(vertical12.x);
                            byteY1 = toBytes.intToBytes(3000 + 1000 - l);
                            byteZ1 = toBytes.intToBytes(900);
                            vertical12.y = 4000 - l;
                        }
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count34++;
                        rectangle37.length -= l;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal12.x);
                    byte[] byteY = toBytes.intToBytes(horizontal12.y);
                    byte[] byteZ = toBytes.intToBytes(900);
                    string[] status = new string[] { "1", "0", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count34++;

                    vertical12.x += 0;
                    vertical12.y += l;
                    rectangle37.length -= l;
                    return;
                }
                //第一行第二个
                if ((length34 + edge2 >= w))
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
           
                    if ((length34 - w + edge2) < 320)
                    {                       
                        byte[] byteX1 = toBytes.intToBytes(horizontal12.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal12.y);
                        byte[] byteZ1 = toBytes.intToBytes(900);
                        if (length34 > w)
                        {
                            byteX1 = toBytes.intToBytes(1200 - w);
                            byteY1 = toBytes.intToBytes(horizontal12.y);
                            byteZ1 = toBytes.intToBytes(900);
                            horizontal12.x = 1200 - w;
                        }
                        string[] status1 = new string[] { "1", "0", "0", "0" };
                        string status21 = string.Join("", status1);
                        int a1 = Convert.ToInt32(status21, 2);
                        byte[] b1 = toBytes.intToBytes(a1);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX1[3];
                        BF.sendbuf[5] = byteX1[2];
                        BF.sendbuf[6] = byteX1[1];
                        BF.sendbuf[7] = byteX1[0];
                        BF.sendbuf[8] = byteY1[3];
                        BF.sendbuf[9] = byteY1[2];
                        BF.sendbuf[10] = byteY1[1];
                        BF.sendbuf[11] = byteY1[0];
                        BF.sendbuf[12] = byteZ1[3];
                        BF.sendbuf[13] = byteZ1[2];
                        BF.sendbuf[14] = byteZ1[1];
                        BF.sendbuf[15] = byteZ1[0];
                        BF.sendbuf[16] = b1[3];
                        BF.sendbuf[17] = b1[2];
                        BF.sendbuf[18] = b1[1];
                        BF.sendbuf[19] = b1[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count34++;
                        k95.x = horizontal12.x;
                        k95.y = 3000 + l;
                        ZARA.x = k95.x;
                        ZARA.y = k95.y;
                        horizontal12.x += (w + gap);
                        horizontal12.y += 0;
                        rectangle38.length = width34 - l;
                        cdarrayList12.Add(ZARA.x);
                        cdarrayList12.Add(ZARA.y);
                        cdarrayList12.Add(rectangle38.length);
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
                    byte[] byteZ = toBytes.intToBytes(900);
                    string[] status = new string[] { "1", "0", "0", "0" };
                    string status2 = string.Join("", status);
                    int a = Convert.ToInt32(status2, 2);
                    byte[] b = toBytes.intToBytes(a);

                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x12;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = byteX[3];
                    BF.sendbuf[5] = byteX[2];
                    BF.sendbuf[6] = byteX[1];
                    BF.sendbuf[7] = byteX[0];
                    BF.sendbuf[8] = byteY[3];
                    BF.sendbuf[9] = byteY[2];
                    BF.sendbuf[10] = byteY[1];
                    BF.sendbuf[11] = byteY[0];
                    BF.sendbuf[12] = byteZ[3];
                    BF.sendbuf[13] = byteZ[2];
                    BF.sendbuf[14] = byteZ[1];
                    BF.sendbuf[15] = byteZ[0];
                    BF.sendbuf[16] = b[3];
                    BF.sendbuf[17] = b[2];
                    BF.sendbuf[18] = b[1];
                    BF.sendbuf[19] = b[0];
                    BF.sendbuf[20] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 21);
                    count34++;

                    k95.x = horizontal12.x;
                    k95.y = 3000 + l;
                    ZARA.x = k95.x;
                    ZARA.y = k95.y;
                    horizontal12.x += (w + gap);
                    horizontal12.y += 0;
                    rectangle38.length = width34 - l;
                    cdarrayList12.Add(ZARA.x);
                    cdarrayList12.Add(ZARA.y);
                    cdarrayList12.Add(rectangle38.length);
                    length34 -= (w + gap);
                    if (rectangle38.length + edge2 < l)
                    {
                        return;
                    }
                    isJudged = true;
                    return;

                }
                //第二列第二个及以上
                for (int i = 0;i< cdarrayList12.Count/3;i++)
                {
                    if ((int)cdarrayList12[3*i+2] + edge2 >= l)
                    {
                        Coordinate k96 = new Coordinate();
                        if (((int)cdarrayList12[3 * i + 2] - l + edge2) < 550)
                        {
                            if ((int)cdarrayList12[3 * i + 2] < l)
                            {
                                k96.x = (int)cdarrayList12[3 * i];
                                k96.y = (int)cdarrayList12[3 * i + 1];
                                wdf.DrawOcupyArea3((int)cdarrayList12[3 * i], (int)cdarrayList12[3 * i + 1] - 3000, w, l);
                            }
                            else
                            {
                                k96.x = (int)cdarrayList12[3 * i];
                                k96.y = 4000 - l;
                                wdf.DrawOcupyArea3((int)cdarrayList12[3 * i], 1000 - l, w, l);
                            }

                        }
                        else
                        {
                            k96.x = (int)cdarrayList12[3 * i];
                            k96.y = (int)cdarrayList12[3 * i + 1];
                            wdf.DrawOcupyArea3((int)cdarrayList12[3 * i], (int)cdarrayList12[3 * i + 1] - 3000, w, l);
                        }

                        if (((int)cdarrayList12[3 * i + 2] - l + edge2) < 550)
                        {
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList12[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList12[3 * i + 1]);
                            byte[] byteZ1 = toBytes.intToBytes(900);
                            if ((int)cdarrayList12[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList12[3 * i]);
                                byteY1 = toBytes.intToBytes(3000 + 1000 - l);
                                byteZ1 = toBytes.intToBytes(900);
                                vertical12.y = 4000 - l;
                            }
                            string[] status1 = new string[] { "1", "0", "0", "0" };
                            string status21 = string.Join("", status1);
                            int a1 = Convert.ToInt32(status21, 2);
                            byte[] b1 = toBytes.intToBytes(a1);

                            BF.sendbuf[0] = 0xFA;
                            BF.sendbuf[1] = 0x12;
                            BF.sendbuf[2] = 0x0E;
                            BF.sendbuf[3] = 0x02;
                            BF.sendbuf[4] = byteX1[3];
                            BF.sendbuf[5] = byteX1[2];
                            BF.sendbuf[6] = byteX1[1];
                            BF.sendbuf[7] = byteX1[0];
                            BF.sendbuf[8] = byteY1[3];
                            BF.sendbuf[9] = byteY1[2];
                            BF.sendbuf[10] = byteY1[1];
                            BF.sendbuf[11] = byteY1[0];
                            BF.sendbuf[12] = byteZ1[3];
                            BF.sendbuf[13] = byteZ1[2];
                            BF.sendbuf[14] = byteZ1[1];
                            BF.sendbuf[15] = byteZ1[0];
                            BF.sendbuf[16] = b1[3];
                            BF.sendbuf[17] = b1[2];
                            BF.sendbuf[18] = b1[1];
                            BF.sendbuf[19] = b1[0];
                            BF.sendbuf[20] = 0xF5;
                            SendMenuCommand(BF.sendbuf, 21);
                            count34++;
                            cdarrayList12[3 * i + 2] = (int)cdarrayList12[3 * i + 2] - l;
                            isJudged = false;
                            if (i == (cdarrayList12.Count/3 -1))
                            {
                                finish_3();
                                WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                            }
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList12[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList12[3 * i + 1]);
                        byte[] byteZ = toBytes.intToBytes(900);
                        string[] status = new string[] { "1", "0", "0", "0" };
                        string status2 = string.Join("", status);
                        int a = Convert.ToInt32(status2, 2);
                        byte[] b = toBytes.intToBytes(a);

                        BF.sendbuf[0] = 0xFA;
                        BF.sendbuf[1] = 0x12;
                        BF.sendbuf[2] = 0x0E;
                        BF.sendbuf[3] = 0x02;
                        BF.sendbuf[4] = byteX[3];
                        BF.sendbuf[5] = byteX[2];
                        BF.sendbuf[6] = byteX[1];
                        BF.sendbuf[7] = byteX[0];
                        BF.sendbuf[8] = byteY[3];
                        BF.sendbuf[9] = byteY[2];
                        BF.sendbuf[10] = byteY[1];
                        BF.sendbuf[11] = byteY[0];
                        BF.sendbuf[12] = byteZ[3];
                        BF.sendbuf[13] = byteZ[2];
                        BF.sendbuf[14] = byteZ[1];
                        BF.sendbuf[15] = byteZ[0];
                        BF.sendbuf[16] = b[3];
                        BF.sendbuf[17] = b[2];
                        BF.sendbuf[18] = b[1];
                        BF.sendbuf[19] = b[0];
                        BF.sendbuf[20] = 0xF5;
                        SendMenuCommand(BF.sendbuf, 21);
                        count34++;

                        cdarrayList12[3 * i] = (int)cdarrayList12[3 * i] + 0;
                        cdarrayList12[3 * i + 1] = (int)cdarrayList12[3 * i + 1] + l;
                        cdarrayList12[3 * i + 2] = (int)cdarrayList12[3 * i + 2] - l;
                        return;
                    }
                }
                finish_3();
                WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
            }    
           
        }
        

        /// <summary>
        /// 摆放到第4层，即将摆满的报警
        /// </summary>
        public static void nearfinish()
        {

        }

        /// <summary>
        /// 码盘摆满
        /// </summary>
        /// 
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
            MessageBox.Show("码盘1码垛完成,请更换码盘!", "警告",MessageBoxButtons.OK,MessageBoxIcon.Information);           
            count = count12 = count13 = count14 = 0;
            width1 = width12 = width13 = width14 = 1000;
            length1 = length12 = length13 = length14 = 1200;
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
            MessageBox.Show("码盘2码垛完成,请更换码盘!", "警告",MessageBoxButtons.OK,MessageBoxIcon.Information);
            count2 = count22 = count23 = count24 = 0;
            width2 = width22 = width23 = width24 = 1000;
            length2 = length22 = length23 = length24 = 1200;           
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
            MessageBox.Show("码盘3码垛完成,请更换码盘!", "警告",MessageBoxButtons.OK,MessageBoxIcon.Information);
            count3 = count32 = count33 = count34 = 0;
            width3 = width32 = width33 = width34 = 1000;
            length3 = length32 = length33 = length34 = 1200;
        }

        public static void WriteToUsbDisk(ArrayList array)
        {
            DateTime dt = DateTime.Now;
            string[] headline = new string[] {"长","宽","高","数量" };
            System.IO.StreamWriter sw = new System.IO.StreamWriter(dt.ToString("yyyy-MM-dd") + "info.txt", false, System.Text.Encoding.GetEncoding("gb2312"));
            try
            {
                int len = 0;
                string line = "";
                string temp = "";
                for (int i = 0; i < headline.Length; i++)
                {
                    temp = headline[i];
                    len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length; //考虑中英文的情况
                    temp = temp.PadRight(len, ' ');
                    line += temp;
                }
                sw.WriteLine(line);
                line = "";
                for (int k = 0;k < usbList.Count/4;k++)
                {
                    for (int j = 4 * k; j < 4 * (k + 1); j++)
                    {
                        temp = array[j].ToString();
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
        /// <summary>
        /// 开启一个线程，每隔一段时间请求坐标
        /// </summary>
        public static void StartThread()
        {
            Thread thread = new Thread(new ThreadStart(GetCoordinate));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
        }
        public static object locker = new object();
        public static void GetCoordinate()
        {
            lock (locker)
            {
            while (xinlei)
            {
                //每隔1s请求一次坐标
                completed = false;
                Thread.Sleep(500);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                BF.sendbuf = new byte[100];
                xinlei = false;
                completed = true;
                confirmCompleted();
            }
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
            lock (locker)
            {
                while (fight)
                {
                     xinlei = false;
                    //发送指令查看报警历史
                    Thread.Sleep(500);
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x01;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    BF.sendbuf = new byte[100];
                    fight = false;
                    xinlei = true;
                    StartThread();
                }
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
            lock (locker)
            {
                while (completed)
                {
                     fight = false;
                    //发送指令查看码垛是否完成
                    Thread.Sleep(500);
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x05;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    BF.sendbuf = new byte[100];
                    completed = false;
                    fight = true;
                    searchAlarmInfo();
                    //xinlei = true;
                    //StartThread();
                }
            }
        }
        

        /// <summary>
        /// 接收扫码枪的数据
        /// </summary>
        public static void ScannerGun()
        {
            larrayList.Add(l);
            xinlei = false;
            fight = false;
            completed = false;

            //if (xinlei == false && fight == false && completed == false)
            //{
                Thread.Sleep(300);
                SendMaduoInfo();
            //}

            if (usbList.Count == 0)
            {
                usbList.Add(l);
                usbList.Add(w);
                usbList.Add(h);
                usbList.Add(1);
                WriteToUsbDisk(usbList);
                return;
            }
            else
            {
                for (int i = 0; i < usbList.Count / 4; i++)
                {
                    if ((l == (int)usbList[4 * i]) && (w == (int)usbList[4 * i + 1]) && (h == (int)usbList[4 * i + 2]))
                    {
                        usbList[4 * i + 3] = (int)usbList[4 * i + 3] + 1;
                        exist = true;
                        WriteToUsbDisk(usbList);
                        break;
                    }
                }
                if (!exist)
                {
                    usbList.Add(l);
                    usbList.Add(w);
                    usbList.Add(h);
                    usbList.Add(1);
                    WriteToUsbDisk(usbList);
                    return;
                }
            }
        }
         
        /// <summary>
        /// 扫码器数据接收与解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void comm_DataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            string str = null;
            str = sp2.ReadExisting();
            if (str != null)
            {
                string result1 = str.Replace("\r","");
                string result2 = result1.Replace("\n", "");
              
                //sap通讯获取条码详细信息
                PackageInfo packageInfo = new PackageInfo();
                packageInfo = WebServiceRq.GetPackinfoFromWeb(result2);
                if(packageInfo == null)
                {
                    return;
                }
                string str1 = packageInfo.Matnr;
                string str2 = packageInfo.Maktx;
                string str3 = packageInfo.Ppaufnr;
                hf.InsertIntoTable(str1,str2,str3);


                //由于sap取出的字符串格式为850.0，所以滤除小数点
                string LENGTH = packageInfo.Laeng;
                string L = LENGTH.Substring(0,LENGTH.Length - 2);
                string WIDTH = packageInfo.Breit;
                string W = WIDTH.Substring(0,WIDTH.Length - 2);
                string HEIGHT = packageInfo.Hoehe;
                string H = HEIGHT.Substring(0,HEIGHT.Length - 2);
                l = int.Parse(L);
                w = int.Parse(W);
                h = int.Parse(H);
                bufarrayList.Add(l);
                bufarrayList.Add(w);
                bufarrayList.Add(h);
                str = null;

                saodao = true;
            }
        }

        /**************************************************************************************************/
        /// <summary>
        /// 接收各种数据并解析
        /// </summary>
        //private int SendStatus = 0;


        //回零失败标志位
        public static bool losehuiling = false;
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
                }



                //解析报警信息
                //第三个数据用于判断是否是报警信息数据
                if ((binary_data_1[1] == 0x02) && (binary_data_1[2] == 0x0E))
                {                    
                    string desc = "";
                    //将16进制字符串转为10进制整型数
                    data2 =Convert.ToInt32(binary_data_1[3].ToString("X2"),16);
                    if(data2 == 10){
                         desc = "电机故障（请尝试检查X、Y、Z电机）";
                    }
                    else if(data2 == 20)
                    {
                         desc = "硬限位故障（请检查硬限位）";
                    }
                    else if(data2 == 30)
                    {
                        desc = "软限位故障（请检查软限位）"; 
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
                if((binary_data_1[1] == 0x11) && (binary_data_1[2] == 0x0E))
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
                    int a = Convert.ToInt32(Convert.ToString(binary_data_1[3], 16), 16);
                    //回零查询
                    if ((a & 1) == 1 || ((a & (1 << 1)) >> 1) == 1 || ((a & (1 << 2)) >> 2) == 1 || ((a & (1 << 3)) >> 3) == 1)
                    {
                        WorkingDetailForm.reset = true;
                        HandSettingForm.huilingflag = false;
                        MessageBox.Show("已经回零","提示");
                        return;
                    }
        
                    return;
                }

                //确认下位机收到停止指令
                if ((binary_data_1[2] == 0x0A) && (binary_data_1[3] == 0x05))
                {
                    HandSettingForm.stoped = true;
                }

                //模拟收到的纸箱数据
                if ((binary_data_1[1] == 0x0D) && (binary_data_1[2] == 0XAA))
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

                    hf.InsertIntoTable("89757","CBA/NCAA","0A002");

                    larrayList.Add(l);
                    xinlei = false;
                    fight = false;
                    completed = false;
             
                    if (xinlei == false && fight == false && completed == false)
                    {
                        lock (locker)
                        {
                             Thread.Sleep(200);
                             BF.sendbuf = new byte[100];
                             SendMaduoInfo();
                             BF.sendbuf = new byte[100];
                        }
                    }
                                                              
                    if(usbList.Count == 0)
                    {
                        usbList.Add(l);
                        usbList.Add(w);
                        usbList.Add(h);
                        usbList.Add(1);
                        WriteToUsbDisk(usbList);
                        return;
                    }
                    else
                    {
                        for (int i = 0;i < usbList.Count/4;i++)
                        {
                            if ((l == (int)usbList[4*i])&&(w == (int)usbList[4*i+1])&&(h == (int)usbList[4*i+2]))
                            {
                                usbList[4 * i + 3] = (int)usbList[4 * i + 3] + 1;
                                exist = true;
                                WriteToUsbDisk(usbList);
                                break;
                            }
                            exist = false;
                        }
                        if (!exist)
                        {
                            usbList.Add(l);
                            usbList.Add(w);
                            usbList.Add(h);
                            usbList.Add(1);
                            WriteToUsbDisk(usbList);
                            return;
                        }
                    }
                }


                if (binary_data_1[1] == 0x02 && binary_data_1[2] == 0x0E && binary_data_1[3] == 0x01 )
                {
                    sendflag = false;
                }

                //码垛完成则收到数据
                if ((binary_data_1[1] == 0x02) && (binary_data_1[2] == 0x0F))
                {
                    int a = Convert.ToInt32(Convert.ToString(binary_data_1[3], 16), 16);
                    //第0位置1，表示码垛完成
                    if ((a & 1) == 1)
                    {
                        wdf.GetTotalNum(++totalNum);

                        //收到指令则从缓存中依此读数据下发
                        if (bufarrayList.Count != 0)
                        {
                            l = (int)bufarrayList[0];
                            w = (int)bufarrayList[1];
                            h = (int)bufarrayList[2];
                            ScannerGun();
                            bufarrayList.RemoveRange(0, 3);
                        }

                        // DropTime();
                        return;
                    }
                    //第一位置1,表示木箱
                    if (((a & (1 << 1)) >> 1) == 1)
                    {
                        return;
                    }
                    //第二位置1,纸箱
                    if (((a & (1 << 2)) >> 2) == 1)
                    {
                        //是否扫到码(定时器2s,未扫到码则提示人工扫码)需要等待2秒才判断
                        Thread.Sleep(2000);
                        if (!saodao)
                        {
                            MessageBox.Show("扫码失败，请人工重试！","警告");
                            return;
                        }
                        saodao = false;
                    }
                    return;
                }

            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.SysTime.Text = "欢迎登陆！当前时间" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sp.IsOpen)
            {
                sp.Close();
            }            
        }
    }
}