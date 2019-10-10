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
    public partial class USBForm : Form
    {
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;

        public static DateTime dt = DateTime.Now;
        string destPath = null;
        string sourcePath = "D:\\C#Project\\码垛机2.0\\码垛机\\bin\\Debug\\todayUSBdata\\";
        string fileName = dt.ToString("yyyy-MM-dd") + "info.txt";
        public USBForm()
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

            this.usblabel.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.usbbutton.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));
            this.button1.Font = new System.Drawing.Font("微软雅黑", 12F * width, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)134));

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
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入   
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo drive in s)
                            {
                                if (drive.DriveType == DriveType.Removable)
                                {
                                    destPath = drive.Name.ToString();
                                    usblabel.Text = "U盘已插入，盘符为:" + destPath;
                                    break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:   //U盘卸载 
                            usblabel.Text = "";
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }

                }

            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }

            base.WndProc(ref m);

        }
        private void usbbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (destPath == null)
                {
                    MessageBox.Show("未识别到U盘，请重新插入!","错误");
                    return;
                }
                File.Copy(System.IO.Path.Combine(sourcePath, fileName), System.IO.Path.Combine(destPath, fileName));
                MessageBox.Show("文件写入成功！");
            }
            catch (IOException copyError)
            {
                MessageBox.Show(copyError.Message);
                return;
            }
        }

        private void usblabel_Click(object sender, EventArgs e)
        {

        }

        private void USBForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void USBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
