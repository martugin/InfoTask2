namespace Experiments
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 287);
            this.Controls.Add(this.butIndicator);
            this.Controls.Add(this.butXml);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butXml;
        private System.Windows.Forms.Button butIndicator;
    }
}

