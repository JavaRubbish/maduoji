using System;
using System.Collections;
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
    public partial class AlarmHistoryForm : Form
    {
        public AlarmHistoryForm()
        {
            SetStyle(
            ControlStyles.AllPaintingInWmPaint |    //不闪烁
            ControlStyles.OptimizedDoubleBuffer    //支持双缓存
            , true);
            InitializeComponent();
            ReadFromTxt();
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
        /// <summary>
        /// 添加报警数据行
        /// </summary>
        public void AddAlarmDataListViewItem(int value, string desc)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToShortTimeString().ToString();

            CheckForIllegalCrossThreadCalls = false;

            ListViewItem lvi = new ListViewItem();
            lvi.Text = date;
            lvi.SubItems.Add(time);
            lvi.SubItems.Add(value.ToString());
            lvi.SubItems.Add(desc);
            listView1.Items.Add(lvi);

            MessageBox.Show(desc,"警告");

            WriteToTxt();

        }

        /// <summary>
        /// 报警信息窗口关闭时，直接将报警数据追加到txt文本中，而不是写在listview中
        /// 因为listview随报警信息窗体的关闭而不可用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="desc"></param>
        public void AddAlarmDataListViewItem2(int value, string desc)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToShortTimeString().ToString();
            List<string> datalist = new List<string>();
            datalist.Add(date);
            datalist.Add(time);
            datalist.Add(value.ToString());
            datalist.Add(desc);
            
            CheckForIllegalCrossThreadCalls = false;            

            MessageBox.Show(desc,"警告");

            WriteToTxt2(datalist);

        }

        //按键清除报警信息
        private void clr_btn_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        /// <summary>
        /// 报警信息写入磁盘
        /// </summary>
        private void WriteToTxt()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("alarmhis.txt", false, System.Text.Encoding.GetEncoding("gb2312"));
            try
            {
                int len = 0;
                string line = "";
                string temp = "";
                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    temp = listView1.Columns[i].Text;
                    len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length; //考虑中英文的情况
                    temp = temp.PadRight(len, ' ');
                    line += temp;
                }
                sw.WriteLine(line);
                line = "";
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    for (int j = 0; j < listView1.Items[i].SubItems.Count; j++)
                    {
                        temp = listView1.Items[i].SubItems[j].Text;
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
        /// 当打开非报警界面，会将原先的listview变为空，导致写入出错
        /// 这里做了特殊处理，将报警信息追加到之前的txt文本
        /// </summary>
        /// <param name="list"></param>
        private void WriteToTxt2(List<string> list)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("alarmhis.txt", true, System.Text.Encoding.GetEncoding("gb2312"));
            try
            {
                int len = 0;
                string line = "";
                string temp = "";                
                    for (int j = 0; j < 4; j++)
                    {
                        temp = list[j];
                        len = 30 - Encoding.Default.GetByteCount(temp) + temp.Length;
                        temp = temp.PadRight(len, ' ');
                        line += temp;
                    }
                    sw.WriteLine(line);
                    line = "";                
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        /// <summary>
        /// 从txt文件读历史报警信息
        /// </summary>
        private void ReadFromTxt()
        {
            //string fname = "C:\\Users\\John\\source\\repos\\码垛机\\码垛机\\bin\\Debug\\alarmhis.txt";
            string fname = "C:\\码垛机\\码垛机\\bin\\Debug\\alarmhis.txt";
            if (!System.IO.File.Exists(fname))
            {
                return;
            }
            StreamReader sr = new StreamReader(fname, Encoding.Default);
            string vline;
            while ((vline = sr.ReadLine()) != null)
            {
                string[] vitems = vline.Split(new string[] { " ","Date","Time","CurrentValue","DetailDescription" }, StringSplitOptions.RemoveEmptyEntries);

                if (vitems.Length <= 0)
                {
                    continue;
                }
                ListViewItem lvi = new ListViewItem();
                lvi.Text = vitems[0];
                for (int i = 1; i < vitems.Length; i++)
                {
                    lvi.SubItems.Add(vitems[i]);
                }
                listView1.Items.Add(lvi);

            }
            sr.Close();//读完一定要关闭流，否则会和上面的写进程冲突

        }

        private void AlarmHistoryForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
