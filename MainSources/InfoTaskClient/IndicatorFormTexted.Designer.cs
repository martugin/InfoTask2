namespace ComClients
{
    partial class IndicatorFormTexted
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
            this.Text0 = new System.Windows.Forms.Label();
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
            // Text0
            // 
            this.Text0.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Text0.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Text0.Location = new System.Drawing.Point(0, 4);
            this.Text0.Name = "Text0";
            this.Text0.Size = new System.Drawing.Size(342, 16);
            this.Text0.TabIndex = 6;
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
            // IndicatorFormTexted
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 106);
            this.ControlBox = false;
            this.Controls.Add(this.Text2);
            this.Controls.Add(this.Text1);
            this.Controls.Add(this.Text0);
            this.Controls.Add(this.Procent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "IndicatorFormTexted";
            this.Text = "InfoTask";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label Text0;
        internal System.Windows.Forms.ProgressBar Procent;
        internal System.Windows.Forms.Label Text1;
        internal System.Windows.Forms.Label Text2;
    }
}