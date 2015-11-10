using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GPSBackgroundTask;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.UI;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace PLIM_GPS
{
    public sealed partial class MainPage : Page
    {
        Geolocator geolocator;


        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            buttonLocation.Content = "Start Location";
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.MapServiceToken = "abcdef-abcdefghijklmno";
            
        }
        


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 20;

            changeButtonState();

            

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                     maximumAge: TimeSpan.FromMinutes(5),
                     timeout: TimeSpan.FromSeconds(10)
                    );
                

                //geolocation.Text = "GPS:" + geoposition.Coordinate.Latitude.ToString("0.00") + ", " + geoposition.Coordinate.Longitude.ToString("0.00");
                MapIcon icon = new MapIcon();
                icon.Location = geoposition.Coordinate.Point;
                icon.Title = "My Location";
                icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location-pin-48.png"));
                MyMap.MapElements.Add(icon);

                MyMap.Center = geoposition.Coordinate.Point;
                MyMap.DesiredPitch = 0;

                var posList = new List<BasicGeoposition>();
                posList.Add(new BasicGeoposition()
                {
                    Latitude = 43.711908,
                    Longitude = 7.271373
                });
                posList.Add(new BasicGeoposition()
                {
                    Latitude = 43.701024,
                    Longitude = 7.275708
                });
                posList.Add(new BasicGeoposition()
                {
                    Latitude = 43.709713,
                    Longitude = 7.274978
                });

                drawRoute(posList);
                
                await MyMap.TrySetViewAsync(geoposition.Coordinate.Point, 15);
            }
            //If an error is catch 2 are the main causes: the first is that you forgot to include ID_CAP_LOCATION in your app manifest. 
            //The second is that the user doesn't turned on the Location Services
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    //geolocation.Text = "location  is disabled in phone settings.";
                }
            }

            // TEST
            List<GPSElement> myData = new List<GPSElement>();
            myData.Add(new GPSElement() { Latitude = "4.0215", Longitude = "8.0546", RegistredAt = "Maintenant 1" });
            myData.Add(new GPSElement() { Latitude = "4.0215", Longitude = "8.0546", RegistredAt = "Maintenant 2" });
            myData.Add(new GPSElement() { Latitude = "4.0215", Longitude = "8.0546", RegistredAt = "Maintenant 2" });

            //await DataManager.SaveDataAsync(myData);

            //await DataManager.ReadDataAsync();

            //var test = await DataManager.RetrieveDataAsync();
           // Debug.WriteLine(test.ToString());
        }

        private void drawRoute(List<BasicGeoposition> pointList)
        {
            MapPolyline line = new MapPolyline();
            line.StrokeColor = Colors.Yellow;
            line.StrokeThickness = 5;
            line.Path = new Geopath(pointList);
            MyMap.MapElements.Add(line);
        }
        
        private void changeButtonState()
        {
            if ((String)buttonLocation.Content == "Start Location")
            {
                buttonLocation.Content = "Stop Location";
            }
            else if ((String)buttonLocation.Content == "Stop Location")
            {
                buttonLocation.Content = "Start Location";
            }
        }

        private void RegisterTask_Click(object sender, RoutedEventArgs e) {
            if (!GPSTask.IsTaskRegistered())
            {
                GPSTask.Register();
            } else
            {
                GPSTask.Unregister();
            }

        }

        private void TrafficCheck_Checked(object sender, RoutedEventArgs e)
        {
            MyMap.TrafficFlowVisible = true;
        }

        private void TrafficCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            MyMap.TrafficFlowVisible = false;
        }
        private void themeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (themeBtn.Label == "Dark")
            {
                MyMap.ColorScheme = MapColorScheme.Dark;
                themeBtn.Label = "Light";
            }
            else if (themeBtn.Label == "Light")
            {
                MyMap.ColorScheme = MapColorScheme.Light;
                themeBtn.Label = "Dark";
            }
        }
        
    }
}
