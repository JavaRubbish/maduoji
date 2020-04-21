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
        public static SerialPort sp3 = new SerialPort();

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
            string fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            hf.ReadFromFile(fname);


            pf = new PrintForm();
            pf.TopLevel = false;
            compareDt = dt.ToString("yyyy-MM-dd");
            string pfname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\todayUSBdata\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            pf.ReadFromFile(pfname);
            string pfname2 = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\toDB\\sapdata.txt";
            pf.ReadFromFile2(pfname2);
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
            //默认使用端口3，波特率115200
            initCommPara(sp, "COM7", 57600);
            initCommPara2(sp2, "COM2", 115200);
            initCommPara3(sp3, "COM3", 115200);
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
        public static PrintForm pf = null;
        //标志位，用于只在工作界面才发送坐标请求指令
        public static bool xinlei = true;
        public static bool fight = true;
        public static bool completed = true;
        //码垛计数，每完成一次，计数加一
        public static int totalNum = 0;
        //留余系数（允许超出边界距离）
        public static int edge = 50;
        public static int edge2 = 0;//3号码盘
        public static int edge3 = 95;//2号码盘，敲重点!!!
        //纸箱间的缝隙
        public static int gap = 40;
        //1号码盘动态间隙
        public static int gap11 = 50;
        public static int gap12 = 45;
        public static int gap13 = 45;
        public static int gap14 = 45;
        //一号盘的2.4层特殊偏移，往里挪
        public static int offset124 = 30;

        //2号码盘动态间隙
        public static int gap2 = 45;

        //纵向间隙
        public static int dst = 45;
        //2号码盘偏移距离
        public static int offset2 = 1580;
        //2.4层的特殊偏移
        public static int offset24 = 20;
        //3号码盘偏移距离
        public static int offset3 = 3230;
        //2.4层特殊偏移(往里去100)
        public static int offset324 = 100;
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
            string fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
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
                    initCommPara(sp, "COM7", 57600);
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

            }
            catch
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

        public static void initCommPara3(SerialPort sp, string comName, int baudRate)
        {
            try
            {
                //
                sp.PortName = comName;
                sp.BaudRate = baudRate;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.DataReceived += new SerialDataReceivedEventHandler(comm_DataReceived3);
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



        /// <summary>
        /// 定义所有全局变量
        /// </summary>

        public static int length1 = 1000;
        public static int width1 = 1000;
        public static int length12 = 1000;
        public static int width12 = 1000;
        public static int length13 = 1000;
        public static int width13 = 1000;
        public static int length14 = 1000;
        public static int width14 = 1000;

        public static int length2 = 1000;
        public static int width2 = 1000;
        public static int length22 = 1000;
        public static int width22 = 1000;
        public static int length23 = 1000;
        public static int width23 = 1000;
        public static int length24 = 1000;
        public static int width24 = 1000;

        public static int length3 = 1000;
        public static int width3 = 1000;
        public static int length32 = 1000;
        public static int width32 = 1000;
        public static int length33 = 1000;
        public static int width33 = 1000;
        public static int length34 = 1000;
        public static int width34 = 1000;

        //全局长宽高
        public static int l = 0;
        public static int w = 0;
        public static int h = 0;

        //全局计数
        public static int count = 0;
        public static int count2 = 0;
        public static int count3 = 0;
        //第三个码盘的状态码
        public static string code = "200";
        //第三个盘的长宽拼接临界点
        public static int divpoint = 350;
        //长短的界定
        public static int upoint = 800;
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
        //u盘数组
        public static ArrayList usbList = new ArrayList();
        //缓存扫到的条码信息,去重
        public static ArrayList bufarrayList = new ArrayList();

        //定义用于存放坐标的数组
        public static ArrayList cdarrayList = new ArrayList();
        public static ArrayList cdarrayList2 = new ArrayList();
        public static ArrayList cdarrayList3 = new ArrayList();
        public static ArrayList cdarrayList4 = new ArrayList();
        public static ArrayList cdarrayList5 = new ArrayList();
        public static ArrayList cdarrayList6 = new ArrayList();
        public static ArrayList cdarrayList7 = new ArrayList();
        public static ArrayList cdarrayList8 = new ArrayList();
        public static ArrayList cdarrayList9 = new ArrayList();
        public static ArrayList cdarrayList10 = new ArrayList();
        public static ArrayList cdarrayList11 = new ArrayList();

        //定义矩形区域
        //1号盘
        public static Rectangle rectangle1 = new Rectangle();
        public static Rectangle rectangle2 = new Rectangle();
        public static Rectangle rectangle14 = new Rectangle();
        public static Rectangle rectangle15 = new Rectangle();
        public static Rectangle rectangle18 = new Rectangle();
        public static Rectangle rectangle19 = new Rectangle();
        public static Rectangle rectangle21 = new Rectangle();
        public static Rectangle rectangle22 = new Rectangle();

        //2号盘
        public static Rectangle rectangle6 = new Rectangle();
        public static Rectangle rectangle7 = new Rectangle();
        public static Rectangle rectangle23 = new Rectangle();
        public static Rectangle rectangle24 = new Rectangle();
        public static Rectangle rectangle27 = new Rectangle();
        public static Rectangle rectangle28 = new Rectangle();
        public static Rectangle rectangle30 = new Rectangle();
        public static Rectangle rectangle31 = new Rectangle();

        //3号盘
        public static Rectangle rectangle11 = new Rectangle();
        public static Rectangle rectangle12 = new Rectangle();
        public static Rectangle rectangle32 = new Rectangle();
        public static Rectangle rectangle33 = new Rectangle();
        public static Rectangle rectangle35 = new Rectangle();
        public static Rectangle rectangle36 = new Rectangle();

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


        //定义三个数组用来存放三个码盘的待打印信息
        public static ArrayList printList1 = new ArrayList();
        public static ArrayList printList2 = new ArrayList();
        public static ArrayList printList3 = new ArrayList();

        //全局判断(为的是让每一行或每一列的第一个箱子的位置只摆放一次)
        public static bool isJudged = false;
        //判断是否走到了最后一行或一列(用于发出码盘摆满的报警)
        public static bool isLastRowOrCol = false;
        //判断是否已存在此规格，用于存储规格数量拷贝进U盘
        public static bool exist = false;

        public static bool sendflag = true;


        //判断是否扫到码
        public static bool saodao = false;

        //串口关闭时可能还在接收数据，此标志位用于判断关闭则不接受
        public static bool isTryCloseSp = false;
        //判断是否重复扫码
        public static ArrayList bufList = new ArrayList();

        public static Coordinate ZARA = new Coordinate();

        //比对时间，跨越24点需要读取新的日期文本数据
        public static string compareDt = null;


        //定义用于存放向数据库写入数据的数组

        List<string> sernr = new List<string>();
        List<string> zaufnr = new List<string>();
        List<string> zmatnr = new List<string>();
        //[] sernr = new string[100];
        //string[] zaufnr = new string[100];
        //string[] zmatnr = new string[100];
        public static void CalculateCoornidateAndSend()
        {

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //扫码拿到的纸箱长宽高信息
            //未维护的箱子或者木箱则不抓
            if (l == 0 || w == 0 || h == 0)
            {
                byte[] byteX = toBytes.intToBytes(0);
                byte[] byteY = toBytes.intToBytes(0);
                byte[] byteZ = toBytes.intToBytes(0);
                string[] status = new string[] { "1", "0", "0", "0", "0" };
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
                return;
            }

            //1号码盘找坐标
            if (h == 265)
            {

                //第1层第一个
                if ((count12 == 0) && (length12 + edge >= l) && (width12 + edge >= w))
                {
                    wdf.changeText1("第一层");
                    //(新的码盘清除上一个盘的数据)清空缓存数组
                    if (cdarrayList3.Count > 0)
                    {
                        cdarrayList3.RemoveRange(0, cdarrayList3.Count);
                    }

                    Coordinate k11 = new Coordinate();
                    k11.x = 0;
                    k11.y = 0;
                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0, 0, l, w);
                    //第一个箱子直接放在原点(0,0,0) 
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    //平衡两边超出距离
                    byte[] byteY = toBytes.intToBytes(1000 + 20);
                    byte[] byteZ = toBytes.intToBytes(0);
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
                    vertical4.y = (w + gap11);
                    horizontal4.x = 500;
                    horizontal4.y = 0;

                    rectangle14.length = 500;
                    width12 -= (w + gap11);
                    return;
                }
                //第一行第二个
                if (rectangle14.length + edge >= l)
                {
                    Coordinate k12 = new Coordinate();
                    k12.x = horizontal4.x;
                    k12.y = horizontal4.y;
                    wdf.DrawOcupyArea(horizontal4.x, horizontal4.y, l, w);

                    byte[] byteX = toBytes.intToBytes(horizontal4.x);
                    byte[] byteY = toBytes.intToBytes(1000 - horizontal4.y + 20);
                    byte[] byteZ = toBytes.intToBytes(0);
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

                    horizontal4.x = horizontal4.x + l + dst;
                    horizontal4.y += 0;
                    rectangle14.length = 0;
                    return;
                }
                //第二行第一个
                if ((width12 + edge >= w) && (!isJudged))
                {
                    Coordinate k13 = new Coordinate();
                    //第2.4个
                    if (count12 == 2 || count12 == 6)
                    {
                        vertical4.x = 500 - l;
                    }
                    if (count12 == 4)
                    {
                        vertical4.x = 0;
                    }
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
                        int mix = (width12 - w) / 3;
                        byte[] byteX1 = toBytes.intToBytes(vertical4.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - vertical4.y + 20);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width12 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical4.x);
                            byteY1 = toBytes.intToBytes(1000 - (1000 - w - mix) + 20);
                            byteZ1 = toBytes.intToBytes(0);
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

                        if (count12 == 3 || count12 == 7)
                        {
                            vertical4.x = 1000 - l;
                        }
                        if (count12 == 5)
                        {
                            vertical4.x = 500;
                        }
                        k13.x = vertical4.x;
                        k13.y = vertical4.y;
                        ZARA.x = k13.x;
                        ZARA.y = k13.y;
                        //vertical4.x += 0;
                        //vertical4.y += (w + gap);
                        rectangle15.length = 500;
                        cdarrayList2.Add(ZARA.x);
                        cdarrayList2.Add(ZARA.y);
                        cdarrayList2.Add(rectangle15.length);
                        width12 -= (w + gap11);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical4.x);
                    byte[] byteY = toBytes.intToBytes(1000 - vertical4.y + 20);
                    byte[] byteZ = toBytes.intToBytes(0);
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

                    if (count12 == 3 || count12 == 7)
                    {
                        vertical4.x = 1000 - l;
                    }
                    if (count12 == 5)
                    {
                        vertical4.x = 500;
                    }
                    k13.x = vertical4.x;
                    k13.y = vertical4.y;
                    ZARA.x = k13.x;
                    ZARA.y = k13.y;
                    //vertical4.x += 0;
                    vertical4.y += (w + gap11);
                    rectangle15.length = 500;
                    cdarrayList2.Add(ZARA.x);
                    cdarrayList2.Add(ZARA.y);
                    cdarrayList2.Add(rectangle15.length);
                    width12 -= (w + gap11);
                    isJudged = true;
                    return;
                }
                //第二行第二个(第三行第二个……)
                for (int i = 0; i < cdarrayList2.Count / 3; i++)
                {
                    if ((int)cdarrayList2[3 * i + 2] + edge >= l)
                    {
                        Coordinate k14 = new Coordinate();
                        k14.x = (int)cdarrayList2[3 * i];
                        k14.y = (int)cdarrayList2[3 * i + 1];
                        wdf.DrawOcupyArea((int)cdarrayList2[3 * i], (int)cdarrayList2[3 * i + 1], l, w);

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList2[3 * i]);
                        byte[] byteY = toBytes.intToBytes(1000 - ((int)cdarrayList2[3 * i + 1]) + 20);
                        byte[] byteZ = toBytes.intToBytes(0);
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

                        cdarrayList2[3 * i] = (int)cdarrayList2[3 * i] + l + dst;
                        cdarrayList2[3 * i + 1] = (int)cdarrayList2[3 * i + 1] + 0;
                        cdarrayList2[3 * i + 2] = 0;
                        isJudged = false;
                        return;
                    }
                }


                //码放第二层
                if (count == 0)
                {
                    for (int i = 0; i < cdarrayList2.Count / 3; i++)
                    {
                        cdarrayList2[3 * i + 2] = 0;
                    }
                    wdf.changeText1("第二层");
                    //工作界面绘图
                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0, 500 - l, w, l);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(500 - l + offset124);
                    byte[] byteZ = toBytes.intToBytes(280);
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
                    vertical.y = l + dst;
                    horizontal.x = w + gap12;
                    horizontal.y = 0;

                    rectangle1.length = 500;
                    length1 -= (w + gap12);
                    return;
                }
                //第一列第二个
                if (rectangle1.length + edge >= l)
                {
                    Coordinate k2 = new Coordinate();
                    vertical.y = 1000 - l;
                    k2.x = vertical.x;
                    k2.y = vertical.y;
                    wdf.DrawOcupyArea(vertical.x, vertical.y, w, l);

                    byte[] byteX = toBytes.intToBytes(vertical.x);
                    byte[] byteY = toBytes.intToBytes(vertical.y + offset124);
                    byte[] byteZ = toBytes.intToBytes(280);
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
                    vertical.y = vertical.y + l + dst;
                    rectangle1.length = 0;
                    return;
                }
                //第一行第二个
                if ((length1 + edge >= w) && (!isJudged))
                {
                    Coordinate k3 = new Coordinate();
                    if (count == 2 || count == 6)
                    {
                        horizontal.y = 0;
                    }
                    if (count == 4)
                    {
                        horizontal.y = 500 - l;
                    }
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
                            k3.x = 1000 - w;
                            k3.y = horizontal.y;
                            wdf.DrawOcupyArea(1000 - w, horizontal.y, w, l);
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
                        int mix = (length1 - w) / 3;//第二层经常侧倾，加上10的安全距离
                        byte[] byteX1 = toBytes.intToBytes(horizontal.x + 10);//
                        byte[] byteY1 = toBytes.intToBytes(horizontal.y + offset124);
                        byte[] byteZ1 = toBytes.intToBytes(280);
                        if (length1 > w)
                        {
                            byteX1 = toBytes.intToBytes(1000 - w - mix);
                            byteY1 = toBytes.intToBytes(horizontal.y + offset124);
                            byteZ1 = toBytes.intToBytes(280);
                            //让这一列上面的元素与其对齐
                            horizontal.x = 1000 - w;
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
                        if (count == 3 || count == 7)
                        {
                            horizontal.y = 500;
                        }
                        if (count == 5)
                        {
                            horizontal.y = 1000 - l;
                        }
                        k3.x = horizontal.x;
                        k3.y = horizontal.y;
                        ZARA.x = k3.x;
                        ZARA.y = k3.y;
                        // horizontal.x += (w + gap);
                        // horizontal.y += 0;
                        rectangle2.length = 500;
                        cdarrayList.Add(ZARA.x);
                        cdarrayList.Add(ZARA.y);
                        cdarrayList.Add(rectangle2.length);
                        length1 -= (w + gap12);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal.x);
                    byte[] byteY = toBytes.intToBytes(horizontal.y + offset124);
                    byte[] byteZ = toBytes.intToBytes(280);
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

                    if (count == 3 || count == 7)
                    {
                        horizontal.y = 500;
                    }
                    if (count == 5)
                    {
                        horizontal.y = 1000 - l;
                    }
                    k3.x = horizontal.x;
                    k3.y = horizontal.y;
                    ZARA.x = k3.x;
                    ZARA.y = k3.y;
                    horizontal.x += (w + gap12);
                    horizontal.y += 0;
                    rectangle2.length = 500;
                    cdarrayList.Add(ZARA.x);
                    cdarrayList.Add(ZARA.y);
                    cdarrayList.Add(rectangle2.length);
                    length1 -= (w + gap12);
                    isJudged = true;
                    return;
                }
                //第二列第二个(第三行第二个……)
                for (int i = 0; i < cdarrayList.Count / 3; i++)
                {
                    if ((int)cdarrayList[3 * i + 2] + edge >= l)
                    {
                        Coordinate k4 = new Coordinate();
                        k4.x = (int)cdarrayList[3 * i];
                        k4.y = (int)cdarrayList[3 * i + 1];
                        wdf.DrawOcupyArea((int)cdarrayList[3 * i], (int)cdarrayList[3 * i + 1], w, l);

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList[3 * i]);
                        //避免戳到，将最后一个箱子往外挪10
                        if (i == (cdarrayList.Count / 3 - 1))
                        {
                            byteX = toBytes.intToBytes((int)cdarrayList[3 * i] + 10);
                        }
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList[3 * i + 1] + offset124);
                        byte[] byteZ = toBytes.intToBytes(280);
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
                        cdarrayList[3 * i] = (int)cdarrayList[3 * i] + 0;
                        cdarrayList[3 * i + 1] = (int)cdarrayList[3 * i + 1] + l + dst;
                        cdarrayList[3 * i + 2] = 0;
                        isJudged = false;
                        return;
                    }
                }


                //如果走到这，说明第3层不能码了，该码放第4层了

                //第4层第一个
                if ((count14 == 0) && (length14 + edge >= l) && (width14 + edge >= w))
                {
                    for (int i = 0; i < cdarrayList.Count / 3; i++)
                    {
                        cdarrayList[3 * i + 2] = 0;
                    }
                    wdf.changeText1("第三层");
                    cdarrayList.RemoveRange(0, cdarrayList.Count);
                    Coordinate k29 = new Coordinate();
                    k29.x = 0;
                    k29.y = 0;
                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0, 0, l, w);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(1000 + 20);
                    byte[] byteZ = toBytes.intToBytes(560);
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
                    vertical6.y = (w + gap13);
                    horizontal6.x = 500;
                    horizontal6.y = 0;

                    rectangle21.length = 500;
                    width14 -= (w + gap13);
                    nearfinish();
                    return;
                }
                //第一列第二个
                if (rectangle21.length + edge >= l)
                {
                    Coordinate k30 = new Coordinate();
                    k30.x = horizontal6.x;
                    k30.y = horizontal6.y;
                    wdf.DrawOcupyArea(horizontal6.x, horizontal6.y, l, w);

                    byte[] byteX = toBytes.intToBytes(horizontal6.x);
                    byte[] byteY = toBytes.intToBytes(1000 - horizontal6.y + 20);
                    byte[] byteZ = toBytes.intToBytes(560);
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

                    horizontal6.x = horizontal6.x + l + dst;
                    horizontal6.y += 0;
                    rectangle21.length = 0;
                    return;
                }
                //第一列第二个
                if ((width14 + edge >= w) && (!isJudged))
                {
                    Coordinate k31 = new Coordinate();
                    //第2.4个
                    if (count14 == 2 || count14 == 6)
                    {
                        vertical6.x = 500 - l;

                    }
                    if (count14 == 4)
                    {
                        vertical6.x = 0;
                    }
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
                        int mix = (width14 - w) / 3;
                        byte[] byteX1 = toBytes.intToBytes(vertical6.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - vertical6.y + 20);
                        byte[] byteZ1 = toBytes.intToBytes(560);
                        if (width14 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical6.x);
                            byteY1 = toBytes.intToBytes(1000 - (1000 - w - mix) + 20);
                            byteZ1 = toBytes.intToBytes(560);
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

                        if (count14 == 3 || count14 == 7)
                        {
                            vertical6.x = 1000 - l;
                        }
                        if (count14 == 5)
                        {
                            vertical6.x = 500;
                        }
                        k31.x = vertical6.x;
                        k31.y = vertical6.y;
                        ZARA.x = k31.x;
                        ZARA.y = k31.y;
                        //vertical6.x += 0;
                        //vertical6.y += (w + gap);
                        rectangle22.length = 500;
                        cdarrayList4.Add(ZARA.x);
                        cdarrayList4.Add(ZARA.y);
                        cdarrayList4.Add(rectangle22.length);
                        width14 -= (w + gap13);
                        isJudged = true;
                        // isLastRowOrCol = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical6.x);
                    byte[] byteY = toBytes.intToBytes(1000 - vertical6.y + 20);
                    byte[] byteZ = toBytes.intToBytes(560);
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

                    if (count14 == 3 || count14 == 7)
                    {
                        vertical6.x = 1000 - l;
                    }
                    if (count14 == 5)
                    {
                        vertical6.x = 500;
                    }
                    k31.x = vertical6.x;
                    k31.y = vertical6.y;
                    ZARA.x = k31.x;
                    ZARA.y = k31.y;
                    vertical6.x += 0;
                    vertical6.y += (w + gap13);
                    rectangle22.length = 500;
                    cdarrayList4.Add(ZARA.x);
                    cdarrayList4.Add(ZARA.y);
                    cdarrayList4.Add(rectangle22.length);
                    width14 -= (w + gap13);
                    isJudged = true;
                    return;
                }
                //第二行第二个
                for (int i = 0; i < cdarrayList4.Count / 3; i++)
                {
                    if ((int)cdarrayList4[3 * i + 2] + edge >= l)
                    {
                        Coordinate k32 = new Coordinate();
                        k32.x = (int)cdarrayList4[3 * i];
                        k32.y = (int)cdarrayList4[3 * i + 1];
                        wdf.DrawOcupyArea((int)cdarrayList4[3 * i], (int)cdarrayList4[3 * i + 1], l, w);

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList4[3 * i]);
                        byte[] byteY = toBytes.intToBytes(1000 - (int)cdarrayList4[3 * i + 1] + 20);
                        byte[] byteZ = toBytes.intToBytes(560);
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

                        cdarrayList4[3 * i] = (int)cdarrayList4[3 * i] + l + dst;
                        cdarrayList4[3 * i + 1] = (int)cdarrayList4[3 * i + 1] + 0;
                        cdarrayList4[3 * i + 2] = 0;
                        isJudged = false;
                        return;
                    }
                }

                //如果走到这，说明第2层不能码了，该码放第3层了             

                if ((count13 == 0) && (width13 + edge >= l) && (length13 + edge >= w))
                {
                    for (int i = 0; i < cdarrayList4.Count / 3; i++)
                    {
                        cdarrayList4[3 * i + 2] = 0;
                    }
                    wdf.changeText1("第四层");
                    cdarrayList4.RemoveRange(0, cdarrayList4.Count);
                    Coordinate k19 = new Coordinate();
                    k19.x = 0;
                    k19.y = 0;
                    WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                    wdf.DrawOcupyArea(0, 500 - l, w, l);
                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0);
                    byte[] byteY = toBytes.intToBytes(500 - l + offset124);
                    byte[] byteZ = toBytes.intToBytes(840);
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
                    vertical5.y = l + dst;
                    horizontal5.x = (w + gap14);
                    horizontal5.y = 0;

                    rectangle18.length = 500;
                    length13 -= (w + gap14);
                    return;
                }
                //第一列第二个
                if (rectangle18.length + edge >= l)
                {
                    Coordinate k20 = new Coordinate();
                    vertical5.y = 1000 - l;
                    k20.x = vertical5.x;
                    k20.y = vertical5.y;
                    wdf.DrawOcupyArea(vertical5.x, vertical5.y, w, l);

                    byte[] byteX = toBytes.intToBytes(vertical5.x);
                    byte[] byteY = toBytes.intToBytes(vertical5.y + offset124);
                    byte[] byteZ = toBytes.intToBytes(840);
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
                    vertical5.y = vertical5.y + l + dst;
                    rectangle18.length = 0;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length13 + edge >= w) && (!isJudged))
                {
                    Coordinate k21 = new Coordinate();
                    if (count13 == 2 || count13 == 6)
                    {
                        horizontal5.y = 0;
                    }
                    if (count13 == 4)
                    {
                        horizontal5.y = 500 - l;
                    }
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
                            k21.x = 1000 - w;
                            k21.y = horizontal5.y;
                            wdf.DrawOcupyArea(1000 - w, horizontal5.y, w, l);
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
                        int mix = (length13 - w) / 3;
                        byte[] byteX1 = toBytes.intToBytes(horizontal5.x + 10);//侧倾的补偿距离10
                        byte[] byteY1 = toBytes.intToBytes(horizontal5.y + offset124);
                        byte[] byteZ1 = toBytes.intToBytes(840);
                        if (length13 > w)
                        {
                            byteX1 = toBytes.intToBytes(1000 - w - mix);
                            byteY1 = toBytes.intToBytes(horizontal5.y + offset124);
                            byteZ1 = toBytes.intToBytes(840);
                            horizontal5.x = 1000 - w;
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

                        if (count13 == 3 || count13 == 7)
                        {
                            horizontal5.y = 500;
                        }
                        if (count13 == 5)
                        {
                            horizontal5.y = 1000 - l;
                        }

                        k21.x = horizontal5.x;
                        k21.y = horizontal5.y;
                        ZARA.x = k21.x;
                        ZARA.y = k21.y;
                        rectangle19.length = 500;
                        cdarrayList3.Add(ZARA.x);
                        cdarrayList3.Add(ZARA.y);
                        cdarrayList3.Add(rectangle19.length);
                        //horizontal5.x += (w + gap);
                        //horizontal5.y += 0;  
                        length13 -= (w + gap14);
                        isJudged = true;
                        return;
                    }
                    byte[] byteX = toBytes.intToBytes(horizontal5.x);
                    byte[] byteY = toBytes.intToBytes(horizontal5.y + offset124);
                    byte[] byteZ = toBytes.intToBytes(840);
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

                    if (count13 == 3 || count13 == 7)
                    {
                        horizontal5.y = 500;
                    }
                    if (count13 == 5)
                    {
                        horizontal5.y = 1000 - l;
                    }
                    k21.x = horizontal5.x;
                    k21.y = horizontal5.y;
                    ZARA.x = k21.x;
                    ZARA.y = k21.y;
                    horizontal5.x += (w + gap14);
                    horizontal5.y += 0;
                    rectangle19.length = 500;
                    cdarrayList3.Add(ZARA.x);
                    cdarrayList3.Add(ZARA.y);
                    cdarrayList3.Add(rectangle19.length);
                    length13 -= (w + gap14);
                    isJudged = true;
                    return;
                }
                //第二列第二个
                for (int i = 0; i < cdarrayList3.Count / 3; i++)
                {
                    if ((int)cdarrayList3[3 * i + 2] + edge >= l)
                    {
                        Coordinate k22 = new Coordinate();
                        k22.x = (int)cdarrayList3[3 * i];
                        k22.y = (int)cdarrayList3[3 * i + 1];
                        wdf.DrawOcupyArea((int)cdarrayList3[3 * i], (int)cdarrayList3[3 * i + 1], w, l);

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList3[3 * i]);
                        //补偿箱子倾斜带来的误差
                        if (i == (cdarrayList3.Count / 3 - 1))
                        {
                            byteX = toBytes.intToBytes((int)cdarrayList3[3 * i] + 10);
                        }
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList3[3 * i + 1] + offset124);
                        byte[] byteZ = toBytes.intToBytes(840);
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
                        cdarrayList3[3 * i + 1] = (int)cdarrayList3[3 * i + 1] + l + dst;
                        cdarrayList3[3 * i + 2] = 0;
                        if (count13 == 8)
                        {
                            finish_1();
                            WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
                        }
                        isJudged = false;
                        return;
                    }
                }
                finish_1();
                WorkingDetailForm.arrayList1.RemoveRange(0, WorkingDetailForm.arrayList1.Count);
            }


            /**************************************************************************
             * ********************** *  *  * ***************************
             * ****************************************
             * ******************** * * * * * * * * *
             * **********************/
            /*************************************************************************************/
            //2号码盘找坐标
            if (h == 300 && w == 240)
            {
                //如果走到这，说明第一层不能码了，该码放第二层了

                //第二层第一个
                if ((count22 == 0) && (length22 + edge3 >= l) && (width22 + edge3 >= w))
                {
                    for (int i = 0; i < cdarrayList7.Count / 3; i++)
                    {
                        cdarrayList7[3 * i + 2] = 0;
                    }
                    wdf.changeText2("第一层");
                    Coordinate k47 = new Coordinate();
                    k47.x = 0 + offset2;
                    k47.y = 0;
                    cdarrayList5.RemoveRange(0, cdarrayList5.Count);
                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0, 0, l, w);

                    //第一个箱子直接放在原点(0,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(offset2);
                    //超出来的距离往两侧分担，一边40
                    byte[] byteY = toBytes.intToBytes(1000 + 40);
                    byte[] byteZ = toBytes.intToBytes(0);
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

                    vertical7.x = 0 + offset2;
                    vertical7.y = 0 + (w + gap2);
                    horizontal7.x = offset2 + l + dst;
                    horizontal7.y = 0;

                    rectangle23.length = length22 - l - dst;
                    width22 -= (w + gap2);
                    return;
                }
                //第一列第二个及以上
                if (rectangle23.length + edge3 >= l)
                {
                    Coordinate k48 = new Coordinate();
                    if (rectangle23.length - l + edge3 < 390)
                    {
                        if (rectangle23.length < l)
                        {
                            k48.x = horizontal7.x;
                            k48.y = horizontal7.y;
                            wdf.DrawOcupyArea2(horizontal7.x - offset2, horizontal7.y, l, w);
                        }
                        else
                        {
                            k48.x = offset2 + 1000 - l;
                            k48.y = horizontal7.y;
                            wdf.DrawOcupyArea2(1000 - l, horizontal7.y, l, w);
                        }
                    }
                    else
                    {
                        k48.x = horizontal7.x;
                        k48.y = horizontal7.y;
                        wdf.DrawOcupyArea2(horizontal7.x - offset2, horizontal7.y, l, w);
                    }

                    arrayList.Add(k48);
                    if (arrayList.Count == 2)
                    {
                        // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    if (rectangle23.length - l + edge3 < 390)
                    {
                        int mix = (rectangle23.length - l) / 3;
                        byte[] byteX1 = toBytes.intToBytes(horizontal7.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - ((horizontal7.y)) + 40);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (rectangle23.length > l)
                        {
                            byteX1 = toBytes.intToBytes(offset2 + 1000 - l - mix);
                            byteY1 = toBytes.intToBytes(1000 - (horizontal7.y) + 40);
                            byteZ1 = toBytes.intToBytes(0);
                            horizontal7.x = offset2 + 1000 - l;
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
                        rectangle23.length = rectangle23.length - l - dst;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal7.x);
                    byte[] byteY = toBytes.intToBytes(1000 - (horizontal7.y) + 40);
                    byte[] byteZ = toBytes.intToBytes(0);
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

                    horizontal7.x = horizontal7.x + l + dst;
                    horizontal7.y += 0;
                    rectangle23.length = rectangle23.length - l - dst;
                    return;
                }
                //第一行第二个
                if ((width22 + edge3 >= w))
                {
                    Coordinate k49 = new Coordinate();
                    if (width22 - w + edge3 < 240)
                    {
                        if (width22 < w)
                        {
                            k49.x = vertical7.x;
                            k49.y = vertical7.y;
                            wdf.DrawOcupyArea2(vertical7.x - offset2, vertical7.y, l, w);
                        }
                        else
                        {
                            k49.x = vertical7.x;
                            k49.y = 1000 - w;
                            wdf.DrawOcupyArea2(vertical7.x - offset2, 1000 - w, l, w);
                        }
                    }
                    else
                    {
                        k49.x = vertical7.x;
                        k49.y = vertical7.y;
                        wdf.DrawOcupyArea2(vertical7.x - offset2, vertical7.y, l, w);
                    }
                    //特殊处理，规定不能超出边缘
                    if (width22 - w + edge3 < 240)
                    {
                        int mix = (int)((width22 - w) * 0.9);
                        byte[] byteX1 = toBytes.intToBytes(vertical7.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - (vertical7.y) + 40);
                        byte[] byteZ1 = toBytes.intToBytes(0);
                        if (width22 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical7.x);
                            byteY1 = toBytes.intToBytes(1000 - ((1000 - w - mix)) + 40);
                            byteZ1 = toBytes.intToBytes(0);
                            vertical7.y = 1000 - w - mix;
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
                        k49.x = vertical7.x + l + dst;
                        k49.y = vertical7.y;
                        ZARA.x = k49.x;
                        ZARA.y = k49.y;
                        vertical7.x += 0;
                        vertical7.y += (w + gap2);
                        rectangle24.length = length22 - l - dst;
                        cdarrayList6.Add(ZARA.x);
                        cdarrayList6.Add(ZARA.y);
                        cdarrayList6.Add(rectangle24.length);
                        width22 -= (w + gap2);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical7.x);
                    byte[] byteY = toBytes.intToBytes(1000 - (vertical7.y) + 40);
                    byte[] byteZ = toBytes.intToBytes(0);
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

                    k49.x = vertical7.x + l + dst;
                    k49.y = vertical7.y;
                    ZARA.x = k49.x;
                    ZARA.y = k49.y;
                    vertical7.x += 0;
                    vertical7.y += (w + gap2);
                    rectangle24.length = length22 - l - dst;
                    cdarrayList6.Add(ZARA.x);
                    cdarrayList6.Add(ZARA.y);
                    cdarrayList6.Add(rectangle24.length);
                    width22 -= (w + gap2);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0; i < cdarrayList6.Count / 3; i++)
                {
                    //不让下面一排的箱子干涉上面一排的码放
                    if (i < (cdarrayList6.Count / 3 - 1))
                    {
                        if ((int)cdarrayList6[3 * i + 2] > (int)cdarrayList6[3 * (i + 1) + 2])
                        {
                            int sub = (int)cdarrayList6[3 * i + 2] - (int)cdarrayList6[3 * (i + 1) + 2];
                            cdarrayList6[3 * i] = (int)cdarrayList6[3 * i] + sub;
                            cdarrayList6[3 * i + 2] = (int)cdarrayList6[3 * i + 2] - sub;
                            //continue;
                        }
                    }
                    if ((int)cdarrayList6[3 * i + 2] + edge3 >= l)
                    {
                        Coordinate k50 = new Coordinate();
                        if ((int)cdarrayList6[3 * i + 2] - l + edge3 < 390)
                        {
                            if ((int)cdarrayList6[3 * i + 2] < l)
                            {
                                k50.x = (int)cdarrayList6[3 * i];
                                k50.y = (int)cdarrayList6[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList6[3 * i], (int)cdarrayList6[3 * i + 1] - 1500, l, w);
                            }
                            else
                            {
                                k50.x = offset2 + 1000 - l - dst;
                                k50.y = (int)cdarrayList6[3 * i + 1];
                                wdf.DrawOcupyArea2(1000 - l, (int)cdarrayList6[3 * i + 1], l, w);
                            }
                        }
                        else
                        {
                            k50.x = (int)cdarrayList6[3 * i];
                            k50.y = (int)cdarrayList6[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList6[3 * i], (int)cdarrayList6[3 * i + 1], l, w);
                        }

                        if ((int)cdarrayList6[3 * i + 2] - l + edge3 < 390)
                        {
                            int mix = ((int)cdarrayList6[3 * i + 2] - l) / 3;
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList6[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes(1000 - ((int)cdarrayList6[3 * i + 1]) + 40);
                            byte[] byteZ1 = toBytes.intToBytes(0);
                            if ((int)cdarrayList6[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(offset2 + 1000 - l - mix);
                                byteY1 = toBytes.intToBytes(1000 - ((int)cdarrayList6[3 * i + 1]) + 40);
                                byteZ1 = toBytes.intToBytes(0);
                                horizontal7.x = offset2 + 1000 - l;
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
                            cdarrayList6[3 * i + 2] = (int)cdarrayList6[3 * i + 2] - l - dst;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList6[3 * i]);
                        byte[] byteY = toBytes.intToBytes(1000 - ((int)cdarrayList6[3 * i + 1]) + 40);
                        byte[] byteZ = toBytes.intToBytes(0);
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

                        cdarrayList6[3 * i] = (int)cdarrayList6[3 * i] + l + dst;
                        cdarrayList6[3 * i + 1] = (int)cdarrayList6[3 * i + 1] + 0;
                        cdarrayList6[3 * i + 2] = (int)cdarrayList6[3 * i + 2] - l - dst;
                        return;
                    }
                }


                if (count2 == 0)
                {
                    for (int i = 0; i < cdarrayList6.Count / 3; i++)
                    {
                        cdarrayList6[3 * i + 2] = 0;
                    }
                    wdf.changeText2("第二层");
                    Coordinate k37 = new Coordinate();
                    k37.x = offset2;
                    k37.y = 0;
                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0, 0, w, l);

                    //第一个箱子直接放在原点(0,1600,0)
                    //发坐标（包含挡板状态）
                    //第二个码盘原点距第一个码盘原点1600mm
                    //所有后续的横坐标都要加上
                    byte[] byteX = toBytes.intToBytes(offset2 - offset24);
                    byte[] byteY = toBytes.intToBytes(0 + 40);
                    byte[] byteZ = toBytes.intToBytes(315);
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

                    vertical2.x = 0 + offset2;
                    vertical2.y = l + dst;
                    horizontal2.x = w + gap2 + offset2;
                    horizontal2.y = 0;

                    rectangle6.length = 1000 - l - dst;
                    length2 -= (w + gap2);
                    return;
                }
                //第一列第二个及以上
                if (rectangle6.length + edge3 >= l)
                {
                    Coordinate k38 = new Coordinate();
                    if ((rectangle6.length - l + edge3) < 390)
                    {
                        if (rectangle6.length < l)
                        {
                            k38.x = vertical2.x;
                            k38.y = vertical2.y;
                            wdf.DrawOcupyArea2(vertical2.x - offset2, vertical2.y, w, l);
                        }
                        else
                        {
                            k38.x = vertical2.x;
                            k38.y = 1000 - l;
                            wdf.DrawOcupyArea2(vertical2.x - offset2, 1000 - l, w, l);
                        }

                    }
                    else
                    {
                        k38.x = vertical2.x;
                        k38.y = vertical2.y;
                        wdf.DrawOcupyArea2(vertical2.x - offset2, vertical2.y, w, l);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle6.length - l + edge3) < 390)
                    {
                        int mix = (rectangle6.length - l) / 3;
                        byte[] byteX1 = toBytes.intToBytes(vertical2.x - offset24);
                        byte[] byteY1 = toBytes.intToBytes(vertical2.y + 70);
                        byte[] byteZ1 = toBytes.intToBytes(315);
                        if (rectangle6.length > l)
                        {
                            byteX1 = toBytes.intToBytes(vertical2.x - offset24);
                            byteY1 = toBytes.intToBytes(1000 - l - mix + 70);
                            byteZ1 = toBytes.intToBytes(315);
                            vertical2.y = 1000 - l;
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
                        rectangle6.length = rectangle6.length - l - dst;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical2.x - offset24);
                    byte[] byteY = toBytes.intToBytes(vertical2.y + 70);
                    byte[] byteZ = toBytes.intToBytes(315);
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
                    vertical2.y = vertical2.y + l + dst;
                    rectangle6.length = rectangle6.length - l - dst;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length2 + edge3 >= w))
                {
                    Coordinate k39 = new Coordinate();
                    if (length2 - w + edge3 < 240)
                    {
                        if (length2 < w)
                        {
                            k39.x = horizontal2.x;
                            k39.y = horizontal2.y;
                            wdf.DrawOcupyArea2(horizontal2.x - offset2, horizontal2.y, w, l);
                        }
                        else
                        {
                            k39.x = offset2 + 1000 - w;
                            k39.y = horizontal2.y;
                            wdf.DrawOcupyArea2(1000 - w, horizontal2.y, w, l);
                        }
                    }
                    else
                    {
                        k39.x = horizontal2.x;
                        k39.y = horizontal2.y;
                        wdf.DrawOcupyArea2(horizontal2.x - offset2, horizontal2.y, w, l);
                    }

                    if (length2 - w + edge3 < 240)
                    {
                        int mix = (int)((length2 - w) * 0.9);
                        byte[] byteX1 = toBytes.intToBytes(horizontal2.x - offset24);
                        byte[] byteY1 = toBytes.intToBytes(horizontal2.y + 40);
                        byte[] byteZ1 = toBytes.intToBytes(315);
                        if (length2 > w)
                        {
                            byteX1 = toBytes.intToBytes(offset2 + 1000 - w - mix - offset24);
                            byteY1 = toBytes.intToBytes(horizontal2.y + 40);
                            byteZ1 = toBytes.intToBytes(315);
                            horizontal2.x = offset2 + 1000 - w - mix;
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
                        k39.y = l + dst;
                        ZARA.x = k39.x;
                        ZARA.y = k39.y;
                        horizontal2.x += (w + gap2);
                        horizontal2.y += 0;
                        rectangle7.length = width2 - l - dst;
                        cdarrayList5.Add(ZARA.x);
                        cdarrayList5.Add(ZARA.y);
                        cdarrayList5.Add(rectangle7.length);
                        length2 -= (w + gap2);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal2.x - offset24);
                    byte[] byteY = toBytes.intToBytes(horizontal2.y + 40);
                    byte[] byteZ = toBytes.intToBytes(315);
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
                    k39.y = l + dst;
                    ZARA.x = k39.x;
                    ZARA.y = k39.y;
                    horizontal2.x += (w + gap2);
                    horizontal2.y += 0;
                    rectangle7.length = width2 - l - dst;
                    cdarrayList5.Add(ZARA.x);
                    cdarrayList5.Add(ZARA.y);
                    cdarrayList5.Add(rectangle7.length);
                    length2 -= (w + gap2);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0; i < cdarrayList5.Count / 3; i++)
                {
                    //不让下面一排的箱子干涉上面一排的码放
                    if (i < (cdarrayList5.Count / 3 - 1))
                    {
                        if ((int)cdarrayList5[3 * i + 2] > (int)cdarrayList5[3 * (i + 1) + 2])
                        {
                            int sub = (int)cdarrayList5[3 * i + 2] - (int)cdarrayList5[3 * (i + 1) + 2];
                            cdarrayList5[3 * i] = (int)cdarrayList5[3 * i] + sub;
                            cdarrayList5[3 * i + 2] = (int)cdarrayList5[3 * i + 2] - sub;
                            //continue;
                        }
                    }
                    if ((int)cdarrayList5[3 * i + 2] + edge3 >= l)
                    {
                        Coordinate k40 = new Coordinate();
                        if (((int)cdarrayList5[3 * i + 2] - l + edge3) < 390)
                        {
                            if ((int)cdarrayList5[3 * i + 2] < l)
                            {
                                k40.x = (int)cdarrayList5[3 * i];
                                k40.y = (int)cdarrayList5[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList5[3 * i] - offset2, (int)cdarrayList5[3 * i + 1], w, l);
                            }
                            else
                            {
                                k40.x = (int)cdarrayList5[3 * i];
                                k40.y = 1000 - l;
                                wdf.DrawOcupyArea2((int)cdarrayList5[3 * i] - offset2, 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k40.x = (int)cdarrayList5[3 * i];
                            k40.y = (int)cdarrayList5[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList5[3 * i] - offset2, (int)cdarrayList5[3 * i + 1], w, l);
                        }

                        //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                        if (((int)cdarrayList5[3 * i + 2] - l + edge3) < 390)
                        {
                            int mix = ((int)cdarrayList5[3 * i + 2] - l) / 3;
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList5[3 * i] - offset24);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList5[3 * i + 1] + 70);
                            byte[] byteZ1 = toBytes.intToBytes(315);
                            if ((int)cdarrayList5[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList5[3 * i] - offset24);
                                byteY1 = toBytes.intToBytes(1000 - l - mix + 70);
                                byteZ1 = toBytes.intToBytes(315);
                                vertical2.y = 1000 - l;
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
                            cdarrayList5[3 * i + 2] = (int)cdarrayList5[3 * i + 2] - l - dst;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList5[3 * i] - offset24);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList5[3 * i + 1] + 70);
                        byte[] byteZ = toBytes.intToBytes(315);
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
                        cdarrayList5[3 * i + 1] = (int)cdarrayList5[3 * i + 1] + l + dst;
                        cdarrayList5[3 * i + 2] = (int)cdarrayList5[3 * i + 2] - l - dst;
                        return;
                    }
                }


                //如果走到这，说明第3层不能码了，该码放第4层了

                //第4层第一个
                if ((count24 == 0) && (length24 + edge3 >= l) && (width24 + edge3 >= w))
                {
                    for (int i = 0; i < cdarrayList5.Count / 3; i++)
                    {
                        cdarrayList5[3 * i + 2] = 0;
                    }
                    wdf.changeText2("第三层");
                    cdarrayList5.RemoveRange(0, cdarrayList5.Count);
                    Coordinate k65 = new Coordinate();
                    k65.x = offset2;
                    k65.y = 0;
                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0, 0, l, w);

                    //第一个箱子直接放在原点(1600,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(offset2);
                    byte[] byteY = toBytes.intToBytes(1000 + 40);
                    byte[] byteZ = toBytes.intToBytes(630);
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

                    vertical9.x = offset2;
                    vertical9.y = w + gap2;
                    horizontal9.x = offset2 + l + dst;
                    horizontal9.y = 0;

                    rectangle30.length = length24 - l - dst;
                    width24 -= (w + gap2);
                    nearfinish();
                    return;
                }
                //第一行第二个及以上
                if (rectangle30.length + edge3 >= l)
                {
                    Coordinate k66 = new Coordinate();
                    if (rectangle30.length - l + edge3 < 390)
                    {
                        if (rectangle30.length < l)
                        {
                            k66.x = horizontal9.x;
                            k66.y = horizontal9.y;
                            wdf.DrawOcupyArea2(horizontal9.x - offset2, horizontal9.y, l, w);
                        }
                        else
                        {
                            k66.x = offset2 + 1000 - l;
                            k66.y = horizontal9.y;
                            wdf.DrawOcupyArea2(1000 - l, horizontal9.y, l, w);
                        }
                    }
                    else
                    {
                        k66.x = horizontal9.x;
                        k66.y = horizontal9.y;
                        wdf.DrawOcupyArea2(horizontal9.x - offset2, horizontal9.y, l, w);
                    }

                    if (rectangle30.length - l + edge3 < 390)
                    {
                        int mix = (rectangle30.length - l) / 3;
                        byte[] byteX1 = toBytes.intToBytes(horizontal9.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - horizontal9.y + 40);
                        byte[] byteZ1 = toBytes.intToBytes(630);
                        if (rectangle30.length > l)
                        {
                            byteX1 = toBytes.intToBytes(offset2 + 1000 - l);
                            byteY1 = toBytes.intToBytes(1000 - horizontal9.y + 40);
                            byteZ1 = toBytes.intToBytes(630);
                            horizontal9.x = offset2 + 1000 - l;
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
                        rectangle30.length = rectangle30.length - l - dst;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal9.x);
                    byte[] byteY = toBytes.intToBytes(1000 - horizontal9.y + 40);
                    byte[] byteZ = toBytes.intToBytes(630);
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

                    horizontal9.x = horizontal9.x + l + dst;
                    horizontal9.y += 0;
                    rectangle30.length = rectangle30.length - l - dst;
                    return;
                }
                //第一列第二个
                if ((width24 + edge3 >= w))
                {
                    Coordinate k67 = new Coordinate();
                    if (width24 - w + edge3 < 240)
                    {
                        if (width24 < w)
                        {
                            k67.x = vertical9.x;
                            k67.y = vertical9.y;
                            wdf.DrawOcupyArea2(vertical9.x - offset2, vertical9.y, l, w);
                        }
                        else
                        {
                            k67.x = vertical9.x;
                            k67.y = 1000 - w;
                            wdf.DrawOcupyArea2(vertical9.x - offset2, 1000 - w, l, w);
                        }
                    }
                    else
                    {
                        k67.x = vertical9.x;
                        k67.y = vertical9.y;
                        wdf.DrawOcupyArea2(vertical9.x - offset2, vertical9.y, l, w);
                    }

                    if (width24 - w + edge < 240)
                    {
                        int mix = (int)((width24 - w) * 0.9);
                        byte[] byteX1 = toBytes.intToBytes(vertical9.x);
                        byte[] byteY1 = toBytes.intToBytes(1000 - vertical9.y + 40);
                        byte[] byteZ1 = toBytes.intToBytes(630);
                        if (width24 > w)
                        {
                            byteX1 = toBytes.intToBytes(vertical9.x);
                            byteY1 = toBytes.intToBytes(1000 - (1000 - w - mix) + 40);
                            byteZ1 = toBytes.intToBytes(630);
                            vertical9.y = 1000 - w - mix;
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
                        k67.x = vertical9.x + l + dst;
                        k67.y = vertical9.y;
                        ZARA.x = k67.x;
                        ZARA.y = k67.y;
                        vertical9.x += 0;
                        vertical9.y += (w + gap2);
                        rectangle31.length = length24 - l - dst;
                        cdarrayList8.Add(ZARA.x);
                        cdarrayList8.Add(ZARA.y);
                        cdarrayList8.Add(rectangle31.length);
                        width24 -= (w + gap2);
                        isJudged = true;
                        isLastRowOrCol = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical9.x);
                    byte[] byteY = toBytes.intToBytes(1000 - vertical9.y + 40);
                    byte[] byteZ = toBytes.intToBytes(630);
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

                    k67.x = vertical9.x + l + dst;
                    k67.y = vertical9.y;
                    ZARA.x = k67.x;
                    ZARA.y = k67.y;
                    vertical9.x += 0;
                    vertical9.y += (w + gap2);
                    rectangle31.length = length24 - l - dst;
                    cdarrayList8.Add(ZARA.x);
                    cdarrayList8.Add(ZARA.y);
                    cdarrayList8.Add(rectangle31.length);
                    width24 -= (w + gap2);
                    isJudged = true;
                    return;
                }
                //第二行第二个及以上
                for (int i = 0; i < cdarrayList8.Count / 3; i++)
                {
                    //不让下面一排的箱子干涉上面一排的码放
                    if (i < (cdarrayList8.Count / 3 - 1))
                    {
                        if ((int)cdarrayList8[3 * i + 2] > (int)cdarrayList8[3 * (i + 1) + 2])
                        {
                            int sub = (int)cdarrayList8[3 * i + 2] - (int)cdarrayList8[3 * (i + 1) + 2];
                            cdarrayList8[3 * i] = (int)cdarrayList8[3 * i] + sub;
                            cdarrayList8[3 * i + 2] = (int)cdarrayList8[3 * i + 2] - sub;
                            //continue;
                        }
                    }
                    if ((int)cdarrayList8[3 * i + 2] + edge3 >= l)
                    {
                        Coordinate k68 = new Coordinate();
                        if ((int)cdarrayList8[3 * i + 2] - l + edge3 < 390)
                        {
                            if ((int)cdarrayList8[3 * i + 2] < l)
                            {
                                k68.x = (int)cdarrayList8[3 * i];
                                k68.y = (int)cdarrayList8[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList8[3 * i] - offset2, (int)cdarrayList8[3 * i + 1], l, w);
                            }
                            else
                            {
                                k68.x = offset2 + 1000 - l;
                                k68.y = (int)cdarrayList8[3 * i + 1];
                                wdf.DrawOcupyArea2(1000 - l, (int)cdarrayList8[3 * i + 1], l, w);
                            }
                        }
                        else
                        {
                            k68.x = (int)cdarrayList8[3 * i];
                            k68.y = (int)cdarrayList8[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList8[3 * i] - offset2, (int)cdarrayList8[3 * i + 1], l, w);
                        }

                        if ((int)cdarrayList8[3 * i + 2] - l + edge3 < 390)
                        {
                            int mix = ((int)cdarrayList8[3 * i + 2] - l) / 3;
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList8[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes(1000 - (int)cdarrayList8[3 * i + 1] + 40);
                            byte[] byteZ1 = toBytes.intToBytes(630);
                            if ((int)cdarrayList8[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes(offset2 + 1000 - l);
                                byteY1 = toBytes.intToBytes(1000 - (int)cdarrayList8[3 * i + 1] + 40);
                                byteZ1 = toBytes.intToBytes(630);
                                horizontal9.x = offset2 + 1000 - l;
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
                            cdarrayList8[3 * i + 2] = (int)cdarrayList8[3 * i + 2] - l - dst;
                            isJudged = false;
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList8[3 * i]);
                        byte[] byteY = toBytes.intToBytes(1000 - (int)cdarrayList8[3 * i + 1] + 40);
                        byte[] byteZ = toBytes.intToBytes(630);
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

                        cdarrayList8[3 * i] = (int)cdarrayList8[3 * i] + l + dst;
                        cdarrayList8[3 * i + 1] = (int)cdarrayList8[3 * i + 1] + 0;
                        cdarrayList8[3 * i + 2] = (int)cdarrayList8[3 * i + 2] - l + dst;
                        return;
                    }
                }

                //如果走到这，说明第2层不能码了，该码放第3层了

                if ((count23 == 0) && (width23 + edge3 >= l) && (length23 + edge3 >= w))
                {
                    for (int i = 0; i < cdarrayList8.Count / 3; i++)
                    {
                        cdarrayList8[3 * i + 2] = 0;
                    }
                    wdf.changeText2("第四层");
                    cdarrayList8.RemoveRange(0, cdarrayList8.Count);
                    Coordinate k55 = new Coordinate();
                    k55.x = 0 + offset2;
                    k55.y = 0;
                    arrayList.Add(k55);
                    if (arrayList.Count == 2)
                    {
                        // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                    wdf.DrawOcupyArea2(0, 0, w, l);

                    //第一个箱子直接放在原点(0,1500,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(0 + offset2);
                    byte[] byteY = toBytes.intToBytes(0 + 40);
                    byte[] byteZ = toBytes.intToBytes(955);
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

                    vertical8.x = 0 + offset2;
                    vertical8.y = l + dst;
                    horizontal8.x = offset2 + w + gap2;
                    horizontal8.y = 0;

                    rectangle27.length = width23 - l - dst;
                    length23 -= (w + gap2);
                    return;
                }
                //第一列第二个及以上
                if (rectangle27.length + edge3 >= l)
                {
                    Coordinate k56 = new Coordinate();
                    if ((rectangle27.length - l + edge3) < 390)
                    {
                        if (rectangle27.length < l)
                        {
                            k56.x = vertical8.x;
                            k56.y = vertical8.y;
                            wdf.DrawOcupyArea2(vertical8.x - offset2, vertical8.y, w, l);
                        }
                        else
                        {
                            k56.x = vertical8.x;
                            k56.y = 1000 - l;
                            wdf.DrawOcupyArea2(vertical8.x - offset2, 1000 - l, w, l);
                        }
                    }
                    else
                    {
                        k56.x = vertical8.x;
                        k56.y = vertical8.y;
                        wdf.DrawOcupyArea2(vertical8.x - offset2, vertical8.y, w, l);
                    }

                    arrayList.Add(k56);
                    if (arrayList.Count == 2)
                    {
                        // CalcCityDistance((Coordinate)arrayList[0], (Coordinate)arrayList[1]);
                        arrayList.RemoveAt(0);
                    }

                    //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                    if ((rectangle27.length - l + edge3) < 390)
                    {
                        int mix = (rectangle27.length - l) / 3;
                        byte[] byteX1 = toBytes.intToBytes(vertical8.x);
                        byte[] byteY1 = toBytes.intToBytes(vertical8.y + 40);
                        byte[] byteZ1 = toBytes.intToBytes(955);
                        if (rectangle27.length > l)
                        {
                            byteX1 = toBytes.intToBytes(vertical8.x);
                            byteY1 = toBytes.intToBytes(1000 - l + 40);
                            byteZ1 = toBytes.intToBytes(955);
                            vertical8.y = 1000 - l;
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
                        rectangle27.length = rectangle27.length - l - dst;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(vertical8.x);
                    byte[] byteY = toBytes.intToBytes(vertical8.y + 40);
                    byte[] byteZ = toBytes.intToBytes(955);
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
                    vertical8.y = vertical8.y + l + dst;
                    rectangle27.length = rectangle27.length - l - dst;
                    return;
                }
                //第一行第二个(第二列第一个)
                if ((length23 + edge3 >= w) && (!isJudged))
                {
                    Coordinate k57 = new Coordinate();
                    if (length23 - w + edge3 < 240)
                    {
                        if (length23 < w)
                        {
                            k57.x = horizontal8.x;
                            k57.y = horizontal8.y;
                            wdf.DrawOcupyArea2(horizontal8.x - offset2, horizontal8.y, w, l);
                        }
                        else
                        {
                            k57.x = offset2 + 1000 - w;
                            k57.y = horizontal8.y;
                            wdf.DrawOcupyArea2(1000 - w, horizontal8.y, w, l);
                        }
                    }
                    else
                    {
                        k57.x = horizontal8.x;
                        k57.y = horizontal8.y;
                        wdf.DrawOcupyArea2(horizontal8.x - offset2, horizontal8.y, w, l);
                    }

                    if (length23 - w + edge3 < 240)
                    {
                        int mix = (int)((length23 - w) * 0.9);
                        byte[] byteX1 = toBytes.intToBytes(horizontal8.x);
                        byte[] byteY1 = toBytes.intToBytes(horizontal8.y + 40);
                        byte[] byteZ1 = toBytes.intToBytes(955);
                        if (length23 > w)
                        {
                            byteX1 = toBytes.intToBytes(offset2 + 1000 - w - mix);
                            byteY1 = toBytes.intToBytes(horizontal8.y + 40);
                            byteZ1 = toBytes.intToBytes(955);
                            horizontal8.x = offset2 + 1000 - w - mix;
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
                        k57.y = l + dst;
                        ZARA.x = k57.x;
                        ZARA.y = k57.y;
                        horizontal8.x += (w + gap2);
                        horizontal8.y += 0;
                        rectangle28.length = width23 - l - dst;
                        cdarrayList7.Add(ZARA.x);
                        cdarrayList7.Add(ZARA.y);
                        cdarrayList7.Add(rectangle28.length);
                        length23 -= (w + gap2);
                        isJudged = true;
                        return;
                    }

                    byte[] byteX = toBytes.intToBytes(horizontal8.x);
                    byte[] byteY = toBytes.intToBytes(horizontal8.y + 40);
                    byte[] byteZ = toBytes.intToBytes(955);
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
                    k57.y = l + dst;
                    ZARA.x = k57.x;
                    ZARA.y = k57.y;
                    horizontal8.x += (w + gap2);
                    horizontal8.y += 0;
                    rectangle28.length = width23 - l - dst;
                    cdarrayList7.Add(ZARA.x);
                    cdarrayList7.Add(ZARA.y);
                    cdarrayList7.Add(rectangle28.length);
                    length23 -= (w + gap2);
                    isJudged = true;
                    return;
                }
                //第二列第二个及以上
                for (int i = 0; i < cdarrayList7.Count / 3; i++)
                {
                    //不让下面一排的箱子干涉上面一排的码放
                    if (i < (cdarrayList7.Count / 3 - 1))
                    {
                        if ((int)cdarrayList7[3 * i + 2] > (int)cdarrayList7[3 * (i + 1) + 2])
                        {
                            int sub = (int)cdarrayList7[3 * i + 2] - (int)cdarrayList7[3 * (i + 1) + 2];
                            cdarrayList7[3 * i] = (int)cdarrayList7[3 * i] + sub;
                            cdarrayList7[3 * i + 2] = (int)cdarrayList7[3 * i + 2] - sub;
                            //continue;
                        }
                    }
                    if ((int)cdarrayList7[3 * i + 2] + edge3 >= l)
                    {
                        Coordinate k58 = new Coordinate();
                        if (((int)cdarrayList7[3 * i + 2] - l + edge3) < 390)
                        {
                            if ((int)cdarrayList7[3 * i + 2] < l)
                            {
                                k58.x = (int)cdarrayList7[3 * i];
                                k58.y = (int)cdarrayList7[3 * i + 1];
                                wdf.DrawOcupyArea2((int)cdarrayList7[3 * i] - offset2, (int)cdarrayList7[3 * i + 1], w, l);
                            }
                            else
                            {
                                k58.x = (int)cdarrayList7[3 * i];
                                k58.y = 1000 - l;
                                wdf.DrawOcupyArea2((int)cdarrayList7[3 * i] - offset2, 1000 - l, w, l);
                            }
                        }
                        else
                        {
                            k58.x = (int)cdarrayList7[3 * i];
                            k58.y = (int)cdarrayList7[3 * i + 1];
                            wdf.DrawOcupyArea2((int)cdarrayList7[3 * i] - offset2, (int)cdarrayList7[3 * i + 1], w, l);
                        }

                        //如果剩余长度放不了下一个箱子，则将当前箱子往外挪
                        if (((int)cdarrayList7[3 * i + 2] - l + edge3) < 390)
                        {
                            int mix = ((int)cdarrayList7[3 * i + 2] - l) / 3;
                            byte[] byteX1 = toBytes.intToBytes((int)cdarrayList7[3 * i]);
                            byte[] byteY1 = toBytes.intToBytes((int)cdarrayList7[3 * i + 1] + 40);
                            byte[] byteZ1 = toBytes.intToBytes(955);
                            if ((int)cdarrayList7[3 * i + 2] > l)
                            {
                                byteX1 = toBytes.intToBytes((int)cdarrayList7[3 * i]);
                                byteY1 = toBytes.intToBytes(1000 - l + 40);
                                byteZ1 = toBytes.intToBytes(955);
                                vertical8.y = 1000 - l;
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
                            cdarrayList7[3 * i + 2] = (int)cdarrayList7[3 * i + 2] - l - dst;
                            isJudged = false;
                            if (i == (cdarrayList7.Count / 3 - 1))
                            {
                                finish_2();
                                WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
                            }
                            return;
                        }

                        byte[] byteX = toBytes.intToBytes((int)cdarrayList7[3 * i]);
                        byte[] byteY = toBytes.intToBytes((int)cdarrayList7[3 * i + 1] + 40);
                        byte[] byteZ = toBytes.intToBytes(955);
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


                        isJudged = false;
                        cdarrayList7[3 * i] = (int)cdarrayList7[3 * i] + 0;
                        cdarrayList7[3 * i + 1] = (int)cdarrayList7[3 * i + 1] + l + dst;
                        cdarrayList7[3 * i + 2] = (int)cdarrayList7[3 * i + 2] - l - dst;
                        return;
                    }
                }
                finish_2();
                WorkingDetailForm.arrayList2.RemoveRange(0, WorkingDetailForm.arrayList2.Count);
            }

            /*********************
             * *****************
             * ***************
             * *************************************************************************/
            //第三个码盘♂
            if (h == 300 && w == 320)
            {
                if (code == "200")
                {
                    if (l <= upoint)
                    {
                        wdf.changeText3("第一层");
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 680;
                        k73.y = 1000 - divpoint;
                        wdf.DrawOcupyArea3(0, 320, 1000-divpoint, w);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(divpoint);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90(最高两位00:-90;01:0;10:90;11:180)
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
                        code = "0";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 680, l, w);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1000);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "1";
                    }
                    return;
                }
                if (code == "0")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, 1000-divpoint, 280);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(divpoint + l);//可能到限位
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "00";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = 320 + offset3;
                        k73.y = 1000;
                        wdf.DrawOcupyArea3(0, 680, l, w);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(0);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90
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
                        code = "01";
                    }
                    return;
                }
                if (code == "1")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 360;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 320, 1000-divpoint, w);
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        //可能会到限位
                        //搞不来
                        byte[] byteY = toBytes.intToBytes(l + divpoint);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "10";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 360;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 320, l, w);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1000);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "11";
                    }
                    return;
                }
                if (code == "00")
                {
                    //短短短
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;//极有可能到限位
                        k73.y = 680;
                        wdf.DrawOcupyArea3(0, 680, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "000";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 320;
                        k73.y = 1000;
                        wdf.DrawOcupyArea3(0, 680, l, w);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(0);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90
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
                        code = "001";
                    }
                    return;
                }
                if (code == "01")
                {
                    //短-长-短
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, 1000-divpoint, 280);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(l + divpoint);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "010";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, l, 280);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1000);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "011";
                    }
                    return;
                }
                if (code == "011")
                {
                    //第二层
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, divpoint, w, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "0110";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 1000;
                        wdf.DrawOcupyArea3(680, 1000-l, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(0);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "0111";
                    }
                    return;
                }
                if (code == "0110")
                {
                    //短-长-短
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - l - divpoint;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, divpoint, 280, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "01100";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 1000 - l, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "01101";
                    }
                    return;
                }
                if (code == "0111")
                {
                    //不管长短,统一贴边               
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 640;
                    wdf.DrawOcupyArea3(320, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(360);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "01112";
                    return;
                }
                if (code == "01112")
                {
                    //不管长短,统一贴边               
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "011122";
                    return;
                }
                if (code == "011122")
                {
                    //第三层，不管长短,统一贴边 
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "0111222";
                    return;
                }
                //此盘最后一个
                if (code == "0111222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //添加报警信息
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "01100")
                {
                    //不管长短，统一左上
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "011002";
                    return;
                }
                if (code == "011002")
                {
                    //第三层
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "0110022";
                    return;
                }
                if (code == "0110022")
                {
                    //此盘最后一个
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 320;
                    wdf.DrawOcupyArea3(320, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(680);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "01101")
                {

                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "011012";
                    return;
                }
                if (code == "011012")
                {
                    //第三层
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "0110122";
                    return;
                }
                //此盘最后一个
                if (code == "0110122")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3+360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }
                
                if (code == "010")
                {
                    //短-长-短
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(1000-divpoint, 0, w, 1000-divpoint);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "0100";
                    }
                    else
                    {
                        WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "0101";
                    }
                    return;
                }
                if (code == "0100")
                {
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    //第二层不管长短顺序码垛三个
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 1000;
                    wdf.DrawOcupyArea3(680, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "01002";
                    return;
                }
                if (code == "0101")
                {
                    if (l <= upoint)
                    {
                        //右上回填，可能过限位
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "01010";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "01011";
                    }
                    return;
                }
                if (code == "01010")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 320;
                    wdf.DrawOcupyArea3(320, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(680);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "010102";
                    return;
                }
                if (code == "01011")
                {
                    if (l <= upoint)
                    {
                        //回填第一层右上角
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "010110";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 0, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(630);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "010111";
                    }
                    return;
                }
                //最后一个结束此盘
                if (code == "010111")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 320;
                    wdf.DrawOcupyArea3(320, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(680);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "010110")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 680;
                    wdf.DrawOcupyArea3(680, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(320);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "0101102";
                    return;
                }
                if (code == "0101102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 360, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "01011022";
                    return;
                }
                //此盘最后一个
                if (code == "01011022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "010102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 680;
                    wdf.DrawOcupyArea3(680, 0, w, l);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(320);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "0101022";
                    return;
                }
                if (code == "0101022")
                {
                    //第三层
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "01010222";
                    return;
                }
                //此盘最后一个
                if (code == "01010222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "01002")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 640;
                    wdf.DrawOcupyArea3(320, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(360);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "010022";
                    return;
                }
                if (code == "010022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "0100222";
                    return;
                }
                if (code == "0100222")
                {
                    //第三层
                    WorkingDetailForm.arrayList3.RemoveRange(0, WorkingDetailForm.arrayList3.Count);
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "01002222";
                    return;
                }
                //此盘最后一个
                if (code == "01002222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "10")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, 1000-divpoint, 280);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        //可能超限位
                        byte[] byteY = toBytes.intToBytes(l + divpoint);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "100";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, l, 280);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1000);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "101";
                    }
                    return;
                }
                if (code == "11")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, 1000 - divpoint, 280);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(l + divpoint);//可能超限位
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "110";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 720;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, l, 280);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1000);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                        code = "111";
                    }
                    return;
                }
                if (code == "000")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 320;
                        k73.y = 1000 - divpoint;
                        wdf.DrawOcupyArea3(0, 680, 1000-divpoint,w);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(divpoint);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90
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
                        code = "0000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 320;
                        k73.y = 1000;
                        wdf.DrawOcupyArea3(1000-l, 680, l, w);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(0);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90
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
                        code = "0001";
                    }
                    return;
                }
                if (code == "001")
                {
                    if (l <= upoint)
                    {
                        //回填，可能超限位
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "0010";
                    }
                    else
                    {
                        //码放第二层
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "0011";
                    }
                    return;
                }
                if (code == "0011")
                {
                    if (l <= upoint)
                    {
                        //回填第一层，可能超限位
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "00110";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(630);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "00111";
                    }
                    return;
                }
                /*mark*/
                if (code == "00111")
                {
                    if (l <= upoint)
                    {
                        //回填第一层，可能超限位
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint + l;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "001110";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 0, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(630);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "001111";
                    }
                    return;
                }
                //此盘最后一个
                if (code == "001111")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 320;
                    wdf.DrawOcupyArea3(320, 0, w, l);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(680);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    //满盘报警
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "001110")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 680;
                    wdf.DrawOcupyArea3(680, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(320);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "0011102";
                    return;
                }
                if (code == "0011102")
                {
                    //顺序码放两个结束
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "00111022";
                    return;
                }
                //此盘最后一个
                if (code == "00111022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }

                if (code == "00110")
                {
                    //顺序码放
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 320;
                    wdf.DrawOcupyArea3(320, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(680);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "001102";
                    return;
                }
                if (code == "001102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 680;
                    wdf.DrawOcupyArea3(680, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(320);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "0011022";
                    return;
                }
                if (code == "0011022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "00110222";
                    return;
                }
                //此盘最后一个
                if (code == "00110222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(0);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "0010")
                {
                    //第二层顺序码放三个
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 1000;
                    wdf.DrawOcupyArea3(680, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "00102";
                    return;
                }
                if (code == "00102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 640;
                    wdf.DrawOcupyArea3(320, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(360);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "001022";
                    return;
                }
                if (code == "001022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "0010222";
                    return;
                }
                if (code == "0010222")
                {
                    //第三层
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "00102222";
                    return;
                }
                //此盘最后一个
                if (code == "00102222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警...
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "0001")
                {
                    //第二层顺序码放三个
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 1000;
                    wdf.DrawOcupyArea3(680, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(0);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "00012";
                    return;
                }
                if (code == "00012")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 640;
                    wdf.DrawOcupyArea3(320, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(360);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "000122";
                    return;
                }
                if (code == "000122")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 1000-l, 280, l);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "0001222";
                    return;
                }
                if (code == "0001222")
                {
                    //第三层顺序码放两个结束
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "00012222";
                    return;
                }
                //最后一个结束此盘
                if (code == "00012222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "000")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 320;
                        k73.y = 1000 - divpoint;
                        wdf.DrawOcupyArea3(0, 680, 1000-divpoint, w);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(divpoint);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90
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
                        code = "0000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 320;
                        k73.y = 1000;
                        wdf.DrawOcupyArea3(1000-divpoint, 680, l, w);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(0);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "0", "1", "0", "0", "1" };//-90
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
                        code = "0001";
                    }
                    return;
                }
                //******************************4月18晚修改
                if (code == "0000")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + divpoint;
                        k73.y = 640;
                        wdf.DrawOcupyArea3(320, 0, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(360);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "00000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 0;
                        wdf.DrawOcupyArea3(0, 0, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1040);//给的坐标在界外
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "00001";
                    }
                    return;
                }
                if (code == "00000")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + l + divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000 - divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "000000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "000001";
                    }
                    return;
                }
                if (code == "00001")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 320;
                    wdf.DrawOcupyArea3(320, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(680);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "000012";
                    return;
                }
                if (code == "000012")
                {
                    //可能下盘不稳有空隙
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;
                    k73.y = 680;
                    wdf.DrawOcupyArea3(680, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(320);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "0000122";
                    return;
                }
                if (code == "0000122")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "00001222";
                    return;
                }
                //此盘最后一个
                if (code == "00001222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "000000")
                {
                    //可能第三层不稳
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 0, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "0000002";
                    return;
                }
                if (code == "0000002")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 360, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "00000022";
                    return;
                }
                //此盘最后一个
                if (code == "00000022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警处理、、、
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "100")
                {
                    //回填
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + l + divpoint;//可能有到达限位风险
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "1001";
                    }
                    return;
                }
                if (code == "1001")
                {
                    if (l <= upoint)
                    {
                        //invert and fill the blank回填第一层
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + l + divpoint;//可能有到达限位风险
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "10010";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "10011";
                    }
                    return;
                }
                if (code == "10010")
                {
                    if (l <= upoint)
                    {

                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, divpoint, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "100100";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "100101";
                    }
                    return;
                }
                if (code == "10011")
                {
                    if (l <= upoint)
                    {
                        //回填第一层，可能过限位
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + l + divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(0);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "100110";
                    }
                    else
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 640;
                        wdf.DrawOcupyArea3(320, 1000-l, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(360);
                        byte[] byteZ = toBytes.intToBytes(630);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "100112";
                    }
                    return;
                }
                //ending this pile结束此盘(可以考虑回填)
                if (code == "100112")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 0, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "200";
                    finish_3();
                    return;
                }
                //*****************
                if (code == "100110")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 1000;//可能有到达限位风险
                    k73.y = 680;
                    wdf.DrawOcupyArea3(680, 0, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(320);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "1001100";
                    return;
                }
                if (code == "1001100")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "10011002";
                    return;
                }
                //码完最后一个结束
                if (code == "10011002")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 720;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警处理
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "100100")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, divpoint, w, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1001000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1001001";
                    }
                    return;
                }
                if (code == "1001000")
                {
                    //invert and fill the blank
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "10010002";
                    return;
                }
                if (code == "1001001")
                {
                    //invert and fill the blank
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "10010012";
                    return;
                }
                //满盘结束
                if (code == "10010002" || code == "10010012")
                {
                    //invert and fill the blank
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警。。。
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "100101")
                {
                    if (l <= upoint)
                    {

                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, divpoint, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1001010";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1001011";
                    }
                    return;
                }
                if (code == "1001010" || code == "1001011")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "10010102";
                    return;
                }
                //结束此盘码垛
                if (code == "10010102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 0, l, 280);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //添加满盘报警..
                    code = "200";
                    finish_3();
                    return;
                }

                if (code == "101")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, divpoint, w, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1010";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 1000;
                        wdf.DrawOcupyArea3(680, 1000-l, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(1000);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "1011";
                    }
                    return;
                }
                if (code == "1010")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 0, 280, l);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "10102";
                    return;
                }
                if (code == "10102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 920;//加了80位移希望小箱子不落入空隙
                    k73.y = 320;
                    wdf.DrawOcupyArea3(0, 1080-l, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                    code = "101022";
                    return;
                }
                if (code == "101022")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1010222";
                    return;
                }
                //此盘最后一个结束
                if (code == "1010222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘报警。。。
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "1011")
                {
                    //不管长短,顺序码两个
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;//可能有到达限位风险
                    k73.y = 640;
                    wdf.DrawOcupyArea3(320, 1000-l, w, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(360);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "10112";
                    return;
                }
                if (code == "10112")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;//可能有到达限位风险
                    k73.y = 280;
                    wdf.DrawOcupyArea3(0, 1000-l, 280, l);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(720);
                    byte[] byteZ = toBytes.intToBytes(315);
                    string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                    code = "101122";
                    return;
                }
                if (code == "101122")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1011222";
                    return;
                }
                //最后一个结束此盘
                if (code == "1011222")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;//可能有到达限位风险
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "110")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 320;
                        wdf.DrawOcupyArea3(320, divpoint, w, l);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(680);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "1100";
                    }
                    else
                    {
                        //可能有危险
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 640;
                        wdf.DrawOcupyArea3(320, 1000-l, w, l);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(360);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "1101";
                    }
                    return;
                }
                if (code == "1100")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(320, divpoint, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "11000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "11001";
                    }
                    return;
                }
                if (code == "1101")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(320, divpoint, w, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "11010";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "11011";
                    }
                    return;
                }
                if (code == "11000")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, divpoint, 280, 1000-divpoint);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "110000";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "110001";
                    }
                    return;
                }
                if (code == "11001")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, divpoint, w, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "110010";
                    }
                    else
                    {
                        //可能有危险
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, l);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "110011";
                    }
                    return;
                }
                if (code == "11010")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, divpoint, 280, 1000-divpoint);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "110100";
                    }
                    else
                    {
                        //还可以细分长于1000和1000以内
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3;
                        k73.y = 280;
                        wdf.DrawOcupyArea3(0, 1000-l, 280, l);
                        
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(720);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "1", "1", "1", "0", "0", "1" };//180
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
                        code = "110101";
                    }
                    return;
                }
                if (code == "11011")
                {
                    if (l <= upoint)
                    {
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000 - divpoint;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, divpoint, w, 1000-divpoint);

                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "110110";
                    }
                    else
                    {
                        //可能有危险落入空隙
                        Coordinate k73 = new Coordinate();
                        k73.x = offset3 + 1000;
                        k73.y = 680;
                        wdf.DrawOcupyArea3(680, 0, w, l);
                        //第一个箱子直接放在原点(3200,0,0)
                        //发坐标（包含挡板状态）
                        byte[] byteX = toBytes.intToBytes(k73.x);
                        byte[] byteY = toBytes.intToBytes(320);
                        byte[] byteZ = toBytes.intToBytes(315);
                        string[] status = new string[] { "0", "1", "1", "0", "0", "1" };//0
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
                        code = "110111";
                    }
                    return;
                }
                if (code == "110110")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 640, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1101102";
                    return;
                }
                if (code == "110111")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1101112";
                    return;
                }
                if (code == "1101102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘
                    code = "200";
                    finish_3();
                    return;
                }
                //此盘最后一个
                if (code == "1101112")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);

                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "110100")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1101002";
                    return;
                }
                if (code == "1101002")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘结束
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "110101")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1101012";
                    return;
                }
                if (code == "1101012")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘结束
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "110010")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1100102";
                    return;
                }
                if (code == "110011")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "1100112";
                    return;
                }
                if (code == "1100102")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    //第一个箱子直接放在原点(3200,0,0)
                    //发坐标（包含挡板状态）
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "1100112")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    //满盘
                    code = "200";
                    finish_3();
                    return;
                }
                if (code == "110000")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 680, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "110002";
                    return;
                }
                //此盘最后一个
                if (code == "110002")
                {
                    Coordinate k73 = new Coordinate();
                    k73.x = offset3 + 360;
                    k73.y = 0;
                    wdf.DrawOcupyArea3(0, 320, l, w);
                    
                    byte[] byteX = toBytes.intToBytes(k73.x);
                    byte[] byteY = toBytes.intToBytes(1000);
                    byte[] byteZ = toBytes.intToBytes(630);
                    string[] status = new string[] { "1", "0", "1", "0", "0", "1" };//90
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
                    code = "200";
                    finish_3();
                    return;
                }
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
            lock (locker)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                Thread.Sleep(100);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                Thread.Sleep(100);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                MessageBox.Show("码盘1码垛完成,请更换码盘!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                count = count12 = count13 = count14 = 0;
                width1 = width12 = width13 = width14 = 1000;
                length1 = length12 = length13 = length14 = 1000;
                // pf.changeTextBoxText(printList1);
                // pf.print_list();

            }
        }

        public static void finish_2()
        {
            lock (locker)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                Thread.Sleep(100);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                Thread.Sleep(100);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                MessageBox.Show("码盘2码垛完成,请更换码盘!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                count2 = count22 = count23 = count24 = 0;
                width2 = width22 = width23 = width24 = 1000;
                length2 = length22 = length23 = length24 = 1000;
                // pf.changeTextBoxText(printList2);
                // pf.print_list();
            }

        }

        public static void finish_3()
        {
            lock (locker)
            {
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                Thread.Sleep(100);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                Thread.Sleep(100);
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0E;
                BF.sendbuf[3] = 0x06;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                MessageBox.Show("码盘3码垛完成,请更换码盘!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        /// <summary>
        /// 将条形码写入文本
        /// </summary>
        /// <param name="array"></param>
        public static void getBarCode(ArrayList array)
        {
            DateTime dt = DateTime.Now;
            string[] headline = new string[] { "时间", "条码" };
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
                for (int k = 0; k < array.Count / 2; k++)
                {
                    for (int j = 2 * k; j < 2 * (k + 1); j++)
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



        public static void WriteToUsbDisk(ArrayList array)
        {
            DateTime dt = DateTime.Now;
            string[] headline = new string[] { "长", "宽", "高", "数量" };
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
                for (int k = 0; k < usbList.Count / 4; k++)
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
        public static object locker2 = new object();
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
            xinlei = false;
            fight = false;
            completed = false;

            //if (xinlei == false && fight == false && completed == false)
            //{
            Thread.Sleep(300);
            SendMaduoInfo();
            //}
        }

        /// <summary>
        /// 扫码器数据接收与解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void comm_DataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            lock (locker2)
            {
                string str = null;
                System.Threading.Thread.Sleep(100);
                str = sp2.ReadExisting();
                //过滤乱码换行字符
                string result1 = str.Replace("\r", "");
                str = result1.Replace("?", "");
                DateTime dt = DateTime.Now;
                //跨越24：00，读取新日期的txt文本
                if (compareDt != dt.ToString("yyyy-MM-dd"))
                {
                    string pfname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\todayUSBdata\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
                    pf.ReadFromFile(pfname);
                }
                // string result2 = result1.Replace("\n", "");
                if (str == "")
                {
                    return;
                }
                //如果返回ERROR，则将其当作木箱
                if (str == "ERROR")
                {
                    saodao = false;
                    return;
                }
                if (str.Length != 10)
                {
                    return;
                }
                if (str != null)
                {
                    if (bufList.Count != 0)
                    {
                        if (str == (string)bufList[0])
                        {
                            return;
                        }
                        bufList.RemoveAt(0);
                        bufList.Add(str);
                        pf.InsertIntoTable(dt.ToString("HH:mm:ss"), str);
                    }
                    if (bufList.Count == 0)
                    {
                        bufList.Add(str);
                        pf.InsertIntoTable(dt.ToString("HH:mm:ss"), str);
                    }
                    //写入条形码
                    //getBarCode(usbList);
                    //sap通讯获取条码详细信息
                    PackageInfo packageInfo = new PackageInfo();
                    packageInfo = WebServiceRq.GetPackinfoFromWeb(str);
                    if (packageInfo == null)
                    {
                        return;
                    }
                    string str1 = packageInfo.Matnr;
                    string str2 = packageInfo.Maktx;
                    string str3 = packageInfo.Ppaufnr;
                    //插入数据详情
                    pf.InsertIntoTable2(str, str3, str1);
                    hf.InsertIntoTable(str1, str2, str3);


                    //由于sap取出的字符串格式为850.0，所以滤除小数点
                    string LENGTH = packageInfo.Laeng;
                    string L = LENGTH.Substring(0, LENGTH.Length - 2);
                    string WIDTH = packageInfo.Breit;
                    string W = WIDTH.Substring(0, WIDTH.Length - 2);
                    string HEIGHT = packageInfo.Hoehe;
                    string H = HEIGHT.Substring(0, HEIGHT.Length - 2);
                    l = int.Parse(L);
                    w = int.Parse(W);
                    h = int.Parse(H);
                    bufarrayList.Add(l);
                    bufarrayList.Add(w);
                    bufarrayList.Add(h);
                    str = null;
                    saodao = true;
                    sp2.DiscardInBuffer();
                    addPrintData(str1, str2, str3);
                }
            }

        }

        static void comm_DataReceived3(object sender, SerialDataReceivedEventArgs e)
        {
            lock (locker2)
            {
                string str = null;
                System.Threading.Thread.Sleep(100);
                str = sp3.ReadExisting();
                //过滤乱码换行字符
                string result1 = str.Replace("\r", "");
                str = result1.Replace("?", "");
                //str = result1.Replace("\n", "");
                DateTime dt = DateTime.Now;
                //跨越24：00，读取新日期的txt文本
                if (compareDt != dt.ToString("yyyy-MM-dd"))
                {
                    string pfname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\todayUSBdata\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
                    pf.ReadFromFile(pfname);
                }
                if (str == "")
                {
                    return;
                }
                //如果返回ERROR，则将其当作木箱,前提是之前的串口接收的也是ERROR
                if (str == "ERROR" && saodao == false)
                {
                    l = 0;
                    w = 0;
                    h = 0;
                    bufarrayList.Add(l);
                    bufarrayList.Add(w);
                    bufarrayList.Add(h);
                    return;
                }
                if (str.Length != 10)
                {
                    return;
                }
                if (str != null)
                {
                    if (bufList.Count != 0)
                    {
                        if (str == (string)bufList[0])
                        {
                            return;
                        }
                        bufList.RemoveAt(0);
                        bufList.Add(str);
                        pf.InsertIntoTable(dt.ToString("HH:mm:ss"), str);
                    }
                    if (bufList.Count == 0)
                    {
                        bufList.Add(str);
                        pf.InsertIntoTable(dt.ToString("HH:mm:ss"), str);
                    }

                    //写入条形码
                    //getBarCode(usbList);

                    //sap通讯获取条码详细信息
                    PackageInfo packageInfo = new PackageInfo();
                    packageInfo = WebServiceRq.GetPackinfoFromWeb(str);
                    if (packageInfo == null)
                    {
                        return;
                    }
                    string str1 = packageInfo.Matnr;
                    string str2 = packageInfo.Maktx;
                    string str3 = packageInfo.Ppaufnr;
                    //插入数据详情
                    pf.InsertIntoTable2(str, str3, str1);
                    hf.InsertIntoTable(str1, str2, str3);


                    //由于sap取出的字符串格式为850.0，所以滤除小数点
                    string LENGTH = packageInfo.Laeng;
                    string L = LENGTH.Substring(0, LENGTH.Length - 2);
                    string WIDTH = packageInfo.Breit;
                    string W = WIDTH.Substring(0, WIDTH.Length - 2);
                    string HEIGHT = packageInfo.Hoehe;
                    string H = HEIGHT.Substring(0, HEIGHT.Length - 2);
                    l = int.Parse(L);
                    w = int.Parse(W);
                    h = int.Parse(H);
                    bufarrayList.Add(l);
                    bufarrayList.Add(w);
                    bufarrayList.Add(h);
                    str = null;
                    addPrintData(str1, str2, str3);
                    sp3.DiscardInBuffer();
                }

            }
        }

        /// <summary>
        /// 添加打印数据
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="str3"></param>
        public static void addPrintData(string str1, string str2, string str3)
        {
            if (w == 0)
            {
                return;
            }
            if (w == 225)
            {
                printList1.Add(str1);
                printList1.Add(str2);
                printList1.Add(str3);
                return;
            }
            if (w == 240)
            {
                printList2.Add(str1);
                printList2.Add(str2);
                printList2.Add(str3);
                return;
            }
            if (w == 320)
            {
                printList3.Add(str1);
                printList3.Add(str2);
                printList3.Add(str3);
                return;
            }
        }

        /**************************************************************************************************/
        /// <summary>
        /// 接收各种数据并解析
        /// </summary>
        //private int SendStatus = 0;


        //回零失败标志位
        public static bool losehuiling = false;
        static void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //如果关闭串口，则不接收数据
            if (isTryCloseSp)
            {
                return;
            }
            byte[] binary_data_1 = null;
            List<byte> buffer = new List<byte>(4096);
            System.Threading.Thread.Sleep(100);
            int n = sp.BytesToRead;
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            sp.Read(buf, 0, n);//读取缓冲数据
            /****************************************协议解析**********************************************/
            bool data_1_cached = false;
            buffer.AddRange(buf);//缓存数据
            while (buffer.Count >= 3)
            {
                if (buffer[0] == 0xFA)
                {
                    int len = buffer[1];//数据长度
                    if (buffer.Count < len + 3) break;//长度不够直接退出
                    binary_data_1 = new byte[len + 3];
                    buffer.CopyTo(0, binary_data_1, 0, len + 3);//复制一条完整数据到具体的数据缓存
                    data_1_cached = true;
                    buffer.RemoveRange(0, len + 3);//正确分析一条数据，从缓存中移除数据
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
                    if (binary_data_1[3] == 0x01)
                    {
                        WorkingDetailForm.isReceived1 = true;
                    }
                    if (binary_data_1[3] == 0x02)
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
                    data2 = Convert.ToInt32(binary_data_1[3].ToString("X2"), 16);
                    if (data2 == 10)
                    {
                        desc = "电机故障（请尝试检查X、Y、Z电机）";
                    }
                    else if (data2 == 20)
                    {
                        desc = "硬限位故障（请检查硬限位）";
                    }
                    else if (data2 == 30)
                    {
                        desc = "软限位故障（请检查软限位）";
                    }
                    if (desc == "")
                    {
                        return;
                    }
                    if (!ahf.IsDisposed)
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
                if ((binary_data_1[1] == 0x11) && (binary_data_1[2] == 0x0E))
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
                    wdf.SetCoordinate(x_value, z_value, y_value, o_value);

                    hsf.getXYZOCoordinate(x_value, y_value, z_value, o_value);
                    return;
                }
                //解析IO状态数据
                if ((binary_data_1[1] == 0x04) && (binary_data_1[2] == 0x0C))
                {
                    int group1 = Convert.ToInt32(Convert.ToString(binary_data_1[5], 16), 16);
                    int group2 = Convert.ToInt32(Convert.ToString(binary_data_1[4], 16), 16);
                    int group3 = Convert.ToInt32(Convert.ToString(binary_data_1[3], 16), 16);
                    MainSettingForm.iof.getIOStatus(group1, group2, group3);
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
                        MessageBox.Show("已经回零", "提示");
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

                    hf.InsertIntoTable("89757", "CBA/NCAA", "0A002");

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

                    if (usbList.Count == 0)
                    {
                        usbList.Add(l);
                        usbList.Add(w);
                        usbList.Add(h);
                        usbList.Add(1);
                        // WriteToUsbDisk(usbList);
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
                                // WriteToUsbDisk(usbList);
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
                            //WriteToUsbDisk(usbList);
                            return;
                        }
                    }
                }




                if (binary_data_1[1] == 0x02 && binary_data_1[2] == 0x0E && binary_data_1[3] == 0x01)
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
                        //收到指令则从缓存中依此读数据下发
                        if (bufarrayList.Count != 0)
                        {
                            l = (int)bufarrayList[0];
                            w = (int)bufarrayList[1];
                            h = (int)bufarrayList[2];
                            ScannerGun();
                            bufarrayList.RemoveRange(0, 3);
                            wdf.GetTotalNum(++totalNum);
                        }

                        return;
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

        //释放串口资源
        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        //关闭窗体后将串口资源释放
        private void HomeForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            isTryCloseSp = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            return;
            //每半小时提交一次数据到数据库
            pf.DivideDBDataAndSendToRq(sernr, zaufnr, zmatnr);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pf.DivideDBDataAndSendToRq(sernr, zaufnr, zmatnr);
        }
    }
}