using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace GPSBackgroundTask
{
    public sealed class GPSTask : IBackgroundTask
    {

        static string TaskName = "GPSTask";
        private Geolocator geolocator;
        

        public async static void Register(Geoposition position)
        {
            if (!IsTaskRegistered())
            {
                // Demande de tâches background en async
                var result = await BackgroundExecutionManager.RequestAccessAsync();
                
                // Construction de la tâche en background
                var builder = new BackgroundTaskBuilder();
                builder.Name = TaskName;
                builder.TaskEntryPoint = typeof(GPSTask).FullName;

                // Préparation de la GeoFence ==> barrière vituelle, 100m autour de la position, pendant plus de 5 secondes
                Geocircle circle = new Geocircle(position.Coordinate.Point.Position, 100);
                var geofence = new Geofence(TaskName+"Register", circle, MonitoredGeofenceStates.Exited, true, TimeSpan.FromSeconds(5));
                GeofenceMonitor geoMonitor = GeofenceMonitor.Current;
                geoMonitor.Geofences.Add(geofence);

                // Déclenchement de la tâche suivant la position
                builder.SetTrigger(new LocationTrigger(LocationTriggerType.Geofence));
                BackgroundTaskRegistration theTask = builder.Register();

                // Show notification
                var toastXmlContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                var txtNodes = toastXmlContent.GetElementsByTagName("text");
                txtNodes[0].AppendChild(toastXmlContent.CreateTextNode("Task registred."));

                var toast = new ToastNotification(toastXmlContent);
                var toastNotifier = ToastNotificationManager.CreateToastNotifier();
                toastNotifier.Show(toast);
            }
        }

        public static void Unregister()
        {
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(kvp => kvp.Value.Name == TaskName);

            if (entry.Value != null)
                entry.Value.Unregister(true);

            // Show notification
            var toastXmlContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var txtNodes = toastXmlContent.GetElementsByTagName("text");
            txtNodes[0].AppendChild(toastXmlContent.CreateTextNode("Task unregistred."));

            var toast = new ToastNotification(toastXmlContent);
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);
        }

        public static bool IsTaskRegistered()
        {
            var taskRegistered = false;
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(kvp => kvp.Value.Name == TaskName);

            if (entry.Value != null)
                taskRegistered = true;

            return taskRegistered;
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Deferral for AsyncWork
            var deferral = taskInstance.GetDeferral();

            // Canceled case
            taskInstance.Canceled += (s, e) => {
                // TODO faire le ménage en cas d'annulation
                Debug.WriteLine("Task canceled!!!!");
            };

            // Progress indication
            taskInstance.Progress = 0;

            // Geolocator settings if not already done
            geolocator = new Geolocator()
            {
                DesiredAccuracy = PositionAccuracy.High,
                MovementThreshold = 15,
                ReportInterval = 2000
            };

            Geoposition startRunPosition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromSeconds(20),
                timeout: TimeSpan.FromSeconds(10)
                );

            Debug.WriteLine("Start Position : ", startRunPosition.Coordinate.Point.Position.Latitude.ToString(), "    ", startRunPosition.Coordinate.Point.Position.Latitude.ToString());

            geolocator.PositionChanged += (s, e) =>
            {
                Debug.WriteLine("Position : ", e.Position.Coordinate.Point.Position.Latitude.ToString(), "    ", e.Position.Coordinate.Point.Position.Latitude.ToString());
            };

            // Create a toast notification to show a geofence has been hit
            var toastXmlContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var txtNodes = toastXmlContent.GetElementsByTagName("text");
            txtNodes[0].AppendChild(toastXmlContent.CreateTextNode("Time triggered toast!"));
            txtNodes[1].AppendChild(toastXmlContent.CreateTextNode("OK"));

            var toast = new ToastNotification(toastXmlContent);
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);

            taskInstance.Progress = 100;

            // Finish the task
            deferral.Complete();
        }
    }
}
