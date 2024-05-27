namespace Weather_Forecast
{
    partial class HorizontalTab
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel2 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(26, 27);
            label1.Name = "label1";
            label1.Size = new Size(64, 32);
            label1.TabIndex = 0;
            label1.Text = "Date";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Location = new Point(26, 82);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(74, 73);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(117, 82);
            label2.Name = "label2";
            label2.Size = new Size(51, 28);
            label2.TabIndex = 2;
            label2.Text = "Min ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(117, 132);
            label3.Name = "label3";
            label3.Size = new Size(49, 28);
            label3.TabIndex = 3;
            label3.Text = "Max";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(26, 160);
            label4.Name = "label4";
            label4.Size = new Size(112, 28);
            label4.TabIndex = 4;
            label4.Text = "Description";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI Semilight", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.White;
            label5.Location = new Point(192, 82);
            label5.Name = "label5";
            label5.Size = new Size(116, 28);
            label5.TabIndex = 5;
            label5.Text = "Precipitation";
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(0, 62, 124);
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label2);
            panel2.Dock = DockStyle.Fill;
            panel2.ForeColor = Color.FromArgb(0, 62, 124);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(433, 202);
            panel2.TabIndex = 11;
            panel2.Click += HorizontalTab_Click;
            panel2.Paint += panel2_Paint;
            panel2.DoubleClick += HorizontalTab_Click;
            // 
            // HorizontalTab
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 62, 124);
            Controls.Add(panel2);
            Name = "HorizontalTab";
            Size = new Size(433, 202);
            Load += HorizontalTab_Load;
            Click += HorizontalTab_Click;
            DoubleClick += HorizontalTab_DoubleClick;
            MouseEnter += HorizontalTab_MouseEnter;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Panel panel2;
    }
}
