using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Geolocation;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Weather_Forecast
{
    public partial class Summary : UserControl
    {
        public Summary()
        {
            InitializeComponent();
            this.Width = WeatherForecaster.Width;

        }

        private void Summary_Load(object sender, EventArgs e)
        {

            designCSS(this);
            SummaryGraph(WeatherForecaster.locationname, DateTime.Today);

        }
        public void UpdateSummaryForDate(DateTime date)
        {
            try
            {

                SummaryGraph(WeatherForecaster.locationname, date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while Updating Summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //public DateTime GetCurrentDateTime(string timezone)
        //{
        //    using (WebClient web = new WebClient())
        //    {
        //        try
        //        {
        //            string url = $"http://worldtimeapi.org/api/timezone/{timezone}";
        //            var json = web.DownloadString(url);
        //            var result = JsonConvert.DeserializeObject<WorldTimeApiResponse>(json);
        //            return DateTime.Parse(result.datetime);
        //        }
        //        catch (WebException ex)
        //        {
        //            MessageBox.Show($"Error fetching date and time: {ex.Message}");
        //            return DateTime.MinValue;
        //        }
        //    }
        //}
        public Control SummaryGraph(string location, DateTime date)
        {
            using (WebClient web = new WebClient())
            {
                try
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={apikey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;
                    HashSet<DateTime> uniqueDates = new HashSet<DateTime>();
                    Dictionary<DateTime, int> temperatures = new Dictionary<DateTime, int>();
                    foreach (var item in output.list)
                    {
                        //if (item.dt_txt.Date == DateTime.Today.Date)
                        if (item.dt_txt.Date == date.Date)
                        {
                            temperatures.Add(item.dt_txt, Convert.ToInt32(item.main.temp));
                        }
                    }
                    Bitmap lineChartBitmap = GeneratehourlylineChart(temperatures);
                    Image lineChartImage = (Image)lineChartBitmap;
                    panel1.BackgroundImage = lineChartImage;
                    panel1.BackgroundImageLayout = ImageLayout.Stretch; // Optional: Adjust layout

                    return panel1;
                }

                catch (FormatException ex)
                {
                    MessageBox.Show($"Error parsing date: {ex.Message}");
                    return null;
                }
            }
        }
       public static Bitmap GeneratehourlylineChart(Dictionary<DateTime, int> temperatures)
        {
            try
            {

                if (temperatures == null || temperatures.Count == 0)
                    throw new ArgumentException("Temperature list cannot be null or empty.");

                Chart chart1 = new Chart();
                chart1.Width = WeatherForecaster.Width;
                chart1.Height = 365;
                // Set up chart properties
                chart1.Series.Add(new Series("Temperature")); // Assign a name to the series
                chart1.Series["Temperature"].ChartType = SeriesChartType.Spline;
                chart1.Series["Temperature"].IsValueShownAsLabel = true; // Display values on the line chart
                chart1.Series["Temperature"].LabelFormat = "{0} °C"; // Set custom format for data point labels
                chart1.Legends.Add(new Legend());
                chart1.Legends[0].Enabled = false; // Disable legend
                chart1.ChartAreas.Add(new ChartArea());
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false; // Disable Y-axis grid lines
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false; // Disable Y-axis grid lines
                chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
                chart1.ChartAreas[0].ShadowColor = Color.FromArgb(0, 62, 124);
                chart1.ChartAreas[0].BackColor = Color.FromArgb(0, 62, 124);
                chart1.ChartAreas[0].AxisY.Minimum = temperatures.Values.Max() / 2;
                chart1.ChartAreas[0].AxisY.Interval = temperatures.Values.Max(); // Set interval to a large value or disable it

                chart1.ChartAreas[0].AxisY.IsLabelAutoFit = false;
                chart1.ChartAreas[0].AxisY.LabelStyle.Enabled = false;

                foreach (var entry in temperatures)
                {
                    chart1.Series["Temperature"].Points.AddXY(entry.Key.ToString("hh:mm tt"), entry.Value);
                }

                // Customize axis labels
                chart1.ChartAreas[0].AxisX.Title = "Time";
                chart1.ChartAreas[0].AxisY.Title = "Temperature (°C)";


                // Create a MemoryStream to hold the chart image
                MemoryStream ms = new MemoryStream();
                chart1.SaveImage(ms, ChartImageFormat.Png);

                // Create a Bitmap from the MemoryStream
                Bitmap lineChartBitmap = new Bitmap(ms);

                return lineChartBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void designCSS(Control panel)
        {
            try
            {
                // Set the border radius for panel1
                int cornerRadius = 25; // You can adjust this value to change the roundness of the corners
                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90); // Top-left corner
                path.AddArc(panel.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90); // Top-right corner
                path.AddArc(panel.Width - cornerRadius, panel.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90); // Bottom-right corner
                path.AddArc(0, panel.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90); // Bottom-left corner
                path.CloseFigure();
                Color backColor = panel.BackColor;
                Color transparentColor = Color.FromArgb((int)(255 * 0.8755), 0, 62, 124);
                panel.BackColor = transparentColor;
                panel.ForeColor = Color.LightGray; // Border color
                panel.Region = new Region(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Summary_Click(object sender, EventArgs e)
        {

        }
    }
}

