using Windows.Devices.Geolocation;

namespace PLIM_GPS
{
    public sealed class PassedData
    {
        public string Name { get; set; }
        public BasicGeoposition[] geo { get; set; }
    }
}
