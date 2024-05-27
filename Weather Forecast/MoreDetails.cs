using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;


namespace Weather_Forecast
{
    public partial class MoreDetails : UserControl
    {
        public string Umbrellatext;
        public string outdoorstext;
        public string clothingtext;
        public string drivingtext;

        public MoreDetails()
        {
            InitializeComponent();
            this.Width = WeatherForecaster.Width;
        }
        static Bitmap GenerateWeeklyDonutChart(int sunnyCloudyClearCount, int rainySnowyCount, string title)
        {
            
            try
            {
                Chart chart1 = new Chart();
                chart1.Width = 300;
                chart1.Height = 300;
                chart1.BackColor = Color.Transparent;

                // Set up chart properties
                chart1.Series.Add(new Series("s2"));
                chart1.Series["s2"].ChartType = SeriesChartType.Doughnut;
                chart1.Series["s2"].IsValueShownAsLabel = true;

                // Add data to the chart
                chart1.Series["s2"].Points.AddXY("Sunny, Cloudy, Clear", sunnyCloudyClearCount);
                chart1.Series["s2"].Points.AddXY("Rainy, Snowy", rainySnowyCount);

                // Customize segment colors
                chart1.Series["s2"].Points[0].Color = Color.Orange;
                chart1.Series["s2"].Points[1].Color = Color.Blue;

                // Set up chart area properties
                ChartArea chartArea = new ChartArea();
                chartArea.BackColor = Color.Transparent;
                chartArea.Area3DStyle.Enable3D = true;
                chartArea.ShadowColor = Color.Gray;

                chart1.ChartAreas.Add(chartArea);

                // Disable the legend
                Legend legend = new Legend();
                legend.Enabled = false;
                chart1.Legends.Add(legend);

                // Create a title if needed
                Title chartTitle = new Title();
                chartTitle.Text = title;
                chartTitle.Font = new Font("Arial", 14, FontStyle.Bold);
                chartTitle.BackColor = Color.Transparent; // Set title background to transparent
                chart1.Titles.Add(chartTitle);

                // Create a MemoryStream to hold the chart image
                using (MemoryStream ms2 = new MemoryStream())
                {
                    chart1.SaveImage(ms2, ChartImageFormat.Png);

                    // Create a Bitmap from the MemoryStream
                    Bitmap pieChartBitmap = new Bitmap(ms2);

                    return pieChartBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private void MoreDetails_Load(object sender, EventArgs e)
        {
            Weeklydata(WeatherForecaster.locationname);
            sunsetsrises(WeatherForecaster.locationname);
            minmaxtemp(WeatherForecaster.locationname);
            Suggestionsforday(WeatherForecaster.locationname);
            GetMoonTimes(DateTime.Today.Date.ToString(), WeatherForecaster.locationname);
        }

        public void Weeklydata(string location)
        {
            try { 
            using (WebClient web = new WebClient())
            {
                string apikey = "329cec969cefc40090ac7b5d60221eaf";
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={apikey}";

                var json = web.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                WeatherParameters.root output = result;

                // Count the number of "sunny, cloudy, clear" days and "rainy, snowy" days
                int sunnyCloudyClearCount = 0;
                int rainySnowyCount = 0;

                foreach (var item in output.list)
                {
                    if (item.weather[0].main.Contains("Clouds") || item.weather[0].main.Contains("Clear") || item.weather[0].main.Contains("Sunny"))
                        sunnyCloudyClearCount++;
                    else if (item.weather[0].main.Contains("Rain") || item.weather[0].main.Contains("Snow"))
                        rainySnowyCount++;
                }

                // Generate donut chart based on the counts
                panel1.BackgroundImage = GenerateWeeklyDonutChart(sunnyCloudyClearCount, rainySnowyCount, "Weekly Weather Forecast");
                    panel1.BackgroundImageLayout = ImageLayout.Zoom;
                    label20.Text = sunnyCloudyClearCount.ToString();
                label21.Text = rainySnowyCount.ToString();
                label40.Text = "\u25B2";
                label41.Text = "\u25BC";


            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving Weekly Weather details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void minmaxtemp(string location)
        {
            try { 
            using (WebClient web = new WebClient())
            {
                string apikey = "329cec969cefc40090ac7b5d60221eaf";
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={apikey}";
                var json = web.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                WeatherParameters.root output = result;
                int AvgmaxTemp = 0;
                int AvgminTemp = 0;
                int count = 0;
                foreach (var item in output.list)
                {

                    if (item.dt_txt.Date == DateTime.Today.Date)
                    {
                        count++;
                        AvgmaxTemp += Convert.ToInt32(item.main.temp_max);
                        AvgminTemp += Convert.ToInt32(item.main.temp_min);
                    }
                    if (item.dt_txt.Date == DateTime.Today.Date)
                    {
                        if (item.dt_txt.Hour == 12)
                        {

                            label2.Text = "The High will be " + item.main.temp_max + "°C";

                        }
                        if (item.dt_txt.Hour == 21)
                        {
                            label3.Text = "The Low will be " + item.main.temp_min + "°C";


                        }

                        if (item.dt_txt.Hour == 12)
                        {

                            string message = string.Format("{0}", item.weather[0].description);
                            label12.Text = WeatherForecaster.GenerateWeatherMessage(message);
                            string icon = item.weather[0].icon;
                            string iconUrl = $"http://openweathermap.org/img/wn/{icon}.png";
                            byte[] imageData = web.DownloadData(iconUrl);
                            Image Imagedata;
                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                            {
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                pictureBox1.Image = System.Drawing.Image.FromStream(ms);

                            }

                        }
                        if (item.dt_txt.Hour == 21)
                        {
                            string message = string.Format("{0}", item.weather[0].description);
                            label13.Text = WeatherForecaster.GenerateWeatherMessage(message);
                            string icon = item.weather[0].icon;
                            string iconUrl = $"http://openweathermap.org/img/wn/{icon}.png";
                            byte[] imageData = web.DownloadData(iconUrl);
                            Image Imagedata;
                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                            {
                                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                                pictureBox2.Image = System.Drawing.Image.FromStream(ms);

                            }
                        }
                    }

                }
                if (count > 0)
                {

                    label22.Text = "The High will be " + AvgmaxTemp / count + "°C";
                    label23.Text = "The Low will be " + AvgminTemp / count + "°C";
                }
                else
                {
                    label2.Text = "--";
                    label3.Text = "--";
                }

            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void sunsetsrises(string location)
        {
            try { 
            using (WebClient web = new WebClient())
            {
                string apikey = "329cec969cefc40090ac7b5d60221eaf";
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={apikey}";
                var json = web.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                WeatherParameters.root output = result;
                double latitude = output.coord.lat;
                double longitude = output.coord.lon;

                int sunrises = output.sys.sunrise;
                int sunsets = output.sys.sunset;

                DateTime sunriseUtc = UnixTimeStampToDateTime(sunrises);
                DateTime sunsetUtc = UnixTimeStampToDateTime(sunsets);
                TimeSpan difference = sunsetUtc - sunriseUtc;

                label14.Text = sunriseUtc.ToString("h:mm tt"); // Display sunrise time with AM/PM indication
                label15.Text = sunsetUtc.ToString("h:mm tt"); // Display sunset time with AM/PM indication
                int totalHours = (int)difference.TotalHours;
                int totalMinutes = difference.Minutes;

                // Create a formatted string
                string diffText = $"{totalHours} hr {totalMinutes} min";

                // Set the label text
                label8.Text = diffText;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            try { 
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DateTime.Today;
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void label6_Paint_1(object sender, PaintEventArgs e)
        {
            // Clear any existing text in the label

            // Define the arc parameters
            float startAngle = 180; // Starting angle of the arc
            float sweepAngle = 180; // Sweep angle of the arc for half arc
            float padding = 10; // Padding from the label's edge
            float radius = Math.Min(220, 220) / 2 - padding; // Radius of the arc

            // Center of the label
            float centerX = 220 / 2f;
            float centerY = 220 / 2f;

            // Calculate the bounding rectangle for the arc
            float rectX = centerX - radius;
            float rectY = centerY - radius;
            float diameter = radius * 2;

            // Create a GraphicsPath and add an arc
            GraphicsPath arcPath = new GraphicsPath();
            arcPath.AddArc(rectX, rectY, diameter, diameter, startAngle, sweepAngle);

            // Fill the path with yellow color
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(Color.Transparent))
            {
                e.Graphics.FillPath(brush, arcPath);
            }

            // Optional: Draw the outline of the arc for better visibility
            using (Pen pen = new Pen(Color.Yellow, 4))
            {
                e.Graphics.DrawPath(pen, arcPath);
            }

        }
        public void Suggestionsforday(string location)
        {

            using (WebClient web = new WebClient())
            {
                string apikey = "329cec969cefc40090ac7b5d60221eaf";
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={apikey}";
                var json = web.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                WeatherParameters.root output = result;
                foreach (var item in output.list)
                {
                    if (item.dt_txt.Date == DateTime.Today.Date)
                    {
                        string weatherdescription = string.Format("{0}", item.weather[0].description);
                        double visiblity = item.visibility / 1000.0;
                        double temperature = item.main.temp;
                        double windspeed = item.wind.speed * 3.6;
                        double feelslike = item.main.feels_like;
                        double humidity = item.main.humidity;

                        Umbrellatext = UmbrellaMessage(weatherdescription);
                        outdoorstext = OutdoorsMessage(weatherdescription, temperature, feelslike, windspeed);
                        clothingtext = ClothingMessage(weatherdescription, temperature, humidity, windspeed);
                        drivingtext = DrivingConditionsMessage(visiblity, temperature, windspeed, weatherdescription); ;
                        label28.Text = Umbrellatext;
                        label29.Text = outdoorstext;
                        label30.Text = clothingtext;
                        label31.Text = drivingtext;

                    }
                }
            }
        }
        public static string UmbrellaMessage(string weatherDescription)
        {
            switch (weatherDescription.ToLower())
            {
                case "clear":
                case "clear sky":
                    return "No need";
                case "fog":
                case "snow":
                case "few clouds":
                    return "Likely no needed";
                case "scattered clouds":
                case "broken clouds":
                case "overcast clouds":
                case "mist":
                case "haze":
                    return "Likely needed";
                case "light rain":
                case "moderate rain":
                case "drizzle":
                    return "Need";
                case "thunderstorm with light rain":
                case "thunderstorm with rain":
                case "thunderstorm with heavy rain":
                case "heavy rain":
                case "thunderstorm":
                    return "Must";
                default:
                    return "uncertain";
            }
        }
        public static string DrivingConditionsMessage(double visibility, double temperature, double windSpeed, string weatherDescription)
        {
            // Define thresholds for each factor
            const double VisibilityPoorThreshold = 5.0; // km
            const double VisibilityFairThreshold = 10.0; // km
            const double VisibilityGoodThreshold = 15.0; // km
            const double VisibilityGreatThreshold = 20.0; // km
            const double TemperaturePoorThreshold = 45.0; // degrees Celsius
            const double TemperatureFairThreshold = 35.0; // degrees Celsius
            const double TemperatureGoodThreshold = 25.0; // degrees Celsius
            const double TemperatureGreatThreshold = 15.0; // degrees Celsius
            const double WindSpeedPoorThreshold = 30.0; // km/h
            const double WindSpeedFairThreshold = 20.0; // km/h
            const double WindSpeedGoodThreshold = 15.0; // km/h
            const double WindSpeedGreatThreshold = 10.0; // km/h

            // Determine the driving conditions category based on the factors
            switch (weatherDescription.ToLower())
            {
                case "clear":
                case "clear sky":
                    if (visibility > VisibilityGreatThreshold && temperature < TemperatureGreatThreshold && windSpeed < WindSpeedGreatThreshold)
                        return "Great";
                    else if (visibility > VisibilityGoodThreshold && temperature < TemperatureGoodThreshold && windSpeed < WindSpeedGoodThreshold)
                        return "Good";
                    else if (visibility > VisibilityFairThreshold && temperature < TemperatureFairThreshold && windSpeed < WindSpeedFairThreshold)
                        return "Fair";
                    else
                        return "Poor";
                case "fog":
                case "snow":
                case "few clouds":
                    if (visibility < VisibilityPoorThreshold || temperature > TemperaturePoorThreshold || windSpeed > WindSpeedPoorThreshold)
                        return "Very poor";
                    else if (visibility < VisibilityFairThreshold || temperature > TemperatureFairThreshold || windSpeed > WindSpeedFairThreshold)
                        return "Poor";
                    else if (visibility < VisibilityGoodThreshold || temperature > TemperatureGoodThreshold || windSpeed > WindSpeedGoodThreshold)
                        return "Fair";
                    else if (visibility < VisibilityGreatThreshold || temperature > TemperatureGreatThreshold || windSpeed > WindSpeedGreatThreshold)
                        return "Good";
                    else
                        return "Great";
                case "scattered clouds":
                case "broken clouds":
                case "overcast clouds":
                case "mist":
                case "haze":
                    if (temperature > TemperaturePoorThreshold || windSpeed > WindSpeedPoorThreshold)
                        return "Very poor";
                    else if (temperature > TemperatureFairThreshold || windSpeed > WindSpeedFairThreshold)
                        return "Poor";
                    else if (temperature > TemperatureGoodThreshold || windSpeed > WindSpeedGoodThreshold)
                        return "Fair";
                    else
                        return "Good";
                case "light rain":
                case "moderate rain":
                case "drizzle":
                    if (windSpeed > WindSpeedPoorThreshold)
                        return "Very poor";
                    else if (windSpeed > WindSpeedFairThreshold)
                        return "Poor";
                    else
                        return "Fair";
                case "thunderstorm with light rain":
                case "thunderstorm with rain":
                case "thunderstorm with heavy rain":
                case "heavy rain":
                case "thunderstorm":
                    return "Very poor";
                default:
                    return "Uncertain";
            }
        }

        public static string OutdoorsMessage(string weatherDescription, double temperature, double feelsLike, double windSpeed)
        {

            const double TemperatureGoodThreshold = 20.0; // degrees Celsius
            const double TemperatureFairThreshold = 15.0; // degrees Celsius
            const double TemperaturePoorThreshold = 10.0; // degrees Celsius

            const double FeelsLikeGreatThreshold = 25.0; // degrees Celsius
            const double FeelsLikeGoodThreshold = 20.0; // degrees Celsius
            const double FeelsLikeFairThreshold = 15.0; // degrees Celsius
            const double FeelsLikePoorThreshold = 10.0; // degrees Celsius

            const double WindSpeedGreatThreshold = 10.0; // km/h
            const double WindSpeedGoodThreshold = 15.0; // km/h
            const double WindSpeedFairThreshold = 20.0; // km/h
            const double WindSpeedPoorThreshold = 25.0; // km/h
            const double WindSpeedVeryPoorThreshold = 30.0; // km/h

            switch (weatherDescription.ToLower())
            {
                case "clear":
                case "clear sky":
                    if (temperature > TemperatureGoodThreshold && windSpeed < WindSpeedGoodThreshold)
                    {
                        if (feelsLike > FeelsLikeGoodThreshold && feelsLike < FeelsLikeGreatThreshold)
                            return "Great"; // Perfect weather for outdoor activities
                        else if (feelsLike > FeelsLikeFairThreshold && feelsLike < FeelsLikeGoodThreshold)
                            return "Good"; // Good weather for outdoor activities
                    }
                    else if ((temperature < TemperaturePoorThreshold || temperature > TemperatureGoodThreshold) || (feelsLike < FeelsLikePoorThreshold || feelsLike > FeelsLikeGreatThreshold))
                    {
                        return "Poor";
                    }
                    else if ((temperature < 5 || temperature > 40) ||
                              (feelsLike < 5 || feelsLike > 40))// Clear skies but temperature and feels-like temperature are not ideal
                    {
                        return "Very poor";
                    }
                    return "Fair"; // Clear skies but temperature or wind speed not ideal
                case "cloudy":
                case "partly cloudy":
                case "mostly cloudy":
                    if (temperature <= TemperatureFairThreshold || windSpeed >= WindSpeedFairThreshold)
                        return "Poor"; // Cloudy with poor temperature or wind speed
                    else if (temperature <= TemperaturePoorThreshold || windSpeed >= WindSpeedGoodThreshold)
                        return "Very poor"; // Cloudy with very poor temperature or wind speed
                    else if (temperature <= TemperatureGoodThreshold || windSpeed >= WindSpeedGreatThreshold)
                        return "Fair"; // Cloudy but still fair for outdoor activities
                    else if ((temperature < TemperaturePoorThreshold || temperature > TemperatureGoodThreshold) || (feelsLike < FeelsLikePoorThreshold || feelsLike > FeelsLikeGreatThreshold))
                    {
                        return "Poor";
                    }
                    else if ((temperature < 5 || temperature > 40) ||
                              (feelsLike < 5 || feelsLike > 40))
                    {
                        return "Very poor";
                    }
                    else
                        return "Good"; // Cloudy with good temperature and wind speed
                case "rain":
                case "light rain":
                case "moderate rain":
                case "drizzle":
                    if (windSpeed >= WindSpeedPoorThreshold)
                        return "Very poor"; // Rainy weather with poor wind speed
                    else if (temperature <= TemperaturePoorThreshold || feelsLike <= FeelsLikePoorThreshold)
                        return "Poor"; // Rainy weather with poor temperature or feels-like temperature
                    else
                        return "Fair"; // Rainy but still fair for outdoor activities
                case "thunderstorm":
                case "thunderstorm with rain":
                case "thunderstorm with heavy rain":
                    return "Very poor"; // Thunderstorms, unsafe for outdoor activities
                case "snow":
                    if (windSpeed >= WindSpeedGoodThreshold)
                        return "Poor"; // Snowing with poor wind speed
                    else
                        return "Good"; // Snowing, suitable for winter activities
                case "fog":
                    if (temperature <= TemperaturePoorThreshold || feelsLike <= FeelsLikePoorThreshold || windSpeed >= WindSpeedVeryPoorThreshold)
                        return "Very poor"; // Foggy weather with poor temperature, feels-like temperature, or wind speed
                    else
                        return "Poor"; // Foggy, visibility might be reduced
                default:
                    return "Uncertain"; // Unknown weather condition
            }

        }
        public static string ClothingMessage(string weatherDescription, double temperature, double humidity, double windSpeed)
        {

            const double TemperatureGoodThreshold = 20.0; // degrees Celsius
            const double TemperatureFairThreshold = 15.0; // degrees Celsius
            const double TemperaturePoorThreshold = 10.0; // degrees Celsius

            const double HumidityHighThreshold = 70.0; // percentage
            const double HumidityModerateThreshold = 50.0; // percentage
            const double HumidityLowThreshold = 30.0; // percentage

            const double WindSpeedGreatThreshold = 10.0; // km/h
            const double WindSpeedGoodThreshold = 15.0; // km/h
            const double WindSpeedFairThreshold = 20.0; // km/h
            const double WindSpeedPoorThreshold = 25.0; // km/h
            const double WindSpeedVeryPoorThreshold = 30.0; // km/h

            switch (weatherDescription.ToLower())
            {
                case "clear":
                case "clear sky":
                    if (temperature > TemperatureGoodThreshold && windSpeed < WindSpeedGoodThreshold)
                    {
                        if (humidity < HumidityLowThreshold)
                            return "Light jacket"; // Low humidity, clear skies, and light wind call for a light jacket
                        else if (humidity >= HumidityLowThreshold && humidity < HumidityModerateThreshold)
                            return "Long sleeves"; // Moderate humidity, clear skies, and light wind call for long sleeves
                        else
                            return "T-shirt"; // High humidity, clear skies, and light wind call for a T-shirt
                    }
                    else if ((temperature < TemperaturePoorThreshold || temperature > TemperatureGoodThreshold) ||
                             humidity >= HumidityHighThreshold)
                    {
                        return "Shorts"; // Temperature not ideal for lighter clothing or high humidity
                    }
                    return "Long sleeves"; // Clear skies but wind speed not ideal for lighter clothing
                case "cloudy":
                case "partly cloudy":
                case "mostly cloudy":
                    if ((temperature <= TemperatureFairThreshold || windSpeed >= WindSpeedFairThreshold) ||
                        (temperature < TemperaturePoorThreshold || temperature > TemperatureGoodThreshold) ||
                        humidity >= HumidityHighThreshold || temperature <= TemperatureGoodThreshold || windSpeed >= WindSpeedGreatThreshold)
                    {
                        return "Long sleeves"; // Cloudy with poor temperature or wind speed, or high humidity
                    }
                    else
                        return "Light jacket"; // Cloudy with good temperature and wind speed
                case "rain":
                case "light rain":
                case "moderate rain":
                case "drizzle":
                    if (windSpeed >= WindSpeedPoorThreshold)
                        return "Long sleeves"; // Rainy weather with poor wind speed
                    else if (temperature < TemperaturePoorThreshold || humidity >= HumidityModerateThreshold)
                        return "Light jacket"; // Rainy weather with poor temperature or high humidity
                    else
                        return "Long sleeves"; // Rainy but still fair for breathable clothing
                case "thunderstorm":
                case "thunderstorm with rain":
                case "thunderstorm with heavy rain":
                    return "Heavy coat"; // Thunderstorms, necessitating heavy coat
                case "snow":
                    if (windSpeed >= WindSpeedGoodThreshold)
                        return "Heavy coat"; // Snowing with poor wind speed
                    else
                        return "Long sleeves"; // Snowing, suitable for long sleeves
                case "fog":
                    if (temperature < TemperaturePoorThreshold || humidity >= HumidityModerateThreshold || windSpeed >= WindSpeedVeryPoorThreshold)
                        return "Heavy coat"; // Foggy weather with poor temperature, high humidity, or poor wind speed
                    else
                        return "Long sleeves"; // Foggy but still fair for breathable clothing
                default:
                    return "Uncertain"; // Unknown weather condition
            }

        }


        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = Math.Min(label7.Width, label7.Height) / 2; // Radius of the circle
            int centerX = label7.Width / 2; // X coordinate of the center
            int centerY = label7.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath and add an arc representing the first half
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 180, 180);

            // Create a GraphicsPath and add an arc representing the second half
            GraphicsPath arcPath2 = new GraphicsPath();
            arcPath2.AddArc(rect, 0, 180);

            // Fill the first half with one color
            using (Brush brush1 = new SolidBrush(Color.FromArgb(128, 254, 200, 10)))
            {
                g.FillPath(brush1, arcPath1);
            }

            // Fill the second half with another color
            using (Brush brush2 = new SolidBrush(Color.FromArgb(128, 128, 128, 128)))
            {
                g.FillPath(brush2, arcPath2);
            }

            // Optional: Draw the outline of the circle for better visibility
            using (Pen pen = new Pen(Color.Transparent, 3))
            {
                g.DrawEllipse(pen, rect);
            }


        }

        private void label9_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = Math.Min(label9.Width, label9.Height) / 2; // Radius of the circle
            int centerX = label9.Width / 2; // X coordinate of the center
            int centerY = label9.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath and add an arc representing the first half
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 180, 180);

            // Create a GraphicsPath and add an arc representing the second half
            GraphicsPath arcPath2 = new GraphicsPath();
            arcPath2.AddArc(rect, 0, 180);

            // Fill the first half with one color
            using (Brush brush1 = new SolidBrush(Color.FromArgb(128, 128, 128, 128)))
            {
                g.FillPath(brush1, arcPath1);
            }

            // Fill the second half with another color
            using (Brush brush2 = new SolidBrush(Color.FromArgb(128, 254, 200, 10)))
            {
                g.FillPath(brush2, arcPath2);
            }

            // Optional: Draw the outline of the circle for better visibility
            using (Pen pen = new Pen(Color.Transparent, 3))
            {
                g.DrawEllipse(pen, rect);
            }

        }

        private void label38_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = Math.Min(label38.Width, label38.Height) / 3; // Radius of the circle
            int centerX = label38.Width / 2; // X coordinate of the center
            int centerY = label38.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 0, 360);
            // Fill the first half with one color
            using (Brush brush1 = new SolidBrush(Color.Orange))
            {
                g.FillPath(brush1, arcPath1);
            }
        }

        private void label39_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = Math.Min(label39.Width, label39.Height) / 3; // Radius of the circle
            int centerX = label39.Width / 2; // X coordinate of the center
            int centerY = label39.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 0, 360);
            // Fill the first half with one color
            using (Brush brush1 = new SolidBrush(Color.DeepSkyBlue))
            {
                g.FillPath(brush1, arcPath1);
            }

        }
        private async Task GetMoonTimes(string date, string location)
        {
            string apiKey = "7dec88a780f64b06959170935242605"; // Replace with your WeatherAPI key
            string baseUrl = "http://api.weatherapi.com/v1/astronomy.json";
            string url = $"{baseUrl}?key={apiKey}&q={location}&dt={date}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseBody);

                    string moonrise = json["astronomy"]["astro"]["moonrise"].ToString();
                    string moonset = json["astronomy"]["astro"]["moonset"].ToString();

                    // Update the labels on the form
                    label44.Text = moonrise;
                    label45.Text = moonset;
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show("Request error: " + e.Message);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            }
        }




        private void label34_Paint(object sender, PaintEventArgs e)
        {
            if (label34 == null || string.IsNullOrEmpty(Umbrellatext))
            {
                return;
            }
            Graphics g = e.Graphics;
            int radius = Math.Min(label34.Width, label34.Height) / 3; // Radius of the circle
            int centerX = label34.Width / 2; // X coordinate of the center
            int centerY = label34.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 0, 360);
            switch (Umbrellatext.ToLower())
            {
                case "no need":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "likely no need":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "likely needed":
                    using (Brush brush1 = new SolidBrush(Color.Yellow))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "need":
                    using (Brush brush1 = new SolidBrush(Color.Orange))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "must":
                    using (Brush brush1 = new SolidBrush(Color.Red))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                default:
                    using (Brush brush1 = new SolidBrush(Color.FromArgb(0, 62, 124)))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
            }

        }

        private void label35_Paint(object sender, PaintEventArgs e)
        {
            if (label35 == null || string.IsNullOrEmpty(outdoorstext))
            {
                return;
            }
            Graphics g = e.Graphics;
            int radius = Math.Min(label35.Width, label35.Height) / 3; // Radius of the circle
            int centerX = label35.Width / 2; // X coordinate of the center
            int centerY = label35.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 0, 360);
            switch (outdoorstext.ToLower())
            {
                case "great":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "good":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "fair":
                    using (Brush brush1 = new SolidBrush(Color.Yellow))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "poor":
                    using (Brush brush1 = new SolidBrush(Color.Orange))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "very poor":
                    using (Brush brush1 = new SolidBrush(Color.Red))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                default:
                    using (Brush brush1 = new SolidBrush(Color.FromArgb(0, 62, 124)))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;


            }

        }

        private void label36_Paint(object sender, PaintEventArgs e)
        {

            if (label36 == null || string.IsNullOrEmpty(clothingtext))
            {
                return;
            }
            Graphics g = e.Graphics;
            int radius = Math.Min(label36.Width, label36.Height) / 3; // Radius of the circle
            int centerX = label36.Width / 2; // X coordinate of the center
            int centerY = label36.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 0, 360);
            switch (clothingtext.ToLower())
            {
                case "long sleeves":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "breathable clothing":
                    using (Brush brush1 = new SolidBrush(Color.Yellow))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "shorts":
                    using (Brush brush1 = new SolidBrush(Color.Red))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "heavy coat":
                    using (Brush brush1 = new SolidBrush(Color.Red))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "light jacket":
                    using (Brush brush1 = new SolidBrush(Color.Lavender))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "t-shirt":
                    using (Brush brush1 = new SolidBrush(Color.Lavender))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                default:
                    using (Brush brush1 = new SolidBrush(Color.FromArgb(0, 62, 124)))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;


            }

        }

        private void label37_Paint(object sender, PaintEventArgs e)
        {

            if (label35 == null || string.IsNullOrEmpty(drivingtext))
            {
                return;
            }
            Graphics g = e.Graphics;
            int radius = Math.Min(label37.Width, label37.Height) / 3; // Radius of the circle
            int centerX = label37.Width / 2; // X coordinate of the center
            int centerY = label37.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 0, 360);
            switch (drivingtext.ToLower())
            {
                case "no need":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "likely no need":
                    using (Brush brush1 = new SolidBrush(Color.Green))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "likely needed":
                    using (Brush brush1 = new SolidBrush(Color.Yellow))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "need":
                    using (Brush brush1 = new SolidBrush(Color.Orange))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                case "must":
                    using (Brush brush1 = new SolidBrush(Color.Red))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;
                default:
                    using (Brush brush1 = new SolidBrush(Color.FromArgb(0, 62, 124)))
                    {
                        g.FillPath(brush1, arcPath1);

                    }
                    break;


            }

        }

        private void label40_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label47_Click(object sender, EventArgs e)
        {

        }

        private void label47_Paint(object sender, PaintEventArgs e)
        {
            // Define the arc parameters
            float startAngle = 180; // Starting angle of the arc
            float sweepAngle = 360; // Sweep angle of the arc for half arc
            float padding = 10; // Padding from the label's edge
            float radius = Math.Min(220, 220) / 2 - padding; // Radius of the arc

            // Center of the label
            float centerX = 220 / 2f;
            float centerY = 220 / 2f;

            // Calculate the bounding rectangle for the arc
            float rectX = centerX - radius;
            float rectY = centerY - radius;
            float diameter = radius * 2;

            // Create a GraphicsPath and add an arc
            GraphicsPath arcPath = new GraphicsPath();
            arcPath.AddArc(rectX, rectY, diameter, diameter, startAngle, sweepAngle);

            // Fill the path with yellow color
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(Color.Transparent))
            {
                e.Graphics.FillPath(brush, arcPath);
            }

            // Optional: Draw the outline of the arc for better visibility
            using (Pen pen = new Pen(Color.White, 4))
            {
                e.Graphics.DrawPath(pen, arcPath);
            }

        }

        private void label48_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = Math.Min(label9.Width, label9.Height) / 2; // Radius of the circle
            int centerX = label48.Width / 2; // X coordinate of the center
            int centerY = label48.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath and add an arc representing the first half
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 180, 180);

            // Create a GraphicsPath and add an arc representing the second half
            GraphicsPath arcPath2 = new GraphicsPath();
            arcPath2.AddArc(rect, 0, 180);

            // Fill the first half with one color
            using (Brush brush1 = new SolidBrush(Color.WhiteSmoke))
            {
                g.FillPath(brush1, arcPath1);
            }

            // Fill the second half with another color
            using (Brush brush2 = new SolidBrush(Color.Gray))
            {
                g.FillPath(brush2, arcPath2);
            }

            // Optional: Draw the outline of the circle for better visibility
            using (Pen pen = new Pen(Color.Transparent, 3))
            {
                g.DrawEllipse(pen, rect);
            }
        }

        private void label49_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = Math.Min(label49.Width, label49.Height) / 2; // Radius of the circle
            int centerX = label9.Width / 2; // X coordinate of the center
            int centerY = label9.Height / 2; // Y coordinate of the center

            // Define the bounding rectangle for the arc
            RectangleF rect = new RectangleF(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Create a GraphicsPath and add an arc representing the first half
            GraphicsPath arcPath1 = new GraphicsPath();
            arcPath1.AddArc(rect, 180, 180);

            // Create a GraphicsPath and add an arc representing the second half
            GraphicsPath arcPath2 = new GraphicsPath();
            arcPath2.AddArc(rect, 0, 180);

            // Fill the first half with one color
            using (Brush brush1 = new SolidBrush(Color.Gray))
            {
                g.FillPath(brush1, arcPath1);
            }

            // Fill the second half with another color
            using (Brush brush2 = new SolidBrush(Color.WhiteSmoke))
            {
                g.FillPath(brush2, arcPath2);
            }

            // Optional: Draw the outline of the circle for better visibility
            using (Pen pen = new Pen(Color.Transparent, 3))
            {
                g.DrawEllipse(pen, rect);
            }
        }

        private void label49_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            
        }
    }
}
