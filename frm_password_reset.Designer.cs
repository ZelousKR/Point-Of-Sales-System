namespace POS_Application
{
    partial class frm_password_reset
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
            this.label1 = new System.Windows.Forms.Label();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_recovery = new System.Windows.Forms.Button();
            this.txt_recovery = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(11, 202);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 19);
            this.label1.TabIndex = 20;
            this.label1.Text = "Recovery Code";
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_clear.FlatAppearance.BorderSize = 0;
            this.btn_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clear.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_clear.Location = new System.Drawing.Point(269, 255);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(108, 34);
            this.btn_clear.TabIndex = 19;
            this.btn_clear.Text = "Clear";
            this.btn_clear.UseVisualStyleBackColor = false;
            // 
            // btn_recovery
            // 
            this.btn_recovery.BackColor = System.Drawing.Color.DarkOrange;
            this.btn_recovery.FlatAppearance.BorderSize = 0;
            this.btn_recovery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_recovery.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_recovery.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_recovery.Location = new System.Drawing.Point(120, 255);
            this.btn_recovery.Name = "btn_recovery";
            this.btn_recovery.Size = new System.Drawing.Size(119, 34);
            this.btn_recovery.TabIndex = 18;
            this.btn_recovery.Text = "Submit";
            this.btn_recovery.UseVisualStyleBackColor = false;
            // 
            // txt_recovery
            // 
            this.txt_recovery.Location = new System.Drawing.Point(120, 196);
            this.txt_recovery.Multiline = true;
            this.txt_recovery.Name = "txt_recovery";
            this.txt_recovery.Size = new System.Drawing.Size(257, 34);
            this.txt_recovery.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Nirmala UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Info;
            this.label2.Location = new System.Drawing.Point(199, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 19);
            this.label2.TabIndex = 21;
            this.label2.Text = "Logo";
            // 
            // frm_password_reset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(427, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_recovery);
            this.Controls.Add(this.txt_recovery);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frm_password_reset";
            this.Text = "frm_password_reset";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_recovery;
        private System.Windows.Forms.TextBox txt_recovery;
        private System.Windows.Forms.Label label2;
    }
}