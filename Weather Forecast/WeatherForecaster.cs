using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.Projections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Entity.Infrastructure;
using FontAwesome.Sharp;
using static GMap.NET.MapProviders.StrucRoads.SnappedPoint;
using System.Drawing.Imaging;

namespace Weather_Forecast
{
    public partial class WeatherForecaster : Form
    {

        private List<Location> recentLocations = new List<Location>();
        public int temperature;
        public string Iconlabel;
        public static int Width;
        public WebBrowser webBrowser1;
        public static string locationname;
        private bool userSelection = false; // Flag to track user selection
        private GMapControl gMapControl;

        public WeatherForecaster()
        {

            InitializeComponent();
            locationname = "Hyderabad";
            label12.Text = locationname;
            comboBox2.Text = locationname;
            listView2.Visible = false;
            label20.Visible = false;
            panel2.Scroll += panel2_Scroll;
            listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;

        }
        private void panel2_Scroll(object sender, ScrollEventArgs e)
        {
            panel2.Invalidate(); // Redraw the panel to update the scroll bar appearance
        }

        private void UpdateComboBox()
        {
            try
            {
                // Clear existing items 
                comboBox2.DataSource = null;
                comboBox2.Items.Clear();
                //Add recent locations to the ComboBox
                foreach (var location in recentLocations)
                {

                    comboBox2.Items.Add(location.LocationName);
                    if (location.IsPrimary)
                    {
                        comboBox2.Text = location.LocationName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured updatecombo");
                MessageBox.Show("A error while updating comboBox" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DeleteLocation(int id)
        {
            try
            {
                string fileName = "data.json";
                List<Location> locations = new List<Location>();

                if (System.IO.File.Exists(fileName))
                {
                    string existingJson = System.IO.File.ReadAllText(fileName);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        try
                        {
                            locations = JsonConvert.DeserializeObject<List<Location>>(existingJson);
                        }
                        catch (JsonException jsonEx)
                        {
                            // Log JSON error and show message
                            Console.WriteLine("JSON deserialization error: " + jsonEx.Message);
                            MessageBox.Show("An error occurred while reading the locations data. Please check the data file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                var locationToRemove = locations.FirstOrDefault(loc => loc.LocationID == id);
                if (locationToRemove != null)
                {
                    if (locations.Any(loc => !loc.IsPrimary))
                    {
                        locations.Remove(locationToRemove);

                        string jsonData = JsonConvert.SerializeObject(locations, Formatting.Indented);
                        try
                        {
                            System.IO.File.WriteAllText(fileName, jsonData);
                        }
                        catch (IOException ioEx)
                        {
                            // Log file write error and show message
                            Console.WriteLine("File write error: " + ioEx.Message);
                            MessageBox.Show("An error occurred while writing to the file: " + ioEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        //  RefreshRecentLocationsPanel();
                        UpdateComboBox();
                    }
                    else
                    {
                        MessageBox.Show("You cannot delete a primary location", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error details for troubleshooting
                Console.WriteLine("An error occurred in DeleteLocation: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the location: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //public void RefreshRecentLocationsPanel()
        //{
        //    try
        //    {
        //        panel6.Controls.Clear();
        //        List<Location> locations = readRecentLocations();
        //        int recentLocationHeight = panel6.Height;
        //        int recentLocationWidth = 400;
        //        int x = 0;
        //        int y = 0;

        //        foreach (var location in locations)
        //        {
        //            RecentLocations recentLocationTab = new RecentLocations();
        //            recentLocationTab.TabInfo(location.LocationID, location.LocationName, null, 18);
        //            recentLocationTab.Location = new Point(x, y);
        //            recentLocationTab.Size = new Size(recentLocationWidth, recentLocationHeight);
        //            recentLocationTab.BorderStyle = BorderStyle.FixedSingle;
        //            panel6.Controls.Add(recentLocationTab);

        //            x += recentLocationWidth;

        //            if (x + recentLocationWidth > panel6.Width)
        //            {
        //                panel5.AutoScrollPosition = new Point(panel6.HorizontalScroll.Value + recentLocationWidth, panel6.VerticalScroll.Value);
        //                x = 0;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error occured refreshrecent" + ex.Message);
        //        MessageBox.Show("An error occurred while Refershing Locations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //    }
        //}
        private void WeatherForecaster_Load(object sender, EventArgs e)
        {
            try
            {
         
                locationname = "Hyderabad";
                recentLocations = readRecentLocations();
                listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;
                foreach (Location primary in recentLocations)
                {
                    if (primary.IsPrimary == true)
                    {
                        locationname = primary.LocationName;
                    }
                }

                // RefreshRecentLocationsPanel();
                // Bind the ComboBox to the list of locations
                foreach (var item in recentLocations)
                {
                    comboBox2.Items.Add(item.LocationName);
                }
                designCSS(panel1);
                designCSS(panel2);
                designCSS(panel3);
                Dailyweather(locationname);
                WeeklyWeather(locationname);
                LoadMap(locationname);
                LoadWeatherMap(locationname);
                RecentLocationsPanel();
                autosummary();
                comboBox2.Text = locationname;
            }
            catch
            {
                MessageBox.Show("Error occured at weather load");

            }


        }
        public void Dailyweather(string location)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={apikey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;
                    DateTime dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(output.dt).UtcDateTime;
                    DateTime localTime = dateTimeUtc.AddSeconds(output.timezone);
                    comboBox2.Text = textBox1.Text; //for displaying name in combobox
                    label2.Text = localTime.ToString("hh:mm tt");
                    temperature = Convert.ToInt32(output.main.temp);
                    // label3.Text = string.Format("{0:F2}", output.main.temp) + "°C";
                    label3.Text = Math.Round(output.main.temp).ToString() + "°C";
                    string message = string.Format("{0}", output.weather[0].description);
                    label4.Text = GenerateWeatherMessage(message, panel1);
                    label5.Text = string.Format("{0}", output.weather[0].main);
                    label13.Text = AirQuality(location);
                    double windspeed = output.wind.speed * 3.6;
                    WindCalculator windCalculator = new WindCalculator();
                    double windDirectionDegree = output.wind.deg;
                    string windDirection = WindCalculator.CalculateWindDirection(windDirectionDegree);
                    // label14.Text = windspeed.ToString("F2") + " " + windDirection;
                    label14.Text = Math.Round(windspeed).ToString() + " " + windDirection;
                    //  label15.Text = output.main.humidity.ToString("F2") + "%";
                    label15.Text = Math.Round(output.main.humidity).ToString() + "%";
                    double visibility = output.visibility / 1000.0;
                    // label16.Text = visibility.ToString("F2") + "Km";
                    label16.Text = Math.Round(visibility).ToString() + " Km";
                    // label17.Text = output.main.pressure.ToString("F2") + "mb";
                    label17.Text = Math.Round(output.main.pressure).ToString();
                    double Dewpoint = CalculateDewPoint(output.main.temp, output.main.humidity);
                    //label18.Text = Dewpoint.ToString("F2");
                    label18.Text = Math.Round(Dewpoint).ToString();
                    string icon = output.weather[0].icon;
                    Iconlabel = output.weather[0].icon;
                    string iconUrl = $"http://openweathermap.org/img/wn/{icon}.png";

                    try
                    {
                        // Download the icon image from the URL
                        byte[] imageData = web.DownloadData(iconUrl);

                        // Load the image into PictureBox
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                        {
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            pictureBox1.Image = System.Drawing.Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading weather icon: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading weather details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public string AirQuality(string location)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={apikey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;
                    string url2 = $"http://api.openweathermap.org/data/2.5/air_pollution/history?lat={output.coord.lat}&lon={output.coord.lon}&start=1606223802&end=1606482999&appid={apikey}";
                    json = web.DownloadString(url2);
                    result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    output = result;
                    string airQuality;
                    foreach (var item in output.list)
                    {

                        switch (item.main.aqi)
                        {
                            case 1:
                                airQuality = "Good";
                                break;
                            case 2:
                                airQuality = "Fair";
                                break;
                            case 3:
                                airQuality = "Moderate";
                                break;
                            case 4:
                                airQuality = "Poor";
                                break;
                            case 5:
                                airQuality = "Very Poor";
                                break;
                            default:
                                airQuality = "--"; // Handle unexpected values
                                break;
                        }
                        return airQuality;
                    }
                    return "Unknown";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while writing to the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Unknown";

            }

        }
        public void WeeklyWeather(string location)
        {
            try
            {
                panel2.Visible = true;
                panel2.Controls.Clear();
                int horzintaltabheight = 200;
                int horizontalTabwidth = 400;
                int horizontaltabspacing = 5;
                int verticalTabSpacing = 10;    // Spacing between rows

                panel2.AutoScroll = true;
                panel2.HorizontalScroll.Enabled = true;
                panel2.HorizontalScroll.Visible = true;
                panel2.VerticalScroll.Enabled = false;
                panel2.VerticalScroll.Visible = false;
                using (Graphics g = panel2.CreateGraphics())
                {
                    // Call the DrawHorizontalScrollBar method and pass the graphics object
                    DrawHorizontalScrollBar(g);
                }                // panel2.Width = horizontalTabwidth * 6;
                panel2.Width = horzintaltabheight * 9 + horizontaltabspacing * 2; // Adjust panel width based on number of columns

                // Define the horizontal and vertical spacing between tabs
                int horizontalSpacing = 10;
                int verticalSpacing = 10;

                // Define the number of columns for the grid layout
                int columns = 3;

                // Initialize variables to keep track of the current position
                int x = 0;
                int y = 0;
                double Maxtemp = 0;
                double Mintemp = 0;
                using (WebClient web = new WebClient())
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={apikey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;

                    HashSet<DateTime> uniqueDates = new HashSet<DateTime>();
                    foreach (var item in output.list)
                    {
                        DateTime currentDate = item.dt_txt;

                        if (item.dt_txt.Hour == 12)
                        {
                            Maxtemp = item.main.temp_max;
                        }
                        if (item.dt_txt.Hour == 21)
                        {
                            Mintemp = item.main.temp_min;
                        }
                        // Check if the current date is already in the HashSet
                        if (uniqueDates.Contains(currentDate.Date) || currentDate.Hour != 9)
                        {
                            // If the current date is already in the HashSet, skip this iteration
                            continue;
                        }

                        // Add the current date to the HashSet to mark it as seen
                        uniqueDates.Add(currentDate.Date);

                        string message = string.Format("{0}", item.weather[0].description);
                        double Dewpoint = CalculateDewPoint(item.main.temp, item.main.humidity);
                        string icon = item.weather[0].icon;
                        string iconUrl = $"http://openweathermap.org/img/wn/{icon}.png";
                        byte[] imageData = web.DownloadData(iconUrl);
                        Image Imagedata;
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                        {
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            Imagedata = System.Drawing.Image.FromStream(ms);
                        }

                        // Create a new instance of HorizontalTab for each day and pass the data
                        // Create a new instance of HorizontalTab for each unique date and pass the data
                        HorizontalTab horizontalTab = new HorizontalTab();
                        horizontalTab.Location = new Point(x, y);
                        horizontalTab.Size = new Size(horizontalTabwidth, horzintaltabheight);
                        horizontalTab.BorderStyle = BorderStyle.Fixed3D; // Set border style to Fixed3D for 3D appearance
                        horizontalTab.BackColor = Color.Transparent;
                        horizontalTab.TabInfo(item.dt_txt, Convert.ToInt32(Mintemp), Convert.ToInt32(Maxtemp), GenerateWeatherMessage(message, panel1), Convert.ToInt32(Dewpoint), Imagedata);
                        panel2.Controls.Add(horizontalTab);
                        foreach (Control control in panel2.Controls)
                        {
                            if (control is HorizontalTab tabControl)
                            {
                                // Subscribe to the Click event of each horizontal tab
                                tabControl.Click += horizontalTab.HorizontalTab_Click;
                            }
                        }
                        // Update the current position for the next tab
                        // Update x position for the next tab
                        x += horizontalTabwidth + horizontalSpacing;

                        // Check if the next tab exceeds the panel's width
                        /*  if (x + horizontalTabwidth > panel2.Width)
                          {
                              // Adjust the panel's AutoScrollPosition to show horizontal scrollbar
                              panel2.AutoScrollPosition = new Point(panel2.HorizontalScroll.Value + horizontalTabwidth, panel2.VerticalScroll.Value);

                              // Reset x to 0 to start a new row
                              x = 0;
                              // Keep y unchanged to keep adding tabs on the same row
                              y += horzintaltabheight + verticalTabSpacing;

                          }*/

                    }

                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        public double CalculateDewPoint(double temperatureCelsius, double humidityPercentage)
        {
            double dewPoint = temperatureCelsius - ((100 - humidityPercentage) / 5);
            return dewPoint;
        }
        public static string GenerateWeatherMessage(string weatherDescription, Panel panel)
        {
            try
            {

                switch (weatherDescription.ToLower())
                {
                    //  case "clear":
                    case "clear sky":
                        panel.BackgroundImage = Properties.Resources.ClearSky;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "The skies are clear today.";
                    case "few clouds":
                        panel.BackgroundImage = Properties.Resources.FewClouds;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "There are a few clouds in the sky.";

                    case "scattered clouds":
                        panel.BackgroundImage = Properties.Resources.ScatteredClouds;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Expect scattered clouds today.";

                    case "broken clouds":
                        panel.BackgroundImage = Properties.Resources.BrokenClouds;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "The sky is partly cloudy today.";

                    case "overcast clouds":
                        panel.BackgroundImage = Properties.Resources.OvercastClouds;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "The sky is overcast with clouds.";

                    case "light rain":
                        panel.BackgroundImage = Properties.Resources.LightRain;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Expect light rain showers today.";

                    case "moderate rain":
                        panel.BackgroundImage = Properties.Resources.ModerateRain;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Expect moderate rain showers today.";

                    case "heavy rain":
                        panel.BackgroundImage = Properties.Resources.HeavyRain;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Be prepared for heavy rain today.";

                    case "drizzle":
                        panel.BackgroundImage = Properties.Resources.Drizzle;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "There's drizzle in the forecast today.";

                    case "thunderstorm with light rain":
                        panel.BackgroundImage = Properties.Resources._default;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "There's drizzle in the forecast today.";


                    case "thunderstorm with rain":
                        panel.BackgroundImage = Properties.Resources._default;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Expect thunderstorms with light rain today.";

                    case "thunderstorm with heavy rain":
                        panel.BackgroundImage = Properties.Resources._default;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Expect thunderstorms with heavy rain today.";

                    case "thunderstorm":
                        panel.BackgroundImage = Properties.Resources._default;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Be cautious, there might be thunderstorms.";

                    case "snow":
                        panel.BackgroundImage = Properties.Resources._default;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Prepare for snowfall today.";

                    case "mist":
                        panel.BackgroundImage = Properties.Resources.Mist;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "There's mist in the air today.";

                    case "fog":
                        panel.BackgroundImage = Properties.Resources.Fog;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Be cautious, visibility might be low due to fog.";
                    case "haze":
                        panel.BackgroundImage = Properties.Resources.Haze;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Be cautious, there might be haze in the air today.";

                    default:
                        panel.BackgroundImage = Properties.Resources._default;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        return "Weather conditions are uncertain.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating weather details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Weather conditions are uncertain.";

            }
        }
        public static string GenerateWeatherMessage(string weatherDescription)
        {
            try
            {

                switch (weatherDescription.ToLower())
                {
                    //  case "clear":
                    case "clear sky":
                        return "The skies are clear today.";
                    case "few clouds":
                        return "There are a few clouds in the sky.";

                    case "scattered clouds":
                        return "Expect scattered clouds today.";

                    case "broken clouds":
                        return "The sky is partly cloudy today.";

                    case "overcast clouds":
                        return "The sky is overcast with clouds.";

                    case "light rain":
                        return "Expect light rain showers today.";

                    case "moderate rain":
                        return "Expect moderate rain showers today.";

                    case "heavy rain":
                        return "Be prepared for heavy rain today.";

                    case "drizzle":
                        return "There's drizzle in the forecast today.";

                    case "thunderstorm with light rain":
                        return "There's drizzle in the forecast today.";


                    case "thunderstorm with rain":
                        return "Expect thunderstorms with light rain today.";

                    case "thunderstorm with heavy rain":
                        return "Expect thunderstorms with heavy rain today.";

                    case "thunderstorm":
                        return "Be cautious, there might be thunderstorms.";

                    case "snow":
                        return "Prepare for snowfall today.";

                    case "mist":
                        return "There's mist in the air today.";

                    case "fog":
                        return "Be cautious, visibility might be low due to fog.";
                    case "haze":
                        return "Be cautious, there might be haze in the air today.";

                    default:
                        return "Weather conditions are uncertain.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating weather details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Weather conditions are uncertain.";

            }
        }
        public static List<string> SearchPlaces(string query)
        {
            try
            {
                string apiUrl = $"http://api.geonames.org/searchJSON?name_startsWith={query}&maxRows=10&username=avinash1547";
                using (WebClient webClient = new WebClient())
                {
                    string json = webClient.DownloadString(apiUrl);
                    RootObject data = JsonConvert.DeserializeObject<RootObject>(json);

                    // Filter out duplicate places based on name
                    HashSet<string> nameSet = new HashSet<string>();
                    List<string> uniquePlaces = new List<string>();
                    foreach (var place in data.geonames)
                    {
                        if (!nameSet.Contains(place.name))
                        {
                            nameSet.Add(place.name);
                            string displayName = FormatDisplayName(place);
                            uniquePlaces.Add(displayName);
                        }
                    }
                    return uniquePlaces;
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions here, for simplicity, return null
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        private static string FormatDisplayName(geonames place)
        {
            try
            {
                List<string> nameParts = new List<string>();
                if (!string.IsNullOrEmpty(place.name))
                {
                    nameParts.Add(place.name);
                }
                if (!string.IsNullOrEmpty(place.adminName1))
                {
                    nameParts.Add(place.adminName1);
                }
                if (!string.IsNullOrEmpty(place.countryName))
                {
                    nameParts.Add(place.countryName);
                }

                return string.Join(", ", nameParts);
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating weather details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "No results found";
            }
        }

        private int GenerateId(List<Location> locations) // for locationId autogenerate
        {
            return (locations.Count > 0) ? locations.Max(loc => loc.LocationID) + 1 : 1;
        }
        public void RecentLocations(string location, bool isPrimary)
        {
            try
            {
                string fileName = "data.json";
                List<Location> locations = new List<Location>();

                // Check if the file exists and read existing data
                if (System.IO.File.Exists(fileName))
                {
                    string existingJson = System.IO.File.ReadAllText(fileName);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        // Deserialize existing data
                        locations = JsonConvert.DeserializeObject<List<Location>>(existingJson);
                    }
                }
                // Add or update the primary location
                if (isPrimary)
                {
                    var primaryLocation = locations.FirstOrDefault(loc => loc.LocationName == location);
                    if (primaryLocation != null)
                    {
                        foreach (var loc in locations)
                        {
                            loc.IsPrimary = false;
                        }
                        primaryLocation.LocationName = location;
                        primaryLocation.IsPrimary = true;
                        // Move primaryLocation to the beginning of the list
                        locations.Remove(primaryLocation); // Remove from current position
                        locations.Insert(0, primaryLocation); // Insert at index 0

                    }
                    else if (!locations.Any(loc => loc.LocationName == location))
                    {
                        locations.Insert(0, new Location { LocationID = GenerateId(locations), LocationName = location, IsPrimary = true, temperature = temperature, iconlabel = Iconlabel });

                    }
                }
                else
                {
                    if (!locations.Any(loc => loc.LocationName == location))
                    {
                        locations.Add(new Location { LocationID = GenerateId(locations), LocationName = location, IsPrimary = false, temperature = temperature, iconlabel = Iconlabel });

                        var recentNonPrimaryLocations = locations.Where(loc => !loc.IsPrimary).TakeLast(5).ToList();

                        var primaryLocation = locations.FirstOrDefault(loc => loc.IsPrimary);
                        if (primaryLocation != null)
                        {
                            locations = new List<Location> { primaryLocation };
                            locations.AddRange(recentNonPrimaryLocations);
                        }
                        else
                        {
                            locations = recentNonPrimaryLocations;
                        }
                    }
                }
                // Serialize the updated list to JSON format
                string jsonData = JsonConvert.SerializeObject(locations, Formatting.Indented);

                // Write the JSON data to a file
                try
                {
                    System.IO.File.WriteAllText(fileName, jsonData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while writing to the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving recent locations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public List<Location> readRecentLocations()
        {
            try
            {
                string fileName = "data.json";
                List<Location> locations = new List<Location>();

                // Check if the file exists and read existing data
                if (System.IO.File.Exists(fileName))
                {
                    string existingJson = System.IO.File.ReadAllText(fileName);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        // Deserialize existing data
                        try
                        {
                            locations = JsonConvert.DeserializeObject<List<Location>>(existingJson);
                        }
                        catch (JsonException jsonEx)
                        {
                            Console.WriteLine("JSON deserialization error: " + jsonEx.Message);
                            MessageBox.Show("An error occurred while reading the locations data. Please check the data file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                return locations;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving recent locations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public void RecentLocationsPanel()
        {
            try
            {
                panel6.Controls.Clear();
                List<Location> locations = readRecentLocations();
                int tabCount = 2;
                int Recentlocationheight = panel6.Height;
                int Recentlocationwidth = tabCount > 0 ? panel6.Width / tabCount : panel6.Width; // Dynamically calculate width based on the number of tabs

                int x = 0;
                int y = 0;

                foreach (var item in locations)
                {
                    // Create a new instance of RecentLocations for each location
                    RecentLocations recentLocations = new RecentLocations();
                    recentLocations.Location = new Point(x, y);
                    recentLocations.Size = new Size(Recentlocationwidth, Recentlocationheight);
                    recentLocations.BorderStyle = BorderStyle.FixedSingle; // Set border style to FixedSingle
                    recentLocations.TabInfo(item.LocationID, item.LocationName, item.iconlabel, item.temperature, item.IsPrimary);
                    panel6.Controls.Add(recentLocations);

                    // Update x position for the next tab
                    x += Recentlocationwidth;

                    // Check if the next tab exceeds the panel's width
                    if (x + Recentlocationwidth > panel6.Width)
                    {
                        // Move to the next row
                        x = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displayng recent locations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMap(string location)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string apiKey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={apiKey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;

                    // Initialize GMapControl
                    GMapControl gMapControl = new GMapControl
                    {
                        Dock = DockStyle.Fill,
                        MapProvider = GMapProviders.GoogleMap,
                        Position = new PointLatLng(output.coord.lat, output.coord.lon),
                        MinZoom = 1,
                        MaxZoom = 18,
                        Zoom = 10,
                    };
                    // Ensure the panel is initialized
                    if (panel5 == null)
                    {
                        panel5 = new Panel
                        {
                            Dock = DockStyle.Fill
                        };
                        this.Controls.Add(panel5);
                    }
                    panel5.Controls.Clear();
                    panel5.Controls.Add(gMapControl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading Map: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadWeatherMap(string location)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={apikey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;

                    //  gMapControl.Position = new PointLatLng(output.coord.lat, output.coord.lon);

                    // Add the marker for the current position
                    GMapOverlay markersOverlay = new GMapOverlay("markers");
                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(output.coord.lat, output.coord.lon), GMarkerGoogleType.red_pushpin);
                    markersOverlay.Markers.Add(marker);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading map: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void hourlydata(string location)
        {
            try
            {
                int horzintaltabheight = panel3.Height;
                int horizontalTabwidth = 200;
                int horizontaltabspacing = 5;
                // Define the horizontal and vertical spacing between tabs
                int horizontalSpacing = 5;
                int verticalSpacing = 0;
                // Initialize variables to keep track of the current position
                int x = 0;
                int y = 50;
                using (WebClient web = new WebClient())
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={apikey}";
                    var json = web.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WeatherParameters.root>(json);
                    WeatherParameters.root output = result;
                    foreach (var item in output.list)
                    {

                        string message = string.Format("{0}", item.weather[0].description);
                        double Dewpoint = CalculateDewPoint(item.main.temp, item.main.humidity);
                        double windspeed = item.wind.speed * 3.6;
                        WindCalculator windCalculator = new WindCalculator();
                        double windDirectionDegree = item.wind.deg;
                        string windDirection = WindCalculator.CalculateWindDirection(windDirectionDegree);
                        string icon = item.weather[0].icon;
                        string iconUrl = $"http://openweathermap.org/img/wn/{icon}.png";
                        byte[] imageData = web.DownloadData(iconUrl);
                        Image Imagedata;
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                        {
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            Imagedata = System.Drawing.Image.FromStream(ms);
                        }

                        // Create a new instance of HorizontalTab for each day and pass the data
                        // Create a new instance of HorizontalTab for each unique date and pass the data
                        Hourly hourly = new Hourly();
                        hourly.Location = new Point(x, y);
                        hourly.Size = new Size(horizontalTabwidth, horzintaltabheight);
                        hourly.BorderStyle = BorderStyle.FixedSingle; // Set border style to Fixed3D for 3D appearance
                        hourly.BackColor = Color.WhiteSmoke;
                        hourly.TabInfo(Imagedata, Convert.ToInt32(item.main.temp), message, windspeed.ToString("F2") + "" + windDirection, item.dt_txt);
                        panel3.Controls.Add(hourly);

                        // Update the current position for the next tab
                        // Update x position for the next tab
                        x += horizontalTabwidth;

                        // Check if the next tab exceeds the panel's width
                        if (x + horizontalTabwidth > panel3.Width)
                        {
                            // Adjust the panel's AutoScrollPosition to show horizontal scrollbar
                            panel3.AutoScrollPosition = new Point(panel2.HorizontalScroll.Value + horizontalTabwidth, panel3.VerticalScroll.Value);

                            // Reset x to 0 to start a new row
                            x = 0;
                            // Keep y unchanged to keep adding tabs on the same row
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        //private void DisplaySelectedLocationDetails()
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(locationname))
        //        {
        //            // Assuming Dailyweather, WeeklyWeather, LoadMap, LocationName and RecentLocations are defined methods
        //            Dailyweather(locationname);
        //            WeeklyWeather(locationname);
        //            LoadMap(locationname);
        //            RecentLocations(locationname, false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void UpdateListBoxItems()
        {
            try
            {
                string query = textBox1.Text.Trim(); // Trim to remove leading and trailing whitespaces
                if (string.IsNullOrEmpty(query))
                {
                    return;
                }
                List<string> displayNames = SearchPlaces(query);

                if (displayNames != null && displayNames.Count > 0)
                {
                    foreach (var item in displayNames)
                    {
                        listView2.Items.Clear();
                        listView2.Items.Add(item);
                    }
                    listView2.Visible = true;
                }
                else
                {
                    //listView1.Clear();
                    listView2.Items.Add("No results found");
                    listView2.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating listbox: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateListBoxItems();
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)//SearchButton
        {
            try
            {
                locationname = textBox1.Text.Trim();
                label12.Text = locationname;
                RecentLocations(locationname, false);
                listView2.Visible = false;
                refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {

        }


        private void textBox1_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {



        }

        private void WeatherForecaster_Resize(object sender, EventArgs e)
        {
            try
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    panel3.Width = this.ClientSize.Width;
                    panel2.Width = this.ClientSize.Width;
                    toolStrip1.Width = this.ClientSize.Width;
                    Width = this.ClientSize.Width;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

        }
        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {

        }

        private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
        {

        }
        public void autosummary()
        {
            try
            {
                var controlsToRemove = panel3.Controls.OfType<Control>()
                                          .Where(control => !(control is ToolStrip))
                                          .ToList();

                foreach (Control control in controlsToRemove)
                {
                    panel3.Controls.Remove(control);
                    control.Dispose(); // Dispose the removed control
                }
                Summary summary = new Summary();
                panel3.Controls.Add(summary);

            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying Summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void horizontaltab_summary(string location, DateTime date)
        {
            try
            {
                var controlsToRemove = panel3.Controls.OfType<Control>()
                                          .Where(control => !(control is ToolStrip))
                                          .ToList();
                Summary summary = new Summary();
                Control summaryPanel = summary.SummaryGraph(location, date);

                if (summaryPanel != null)
                {
                    panel3.Controls.Add(summaryPanel);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying Summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void toolStripButton1_Click_2(object sender, EventArgs e)
        {
            try
            {
                var controlsToRemove = panel3.Controls.OfType<Control>()
                                            .Where(control => !(control is ToolStrip))
                                            .ToList();

                foreach (Control control in controlsToRemove)
                {
                    panel3.Controls.Remove(control);
                    control.Dispose(); // Dispose the removed control
                }
                Summary summary = new Summary();
                summary.Dock = DockStyle.Fill;
                panel3.Controls.Add(summary);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying Summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try

            {
                var controlsToRemove = panel3.Controls.OfType<Control>()
                                           .Where(control => !(control is ToolStrip))
                                           .ToList();

                foreach (Control control in controlsToRemove)
                {
                    panel3.Controls.Remove(control);
                    control.Dispose(); // Dispose the removed control
                }

                hourlydata(locationname);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying Hourly: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                var controlsToRemove = panel3.Controls.OfType<Control>()
                                           .Where(control => !(control is ToolStrip))
                                           .ToList();

                foreach (Control control in controlsToRemove)
                {
                    panel3.Controls.Remove(control);
                    control.Dispose(); // Dispose the removed control
                }
                MoreDetails moreDetails = new MoreDetails();
                moreDetails.Dock = DockStyle.Fill;
                panel3.Controls.Add(moreDetails);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying More Details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void designCSS(Panel panel)
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
                toolStrip1.Region = new Region(path);
                IconChar iconChar = IconChar.Home;
                IconChar iconChar2 = IconChar.Refresh;
                int iconSize = 32; // Size of the icon in pixels
                                   // Convert the icon to an image
                IconPictureBox iconPictureBox = new IconPictureBox
                {
                    IconChar = iconChar,
                    IconSize = iconSize
                };
                Image iconImage = new Bitmap(iconPictureBox.IconSize, iconPictureBox.IconSize);
                using (Graphics g = Graphics.FromImage(iconImage))
                {
                    iconPictureBox.DrawToBitmap((Bitmap)iconImage, new Rectangle(Point.Empty, iconImage.Size));
                }
                IconPictureBox iconPictureBox2 = new IconPictureBox
                {
                    IconChar = iconChar2,
                    IconSize = iconSize
                };
                Image iconImage2 = new Bitmap(iconPictureBox2.IconSize, iconPictureBox2.IconSize);
                using (Graphics g = Graphics.FromImage(iconImage))
                {
                    iconPictureBox2.DrawToBitmap((Bitmap)iconImage2, new Rectangle(Point.Empty, iconImage2.Size));
                }
                button2.BackColor = Color.FromArgb(0, 62, 124);
                button2.BackgroundImage = iconImage;
                button2.BackgroundImageLayout = ImageLayout.Center;
                button2.Size = new Size(32, 32); // Set the button size equal to the icon size
                button2.FlatStyle = FlatStyle.Flat; // Optional: Makes the button flat for better appearance
                button2.FlatAppearance.BorderSize = 0; // Optional: Remove the button border
                button3.BackColor = Color.FromArgb(0, 62, 124);
                button3.BackgroundImage = iconImage2;
                button3.BackgroundImageLayout = ImageLayout.Center;
                button3.Size = new Size(32, 32); // Set the button size equal to the icon size
                button3.FlatStyle = FlatStyle.Flat; // Optional: Makes the button flat for better appearance
                button3.FlatAppearance.BorderSize = 0; // Optional: Remove the button border
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            DrawHorizontalScrollBar(e.Graphics);

        }
        private void DrawHorizontalScrollBar(Graphics g)
        {
            int scrollBarHeight = 10;
            int scrollBarWidth = panel2.Width;

            Color trackColor = Color.FromArgb(255, 0, 0); // Example: Red color
            Color thumbColor = Color.FromArgb(255, 255, 0); // Example: Red color

            using (SolidBrush trackBrush = new SolidBrush(trackColor))
            using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
            {
                // Calculate the size and position of the thumb
                int maximum = panel2.HorizontalScroll.Maximum + panel2.Width;
                int thumbWidth = Math.Max(10, (int)(((float)panel2.Width / maximum) * panel2.Width));
                int thumbX = (int)(((float)panel2.HorizontalScroll.Value / maximum) * (panel2.Width - thumbWidth));

                // Draw the track
                g.FillRectangle(trackBrush, 0, panel2.Height - scrollBarHeight, scrollBarWidth, scrollBarHeight);

                // Draw the thumb
                g.FillRectangle(thumbBrush, thumbX, panel2.Height - scrollBarHeight, thumbWidth, scrollBarHeight);
            }
            //// Calculate the size and position of the thumb
            //int maximum = panel2.HorizontalScroll.Maximum + panel2.Width;
            //int thumbWidth = Math.Max(10, (int)(((float)panel2.Width / maximum) * panel2.Width));
            //int thumbX = (int)(((float)panel2.HorizontalScroll.Value / maximum) * (panel2.Width - thumbWidth));

            //// Draw the track
            //g.FillRectangle(SystemBrushes.Desktop, 0, panel2.Height - scrollBarHeight, scrollBarWidth, scrollBarHeight);

            //// Draw the thumb
            //g.FillRectangle(SystemBrushes.InactiveBorder, thumbX, panel2.Height - scrollBarHeight, thumbWidth, scrollBarHeight);
        }
        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            userSelection = true; // Set the flag to true when the user interacts with the list box

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //DisplaySelectedLocationDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.SelectedIndex != -1)
                {
                    locationname = comboBox2.Text;
                    label12.Text = locationname;
                    // RecentLocations(locationname, false);

                    refresh();
                    // RefreshRecentLocationsPanel();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while selecting location: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {

            //UpdateComboBox();

        }

        private void button2_Click(object sender, EventArgs e)//primarybutton
        {
            try
            {
                string selectedLocation = comboBox2.Text;
                if (!string.IsNullOrWhiteSpace(selectedLocation))
                {
                    RecentLocations(selectedLocation, true);
                    locationname = selectedLocation;
                    DisplayPrimaryLocationMessage(selectedLocation);
                    button2.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while setting primary location: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DisplayPrimaryLocationMessage(string selectedLocation)
        {
            try
            {
                label20.Text = $"{selectedLocation} has been set as the primary location";
                label20.Visible = true;

                // Optionally, hide the message after a few seconds
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 3000; // 3 seconds
                timer.Tick += (s, args) =>
                {
                    label20.Visible = false;
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying message: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void refresh()
        {
            try
            {
                Dailyweather(locationname);
                WeeklyWeather(locationname);
                LoadMap(locationname);
                autosummary();
                comboBox2.Text = locationname;
                listView2.Visible = false;
                textBox1.Text = string.Empty;


            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while refershing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // RefreshRecentLocationsPanel();
            refresh();
        }


        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //listView2.Visible = true;
        }


        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                textBox1.Text = listView2.SelectedItems[0].Text;
            }
            else
            {
                textBox1.Clear(); // Clear the TextBox if no item is selected
            }
        }
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
    public class geonames
    {
        public string adminCode1 { get; set; }
        public string lng { get; set; }
        public int geonameId { get; set; }
        public string toponymName { get; set; }
        public string countryId { get; set; }
        public string fcl { get; set; }
        public string countryCode { get; set; }
        public string name { get; set; }
        public string fclName { get; set; }
        public AdminCodes1 adminCodes1 { get; set; }
        public string countryName { get; set; }
        public string fcodeName { get; set; }
        public string adminName1 { get; set; }
        public string lat { get; set; }
        public string fcode { get; set; }
    }
    public class AdminCodes1
    {
        public string ISO3166_2 { get; set; }
    }

    public class RootObject
    {
        public int totalResultsCount { get; set; }
        public List<geonames> geonames { get; set; }
    }
    public class WindCalculator
    {
        public static string CalculateWindDirection(double degree)
        {
            try
            {
                // Unicode arrow character representing the arrow
                char arrow = '➤';

                // Normalize the degree to the range [0, 360)
                degree %= 360;

                // Define the Unicode code points for different arrow characters
                int rightArrowCodePoint = 0x27A4; // Code point for '➤'
                int upArrowCodePoint = 0x2191; // Code point for '↑'
                int downArrowCodePoint = 0x2193; // Code point for '↓'
                int leftArrowCodePoint = 0x2190; // Code point for '←'

                // Calculate the new arrow character based on the degree
                if (degree >= 315 || degree < 45)
                {
                    arrow = (char)rightArrowCodePoint; // Right
                }
                else if (degree >= 45 && degree < 135)
                {
                    arrow = (char)upArrowCodePoint; // Up
                }
                else if (degree >= 135 && degree < 225)
                {
                    arrow = (char)leftArrowCodePoint; // Left
                }
                else if (degree >= 225 && degree < 315)
                {
                    arrow = (char)downArrowCodePoint; // Down
                }

                // Convert the character to a string and return
                return arrow.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while refershing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "unknown";
            }
        }

        public class OpenWeatherMapProvider : GMapProvider
        {
            public static readonly OpenWeatherMapProvider Instance;

            static OpenWeatherMapProvider()
            {
                Instance = new OpenWeatherMapProvider();
            }

            public OpenWeatherMapProvider()
            {
                MaxZoom = 18;
                MinZoom = 1;
                RefererUrl = "https://openweathermap.org/";
            }

            public override Guid Id => new Guid("D6361A7E-FF7B-45A9-BA29-836E1AE2A24A");
            public override string Name => "OpenWeatherMap";

            public override PureProjection Projection => MercatorProjection.Instance;

            private readonly GMapProvider[] overlays = new GMapProvider[] { OpenStreetMapProvider.Instance };

            public override GMapProvider[] Overlays => overlays;

            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                try
                {
                    string url = MakeTileImageUrl(pos, zoom, LanguageStr);
                    return GetTileImageUsingHttp(url);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            string MakeTileImageUrl(GPoint pos, int zoom, string language)
            {
                return string.Format(UrlFormat, zoom, pos.X, pos.Y, "YOUR_API_KEY");
            }

            private static readonly string UrlFormat = "https://tile.openweathermap.org/map/temp_new/{0}/{1}/{2}.png?appid={3}";
        }
        public class OpenWeatherMapOverlayProvider : GMapProvider
        {
            public static readonly OpenWeatherMapOverlayProvider Instance = new OpenWeatherMapOverlayProvider();

            public OpenWeatherMapOverlayProvider()
            {
                MaxZoom = 18;
                MinZoom = 1;
                RefererUrl = "https://tile.openweathermap.org/";
            }

            public override Guid Id => new Guid("7B44D71C-63D1-4515-BE54-15B59E7743A9");
            public override string Name => "OpenWeatherMapOverlay";
            public override PureProjection Projection => MercatorProjection.Instance;

            GMapProvider[] overlays;
            public override GMapProvider[] Overlays => overlays ??= new GMapProvider[] { this };

            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                try
                {
                    string apikey = "329cec969cefc40090ac7b5d60221eaf";
                    string url = $"https://tile.openweathermap.org/map/temp_new/{zoom}/{pos.X}/{pos.Y}.png?appid={apikey}";
                    return GetTileImageUsingHttp(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
        }
    
    }
    public class Location
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public bool IsPrimary { get; set; }
        public int temperature { get; set; }
        public string iconlabel { get; set; }
    }
}
/// <summary>
/// 
/// </summary>

public class TransparentPanel : Panel
{
    private Image backgroundImage;
    private float opacity = 1.0f; // Default opacity is 1 (fully opaque)

    public new Image BackgroundImage
    {
        get { return backgroundImage; }
        set { backgroundImage = value; Invalidate(); }
    }

    public float Opacity
    {
        get { return opacity; }
        set
        {
            if (value < 0.0f || value > 1.0f)
                throw new ArgumentOutOfRangeException("Opacity must be between 0 and 1");
            opacity = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (backgroundImage != null)
        {
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix33 = opacity;
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            e.Graphics.DrawImage(
                backgroundImage,
                new Rectangle(0, 0, this.Width, this.Height),
                0, 0, backgroundImage.Width, backgroundImage.Height,
                GraphicsUnit.Pixel,
                imageAttributes);
        }
    }
}
