using GPSBackgroundTask;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace PLIM_GPS
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        private ApplicationDataContainer settings;
        private Geolocator mGeolocator;

        public static string TRACKING_KEY = "AlreadyTracking";

        public bool isAlreadyTracking { get; set; }

        
        public WelcomePage()
        {
            this.InitializeComponent();

            // Initalise le Geolocator et les settings de l'application
            settings = ApplicationData.Current.LocalSettings;
            if (mGeolocator == null)
            {
                mGeolocator = new Geolocator();
                mGeolocator.DesiredAccuracy = PositionAccuracy.High;
            }
            // Vérifie si le suivi de position est déjà lancé ou non
            if (settings.Values.ContainsKey(TRACKING_KEY))
            {
                isAlreadyTracking = (bool)settings.Values[TRACKING_KEY];
            } else
            {
                isAlreadyTracking = false;
            }
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            initInterface();
        }

        private void initInterface()
        {
            // Si on suit déjà, on le note et on passe le bouton en STOP
            if (isAlreadyTracking)
            {
                // Etat tracker
                stateText.Text = "Running";
                stateText.Foreground = new SolidColorBrush(Colors.Green);

                // Boutons
                startButton.Content = "Stop";
                visualizeButton.IsEnabled = true;

                // Textes
                titleText.Text = "Félicitations";
                explanationText.Text = "Félicitations l'enregistrement de votre position GPS a commencé. Vous pouvez l'arrêter à tout moment en utilisant le bouton 'Stop' ci-dessous. Le bouton 'Visualize' vous permet de visualiser les trajets déjà enregistrés par l'application. ";
            }
            else
            {
                // Etat tracker
                stateText.Text = "Stopped";
                stateText.Foreground = new SolidColorBrush(Colors.Red);

                // Boutons
                startButton.Content = "Start";
                visualizeButton.IsEnabled = false;

                // Textes
                titleText.Text = "Bienvenue";
                explanationText.Text = "Bienvenue sur l'appication PLIM GPS grâce à laquelle vous allez pouvoir suivre vos déplacements tout au long de la journée. Pour commencez à suivre votre position, appuyez sur le bouton 'Start' ci - dessous.";
            }
        }


        // Invoqué lorsque l'on clique sur le bouton Start / Stop
        // Démarre l'enregistrement de la position
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAlreadyTracking)
            {
                isAlreadyTracking = true;
                // Récupère position GPS + enregistrement de la tâche de Background
                try
                {
                    Geoposition location = await mGeolocator.GetGeopositionAsync(
                        maximumAge: TimeSpan.FromMinutes(1),
                        timeout: TimeSpan.FromSeconds(10)
                    );

                    GPSTask.Register(location);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("DEBUG : accès au GPS non authorisé.");
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("DEBUG : tentative de localisation annulée.");
                }
            }
            else
            {
                isAlreadyTracking = false;
            }

            settings.Values[TRACKING_KEY] = isAlreadyTracking;
            initInterface();
        }
    }
}
