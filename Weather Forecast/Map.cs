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
    public partial class Map : UserControl
    {
        public Map()
        {
            InitializeComponent();
        }

        private void Map_Load(object sender, EventArgs e)
        {
            Maps();
        }
        public void Maps()
        {
            using (WebClient web = new WebClient())
            {
                string apikey = "329cec969cefc40090ac7b5d60221eaf";
                string url = $"https://tile.openweathermap.org/map/clouds/1/0/1.png?appid={apikey}";
                string imagePath = "map_image.png";
                try
                {
                    // Download the image from the URL
                    if (File.Exists(imagePath))
                    {
                        // Load the image into the PictureBox and fill it
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox1.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        Console.WriteLine("Error: Downloaded image file not found.");
                    }

                }
                catch (Exception ex)
                {
                    // Handle any exceptions, such as network errors or invalid URLs
                    Console.WriteLine($"Error downloading image: {ex.Message}");
                }

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
