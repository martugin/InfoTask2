namespace ProvidersLibrary
{
    partial class ProviderSetupForm
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
            this.ButCheck = new System.Windows.Forms.Button();
            this.ButOK = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.Props = new System.Windows.Forms.DataGridView();
            this.PropNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PropName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PropCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PropValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.Props)).BeginInit();
            this.SuspendLayout();
            // 
            // ButCheck
            // 
            this.ButCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButCheck.Location = new System.Drawing.Point(86, 258);
            this.ButCheck.Name = "ButCheck";
            this.ButCheck.Size = new System.Drawing.Size(193, 36);
            this.ButCheck.TabIndex = 10;
            this.ButCheck.Text = "Проверка соединения";
            this.ButCheck.UseVisualStyleBackColor = true;
            this.ButCheck.Click += new System.EventHandler(this.ButCheck_Click);
            // 
            // ButOK
            // 
            this.ButOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButOK.Location = new System.Drawing.Point(320, 258);
            this.ButOK.Name = "ButOK";
            this.ButOK.Size = new System.Drawing.Size(193, 36);
            this.ButOK.TabIndex = 15;
            this.ButOK.Text = "OK";
            this.ButOK.UseVisualStyleBackColor = true;
            this.ButOK.Click += new System.EventHandler(this.ButOK_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButCancel.Location = new System.Drawing.Point(555, 258);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(193, 36);
            this.ButCancel.TabIndex = 20;
            this.ButCancel.Text = "Отмена";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // Props
            // 
            this.Props.AllowUserToAddRows = false;
            this.Props.AllowUserToDeleteRows = false;
            this.Props.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Props.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Props.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Props.ColumnHeadersVisible = false;
            this.Props.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PropNum,
            this.PropName,
            this.PropCode,
            this.PropValue});
            this.Props.Location = new System.Drawing.Point(2, 0);
            this.Props.Name = "Props";
            this.Props.RowHeadersVisible = false;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Props.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.Props.Size = new System.Drawing.Size(838, 252);
            this.Props.TabIndex = 5;
            this.Props.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Props_CellClick);
            // 
            // PropNum
            // 
            this.PropNum.Frozen = true;
            this.PropNum.HeaderText = "№";
            this.PropNum.MinimumWidth = 10;
            this.PropNum.Name = "PropNum";
            this.PropNum.ReadOnly = true;
            this.PropNum.Width = 30;
            // 
            // PropName
            // 
            this.PropName.Frozen = true;
            this.PropName.HeaderText = "Описание настройки";
            this.PropName.Name = "PropName";
            this.PropName.ReadOnly = true;
            this.PropName.Width = 400;
            // 
            // PropCode
            // 
            this.PropCode.Frozen = true;
            this.PropCode.HeaderText = "Код";
            this.PropCode.Name = "PropCode";
            this.PropCode.ReadOnly = true;
            this.PropCode.Visible = false;
            this.PropCode.Width = 50;
            // 
            // PropValue
            // 
            this.PropValue.Frozen = true;
            this.PropValue.HeaderText = "Значение";
            this.PropValue.Name = "PropValue";
            this.PropValue.Width = 400;
            // 
            // ProviderSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 299);
            this.ControlBox = false;
            this.Controls.Add(this.Props);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.ButOK);
            this.Controls.Add(this.ButCheck);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProviderSetupForm";
            this.ShowIcon = false;
            this.Text = "Настройка провайдера";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ProviderSetupForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Props)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButCheck;
        private System.Windows.Forms.Button ButOK;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.DataGridView Props;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropValue;
    }
}