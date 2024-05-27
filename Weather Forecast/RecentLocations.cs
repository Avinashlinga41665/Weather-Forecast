using FontAwesome.Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Weather_Forecast
{
    public partial class RecentLocations : UserControl
    {
        public int LocationID { get; set; }
        public RecentLocations()
        {
            InitializeComponent();
           

        }
        public void designCSS()
        {
            label3.Visible = true;
            // Define the icon and size
            IconChar iconChar = IconChar.Home;
            int iconSize = 40; // Size of the icon in pixels

            // Create and configure an IconPictureBox to render the icon
            IconPictureBox iconPictureBox = new IconPictureBox
            {
                IconChar = iconChar,
                IconSize = iconSize,
                Size = new Size(iconSize, iconSize),
                ForeColor = Color.Black // Set icon color if needed
            };

            // Create an image from the IconPictureBox
            Bitmap iconImage = new Bitmap(iconPictureBox.Width, iconPictureBox.Height);
            iconPictureBox.DrawToBitmap(iconImage, new Rectangle(Point.Empty, iconImage.Size));

            // Configure the label to use the icon image as background
            label3.BackColor = Color.FromArgb(0,62,124);
            label3.BackgroundImage = iconImage;
            label3.BackgroundImageLayout = ImageLayout.Center;
            label3.Size = new Size(iconSize, iconSize);
            label3.BorderStyle = BorderStyle.None; // Remove border

            // Adjust label properties for better appearance
            label3.Padding = new Padding(5); // Add padding around the icon
            label3.Margin = new Padding(5); // Add margin around the label
            label3.Font = new Font(label3.Font, FontStyle.Bold); // Make the label text bold
        }

        private void RecentLocations_Load(object sender, EventArgs e)
        {
            //WeatherForecaster weatherForecaster = new WeatherForecaster();
            //weatherForecaster.RecentLocationsPanel();


        }
        public void TabInfo(int locationid, String locationname, string icon, int temperature,bool primary)
        {
            try
            {

                int commaIndex = locationname.IndexOf(',');

                if (commaIndex != -1)
                {
                    // Extract the substring from the start of the string up to the first comma
                    string labelText = locationname.Substring(0, commaIndex);

                    // Assign the extracted substring to the Text property of label1
                    label1.Text = labelText;
                }
                else
                {
                    // If no comma is found, assign the entire locationname to label1.Text
                    label1.Text = locationname;
                }
                using (WebClient web = new WebClient())
                {

                    string iconUrl = $"http://openweathermap.org/img/wn/{icon}.png";
                    byte[] imageData = web.DownloadData(iconUrl);
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox1.Image = System.Drawing.Image.FromStream(ms);
                    }
                    label2.Text = temperature.ToString() + "°C";
                    LocationID = locationid;
                    if (primary)
                    {
                        label3.Visible = true;
                        designCSS();
                    }
                    else
                    {
                        label3.Visible =false;
                    }
                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                contextMenuStrip1.Visible = true;
                Point menuLocation = button1.PointToScreen(new Point(button1.Width, 0));
                contextMenuStrip1.Show(menuLocation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WeatherForecaster weatherForecaster = new WeatherForecaster();
                weatherForecaster.DeleteLocation(LocationID);
                weatherForecaster.RecentLocationsPanel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occured remove" + ex.Message);
                MessageBox.Show("An error occurred while removing location: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        
        }
    }
}
