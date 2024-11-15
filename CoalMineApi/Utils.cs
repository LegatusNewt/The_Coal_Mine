using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Utils
{
    public class GeoUtils
    {
        // PostGIS has a method to convert geometries to GeoJSON, however we are using a server side approach
            //I didn't want to rely too much on database specific functions
        public string ConvertToGeoJson<T>(List<T> entities) where T : class
        {
            var features = new List<IFeature>();

            foreach (var entity in entities)
            {
                var geometryProperty = typeof(T).GetProperties()
                    .FirstOrDefault(p => p.PropertyType.BaseType == typeof(Geometry));

                if (geometryProperty == null)
                {
                    throw new InvalidOperationException("No geometry property found on the entity.");
                }

                var geometry = (Geometry)geometryProperty.GetValue(entity);

                var attributes = new AttributesTable();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (property != geometryProperty)
                    {
                        attributes.Add(property.Name, property.GetValue(entity));
                    }
                }

                var feature = new Feature(geometry, attributes);
                features.Add(feature);
            }

            var featureCollection = new FeatureCollection(features);
            var writer = new GeoJsonWriter();
            return writer.Write(featureCollection);
        }
    }
}