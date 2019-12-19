using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 码垛机
{
    public partial class HistoryDataForm : Form
    {
        public static int dec = -1;
        public static int inc = 1;
        public HistoryDataForm()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);
            InitializeComponent();
            AutoScale(this);
            HomeForm.xinlei = false;
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

            this.day_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.mon_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.year_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.total_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.dateTimePicker1.Font = new System.Drawing.Font("微软雅黑", 9F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.lastpg_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.nextpg_btn.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
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
        private void HistoryDataForm_Load(object sender, EventArgs e)
        {
            label4.Text = totalNum().ToString();
            this.WindowState = FormWindowState.Maximized;
        }

        //插入数据到表格
        public void InsertIntoTable(string str1,string str2,string str3)
        {
            CheckForIllegalCrossThreadCalls = false;
            DateTime dt = DateTime.Now;
            //写进日文件夹
            string fname1 = "historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            //写进月文件夹
            string fname2 = "historydata\\month\\" + dt.ToString("yyyy-MM") + "his.txt";
            //写进年文件夹
            string fname3 = "historydata\\year\\" + dt.ToString("yyyy") + "his.txt";
            int temp = int.Parse(label4.Text);
            temp++;
            label4.Text = temp.ToString();
            int row = this.dataGridView1.Rows.Count;
            for (int i = 0; i < row; i++)
            {
                
                if ((string)this.dataGridView1.Rows[i].Cells[0].Value == str1 &&
                   (string)this.dataGridView1.Rows[i].Cells[2].Value == str3)
                {
                    string num = (string)this.dataGridView1.Rows[i].Cells[3].Value;
                    int a = int.Parse(num);
                    a++;
                    this.dataGridView1.Rows[i].Cells[3].Value = a.ToString();
                    
                    
                    WriteToDisk(fname1);
                    WriteToDisk(fname2);
                    WriteToDisk(fname3);
                    return;
                }
            }
            int index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value = str1;
            this.dataGridView1.Rows[index].Cells[1].Value = str2;
            this.dataGridView1.Rows[index].Cells[2].Value = str3;
            this.dataGridView1.Rows[index].Cells[3].Value = "1";
            WriteToDisk(fname1);
            WriteToDisk(fname2);
            WriteToDisk(fname3);

        }

        //把数据写进项目文件夹
        public void WriteToDisk(string fname)
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
                for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
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

        /// <summary>
        /// 每次重启时，将历史数据文本填充到表格
        /// </summary>
        public void ReadFromFile(string filepath)
        {
            //先清空每一行数据，否则多个日期的数据都会写到一个表里
            dataGridView1.Rows.Clear();
            //string fname = "C:\\Users\\John\\source\\repos\\码垛机\\码垛机\\bin\\Debug\\alarmhis.txt";
            if (!System.IO.File.Exists(filepath))
            {
                label4.Text = totalNum().ToString();
                return;
            }
            StreamReader sr = new StreamReader(filepath, Encoding.Default);
            string vline;
            while ((vline = sr.ReadLine()) != null)
            {
                string[] vitems = vline.Split(new string[] { " ", "物料编号", "物料描述", "订单号", "数量" }, StringSplitOptions.RemoveEmptyEntries);

                if (vitems.Length <= 0)
                {
                    continue;
                }
                for (int i = 0; i < vitems.Length/4; i++)
                {
                    int index = this.dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = vitems[4 * i];
                    dataGridView1.Rows[index].Cells[1].Value = vitems[4 * i + 1];
                    dataGridView1.Rows[index].Cells[2].Value = vitems[4 * i + 2];
                    dataGridView1.Rows[index].Cells[3].Value = vitems[4 * i + 3];
                }
                label4.Text = totalNum().ToString();
            }
            sr.Close();//读完一定要关闭流，否则会和上面的写进程冲突

        }

        /// <summary>
        /// 计算总数
        /// </summary>
        /// <returns></returns>
        private int totalNum()
        {
            int sum = 0;
            for (int i = 0;i < dataGridView1.Rows.Count - 1;i++)
            {
                sum += int.Parse((string)dataGridView1.Rows[i].Cells[3].Value);
            }
            return sum;
        }

        private void day_btn_Click(object sender, EventArgs e)
        {
            //上下页按钮计数复位
            dec = -1;
            inc = 1;
            DateTime dt = DateTime.Now;
            string fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            ReadFromFile(fname);
            this.day_btn.BackColor = Color.Yellow;
            this.mon_btn.BackColor = Color.Cornsilk;
            this.year_btn.BackColor = Color.Cornsilk;
        }

        private void mon_btn_Click(object sender, EventArgs e)
        {
            dec = -1;
            inc = 1;
            DateTime dt = DateTime.Now;
            string fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\month\\" + dt.ToString("yyyy-MM") + "his.txt";
            ReadFromFile(fname);
            this.day_btn.BackColor = Color.Cornsilk;
            this.mon_btn.BackColor = Color.Yellow;
            this.year_btn.BackColor = Color.Cornsilk;
        }

        private void year_btn_Click(object sender, EventArgs e)
        {
            dec = -1;
            inc = 1;
            DateTime dt = DateTime.Now;
            string fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\year\\" + dt.ToString("yyyy") + "his.txt";
            ReadFromFile(fname);
            this.day_btn.BackColor = Color.Cornsilk;
            this.mon_btn.BackColor = Color.Cornsilk;
            this.year_btn.BackColor = Color.Yellow;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lastpg_btn_Click(object sender, EventArgs e)
        {
            string fname = null;
            if (this.day_btn.BackColor == Color.Yellow)
            {
                DateTime dt = DateTime.Now.AddDays(dec).Date;
                if (dec == 0)
                {
                    dt = DateTime.Now;
                }
                fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            }
            if (this.mon_btn.BackColor == Color.Yellow)
            {
                DateTime dt = DateTime.Now.AddMonths(dec).Date;
                if (dec == 0)
                {
                    dt = DateTime.Now;
                }
                fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\month\\" + dt.ToString("yyyy-MM") + "his.txt";
            }
            if (this.year_btn.BackColor == Color.Yellow)
            {
                DateTime dt = DateTime.Now.AddYears(dec).Date;
                if (dec == 0)
                {
                    dt = DateTime.Now;
                }
                fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\year\\" + dt.ToString("yyyy") + "his.txt";
            }
            ReadFromFile(fname);
            dec--;
            inc--;
        }

        private void nextpg_btn_Click(object sender, EventArgs e)
        {
            string fname = null;
            if (this.day_btn.BackColor == Color.Yellow)
            {
                DateTime dt = DateTime.Now.AddDays(inc).Date;
                if (inc == 0)
                {
                    dt = DateTime.Now;
                }
                fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            }
            if (this.mon_btn.BackColor == Color.Yellow)
            {
                DateTime dt = DateTime.Now.AddMonths(inc).Date;
                if (inc == 0)
                {
                    dt = DateTime.Now;
                }
                fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\month\\" + dt.ToString("yyyy-MM") + "his.txt";
            }
            if (this.year_btn.BackColor == Color.Yellow)
            {
                DateTime dt = DateTime.Now.AddYears(inc).Date;
                if (inc == 0)
                {
                    dt = DateTime.Now;
                }
                fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\year\\" + dt.ToString("yyyy") + "his.txt";
            }
            ReadFromFile(fname);
            inc++;
            dec++;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker1.Value;
            string fname = "C:\\码垛机2.1.1\\码垛机\\bin\\Debug\\historydata\\day\\" + dt.ToString("yyyy-MM-dd") + "his.txt";
            ReadFromFile(fname);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
