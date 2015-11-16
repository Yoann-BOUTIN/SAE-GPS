using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HAC.Algorithm;
using HAC.Fusions;
using HAC.Metrics;
using HAC;
using GPSBackgroundTask;
using Newtonsoft.Json.Linq;

// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace PLIM_GPS
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class VisualisationPage : Page
    {

        public int Suspending { get; private set; }
        public int OnSuspending { get; private set; }
        public List<PassedData> listCoordonnee;
        public VisualisationPage()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

#if WINDOWS_PHONE_APP
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif

        }

#if WINDOWS_PHONE_APP
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }
#endif

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            listCoordonnee = new List<PassedData>();
            mapTrajet.MapServiceToken = "abcdef-abcdefghijklmno";
        }




        private void listeTrajet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cluster myobject = (sender as ListBox).SelectedItem as Cluster;
            displayMap(myobject.GetElements());

        }



        public async void displayMap(Element[] trajet)
        {

           // try
            //{
                MapIcon icon = new MapIcon();
                Geopoint depart = new Geopoint(new BasicGeoposition()
                {
                    Latitude = Convert.ToDouble(trajet[0].GetDataPoints()[1].ToString().Replace('.',',')),
                    Longitude = Convert.ToDouble(trajet[0].GetDataPoints()[2].ToString().Replace('.', ','))
                });
                icon.Location = depart;
                icon.Title = "My Location";
                icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location-pin-48.png"));
                mapTrajet.MapElements.Add(icon);

                mapTrajet.Center = depart;
                mapTrajet.DesiredPitch = 0;

                var posList = new List<BasicGeoposition>();
                for (int i = 0; i < trajet.Length; i++)
                {
                    posList.Add(new BasicGeoposition()
                    {
                        Latitude = Convert.ToDouble(trajet[i].GetDataPoints()[1].ToString().Replace('.', ',')),
                        Longitude = Convert.ToDouble(trajet[i].GetDataPoints()[2].ToString().Replace('.', ','))
                    });
                }

                drawRoute(posList);

                await mapTrajet.TrySetViewAsync(depart, 15);
          /*  }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    //geolocation.Text = "location  is disabled in phone settings.";
                }
            }
            */
        }

        private void drawRoute(List<BasicGeoposition> pointList)
        {
            mapTrajet.MapElements.Clear();
            MapPolyline line = new MapPolyline();
            line.StrokeColor = Colors.Blue;
            line.StrokeThickness = 5;
            line.Path = new Geopath(pointList);
            mapTrajet.MapElements.Add(line);
        }

        public class PassedData
        {
            public string name { get; set; }
            //public BasicGeoposition[] geo { get; set; }
            public GPSElement[] geo { get; set; }
        }





        
    }
}

