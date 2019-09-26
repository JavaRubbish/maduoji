namespace 码垛机
{
    partial class MainSettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainSettingForm));
            this.aboutus_btn = new System.Windows.Forms.Button();
            this.ioset_btn = new System.Windows.Forms.Button();
            this.handset_btn = new System.Windows.Forms.Button();
            this.Upan_btn = new System.Windows.Forms.Button();
            this.ruanxianweiset_btn = new System.Windows.Forms.Button();
            this.userset_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // aboutus_btn
            // 
            this.aboutus_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("aboutus_btn.BackgroundImage")));
            this.aboutus_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.aboutus_btn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.aboutus_btn.Location = new System.Drawing.Point(861, 295);
            this.aboutus_btn.Name = "aboutus_btn";
            this.aboutus_btn.Size = new System.Drawing.Size(181, 69);
            this.aboutus_btn.TabIndex = 11;
            this.aboutus_btn.Text = "关于我们";
            this.aboutus_btn.UseVisualStyleBackColor = true;
            this.aboutus_btn.Click += new System.EventHandler(this.aboutus_btn_Click);
            // 
            // ioset_btn
            // 
            this.ioset_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ioset_btn.BackgroundImage")));
            this.ioset_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ioset_btn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ioset_btn.Location = new System.Drawing.Point(511, 295);
            this.ioset_btn.Name = "ioset_btn";
            this.ioset_btn.Size = new System.Drawing.Size(181, 69);
            this.ioset_btn.TabIndex = 10;
            this.ioset_btn.Text = "IO调试";
            this.ioset_btn.UseVisualStyleBackColor = true;
            this.ioset_btn.Click += new System.EventHandler(this.ioset_btn_Click);
            // 
            // handset_btn
            // 
            this.handset_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("handset_btn.BackgroundImage")));
            this.handset_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.handset_btn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.handset_btn.Location = new System.Drawing.Point(159, 295);
            this.handset_btn.Name = "handset_btn";
            this.handset_btn.Size = new System.Drawing.Size(181, 69);
            this.handset_btn.TabIndex = 9;
            this.handset_btn.Text = "手动调试";
            this.handset_btn.UseVisualStyleBackColor = true;
            this.handset_btn.Click += new System.EventHandler(this.handset_btn_Click);
            // 
            // Upan_btn
            // 
            this.Upan_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Upan_btn.BackgroundImage")));
            this.Upan_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Upan_btn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Upan_btn.Location = new System.Drawing.Point(861, 88);
            this.Upan_btn.Name = "Upan_btn";
            this.Upan_btn.Size = new System.Drawing.Size(181, 69);
            this.Upan_btn.TabIndex = 8;
            this.Upan_btn.Text = "U盘助手";
            this.Upan_btn.UseVisualStyleBackColor = true;
            this.Upan_btn.Click += new System.EventHandler(this.Upan_btn_Click);
            // 
            // ruanxianweiset_btn
            // 
            this.ruanxianweiset_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ruanxianweiset_btn.BackgroundImage")));
            this.ruanxianweiset_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ruanxianweiset_btn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ruanxianweiset_btn.Location = new System.Drawing.Point(511, 88);
            this.ruanxianweiset_btn.Name = "ruanxianweiset_btn";
            this.ruanxianweiset_btn.Size = new System.Drawing.Size(181, 69);
            this.ruanxianweiset_btn.TabIndex = 7;
            this.ruanxianweiset_btn.Text = "软限位设定";
            this.ruanxianweiset_btn.UseVisualStyleBackColor = true;
            this.ruanxianweiset_btn.Click += new System.EventHandler(this.ruanxianweiset_btn_Click);
            // 
            // userset_btn
            // 
            this.userset_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("userset_btn.BackgroundImage")));
            this.userset_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.userset_btn.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userset_btn.Location = new System.Drawing.Point(159, 88);
            this.userset_btn.Name = "userset_btn";
            this.userset_btn.Size = new System.Drawing.Size(181, 69);
            this.userset_btn.TabIndex = 6;
            this.userset_btn.Text = "用户设置";
            this.userset_btn.UseVisualStyleBackColor = true;
            this.userset_btn.Click += new System.EventHandler(this.userset_btn_Click);
            // 
            // MainSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1218, 500);
            this.Controls.Add(this.aboutus_btn);
            this.Controls.Add(this.ioset_btn);
            this.Controls.Add(this.handset_btn);
            this.Controls.Add(this.Upan_btn);
            this.Controls.Add(this.ruanxianweiset_btn);
            this.Controls.Add(this.userset_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainSettingForm";
            this.Text = "MainSettingForm";
            this.Load += new System.EventHandler(this.MainSettingForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button aboutus_btn;
        private System.Windows.Forms.Button ioset_btn;
        private System.Windows.Forms.Button handset_btn;
        private System.Windows.Forms.Button Upan_btn;
        private System.Windows.Forms.Button ruanxianweiset_btn;
        private System.Windows.Forms.Button userset_btn;
    }
}