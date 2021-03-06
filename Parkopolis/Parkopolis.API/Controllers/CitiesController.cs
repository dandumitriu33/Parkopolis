using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Parkopolis.API.MockData;
using Parkopolis.API.Models;
using AutoMapper;
using Parkopolis.API.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Parkopolis.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly IParkopolisRepository _repository;
        private readonly IMapper _mapper;


        public CitiesController(IParkopolisRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult GetCities()
        {
            return Ok(_repository.GetAllCities());
            //return Ok(_mapper.Map<IEnumerable<CityDto>>(_repository.GetAllCities()));
        }

        [HttpGet("{cityId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult GetCity(int cityId)
        {
            if (!_repository.CityExists(cityId)) return NotFound();

            return Ok(_mapper.Map<CityDto>(_repository.GetCityById(cityId)));
        }

        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult AddCity([FromBody] City cityToAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            _repository.AddCity(cityToAdd);
            _repository.Save();
            return NoContent();
        }

        [HttpDelete("{cityId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult RemoveCity(int cityId)
        {
           if(!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var city = _repository.GetCityById(cityId);
            _repository.RemoveCity(city);
            return NoContent();
        }

        [HttpPut("{cityId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult UpdateCity(int cityId, [FromBody] City city )
        {
            if (!_repository.CityExists(cityId)) return NotFound("City not found");
            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }

            _repository.UpdateCity(cityId, city);


            return NoContent();
        }

        
    }
}
