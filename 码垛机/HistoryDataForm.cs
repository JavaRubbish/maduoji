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

            // this.label1.Font = new System.Drawing.Font("黑体", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
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
                    WriteToDisk();
                    return;
                }
            }
            int index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value = str1;
            this.dataGridView1.Rows[index].Cells[1].Value = str2;
            this.dataGridView1.Rows[index].Cells[2].Value = str3;
            this.dataGridView1.Rows[index].Cells[3].Value = "1";
            WriteToDisk();
        }

        //把数据写进项目文件夹
        public void WriteToDisk()
        {
            DateTime dt = DateTime.Now;
            System.IO.StreamWriter sw = new System.IO.StreamWriter("historydata\\day\\" + dt.ToString("yyyy-MM-dd") +"his.txt", false, System.Text.Encoding.GetEncoding("gb2312"));
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
            //string fname = "C:\\Users\\John\\source\\repos\\码垛机\\码垛机\\bin\\Debug\\alarmhis.txt";
            if (!System.IO.File.Exists(filepath))
            {
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
    }
}
