using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace GPSBackgroundTask
{
    public sealed class PassedData
    {
        public string Name { get; set; }
        public BasicGeoposition[] geo { get; set; }
    }
}
