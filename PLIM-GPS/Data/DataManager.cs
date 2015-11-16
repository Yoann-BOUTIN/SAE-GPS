﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using System.IO;
using Windows.Foundation;

namespace PLIM_GPS
{
    public static class DataManager
    {
        // Name of our storage file
        private const string storageFile = "GPSBrutData.json";
        private const string clusterFile = "GPSClusterData.json";


        #region EXPOSED METHODS
        // PUBLIC exposed operations of the DataManager ==> avoid error for exposed Task
        public static IAsyncOperation<bool> SaveDataAsync(IList<GPSElement> positions)
        {
            return DataManager.WriteInFile(positions).AsAsyncOperation();
        }


        public static IAsyncOperation<IList<GPSElement>> GetGPSBrutDataAsync()
        {
            return DataManager.ReadGPSBrutDataAsync().AsAsyncOperation();
        }

        public static IAsyncOperation<bool> WriteClusterAsync(IList<PassedData> dataList)
        {
            return DataManager.WriteClusterInFile(dataList).AsAsyncOperation();
        }

        public static IAsyncOperation<IList<PassedData>> GetClusterListAsync()
        {
            return DataManager.ReadFromClusterFile().AsAsyncOperation();
        }

        #endregion



        #region HIDDEN METHODS
        // Real tasks to perform
        // Read and deserialize the element, return the list of GPSElement as cluster
        public static async Task<IList<GPSElement>> ReadGPSBrutDataAsync() 
        {
            List<GPSElement> brutDatas;
            var JSONSerializer = new DataContractJsonSerializer(typeof(List<GPSElement>));
            var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(storageFile);

            brutDatas = (List<GPSElement>)JSONSerializer.ReadObject(stream);

            return brutDatas;
        }

        // Write the List as 
        public static async Task<bool> WriteInFile (IList<GPSElement> positions)
        {
            if (positions == null)
            {
                return false;
            }

            var serializer = new DataContractJsonSerializer(typeof(List<GPSElement>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(storageFile, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, positions);
            }

            return true;
        }

        public static async Task<bool> WriteClusterInFile(IList<PassedData> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var serializer = new DataContractJsonSerializer(typeof(List<PassedData>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(clusterFile, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, dataList);
            }

            return true;
        }

        public static async Task<IList<PassedData>> ReadFromClusterFile()
        {
            List<PassedData> clusters;
            var JSONSerializer = new DataContractJsonSerializer(typeof(List<PassedData>));
            var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(clusterFile);

            clusters = (List<PassedData>)JSONSerializer.ReadObject(stream);

            return clusters;
        }
        #endregion
    }
}