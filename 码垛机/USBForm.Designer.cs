namespace 码垛机
{
    partial class USBForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USBForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.usbbutton = new System.Windows.Forms.Button();
            this.usblabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.usbbutton);
            this.panel1.Controls.Add(this.usblabel);
            this.panel1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(17, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1219, 659);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightSalmon;
            this.button1.Location = new System.Drawing.Point(301, 485);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 62);
            this.button1.TabIndex = 2;
            this.button1.Text = "返回";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // usbbutton
            // 
            this.usbbutton.BackColor = System.Drawing.Color.LightCyan;
            this.usbbutton.Image = ((System.Drawing.Image)(resources.GetObject("usbbutton.Image")));
            this.usbbutton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.usbbutton.Location = new System.Drawing.Point(276, 276);
            this.usbbutton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.usbbutton.Name = "usbbutton";
            this.usbbutton.Size = new System.Drawing.Size(197, 75);
            this.usbbutton.TabIndex = 1;
            this.usbbutton.Text = "拷贝今日数据";
            this.usbbutton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.usbbutton.UseVisualStyleBackColor = false;
            this.usbbutton.Click += new System.EventHandler(this.usbbutton_Click);
            // 
            // usblabel
            // 
            this.usblabel.AutoSize = true;
            this.usblabel.BackColor = System.Drawing.Color.Transparent;
            this.usblabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.usblabel.Location = new System.Drawing.Point(297, 160);
            this.usblabel.Name = "usblabel";
            this.usblabel.Size = new System.Drawing.Size(252, 20);
            this.usblabel.TabIndex = 0;
            this.usblabel.Text = "请插入U盘(请勿提前插入)";
            this.usblabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.usblabel.Click += new System.EventHandler(this.usblabel_Click);
            // 
            // USBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1243, 671);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "USBForm";
            this.Text = "USBForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.USBForm_FormClosing);
            this.Load += new System.EventHandler(this.USBForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button usbbutton;
        private System.Windows.Forms.Label usblabel;
        private System.Windows.Forms.Button button1;
    }
}