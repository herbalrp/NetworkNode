namespace RouterClient
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.CoDostalem = new System.Windows.Forms.TextBox();
            this.CoWysylam = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // CoDostalem
            // 
            this.CoDostalem.Location = new System.Drawing.Point(45, 129);
            this.CoDostalem.Multiline = true;
            this.CoDostalem.Name = "CoDostalem";
            this.CoDostalem.Size = new System.Drawing.Size(395, 82);
            this.CoDostalem.TabIndex = 6;
            this.CoDostalem.Text = "Otrzymane dane";
            this.CoDostalem.TextChanged += new System.EventHandler(this.textBox4_TextChanged_1);
            // 
            // CoWysylam
            // 
            this.CoWysylam.Location = new System.Drawing.Point(201, 12);
            this.CoWysylam.Multiline = true;
            this.CoWysylam.Name = "CoWysylam";
            this.CoWysylam.Size = new System.Drawing.Size(239, 111);
            this.CoWysylam.TabIndex = 9;
            this.CoWysylam.Text = "Co wysylam:";
            this.CoWysylam.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(45, 13);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(106, 41);
            this.button5.TabIndex = 11;
            this.button5.Text = "Wybierz plik konfiguracyjny";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(45, 59);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(106, 33);
            this.button6.TabIndex = 12;
            this.button6.Text = "Polacz z kablami";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 244);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.CoWysylam);
            this.Controls.Add(this.CoDostalem);
            this.Name = "Form1";
            this.Text = "Wezel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox CoDostalem;
        private System.Windows.Forms.TextBox CoWysylam;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}

