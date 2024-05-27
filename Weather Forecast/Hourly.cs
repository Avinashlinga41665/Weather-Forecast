using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Weather_Forecast
{
    public partial class Hourly : UserControl
    {
        public Hourly()
        {
            InitializeComponent();
        }

        private void Hourly_Load(object sender, EventArgs e)
        {
            designCSS(panel2);

        }
        public void TabInfo(Image icon, int temperature, string description, string windspeed, DateTime hour)
        {

            pictureBox1.Image = icon;
            label1.Text = temperature.ToString() + "°C";
            label2.Text = description.ToString();
            label3.Text = windspeed;
            label4.Text = hour.ToString("t");
            label5.Text = hour.Date.ToString("M");


        }
        public void designCSS(Control Control)
        {
            // Set the border radius for panel1
            int cornerRadius = 25; // You can adjust this value to change the roundness of the corners
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90); // Top-left corner
            path.AddArc(Control.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90); // Top-right corner
            path.AddArc(Control.Width - cornerRadius, Control.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right corner
            path.AddArc(0, Control.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left corner
            path.CloseFigure();
            Color backColor = Control.BackColor;
            Color transparentColor = Color.FromArgb((int)(255 * 0.8755), 0, 62, 124);
            Control.BackColor = transparentColor;
            Control.ForeColor = Color.LightGray; // Border color
            Control.Region = new Region(path);

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
