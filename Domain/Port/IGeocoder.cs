using System.Device.Location;

namespace Domain.Port
{
    public interface IGeocoder : DDD.Port
    {
        GeoCoordinate GeoCode(string address);
    }
}
