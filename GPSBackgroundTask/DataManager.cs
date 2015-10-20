using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using System.IO;
using Windows.UI.Notifications;
using Windows.Foundation;

namespace GPSBackgroundTask
{
    public static class DataManager
    {
        private const string fileName = "GPSData.json";


        public static IAsyncOperation<bool> SaveDataAsync(IList<GPSElement> positions)
        {
            return DataManager.WriteInFile(positions).AsAsyncOperation();
        }

        public static IAsyncOperation<bool> ReadDataAsync()
        {
            return DataManager.ReadFromFile().AsAsyncOperation();
        }


        private static Task<IList<GPSElement>> DeserialiazeCluster()
        {
            return new Task.Run(() => );
        }

        private static async Task<bool> WriteInFile (IList<GPSElement> positions)
        {
            if (positions == null)
            {
                return false;
            }

            var serializer = new DataContractJsonSerializer(typeof(List<GPSElement>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, positions);
            }
            // Show notification
            var toastXmlContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var txtNodes = toastXmlContent.GetElementsByTagName("text");
            txtNodes[0].AppendChild(toastXmlContent.CreateTextNode("DataManager"));
            txtNodes[1].AppendChild(toastXmlContent.CreateTextNode("SUCCESS WRITE"));

            var toast = new ToastNotification(toastXmlContent);
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);

            return true;
        }

        private static async Task<bool> ReadFromFile()
        {
            string fileContent = String.Empty;
            var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName);
            using (StreamReader reader = new StreamReader(stream))
            {
                fileContent = await reader.ReadToEndAsync();
            }


            // Show notification
            var toastXmlContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var txtNodes = toastXmlContent.GetElementsByTagName("text");
            txtNodes[0].AppendChild(toastXmlContent.CreateTextNode("DataManager"));
            txtNodes[1].AppendChild(toastXmlContent.CreateTextNode(fileContent));

            var toast = new ToastNotification(toastXmlContent);
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);

            return true;
        }
    }
}
