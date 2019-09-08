using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 码垛机.HomeForm;

namespace 码垛机
{
    public partial class WorkingDetailForm : Form
    {
        public WorkingDetailForm()
        {
            InitializeComponent();
        }

        private void WorkingDetailForm_Load(object sender, EventArgs e)
        {
        }

        public static bool isReceived1 = false;
        public static bool isReceived2 = false;
        public static bool isReceived3 = false;
        public static bool isReceived4 = false;

        public static ArrayList arrayList1 = new ArrayList();
        public static ArrayList arrayList2 = new ArrayList();
        public static ArrayList arrayList3 = new ArrayList();
        /// <summary>
        /// 绘制码盘已经占用的区域
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawOcupyArea(int x, int y,int length,int width)
        {
            int a =(int) (0.15 * x);
            int b =(int) (0.15 * y);
            int c = (int)(0.15 * length);
            int d =(int) (0.15 * width);
            arrayList1.Add(a);
            arrayList1.Add(b);
            arrayList1.Add(c);
            arrayList1.Add(d);
            Graphics gp = panel6.CreateGraphics();
            if (arrayList1.Count == 4)
            {
                gp.Clear(panel6.BackColor);
            }
            
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new Point(a, b), new Size(c, d));
            //gp.DrawRectangle(new Pen(Brushes.Red, 5f), rect); //线
            gp.FillRectangle(Brushes.Blue, rect); //填充
            gp.Flush();

        }
        public void DrawOcupyArea2(int x, int y, int length, int width)
        {
            int a = (int)(0.15 * x);
            int b = (int)(0.15 * y);
            int c = (int)(0.15 * length);
            int d = (int)(0.15 * width);
            arrayList2.Add(a);
            arrayList2.Add(b);
            arrayList2.Add(c);
            arrayList2.Add(d);
            Graphics gp = panel7.CreateGraphics();
            if (arrayList2.Count == 4)
            {
                gp.Clear(panel7.BackColor);
            }
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new Point(a, b), new Size(c, d));
            //gp.DrawRectangle(new Pen(Brushes.Red, 5f), rect); //线
            gp.FillRectangle(Brushes.Blue, rect); //填充
            gp.Flush();

        }
        public void DrawOcupyArea3(int x, int y, int length, int width)
        {
            int a = (int)(0.15 * x);
            int b = (int)(0.15 * y);
            int c = (int)(0.15 * length);
            int d = (int)(0.15 * width);
            arrayList3.Add(a);
            arrayList3.Add(b);
            arrayList3.Add(c);
            arrayList3.Add(d);
            Graphics gp = panel8.CreateGraphics();
            if (arrayList3.Count == 4)
            {
                gp.Clear(panel8.BackColor);
            }
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new Point(a, b), new Size(c, d));
            //gp.DrawRectangle(new Pen(Brushes.Red, 5f), rect); //线
            gp.FillRectangle(Brushes.Blue, rect); //填充
            gp.Flush();

        }
        /// <summary>
        /// 改变码盘旁边的标签提示当前码垛到哪一层
        /// </summary>
        public void changeText1(string str)
        {
            CheckForIllegalCrossThreadCalls = false;
            label1.Text = str;
        }

        public void changeText2(string str)
        {
            CheckForIllegalCrossThreadCalls = false;
            label2.Text = str;
        }
        public void changeText3(string str)
        {
            CheckForIllegalCrossThreadCalls = false;
            label3.Text = str;
        }
        public void GetTotalNum(int a)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.label22.Text = a.ToString();
        }

        public void SetCoordinate(int x,int z,int y,int o)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.label14.Text = x.ToString();
            this.label18.Text = z.ToString();
            this.label19.Text = y.ToString();
            this.label20.Text = o.ToString();
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clr_btn4_Click(object sender, EventArgs e)
        {
            while (!isReceived4)
            {
                Thread.Sleep(500);
                HomeForm.xinlei = false;
                HomeForm.fight = false;
                HomeForm.completed = false;
                label22.Text = 0.ToString();
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0D;
                BF.sendbuf[3] = 0x04;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                HomeForm.xinlei = true;
                HomeForm.fight = true;
                HomeForm.completed = true;
            }
            isReceived4 = false;          
        }


        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void str_btn_Click(object sender, EventArgs e)
        {
            while (!isReceived1)
            {
                Thread.Sleep(500);
                HomeForm.xinlei = false;
                HomeForm.fight = false;
                HomeForm.completed = false;
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0D;
                BF.sendbuf[3] = 0x01;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                HomeForm.xinlei = true;
                HomeForm.fight = true;
                HomeForm.completed = true;
            }
            isReceived1 = false;
        }
            
        private void pause_btn_Click(object sender, EventArgs e)
        {
            while (!isReceived2)
            {
                Thread.Sleep(500);
                HomeForm.xinlei = false;
                HomeForm.fight = false;
                HomeForm.completed = false;
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0D;
                BF.sendbuf[3] = 0x02;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                HomeForm.xinlei = true;
                HomeForm.fight = true;
                HomeForm.completed = true;
            }
            isReceived2 = false;
        }
        /// <summary>
        /// 回零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            while (!isReceived3)
            {
                Thread.Sleep(500);
                HomeForm.xinlei = false;
                HomeForm.fight = false;
                HomeForm.completed = false;
                BF.sendbuf[0] = 0xFA;
                BF.sendbuf[1] = 0x02;
                BF.sendbuf[2] = 0x0D;
                BF.sendbuf[3] = 0x03;
                BF.sendbuf[4] = 0xF5;
                SendMenuCommand(BF.sendbuf, 5);
                HomeForm.xinlei = true;
                HomeForm.fight = true;
                HomeForm.completed = true;
            }
            isReceived3 = false;            
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0;i < (arrayList1.Count/4);i++)
            {
                Graphics gp = panel6.CreateGraphics();
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new Point((int)arrayList1[4*i], (int)arrayList1[4*i+1]), new Size((int)arrayList1[4*i+2], (int)arrayList1[4*i+3]));
                //gp.DrawRectangle(new Pen(Brushes.Red, 5f), rect); //线
                gp.FillRectangle(Brushes.Blue, rect); //填充
                gp.Flush();
            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < (arrayList2.Count / 4); i++)
            {
                Graphics gp = panel7.CreateGraphics();
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new Point((int)arrayList2[4 * i], (int)arrayList2[4 * i + 1]), new Size((int)arrayList2[4 * i + 2], (int)arrayList2[4 * i + 3]));
                //gp.DrawRectangle(new Pen(Brushes.Red, 5f), rect); //线
                gp.FillRectangle(Brushes.Blue, rect); //填充
                gp.Flush();
            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < (arrayList3.Count / 4); i++)
            {
                Graphics gp = panel8.CreateGraphics();
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new Point((int)arrayList3[4 * i], (int)arrayList3[4 * i + 1]), new Size((int)arrayList3[4 * i + 2], (int)arrayList3[4 * i + 3]));
                //gp.DrawRectangle(new Pen(Brushes.Red, 5f), rect); //线
                gp.FillRectangle(Brushes.Blue, rect); //填充
                gp.Flush();
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
