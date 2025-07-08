//using HealthApi.Models;
//using NetTopologySuite.Features;
//using NetTopologySuite.Geometries;
//using NetTopologySuite.IO;
//using Newtonsoft.Json;
//using System.Text.Json;


//using (var reader = new StreamReader("geoBoundaries-EGY-ADM2_simplified.geojson"))
//{
//    var json = await reader.ReadToEndAsync();

//    var serializer = GeoJsonSerializer.Create();
//    var stringReader = new StringReader(json);
//    var featureCollection = (FeatureCollection)serializer.Deserialize(new JsonTextReader(stringReader), typeof(FeatureCollection));

//    using var context = new AppDbContext(); // Replace with your actual context

//    foreach (var feature in featureCollection)
//    {
//        var geometry = feature.Geometry as Polygon;
//        var properties = feature.Attributes;

//        var yourModel = new RiskArea
//        {
//            AreaName = properties.Exists("shapeName") ? properties["shapeName"].ToString() : null,
//            Geometry = geometry,
//            RiskLevel = "Low", // Adjust this based on your requirements
//            AreaGeometry = "None Format", // Adjust this based on your requirements

//        };

//        context.RiskAreas.Add(yourModel);
//    }

//    context.SaveChanges();
//}


