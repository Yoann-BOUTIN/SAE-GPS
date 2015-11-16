using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PLIM_GPS.HAC
{

    class PassedData
    {
        public string name { get; set; }
        public BasicGeoposition[] geo { get; set; }
    }
}
