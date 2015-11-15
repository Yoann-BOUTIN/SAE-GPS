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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            listCoordonnee = new List<PassedData>();
            testSendData();
            mapTrajet.MapServiceToken = "abcdef-abcdefghijklmno";
            makeasamplelist();
        }

       


        private void listeTrajet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listeTrajet.SelectedIndex != -1)
            {
                PassedData myobject = (sender as ListBox).SelectedItem as PassedData;

                displayMap(myobject.geo);
            }
            

        }

        public void makeasamplelist()
        {
            if(listCoordonnee != null)
            {
                /*if(listCoordonnee.Count != 0)
                {
                    listeTrajet.ItemsSource = null;
                }*/
                for (int i = 0; i < listCoordonnee.Count; i++)
                {
                    //Create a new instace of the class
                    PassedData obj = new PassedData();

                    //Add the sample data
                    obj.name = listCoordonnee[i].name;
                    obj.geo = listCoordonnee[i].geo;


                    //Add the the item object into the listbox
                    listeTrajet.Items.Add(obj);
                }
            }
            
        }

        public async void displayMap(BasicGeoposition[] trajet)
        {
            try {
                MapIcon icon = new MapIcon();
                Geopoint depart = new Geopoint(trajet[0]);
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
                        Latitude = trajet[i].Latitude,
                        Longitude = trajet[i].Longitude
                    });
                }

                drawRoute(posList);

                await mapTrajet.TrySetViewAsync(depart, 15);
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    //geolocation.Text = "location  is disabled in phone settings.";
                }
            }
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
            public BasicGeoposition[] geo { get; set; }
        }

        public void testSendData()
        {
            PassedData data = new PassedData();
            data.name = "Test1";
            data.geo = new BasicGeoposition[3] {
                        new BasicGeoposition()
                        {
                            Latitude = 43.711365,
                            Longitude = 7.271298
                        }, new BasicGeoposition()
                        {
                            Latitude = 43.701024,
                            Longitude = 7.275708
                        },
                        new BasicGeoposition()
                        {
                            Latitude = 43.709713,
                            Longitude = 7.274978
                        }
                    };
            
            PassedData data2 = new PassedData();
            data2.name = "Test2";
            data2.geo = new BasicGeoposition[2] {
                        new BasicGeoposition()
                        {
                            Latitude = 43.711365,
                            Longitude = 7.271298
                        },
                        new BasicGeoposition()
                        {
                            Latitude = 43.709713,
                            Longitude = 7.274978
                        }
                    };
            listCoordonnee.Add(data);
            listCoordonnee.Add(data2);
        }

        private async void renameButton_Click(object sender, RoutedEventArgs e)
        {
            PassedData data = (sender as Button).DataContext as PassedData;
            

            for (int i = 0; i < listCoordonnee.Count; i++)
            {
                if(listCoordonnee[i].name == data.name)
                {
                    indexForRename = i;
                }
            }
            

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Comment voulez vous renommer votre trajet ?",
                MaxWidth = ActualWidth
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
                    listCoordonnee[indexForRename].name = text.Text;
                    data.name = listCoordonnee[indexForRename].name;
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
