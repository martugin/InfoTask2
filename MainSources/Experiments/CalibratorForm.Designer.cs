namespace Experiments
{
    partial class CalibratorForm
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
            this.components = new System.ComponentModel.Container();
            this.ButStart = new System.Windows.Forms.Button();
            this.PeriodLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Signal1 = new System.Windows.Forms.ComboBox();
            this.Signal2 = new System.Windows.Forms.ComboBox();
            this.Signal3 = new System.Windows.Forms.ComboBox();
            this.ButStop = new System.Windows.Forms.Button();
            this.Value1 = new System.Windows.Forms.TextBox();
            this.Value2 = new System.Windows.Forms.TextBox();
            this.Value3 = new System.Windows.Forms.TextBox();
            this.ButInit = new System.Windows.Forms.Button();
            this.ButClose = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.LateLength = new System.Windows.Forms.TextBox();
            this.ValuesTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ButStart
            // 
            this.ButStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButStart.Location = new System.Drawing.Point(12, 60);
            this.ButStart.Name = "ButStart";
            this.ButStart.Size = new System.Drawing.Size(138, 52);
            this.ButStart.TabIndex = 0;
            this.ButStart.Text = "Start";
            this.ButStart.UseVisualStyleBackColor = true;
            this.ButStart.Click += new System.EventHandler(this.ButStart_Click);
            // 
            // PeriodLength
            // 
            this.PeriodLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodLength.Location = new System.Drawing.Point(255, 9);
            this.PeriodLength.Name = "PeriodLength";
            this.PeriodLength.Size = new System.Drawing.Size(56, 22);
            this.PeriodLength.TabIndex = 1;
            this.PeriodLength.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(168, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Интервал";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(168, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Сигнал 1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(168, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Сигнал 2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(168, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Сигнал 3";
            // 
            // Signal1
            // 
            this.Signal1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Signal1.FormattingEnabled = true;
            this.Signal1.Items.AddRange(new object[] {
            "State",
            "Bool",
            "Int",
            "Real",
            "Time",
            "String"});
            this.Signal1.Location = new System.Drawing.Point(255, 65);
            this.Signal1.Name = "Signal1";
            this.Signal1.Size = new System.Drawing.Size(121, 24);
            this.Signal1.TabIndex = 7;
            this.Signal1.Text = "Bool";
            // 
            // Signal2
            // 
            this.Signal2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Signal2.FormattingEnabled = true;
            this.Signal2.Items.AddRange(new object[] {
            "State",
            "Bool",
            "Int",
            "Real",
            "Time",
            "String"});
            this.Signal2.Location = new System.Drawing.Point(255, 92);
            this.Signal2.Name = "Signal2";
            this.Signal2.Size = new System.Drawing.Size(121, 24);
            this.Signal2.TabIndex = 8;
            this.Signal2.Text = "Int";
            // 
            // Signal3
            // 
            this.Signal3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Signal3.FormattingEnabled = true;
            this.Signal3.Items.AddRange(new object[] {
            "State",
            "Bool",
            "Int",
            "Real",
            "Time",
            "String"});
            this.Signal3.Location = new System.Drawing.Point(255, 120);
            this.Signal3.Name = "Signal3";
            this.Signal3.Size = new System.Drawing.Size(121, 24);
            this.Signal3.TabIndex = 9;
            this.Signal3.Text = "Real";
            // 
            // ButStop
            // 
            this.ButStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButStop.Location = new System.Drawing.Point(12, 116);
            this.ButStop.Name = "ButStop";
            this.ButStop.Size = new System.Drawing.Size(138, 52);
            this.ButStop.TabIndex = 10;
            this.ButStop.Text = "Stop";
            this.ButStop.UseVisualStyleBackColor = true;
            this.ButStop.Click += new System.EventHandler(this.ButStop_Click);
            // 
            // Value1
            // 
            this.Value1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Value1.Location = new System.Drawing.Point(382, 65);
            this.Value1.Name = "Value1";
            this.Value1.Size = new System.Drawing.Size(145, 22);
            this.Value1.TabIndex = 11;
            // 
            // Value2
            // 
            this.Value2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Value2.Location = new System.Drawing.Point(382, 92);
            this.Value2.Name = "Value2";
            this.Value2.Size = new System.Drawing.Size(145, 22);
            this.Value2.TabIndex = 12;
            // 
            // Value3
            // 
            this.Value3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Value3.Location = new System.Drawing.Point(382, 120);
            this.Value3.Name = "Value3";
            this.Value3.Size = new System.Drawing.Size(145, 22);
            this.Value3.TabIndex = 13;
            // 
            // ButInit
            // 
            this.ButInit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButInit.Location = new System.Drawing.Point(12, 5);
            this.ButInit.Name = "ButInit";
            this.ButInit.Size = new System.Drawing.Size(138, 52);
            this.ButInit.TabIndex = 14;
            this.ButInit.Text = "Init";
            this.ButInit.UseVisualStyleBackColor = true;
            this.ButInit.Click += new System.EventHandler(this.ButInit_Click);
            // 
            // ButClose
            // 
            this.ButClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButClose.Location = new System.Drawing.Point(12, 173);
            this.ButClose.Name = "ButClose";
            this.ButClose.Size = new System.Drawing.Size(138, 52);
            this.ButClose.TabIndex = 15;
            this.ButClose.Text = "Close";
            this.ButClose.UseVisualStyleBackColor = true;
            this.ButClose.Click += new System.EventHandler(this.ButClose_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(168, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 16);
            this.label5.TabIndex = 17;
            this.label5.Text = "Задержка";
            // 
            // LateLength
            // 
            this.LateLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LateLength.Location = new System.Drawing.Point(255, 35);
            this.LateLength.Name = "LateLength";
            this.LateLength.Size = new System.Drawing.Size(56, 22);
            this.LateLength.TabIndex = 16;
            this.LateLength.Text = "0";
            // 
            // ValuesTimer
            // 
            this.ValuesTimer.Tick += new System.EventHandler(this.ValuesTimer_Tick);
            // 
            // CalibratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 231);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LateLength);
            this.Controls.Add(this.ButClose);
            this.Controls.Add(this.ButInit);
            this.Controls.Add(this.Value3);
            this.Controls.Add(this.Value2);
            this.Controls.Add(this.Value1);
            this.Controls.Add(this.ButStop);
            this.Controls.Add(this.Signal3);
            this.Controls.Add(this.Signal2);
            this.Controls.Add(this.Signal1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PeriodLength);
            this.Controls.Add(this.ButStart);
            this.Name = "CalibratorForm";
            this.Text = "Calibrator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButStart;
        private System.Windows.Forms.TextBox PeriodLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox Signal1;
        private System.Windows.Forms.ComboBox Signal2;
        private System.Windows.Forms.ComboBox Signal3;
        private System.Windows.Forms.Button ButStop;
        private System.Windows.Forms.TextBox Value1;
        private System.Windows.Forms.TextBox Value2;
        private System.Windows.Forms.TextBox Value3;
        private System.Windows.Forms.Button ButInit;
        private System.Windows.Forms.Button ButClose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox LateLength;
        private System.Windows.Forms.Timer ValuesTimer;
    }
}