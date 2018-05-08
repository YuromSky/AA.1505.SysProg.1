namespace Interface
{
    partial class Form3
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
            Form3.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();

            ((System.ComponentModel.ISupportInitialize)(Form3.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView3
            // 
            Form3.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Form3.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                    this.Column16,
                    this.Column10,
                    this.Column11,
                    this.Column12,
                    this.Column13,
                    this.Column14,
                    this.Column15});
            Form3.dataGridView3.Location = new System.Drawing.Point(11, 23);
            Form3.dataGridView3.Name = "dataGridView3";
            Form3.dataGridView3.Size = new System.Drawing.Size(550, 406);
            Form3.dataGridView3.TabIndex = 6;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "ID";
            this.Column10.Name = "Column10";
            this.Column10.Width = 50;
            // 
            // Column11
            // 
            this.Column11.HeaderText = "Имя";
            this.Column11.Name = "Column11";
            this.Column11.Width = 150;
            // 
            // Column12
            // 
            this.Column12.HeaderText = "Здоровье";
            this.Column12.Name = "Column12";
            this.Column12.Width = 70;
            // 
            // Column13
            // 
            this.Column13.HeaderText = "Очки";
            this.Column13.Name = "Column13";
            this.Column13.Width = 70;
            // 
            // Column14
            // 
            this.Column14.HeaderText = "Жив";
            this.Column14.Name = "Column14";
            this.Column14.Width = 50;
            // 
            // Column15
            // 
            this.Column15.HeaderText = "Баллы";
            this.Column15.Name = "Column15";
            this.Column15.Width = 50;
            // 
            // Column16
            // 
            this.Column16.HeaderText = "Место";
            this.Column16.Name = "Column16";
            this.Column16.Width = 50;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 441);
            this.Controls.Add(Form3.dataGridView3);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load); this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(Form3.dataGridView3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        public static System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
    }
}







//private void InitializeComponent()
//{
//    this.label1 = new System.Windows.Forms.Label();
//    this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column9 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
//    this.dataGridView3 = new System.Windows.Forms.DataGridView();
//    this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
//    this.Column14 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
//    ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
//    this.SuspendLayout();
//    // 
//    // label1
//    // 
//    this.label1.AutoSize = true;
//    this.label1.Location = new System.Drawing.Point(8, 6);
//    this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
//    this.label1.Name = "label1";
//    this.label1.Size = new System.Drawing.Size(46, 13);
//    this.label1.TabIndex = 5;
//    this.label1.Text = "Раунд 1";
//    // 
//    // Column1
//    // 
//    this.Column1.HeaderText = "Робот";
//    this.Column1.Name = "Column1";
//    this.Column1.Width = 150;
//    // 
//    // Column2
//    // 
//    this.Column2.HeaderText = "Энергия";
//    this.Column2.Name = "Column2";
//    this.Column2.Width = 80;
//    // 
//    // Column3
//    // 
//    this.Column3.HeaderText = "Убийства";
//    this.Column3.Name = "Column3";
//    this.Column3.Width = 80;
//    // 
//    // Column4
//    // 
//    this.Column4.HeaderText = "Баллы";
//    this.Column4.Name = "Column4";
//    this.Column4.Width = 80;
//    // 
//    // Column5
//    // 
//    this.Column5.HeaderText = "ID";
//    this.Column5.Name = "Column5";
//    // 
//    // Column6
//    // 
//    this.Column6.HeaderText = "Имя";
//    this.Column6.Name = "Column6";
//    this.Column6.Width = 150;
//    // 
//    // Column7
//    // 
//    this.Column7.HeaderText = "Здоровье";
//    this.Column7.Name = "Column7";
//    // 
//    // Column8
//    // 
//    this.Column8.HeaderText = "Очки";
//    this.Column8.Name = "Column8";
//    // 
//    // Column9
//    // 
//    this.Column9.HeaderText = "Жив";
//    this.Column9.Name = "Column9";
//    this.Column9.Width = 50;
//    // 
//    // dataGridView3
//    // 
//    this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
//    this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
//            this.Column10,
//            this.Column11,
//            this.Column12,
//            this.Column13,
//            this.Column14});
//    this.dataGridView3.Location = new System.Drawing.Point(11, 23);
//    this.dataGridView3.Name = "dataGridView3";
//    this.dataGridView3.Size = new System.Drawing.Size(518, 406);
//    this.dataGridView3.TabIndex = 6;
//    // 
//    // Column10
//    // 
//    this.Column10.HeaderText = "ID";
//    this.Column10.Name = "Column10";
//    this.Column10.Width = 50;
//    // 
//    // Column11
//    // 
//    this.Column11.HeaderText = "Имя";
//    this.Column11.Name = "Column11";
//    this.Column11.Width = 150;
//    // 
//    // Column12
//    // 
//    this.Column12.HeaderText = "Здоровье";
//    this.Column12.Name = "Column12";
//    this.Column12.Width = 70;
//    // 
//    // Column13
//    // 
//    this.Column13.HeaderText = "Очки";
//    this.Column13.Name = "Column13";
//    this.Column13.Width = 70;
//    // 
//    // Column14
//    // 
//    this.Column14.HeaderText = "Жив";
//    this.Column14.Name = "Column14";
//    this.Column14.Width = 50;
//    // 
//    // Form3
//    // 
//    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
//    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//    this.ClientSize = new System.Drawing.Size(539, 441);
//    this.Controls.Add(this.dataGridView3);
//    this.Controls.Add(this.label1);
//    this.Margin = new System.Windows.Forms.Padding(2);
//    this.Name = "Form3";
//    this.Text = "Form3";
//    this.Load += new System.EventHandler(this.Form3_Load);
//    ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
//    this.ResumeLayout(false);
//    this.PerformLayout();

//}