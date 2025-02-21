using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }


        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterest(int cityId)
        {
            throw new Exception("Exception sample");

            try
            {
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}",
                    ex);
                return StatusCode(500,
                    "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]

        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointofinterestid)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                return NotFound();
            }

            // find point of interest
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (cityId == 0)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                            c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest);
        }


        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (cityId == 0)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }


        [HttpPatch("{pointofinterestid}")]

        public ActionResult PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestid,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (cityId == 0)
            {
                return NotFound();
            }

            var pointOfInterestFromeStore = city.PointsOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestid);
            if (pointOfInterestFromeStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                    new PointOfInterestForUpdateDto()
                    {
                        Name = pointOfInterestFromeStore.Name,
                        Description = pointOfInterestFromeStore.Description
                    };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromeStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromeStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }


        [HttpDelete("{pointOfInterestId}")]

        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (cityId == 0)
            {
                return NotFound();
            }

            var pointOfInterestFromeStore = city.PointsOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestFromeStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromeStore);
            return NoContent();
        }
    }
}
