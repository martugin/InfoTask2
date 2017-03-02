namespace ComClients
{
    partial class TimedIndicatorForm
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
            this.Procent = new System.Windows.Forms.ProgressBar();
            this.PeriodEnd = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PeriodBegin = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Text1 = new System.Windows.Forms.Label();
            this.Text2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Procent
            // 
            this.Procent.Location = new System.Drawing.Point(2, 23);
            this.Procent.Name = "Procent";
            this.Procent.Size = new System.Drawing.Size(340, 23);
            this.Procent.TabIndex = 0;
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.PeriodEnd.Location = new System.Drawing.Point(192, 4);
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.Size = new System.Drawing.Size(150, 16);
            this.PeriodEnd.TabIndex = 8;
            this.PeriodEnd.Text = "00.00.0000 00:00:00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(168, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "до";
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodBegin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.PeriodBegin.Location = new System.Drawing.Point(20, 4);
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.Size = new System.Drawing.Size(147, 16);
            this.PeriodBegin.TabIndex = 6;
            this.PeriodBegin.Text = "00.00.0000 00:00:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "от";
            // 
            // Text1
            // 
            this.Text1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Text1.Location = new System.Drawing.Point(0, 49);
            this.Text1.Name = "Text1";
            this.Text1.Size = new System.Drawing.Size(342, 20);
            this.Text1.TabIndex = 9;
            this.Text1.Text = "***";
            this.Text1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Text2
            // 
            this.Text2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Text2.Location = new System.Drawing.Point(0, 69);
            this.Text2.Name = "Text2";
            this.Text2.Size = new System.Drawing.Size(342, 20);
            this.Text2.TabIndex = 10;
            this.Text2.Text = "***";
            this.Text2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimedIndicatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 101);
            this.ControlBox = false;
            this.Controls.Add(this.Text2);
            this.Controls.Add(this.Text1);
            this.Controls.Add(this.PeriodEnd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PeriodBegin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Procent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TimedIndicatorForm";
            this.Text = "InfoTask";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.ProgressBar Procent;
        internal System.Windows.Forms.Label PeriodEnd;
        internal System.Windows.Forms.Label PeriodBegin;
        internal System.Windows.Forms.Label Text1;
        internal System.Windows.Forms.Label Text2;
    }
}