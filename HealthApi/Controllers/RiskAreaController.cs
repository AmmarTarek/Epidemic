using HealthApi.Models;
using HealthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO;

namespace HealthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskAreaController : Controller
    {
        IRiskAreaRepository RisRepo;
        AppDbContext context;

        public RiskAreaController(IRiskAreaRepository _risRepo , AppDbContext context)
        {
            RisRepo = _risRepo;
            this.context = context;

        }
        [HttpGet]
        public ActionResult AllAreas()
        {
            List<RiskArea> RiskAreas = RisRepo.GetAll();
            return Ok(RiskAreas);
        }


        [HttpGet("by-id/{id:int}")]
        public IActionResult GetById(int id)
        {

            if (ModelState.IsValid)
            {
                RiskArea RiskArea_fromREq = RisRepo.GetById(id);

                return Ok(RiskArea_fromREq);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("by-level/{level}")]
        public IActionResult GetByRiskLevel(string level)
        {

            if (ModelState.IsValid)
            {
                List<RiskArea> RiskAreas_fromREq = RisRepo.GetByRiskLevel(level);

                return Ok(RiskAreas_fromREq);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("EditShape")]
        public async Task<IActionResult> SaveCleanedPolygon(int oldAreaId)
        {
            try
            {
                var oldArea = context.RiskAreas.FirstOrDefault(i => i.AreaId == oldAreaId);
                if (oldArea == null)
                    return NotFound("Area not found.");

                // 1. Read original WKT (with Z)
                var reader = new WKTReader();
                var geometry3D = reader.Read(oldArea.AreaGeometry);

                // 2. Remove Z using GeometryEditor and valid factory
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 0);
                var make2D = new GeometryEditor(geometryFactory);
                var geometry2D = make2D.Edit(geometry3D, new GeometryEditor.CoordinateSequenceOperation((seq, factory) =>
                {
                    int count = seq.Count;
                    var coordinateFactory = geometryFactory.CoordinateSequenceFactory;
                    var newSeq = coordinateFactory.Create(count, 2); // Correct factory call

                    for (int i = 0; i < count; i++)
                    {
                        newSeq.SetX(i, seq.GetX(i));
                        newSeq.SetY(i, seq.GetY(i));
                    }

                    return newSeq;
                }));

                geometry2D.SRID = 0;

                // 3. Save cleaned polygon
                oldArea.Geometry = (Polygon)geometry2D;
                context.Update(oldArea);
                await context.SaveChangesAsync();

                return Ok("Area cleaned and saved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("GetRiskAreas")]
        public async Task<IActionResult> GetRiskAreas()
        {
            var areas = await context.RiskAreas.ToListAsync();

            var featureCollection = new FeatureCollection();

            foreach (var area in areas)
            {
                var attributes = new AttributesTable
            {
                { "id", area.AreaId },
                { "name", area.AreaName },
                { "riskLevel", area.RiskLevel }
            };

                var feature = new Feature(area.Geometry, attributes);
                featureCollection.Add(feature);
            }

            var writer = new GeoJsonWriter();
            var geoJson = writer.Write(featureCollection);

            return Content(geoJson, "application/json");
        }




    }
}
