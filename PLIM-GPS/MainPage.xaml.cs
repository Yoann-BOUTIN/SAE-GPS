﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using GPSBackgroundTask;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.Services.Maps;
using System.Text;
using Windows.UI.Popups;
using Windows.UI.Core;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace PLIM_GPS
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        
        Geolocator geolocator;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.MapServiceToken = "abcdef-abcdefghijklmno";
            
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                     maximumAge: TimeSpan.FromMinutes(5),
                     timeout: TimeSpan.FromSeconds(10)
                    );
                

                //With this 2 lines of code, the app is able to write on a Text Label the Latitude and the Longitude, given by {{Icode|geoposition}}
                geolocation.Text = "GPS:" + geoposition.Coordinate.Latitude.ToString("0.00") + ", " + geoposition.Coordinate.Longitude.ToString("0.00");
                MapIcon icon = new MapIcon();
                icon.Location = geoposition.Coordinate.Point;
                icon.Title = "My Location";
                icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location-pin-48.png"));
                MyMap.MapElements.Add(icon);

                MyMap.Center = geoposition.Coordinate.Point;
                MyMap.DesiredPitch = 0;

                await MyMap.TrySetViewAsync(geoposition.Coordinate.Point, 15);

            }
            //If an error is catch 2 are the main causes: the first is that you forgot to include ID_CAP_LOCATION in your app manifest. 
            //The second is that the user doesn't turned on the Location Services
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    geolocation.Text = "location  is disabled in phone settings.";
                }
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