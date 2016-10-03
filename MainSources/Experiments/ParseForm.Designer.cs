namespace Experiments
{
    partial class ParseForm
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
            this.ButGenField = new System.Windows.Forms.Button();
            this.ButSubCondition = new System.Windows.Forms.Button();
            this.ButCondition = new System.Windows.Forms.Button();
            this.Result = new System.Windows.Forms.TextBox();
            this.Formula = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ButGenField
            // 
            this.ButGenField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButGenField.Location = new System.Drawing.Point(114, 128);
            this.ButGenField.Name = "ButGenField";
            this.ButGenField.Size = new System.Drawing.Size(99, 71);
            this.ButGenField.TabIndex = 11;
            this.ButGenField.Text = "Поле генерации";
            this.ButGenField.UseVisualStyleBackColor = true;
            this.ButGenField.Click += new System.EventHandler(this.ButGenField_Click);
            // 
            // ButSubCondition
            // 
            this.ButSubCondition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButSubCondition.Location = new System.Drawing.Point(3, 163);
            this.ButSubCondition.Name = "ButSubCondition";
            this.ButSubCondition.Size = new System.Drawing.Size(112, 36);
            this.ButSubCondition.TabIndex = 10;
            this.ButSubCondition.Text = "Подусловие";
            this.ButSubCondition.UseVisualStyleBackColor = true;
            this.ButSubCondition.Click += new System.EventHandler(this.ButSubCondition_Click);
            // 
            // ButCondition
            // 
            this.ButCondition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButCondition.Location = new System.Drawing.Point(3, 128);
            this.ButCondition.Name = "ButCondition";
            this.ButCondition.Size = new System.Drawing.Size(112, 36);
            this.ButCondition.TabIndex = 9;
            this.ButCondition.Text = "Условие";
            this.ButCondition.UseVisualStyleBackColor = true;
            this.ButCondition.Click += new System.EventHandler(this.ButCondition_Click);
            // 
            // Result
            // 
            this.Result.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Result.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Result.Location = new System.Drawing.Point(3, 203);
            this.Result.Multiline = true;
            this.Result.Name = "Result";
            this.Result.Size = new System.Drawing.Size(559, 185);
            this.Result.TabIndex = 8;
            // 
            // Formula
            // 
            this.Formula.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Formula.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Formula.Location = new System.Drawing.Point(3, 2);
            this.Formula.Multiline = true;
            this.Formula.Name = "Formula";
            this.Formula.Size = new System.Drawing.Size(559, 123);
            this.Formula.TabIndex = 7;
            // 
            // ParseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 389);
            this.Controls.Add(this.ButGenField);
            this.Controls.Add(this.ButSubCondition);
            this.Controls.Add(this.ButCondition);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.Formula);
            this.Name = "ParseForm";
            this.Text = "ParseForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButGenField;
        private System.Windows.Forms.Button ButSubCondition;
        private System.Windows.Forms.Button ButCondition;
        private System.Windows.Forms.TextBox Result;
        private System.Windows.Forms.TextBox Formula;
    }
}