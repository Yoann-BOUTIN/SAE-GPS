using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        public int indexForRename;
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

        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            listCoordonnee = new List<PassedData>();
            listCoordonnee = (List<PassedData>) await DataManager.GetClusterListAsync();
            mapTrajet.MapServiceToken = "abcdef-abcdefghijklmno";
            makeasamplelist();
        }



        // Changement du trajet à afficher
        private void listeTrajet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listeTrajet.SelectedIndex != -1)
            {
                PassedData myobject = (sender as ListBox).SelectedItem as PassedData;

                displayMap(myobject.geo);
            }
            

        }

        // Création de tous les trajets dans la ListBox
        public void makeasamplelist()
        {
            if(listCoordonnee != null)
            {
                for (int i = 0; i < listCoordonnee.Count; i++)
                {
                    //Création d'un nouveau trajet
                    PassedData obj = new PassedData();


                    obj.Name = listCoordonnee[i].Name;
                    obj.geo = listCoordonnee[i].geo;


                    //Ajout du trajet dans la ListBox
                    listeTrajet.Items.Add(obj);
                }
            }
            
        }

        // Fonction d'affichage de la map avec les points du trajet sélectionné
        public async void displayMap(BasicGeoposition[] trajet)
        {
            try {
                mapTrajet.MapElements.Clear();
                MapIcon icon = new MapIcon();
                Geopoint depart = new Geopoint(trajet[0]);
                icon.Location = depart;
                icon.Title = "My Location";
                icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location-pin-24.png"));
                icon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5);
                mapTrajet.MapElements.Add(icon);

                mapTrajet.Center = depart;
                mapTrajet.DesiredPitch = 0;
                if (trajet.Length > 1)
                {
                    // ---------------- Boucle permettant de tracet les points ------------------
                    for(int i = 1; i < trajet.Length; i++)
                    {
                        var ic = new MapIcon();
                        var pt = new Geopoint(trajet[i]);
                        ic.Location = pt;
                        ic.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location-pin-24.png"));
                        ic.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1);
                        mapTrajet.MapElements.Add(ic);
                    }
                    // ---------------- Boucle permettant de tracet les trajets -----------------
                    /*var posList = new List<BasicGeoposition>();
                    for (int i = 0; i < trajet.Length; i++)
                    {
                        posList.Add(new BasicGeoposition()
                        {
                            Latitude = trajet[i].Latitude,
                            Longitude = trajet[i].Longitude
                        });
                    }

                    drawRoute(posList);*/ 
                }

                await mapTrajet.TrySetViewAsync(depart, 15);
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    
                }
            }
        }

        // Fonction dessinant les trajets
        private void drawRoute(List<BasicGeoposition> pointList)
        {
            MapPolyline line = new MapPolyline();
            line.StrokeColor = Colors.Blue;
            line.StrokeThickness = 5;
            line.Path = new Geopath(pointList);
            mapTrajet.MapElements.Add(line);
        }

        // Fonction renommant les trajets
        private async void renameButton_Click(object sender, RoutedEventArgs e)
        {
            PassedData data = (sender as Button).DataContext as PassedData;
            

            for (int i = 0; i < listCoordonnee.Count; i++)
            {
                if(listCoordonnee[i].Name == data.Name)
                {
                    indexForRename = i;
                }
            }
            

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Renommez votre trajet"
            };
            var text = new TextBox
            {
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                PlaceholderText = "Nom du trajet..."
            };

            dialog.Content = text;
            dialog.PrimaryButtonText = "Valider";
            dialog.IsPrimaryButtonEnabled = true;
            dialog.PrimaryButtonClick += delegate
            {
                if(text.Text != null || text.Text != "" || text.Text != " ")
                {
                    listCoordonnee[indexForRename].Name = text.Text;
                    data.Name = listCoordonnee[indexForRename].Name;
                    listeTrajet.Items.Remove(data);
                    listeTrajet.UpdateLayout();
                    listeTrajet.Items.Insert(indexForRename, data);
                    listeTrajet.SelectedIndex = indexForRename;
                }
                
            };
            dialog.SecondaryButtonText = "Annuler";
            dialog.SecondaryButtonClick += delegate {
                
            };

            await dialog.ShowAsync();
        }

    }
}
