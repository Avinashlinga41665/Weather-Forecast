using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Weather_Forecast
{
    public partial class HorizontalTab : UserControl
    {

        public event Action<DateTime> DateSelected;

        public HorizontalTab()
        {
            InitializeComponent();
            this.Width = WeatherForecaster.Width;
        }
                  
        private void HorizontalTab_Load(object sender, EventArgs e)
        {
            designCSS(panel2);
        }
        public void TabInfo(DateTime date, int mintemp, int maxtemp, string description, int precipitation, Image icon)
        {
            label1.Text = date.ToString();
            label2.Text = mintemp.ToString() + "°C";
            label3.Text = maxtemp.ToString() + "°C";
            label4.Text = description;
            label5.Text = precipitation.ToString() + " mm"; // Assuming precipitation is in millimeters
            pictureBox1.Image = icon;

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
            Color transparentColor = Color.FromArgb((int)(255 * 0.8755), 0, 62, 124);
            Control.BackColor = transparentColor;
            Control.ForeColor = Color.LightGray; // Border color
            Control.Region = new Region(path);

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
           

        }


        private void HorizontalTab_DoubleClick(object sender, EventArgs e)
        {

        }

        private void HorizontalTab_MouseEnter(object sender, EventArgs e)
        {

        }

        public void HorizontalTab_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Horizontal tab clicked!"); 
            WeatherForecaster weatherForecaster = new WeatherForecaster();
            weatherForecaster.horizontaltab_summary(WeatherForecaster.locationname, Convert.ToDateTime(label1.Text));
            weatherForecaster.hourlydata(WeatherForecaster.locationname);
        
        }

        private void panel2_Click(object sender, EventArgs e)
        {

            WeatherForecaster weatherForecaster = new WeatherForecaster();
            weatherForecaster.horizontaltab_summary(WeatherForecaster.locationname, Convert.ToDateTime(label1.Text));
            weatherForecaster.hourlydata(WeatherForecaster.locationname);
            Summary summary = new Summary();
            Hourly hourly = new Hourly();
            MoreDetails details = new MoreDetails();
        }

        private T FindParent<T>(Control control) where T : Control
        { //for accessing the parent control
            Control? parent = control.Parent;
            while (parent != null)
            {
                if (parent is T typedParent)
                {
                    return typedParent;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}
