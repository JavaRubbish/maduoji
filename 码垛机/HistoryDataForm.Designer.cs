﻿namespace 码垛机
{
    partial class HistoryDataForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryDataForm));
            this.total_btn = new System.Windows.Forms.Button();
            this.lastpg_btn = new System.Windows.Forms.Button();
            this.nextpg_btn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.year_btn = new System.Windows.Forms.Button();
            this.mon_btn = new System.Windows.Forms.Button();
            this.day_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // total_btn
            // 
            this.total_btn.BackColor = System.Drawing.Color.Cornsilk;
            this.total_btn.FlatAppearance.BorderSize = 0;
            this.total_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.total_btn.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.total_btn.Location = new System.Drawing.Point(980, 1);
            this.total_btn.Name = "total_btn";
            this.total_btn.Size = new System.Drawing.Size(199, 49);
            this.total_btn.TabIndex = 3;
            this.total_btn.Text = "总计";
            this.total_btn.UseVisualStyleBackColor = false;
            // 
            // lastpg_btn
            // 
            this.lastpg_btn.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lastpg_btn.Location = new System.Drawing.Point(15, 238);
            this.lastpg_btn.Name = "lastpg_btn";
            this.lastpg_btn.Size = new System.Drawing.Size(160, 56);
            this.lastpg_btn.TabIndex = 4;
            this.lastpg_btn.Text = "上页";
            this.lastpg_btn.UseVisualStyleBackColor = true;
            this.lastpg_btn.Click += new System.EventHandler(this.lastpg_btn_Click);
            // 
            // nextpg_btn
            // 
            this.nextpg_btn.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nextpg_btn.Location = new System.Drawing.Point(15, 341);
            this.nextpg_btn.Name = "nextpg_btn";
            this.nextpg_btn.Size = new System.Drawing.Size(160, 56);
            this.nextpg_btn.TabIndex = 5;
            this.nextpg_btn.Text = "下页";
            this.nextpg_btn.UseVisualStyleBackColor = true;
            this.nextpg_btn.Click += new System.EventHandler(this.nextpg_btn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("黑体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView1.Location = new System.Drawing.Point(40, 54);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.PaleGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("黑体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(910, 440);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "物料编号";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "物料描述";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "订单号";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "数量";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(2, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "日期";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(1, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "总产量";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(20, 76);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(152, 25);
            this.dateTimePicker1.TabIndex = 11;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(70, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "3333";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nextpg_btn);
            this.groupBox1.Controls.Add(this.lastpg_btn);
            this.groupBox1.Location = new System.Drawing.Point(980, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(199, 438);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(88, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 26);
            this.label5.TabIndex = 14;
            // 
            // year_btn
            // 
            this.year_btn.BackColor = System.Drawing.Color.Cornsilk;
            this.year_btn.FlatAppearance.BorderSize = 0;
            this.year_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.year_btn.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.year_btn.Location = new System.Drawing.Point(673, 1);
            this.year_btn.Name = "year_btn";
            this.year_btn.Size = new System.Drawing.Size(199, 49);
            this.year_btn.TabIndex = 15;
            this.year_btn.Text = "年";
            this.year_btn.UseVisualStyleBackColor = false;
            this.year_btn.Click += new System.EventHandler(this.year_btn_Click);
            // 
            // mon_btn
            // 
            this.mon_btn.BackColor = System.Drawing.Color.Cornsilk;
            this.mon_btn.FlatAppearance.BorderSize = 0;
            this.mon_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mon_btn.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mon_btn.Location = new System.Drawing.Point(351, 1);
            this.mon_btn.Name = "mon_btn";
            this.mon_btn.Size = new System.Drawing.Size(199, 49);
            this.mon_btn.TabIndex = 16;
            this.mon_btn.Text = "月";
            this.mon_btn.UseVisualStyleBackColor = false;
            this.mon_btn.Click += new System.EventHandler(this.mon_btn_Click);
            // 
            // day_btn
            // 
            this.day_btn.BackColor = System.Drawing.Color.Yellow;
            this.day_btn.FlatAppearance.BorderSize = 0;
            this.day_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.day_btn.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.day_btn.Location = new System.Drawing.Point(40, 1);
            this.day_btn.Name = "day_btn";
            this.day_btn.Size = new System.Drawing.Size(199, 49);
            this.day_btn.TabIndex = 17;
            this.day_btn.Text = "日";
            this.day_btn.UseVisualStyleBackColor = false;
            this.day_btn.Click += new System.EventHandler(this.day_btn_Click);
            // 
            // HistoryDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1218, 500);
            this.Controls.Add(this.day_btn);
            this.Controls.Add(this.mon_btn);
            this.Controls.Add(this.year_btn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.total_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HistoryDataForm";
            this.Text = "HistoryDataForm";
            this.Load += new System.EventHandler(this.HistoryDataForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button total_btn;
        private System.Windows.Forms.Button lastpg_btn;
        private System.Windows.Forms.Button nextpg_btn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button year_btn;
        private System.Windows.Forms.Button mon_btn;
        private System.Windows.Forms.Button day_btn;
    }
}