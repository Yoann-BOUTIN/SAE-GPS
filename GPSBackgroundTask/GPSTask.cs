using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace GPSBackgroundTask
{
    public sealed class GPSTask : IBackgroundTask
    {

        static string TaskName = "GPSTask";

        public async static void Register()
        {
            if (!IsTaskRegistered())
            {
                var result = await BackgroundExecutionManager.RequestAccessAsync();
                var builder = new BackgroundTaskBuilder();
                builder.Name = TaskName;
                builder.TaskEntryPoint = typeof(GPSTask).FullName;
                builder.SetTrigger(new TimeTrigger(15, false));
                await BackgroundExecutionManager.RequestAccessAsync();
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

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Deferral for AsyncWork
            var deferral = taskInstance.GetDeferral();

            // Canceled case
            taskInstance.Canceled += (s, e) => {
                // TODO faire le ménage en cas d'annulation
            };

            // Progress indication
            taskInstance.Progress = 0;

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
