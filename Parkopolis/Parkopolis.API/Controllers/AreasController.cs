﻿using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Parkopolis.API.MockData;
using Parkopolis.API.Models;
using Parkopolis.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parkopolis.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/areas")]
    public class AreasController : ControllerBase
    {
        IMapper _mapper;
        IParkopolisRepository _repo;

        public AreasController (IMapper mapper, IParkopolisRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult GetAreas(int cityId)
        {
            if(!_repo.CityExists(cityId))
            {
                return NotFound("City Not Found");
            }

            return Ok(_mapper.Map<IEnumerable<AreaForDisplayDto>>(_repo.GetAllAreasForCity(cityId)));
        }

        [HttpGet("{areaId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult GetArea(int cityId, int areaId)
        {
            if (!_repo.CityExists(cityId)) return NotFound();

            if (!_repo.AreaExists(areaId)) return NotFound();

            var result = _repo.GetAreaById(areaId);

            return Ok(result);
        }

        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult AddArea([FromBody] Area area, int cityId)
        {
            if (!_repo.CityExists(area.CityId))
            {
                return NotFound("City in request body does not exist");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            area.CityId = cityId;

            _repo.AddArea(area);
            return NoContent();
        }

        [HttpDelete("{areaId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult DeleteArea(int cityId, int areaId)
        {
            if(!_repo.CityExists(cityId))
            {
                return NotFound("No city");
            }
            if(!_repo.AreaExists(areaId))
            {
                return NotFound("Area not found");
            }

            _repo.RemoveArea(_repo.GetAreaById(areaId));
            return NoContent();
        }
        [HttpPut("{areaId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult UpdateArea (int cityId, int areaId, Area area)
        {
            if (!_repo.CityExists(cityId))
            {
                return NotFound("No city");
            }
            if (!_repo.AreaExists(areaId))
            {
                return NotFound("Area not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            area.CityId = cityId;

            _repo.UpdateArea(areaId, area);
            return NoContent();
        }

    }
}
