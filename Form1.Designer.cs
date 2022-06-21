namespace WorkTimeLogger
{
    partial class Form1
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
            this.btnGenerateText = new System.Windows.Forms.Button();
            this.numFirstRow = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDateColumn = new System.Windows.Forms.TextBox();
            this.txtTimeColumn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTextColumn = new System.Windows.Forms.TextBox();
            this.numLastRow = new System.Windows.Forms.NumericUpDown();
            this.dlgOpenSpreadsheet = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLastRow)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerateText
            // 
            this.btnGenerateText.Location = new System.Drawing.Point(54, 622);
            this.btnGenerateText.Name = "btnGenerateText";
            this.btnGenerateText.Size = new System.Drawing.Size(249, 55);
            this.btnGenerateText.TabIndex = 0;
            this.btnGenerateText.Text = "Generate Text";
            this.btnGenerateText.UseVisualStyleBackColor = true;
            this.btnGenerateText.Click += new System.EventHandler(this.button1_Click);
            // 
            // numFirstRow
            // 
            this.numFirstRow.Location = new System.Drawing.Point(211, 206);
            this.numFirstRow.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numFirstRow.Name = "numFirstRow";
            this.numFirstRow.Size = new System.Drawing.Size(122, 31);
            this.numFirstRow.TabIndex = 2;
            this.numFirstRow.Value = new decimal(new int[] {
            3634,
            0,
            0,
            0});
            this.numFirstRow.ValueChanged += new System.EventHandler(this.numFirstRow_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 212);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "First Row";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 279);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Last Row";
            // 
            // txtDateColumn
            // 
            this.txtDateColumn.Location = new System.Drawing.Point(211, 355);
            this.txtDateColumn.Name = "txtDateColumn";
            this.txtDateColumn.Size = new System.Drawing.Size(122, 31);
            this.txtDateColumn.TabIndex = 6;
            this.txtDateColumn.Text = "2";
            // 
            // txtTimeColumn
            // 
            this.txtTimeColumn.Location = new System.Drawing.Point(211, 438);
            this.txtTimeColumn.Name = "txtTimeColumn";
            this.txtTimeColumn.Size = new System.Drawing.Size(122, 31);
            this.txtTimeColumn.TabIndex = 7;
            this.txtTimeColumn.Text = "6";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 355);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 25);
            this.label3.TabIndex = 8;
            this.label3.Text = "Date Column";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 438);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "Time Column";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 507);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Text Column";
            // 
            // txtTextColumn
            // 
            this.txtTextColumn.Location = new System.Drawing.Point(211, 507);
            this.txtTextColumn.Name = "txtTextColumn";
            this.txtTextColumn.Size = new System.Drawing.Size(122, 31);
            this.txtTextColumn.TabIndex = 11;
            this.txtTextColumn.Text = "10";
            // 
            // numLastRow
            // 
            this.numLastRow.Location = new System.Drawing.Point(211, 279);
            this.numLastRow.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numLastRow.Name = "numLastRow";
            this.numLastRow.Size = new System.Drawing.Size(122, 31);
            this.numLastRow.TabIndex = 12;
            this.numLastRow.Value = new decimal(new int[] {
            3641,
            0,
            0,
            0});
            this.numLastRow.ValueChanged += new System.EventHandler(this.numLastRow_ValueChanged);
            // 
            // dlgOpenSpreadsheet
            // 
            this.dlgOpenSpreadsheet.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Location = new System.Drawing.Point(613, 124);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(41, 37);
            this.btnBrowseFile.TabIndex = 13;
            this.btnBrowseFile.Text = "...";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(211, 124);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(396, 31);
            this.txtFileName.TabIndex = 14;
            this.txtFileName.Text = "X:\\Meritus\\Meritus Time Sheet.xlsx";
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 742);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.btnBrowseFile);
            this.Controls.Add(this.numLastRow);
            this.Controls.Add(this.txtTextColumn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTimeColumn);
            this.Controls.Add(this.txtDateColumn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numFirstRow);
            this.Controls.Add(this.btnGenerateText);
            this.Name = "Form1";
            this.Text = "Work Time Logger";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numFirstRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLastRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateText;
        private System.Windows.Forms.NumericUpDown numFirstRow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDateColumn;
        private System.Windows.Forms.TextBox txtTimeColumn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTextColumn;
        private System.Windows.Forms.NumericUpDown numLastRow;
        private System.Windows.Forms.OpenFileDialog dlgOpenSpreadsheet;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.TextBox txtFileName;
    }
}

