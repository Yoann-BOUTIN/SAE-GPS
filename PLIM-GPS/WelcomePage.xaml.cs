using GPSBackgroundTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PLIM_GPS
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {

        #region PROPERTIES
        public Geolocator MyGeolocator { get; set; }
        public bool IsTracking { get; set; }
        public List<GPSElement> SavedPositions { get; set; }
        Windows.System.Display.DisplayRequest Display { get; set; }
        #endregion


        #region SYSTEM METHODS

        public WelcomePage()
        {
            this.InitializeComponent();
            // Initialisation du Geolocator
            if (MyGeolocator == null)
            {
                MyGeolocator = new Geolocator();
                // Précision Haute, si on bouge de 15m, rapport de position toutes les 2s (2000ms)
                MyGeolocator.DesiredAccuracy = PositionAccuracy.High;
                MyGeolocator.MovementThreshold = 15;
                MyGeolocator.ReportInterval = 2000;
            }
            // Initialisation de la liste de position
            if (SavedPositions == null)
            {
                SavedPositions = new List<GPSElement>();
            }
            // Initialise le cache pour la navigation
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Display = new Windows.System.Display.DisplayRequest();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            refreshUI();
        }
        #endregion

        #region UI METHODS
        private void refreshUI()
        {
            if (IsTracking)
            {
                // Etat tracker
                stateText.Text = "Running";
                stateText.Foreground = new SolidColorBrush(Colors.Green);

                // Boutons
                startButton.Content = "Stop";

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

                // Textes
                titleText.Text = "Bienvenue";
                explanationText.Text = "Bienvenue sur l'appication PLIM GPS grâce à laquelle vous allez pouvoir suivre vos déplacements tout au long de la journée. Pour commencez à suivre votre position, appuyez sur le bouton 'Start' ci - dessous.";
            }
        }


        // Invoqué lorsque l'on clique sur le bouton Start / Stop
        // Démarre l'enregistrement de la position
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsTracking)
            {
                // Demande de laisser l'écran allumer
                Display.RequestActive();

                stateText.Text = "In progress...";
                stateText.Foreground = new SolidColorBrush(Colors.Orange);

                // Récupération de la première position
                Geoposition startPosition = await MyGeolocator.GetGeopositionAsync(
                        maximumAge : TimeSpan.FromSeconds(50),
                        timeout : TimeSpan.FromSeconds(10)
                    );

                // Méthode lorsque la position change
                MyGeolocator.PositionChanged += OnPositionChanged;

                IsTracking = true;
            }
            else
            {
                // Désactive l'écran allumer
                Display.RequestRelease();

                stateText.Text = "Waiting to stop...";
                stateText.Foreground = new SolidColorBrush(Colors.Orange);

                // Sauvegarde des positions enregistrées
                await DataManager.SaveDataAsync(SavedPositions);

                // On arrête de suivre sa position
                MyGeolocator.PositionChanged -= OnPositionChanged;

                IsTracking = false;
                //TODO appel de la fonction de clustering

            }
            // MAJ de l'UI
            refreshUI();
        }

        private void visualizeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VisualisationPage));
        }
        #endregion

        #region GEOLOCATION METHODS
        private void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            Debug.WriteLine("Time : " + System.DateTime.Now);
            Debug.WriteLine("Latitude : " + e.Position.Coordinate.Point.Position.Latitude.ToString());
            Debug.WriteLine("Longitude : " + e.Position.Coordinate.Point.Position.Longitude.ToString());

            // Création du GPSElement à sauvegarder
            GPSElement positionToSaved = new GPSElement() {
                Latitude = e.Position.Coordinate.Point.Position.Latitude,
                Longitude = e.Position.Coordinate.Point.Position.Longitude,
                RegistredAt = DateTime.Now.ToString()
            };

            SavedPositions.Add(positionToSaved);
        }
        #endregion
    }
}