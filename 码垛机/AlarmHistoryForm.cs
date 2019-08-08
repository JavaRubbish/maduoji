using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            InitializeComponent();
            
        }
        /// <summary>
        /// 添加报警数据行
        /// </summary>
        public void AddAlarmDataListViewItem(int value,string desc)
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

            WriteToTxt();

        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AlarmHistoryForm_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
    }
}
