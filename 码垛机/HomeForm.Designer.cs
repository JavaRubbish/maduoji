namespace 码垛机
{
    partial class HomeForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.work_btn = new System.Windows.Forms.Button();
            this.historydata_btn = new System.Windows.Forms.Button();
            this.setting_btn = new System.Windows.Forms.Button();
            this.alarmhistory_btn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SysTime = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(12, 78);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1219, 499);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // work_btn
            // 
            this.work_btn.BackColor = System.Drawing.Color.RoyalBlue;
            this.work_btn.FlatAppearance.BorderColor = System.Drawing.Color.RoyalBlue;
            this.work_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.work_btn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.work_btn.Image = ((System.Drawing.Image)(resources.GetObject("work_btn.Image")));
            this.work_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.work_btn.Location = new System.Drawing.Point(149, 594);
            this.work_btn.Margin = new System.Windows.Forms.Padding(0);
            this.work_btn.Name = "work_btn";
            this.work_btn.Size = new System.Drawing.Size(144, 55);
            this.work_btn.TabIndex = 1;
            this.work_btn.Text = "工作界面";
            this.work_btn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.work_btn.UseVisualStyleBackColor = false;
            this.work_btn.Click += new System.EventHandler(this.work_btn_Click);
            // 
            // historydata_btn
            // 
            this.historydata_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.historydata_btn.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.historydata_btn.FlatAppearance.BorderSize = 0;
            this.historydata_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.historydata_btn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.historydata_btn.Image = ((System.Drawing.Image)(resources.GetObject("historydata_btn.Image")));
            this.historydata_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.historydata_btn.Location = new System.Drawing.Point(397, 594);
            this.historydata_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.historydata_btn.Name = "historydata_btn";
            this.historydata_btn.Size = new System.Drawing.Size(144, 55);
            this.historydata_btn.TabIndex = 2;
            this.historydata_btn.Text = "历史数据";
            this.historydata_btn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.historydata_btn.UseVisualStyleBackColor = false;
            this.historydata_btn.Click += new System.EventHandler(this.historydata_btn_Click);
            // 
            // setting_btn
            // 
            this.setting_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.setting_btn.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.setting_btn.FlatAppearance.BorderSize = 0;
            this.setting_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setting_btn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.setting_btn.Image = ((System.Drawing.Image)(resources.GetObject("setting_btn.Image")));
            this.setting_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.setting_btn.Location = new System.Drawing.Point(691, 594);
            this.setting_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.setting_btn.Name = "setting_btn";
            this.setting_btn.Size = new System.Drawing.Size(144, 55);
            this.setting_btn.TabIndex = 3;
            this.setting_btn.Text = "设置调试";
            this.setting_btn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.setting_btn.UseVisualStyleBackColor = false;
            this.setting_btn.Click += new System.EventHandler(this.setting_btn_Click);
            // 
            // alarmhistory_btn
            // 
            this.alarmhistory_btn.BackColor = System.Drawing.Color.Gainsboro;
            this.alarmhistory_btn.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.alarmhistory_btn.FlatAppearance.BorderSize = 0;
            this.alarmhistory_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.alarmhistory_btn.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.alarmhistory_btn.Image = ((System.Drawing.Image)(resources.GetObject("alarmhistory_btn.Image")));
            this.alarmhistory_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.alarmhistory_btn.Location = new System.Drawing.Point(960, 594);
            this.alarmhistory_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.alarmhistory_btn.Name = "alarmhistory_btn";
            this.alarmhistory_btn.Size = new System.Drawing.Size(144, 55);
            this.alarmhistory_btn.TabIndex = 4;
            this.alarmhistory_btn.Text = "报警历史";
            this.alarmhistory_btn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.alarmhistory_btn.UseVisualStyleBackColor = false;
            this.alarmhistory_btn.Click += new System.EventHandler(this.alarmhistory_btn_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel2.Controls.Add(this.SysTime);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Location = new System.Drawing.Point(12, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1219, 70);
            this.panel2.TabIndex = 5;
            // 
            // SysTime
            // 
            this.SysTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SysTime.AutoSize = true;
            this.SysTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SysTime.ForeColor = System.Drawing.Color.Snow;
            this.SysTime.Location = new System.Drawing.Point(788, 31);
            this.SysTime.Name = "SysTime";
            this.SysTime.Size = new System.Drawing.Size(102, 25);
            this.SysTime.TabIndex = 1;
            this.SysTime.Text = "欢迎登陆！";
            this.SysTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3.BackgroundImage")));
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(171, 70);
            this.panel3.TabIndex = 0;
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1244, 679);
            this.Controls.Add(this.alarmhistory_btn);
            this.Controls.Add(this.setting_btn);
            this.Controls.Add(this.historydata_btn);
            this.Controls.Add(this.work_btn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1259, 716);
            this.Name = "HomeForm";
            this.Text = "工作界面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button work_btn;
        private System.Windows.Forms.Button historydata_btn;
        private System.Windows.Forms.Button setting_btn;
        private System.Windows.Forms.Button alarmhistory_btn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label SysTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}

