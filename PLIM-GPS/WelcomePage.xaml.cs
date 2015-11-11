using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace PLIM_GPS
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {

        private Geolocator mGeolocator;

        public bool isAlreadyTracking { get; set; }

        
        public WelcomePage()
        {
            this.InitializeComponent();
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
        private void startButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
