﻿namespace Experiments
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.butXml = new System.Windows.Forms.Button();
            this.butIndicator = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.butItDir = new System.Windows.Forms.Button();
            this.butJustIndicator = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // butXml
            // 
            this.butXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butXml.Location = new System.Drawing.Point(12, 12);
            this.butXml.Name = "butXml";
            this.butXml.Size = new System.Drawing.Size(119, 46);
            this.butXml.TabIndex = 0;
            this.butXml.Text = "XML";
            this.butXml.UseVisualStyleBackColor = true;
            this.butXml.Click += new System.EventHandler(this.butXml_Click);
            // 
            // butIndicator
            // 
            this.butIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butIndicator.Location = new System.Drawing.Point(152, 12);
            this.butIndicator.Name = "butIndicator";
            this.butIndicator.Size = new System.Drawing.Size(119, 46);
            this.butIndicator.TabIndex = 1;
            this.butIndicator.Text = "Indicator";
            this.butIndicator.UseVisualStyleBackColor = true;
            this.butIndicator.Click += new System.EventHandler(this.butIndicator_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(12, 128);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 46);
            this.button1.TabIndex = 2;
            this.button1.Text = "CloneAsync";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // butItDir
            // 
            this.butItDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butItDir.Location = new System.Drawing.Point(12, 64);
            this.butItDir.Name = "butItDir";
            this.butItDir.Size = new System.Drawing.Size(119, 46);
            this.butItDir.TabIndex = 3;
            this.butItDir.Text = "InfoTaskDir";
            this.butItDir.UseVisualStyleBackColor = true;
            this.butItDir.Click += new System.EventHandler(this.butItDir_Click);
            // 
            // butJustIndicator
            // 
            this.butJustIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butJustIndicator.Location = new System.Drawing.Point(152, 64);
            this.butJustIndicator.Name = "butJustIndicator";
            this.butJustIndicator.Size = new System.Drawing.Size(119, 46);
            this.butJustIndicator.TabIndex = 4;
            this.butJustIndicator.Text = "JustIndicator";
            this.butJustIndicator.UseVisualStyleBackColor = true;
            this.butJustIndicator.Click += new System.EventHandler(this.butJustIndicator_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(152, 192);
            this.progressBar1.MarqueeAnimationSpeed = 1;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(186, 20);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(152, 128);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(119, 46);
            this.button2.TabIndex = 6;
            this.button2.Text = "ProgressBar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.Location = new System.Drawing.Point(12, 180);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(119, 46);
            this.button3.TabIndex = 7;
            this.button3.Text = "CloneLogika";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 287);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.butJustIndicator);
            this.Controls.Add(this.butItDir);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.butIndicator);
            this.Controls.Add(this.butXml);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butXml;
        private System.Windows.Forms.Button butIndicator;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button butItDir;
        private System.Windows.Forms.Button butJustIndicator;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

