using System.Device.Location;

namespace Domain.Port
{
    public abstract class IGeocoder
    {
        public abstract GeoCoordinate GeoCode(string address);
    }
}
