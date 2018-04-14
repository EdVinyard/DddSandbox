using System.Device.Location;

namespace Domain.Port
{
    public abstract class IGeocoder : DDD.Port
    {
        public abstract GeoCoordinate GeoCode(string address);
    }
}
