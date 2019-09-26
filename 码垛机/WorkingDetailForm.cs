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
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);

            InitializeComponent();
            AutoScale(this);
        }

        public void AutoScale(Form frm)
        {
            frm.Tag = frm.Width.ToString() + "," + frm.Height.ToString();
            frm.SizeChanged += new EventHandler(frm_SizeChanged);
        }

        void frm_SizeChanged(object sender, EventArgs e)
        {
            string[] tmp = ((Form)sender).Tag.ToString().Split(',');
            float width = (float)((Form)sender).Width / (float)Convert.ToInt16(tmp[0]);
            float heigth = (float)((Form)sender).Height / (float)Convert.ToInt16(tmp[1]);

            ((Form)sender).Tag = ((Form)sender).Width.ToString() + "," + ((Form)sender).Height;

            this.str_btn.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.pause_btn.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button6.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.clr_btn4.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label1.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label2.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label3.Font = new System.Drawing.Font("宋体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label12.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label21.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label22.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label13.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label14.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label15.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label16.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label17.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label18.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label19.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label20.Font = new System.Drawing.Font("微软雅黑", 10.8F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));



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

        private void WorkingDetailForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
        //工作界面按钮指令下发接收确认标志位
        public static bool isReceived1 = false;
        public static bool isReceived2 = false;
        public static bool isReceived3 = false;

        //工作界面回零确认标志位
        public static bool reset = false;

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
            int a =(int) (0.15 * x * HomeForm.change_w);
            int b =(int) (0.15 * y * HomeForm.change_l);
            int c =(int) (0.15 * length * HomeForm.change_w);
            int d =(int) (0.15 * width * HomeForm.change_l);
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
            gp.FillRectangle(Brushes.Khaki, rect); //填充
            gp.Flush();

        }
        public void DrawOcupyArea2(int x, int y, int length, int width)
        {
            int a = (int)(0.15 * x * HomeForm.change_w);
            int b = (int)(0.15 * y * HomeForm.change_l);
            int c = (int)(0.15 * length * HomeForm.change_w);
            int d = (int)(0.15 * width * HomeForm.change_l);
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
            gp.FillRectangle(Brushes.Khaki, rect); //填充
            gp.Flush();

        }
        public void DrawOcupyArea3(int x, int y, int length, int width)
        {
            int a = (int)(0.15 * x * HomeForm.change_w);
            int b = (int)(0.15 * y * HomeForm.change_l);
            int c = (int)(0.15 * length * HomeForm.change_w);
            int d = (int)(0.15 * width * HomeForm.change_l);
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
            gp.FillRectangle(Brushes.Khaki, rect); //填充
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
            if (MainSettingForm.flag)
            {
                MessageBox.Show("没有权限执行此操作，请联系管理员！","警告",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            else
            {
                label22.Text = "0";
                MessageBox.Show("数据成功清零！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }


        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void str_btn_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!isReceived1)
                {
                    Thread.Sleep(500);
                    //HomeForm.xinlei = false;
                    //HomeForm.fight = false;
                    //HomeForm.completed = false;
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0D;
                    BF.sendbuf[3] = 0x01;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(500);
                    //HomeForm.xinlei = true;
                    //HomeForm.fight = true;
                    //HomeForm.completed = true;
                }
                isReceived1 = false;
            }
        }
            
        private void pause_btn_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!isReceived2)
                {                                       
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0D;
                    BF.sendbuf[3] = 0x02;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(500);               
                }
                isReceived2 = false;
            }
        }
        /// <summary>
        /// 回零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            lock (locker)
            {
                while (!isReceived3)
                {                   
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0D;
                    BF.sendbuf[3] = 0x03;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(500);                 
                }
                isReceived3 = false;
                while (!reset)
                {
                    BF.sendbuf[0] = 0xFA;
                    BF.sendbuf[1] = 0x02;
                    BF.sendbuf[2] = 0x0E;
                    BF.sendbuf[3] = 0x04;
                    BF.sendbuf[4] = 0xF5;
                    SendMenuCommand(BF.sendbuf, 5);
                    Thread.Sleep(500);
                }
                reset = false;
            }                   
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

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }
    }
}
