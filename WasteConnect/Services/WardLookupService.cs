using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace WasteConnect.Services
{
    public class WardLookupService : IWardLookupService
    {
        private readonly List<WardBoundary> _wards;
        private readonly GeometryFactory _geometryFactory;

        public WardLookupService(IWebHostEnvironment environment)
        {
            _geometryFactory = NtsGeometryServices.Instance
                .CreateGeometryFactory(srid: 4326);

            var filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "GIS",
                "msunduzi-wards-2026.geojson");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "The Msunduzi ward GeoJSON file was not found.",
                    filePath);
            }

            var geoJson = File.ReadAllText(filePath);

            var serializer = GeoJsonSerializer.Create();

            using var stringReader = new StringReader(geoJson);
            using var jsonReader =
                new Newtonsoft.Json.JsonTextReader(stringReader);

            var featureCollection =
                serializer.Deserialize<FeatureCollection>(jsonReader);

            if (featureCollection == null)
            {
                throw new InvalidOperationException(
                    "The ward GeoJSON file could not be read.");
            }

            _wards = featureCollection
                .Select(feature =>
                {
                    var wardValue =
                        feature.Attributes["WardNo"];

                    return new WardBoundary
                    {
                        WardNumber =
                            Convert.ToInt32(wardValue),

                        Geometry =
                            feature.Geometry
                    };
                })
                .ToList();
        }

        public Task<int?> FindWardNumberAsync(
            double latitude,
            double longitude)
        {
            if (latitude < -90 || latitude > 90)
            {
                return Task.FromResult<int?>(null);
            }

            if (longitude < -180 || longitude > 180)
            {
                return Task.FromResult<int?>(null);
            }

            var location =
                _geometryFactory.CreatePoint(
                    new Coordinate(
                        longitude,
                        latitude));

            var matchingWard =
                _wards.FirstOrDefault(
                    ward =>
                        ward.Geometry.Covers(location));

            return Task.FromResult<int?>(
                matchingWard?.WardNumber);
        }

        private class WardBoundary
        {
            public int WardNumber { get; set; }

            public Geometry Geometry { get; set; } =
                default!;
        }
    }
}