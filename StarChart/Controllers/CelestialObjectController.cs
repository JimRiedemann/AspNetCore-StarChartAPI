using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();
            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Name == name);
            if (!celestialObjects.Any())
                return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            foreach (var celestialObject in _context.CelestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(_context.CelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            var id = _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var foundCelestialObject = _context.CelestialObjects.Find(id);
            if (foundCelestialObject == null)
                return NotFound();

            foundCelestialObject.Name = celestialObject.Name;
            foundCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            foundCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();

            celestialObject.Name = name;
            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(co => co.Id == id || co.OrbitedObjectId == id)
                .ToList();
            if (!celestialObjects.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
