using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GPSBackgroundTask;

// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace PLIM_GPS
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {

        private Geolocator geolocator;

        public bool isAlreadyTracking { get; set; }

        public WelcomePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            startButton.Content = "Start";
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        // Invoqué lorsque l'on clique sur le bouton Start / Stop
        // Démarre l'enregistrement de la position
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {

            testSendData();

            if ((String)startButton.Content == "Start")
            {
                stateText.Text = "In progress...";
                stateText.Foreground = new SolidColorBrush(Colors.Orange);
                geolocator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 20;

                try
                {
                    Geoposition geoposition = await geolocator.GetGeopositionAsync(
                         maximumAge: TimeSpan.FromMinutes(5),
                         timeout: TimeSpan.FromSeconds(10)
                        );


                    geoText.Text = geoposition.Coordinate.Point.Position.Latitude.ToString() + ", " + geoposition.Coordinate.Point.Position.Longitude.ToString();

                    
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
            }
            
            changeButtonState();
        }

        private void changeButtonState()
        {
            if ((String)startButton.Content == "Start")
            {
                startButton.Content = "Stop";
                stateText.Text = "Running";
                stateText.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if ((String)startButton.Content == "Stop")
            {
                startButton.Content = "Start";
                stateText.Text = "Stopped";
                stateText.Foreground = new SolidColorBrush(Colors.Red);

                if (!GPSTask.IsTaskRegistered())
                {
                    GPSTask.Register();
                }


            }
        }

        private void visualizeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VisualisationPage));
        }

        public async void testSendData()
        {

            String savedData = await DataManager.ReadDataAsync();

            //prend les données Json et les sauvegarde dans un objet Json array
            JArray jsonVal = JArray.Parse(savedData) as JArray;
            dynamic GPSElements = jsonVal;
            int length = 0;
            foreach (dynamic elmt in GPSElements)
            {
                length++;
            }
            var elements = new Element[length];
            int j = 0;
            foreach (dynamic elmt in GPSElements)
            {
                elements[j] = new Element((j + 1).ToString(), new object[] { elmt.RegistredAt.ToString(), elmt.Latitude.ToString(), elmt.Longitude.ToString() });
                j++;
            }



            HacStart hacStart = new HacStart(elements, new SingleLinkage(), new JaccardDistance());
            var clusters = hacStart.Cluster(2f, 2);
            String result = "";
            for (int i = 0; i < clusters.Count(); i++)
            {
                result += "---Cluster " + (i + 1) + "---\n";
                clusters[i].Name = "Cluster " + (i + 1);
                listeTrajet.Items.Add(clusters[i]);
                foreach (Element e in clusters[i])
                    result += "Element " + e.RegistredAt + " \n";
            }
        }

    }
}
