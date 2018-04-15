using Domain.Port;
using System;
using System.Collections.Generic;
using System.Device.Location;

namespace Application
{
    // TODO: Implement a real GeoCoder.
    public class FakeGeocoder : IGeocoder
    {
        private static readonly Random _prng = new Random();
        private static string ToKey(string address) => address.ToLower();
        private static readonly Dictionary<string, GeoCoordinate> Cache =
            new Dictionary<string, GeoCoordinate>();

        /// <summary>
        /// Never returns latitude or longitude values that are zero, 
        /// for ease of testing.
        /// </summary>
        public override GeoCoordinate GeoCode(string address)
        {
            GeoCoordinate coordinate;
            var cacheKey = ToKey(address);
            if (!Cache.TryGetValue(cacheKey, out coordinate))
            {
                coordinate = new GeoCoordinate()
                {
                    Latitude = 30 + _prng.NextDouble(),
                    Longitude = 97 + _prng.NextDouble(),
                };
                Cache.Add(cacheKey, coordinate);
            }

            return coordinate;
        }
    }
}
