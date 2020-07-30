﻿using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Parkopolis.API.MockData;
using Parkopolis.API.Models;
using Parkopolis.API.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Parkopolis.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/areas/{areaId}/parkinglots")]
    public class ParkingLotsController : ControllerBase
    {
        IMapper _mapper;
        IParkopolisRepository _repo;

        public ParkingLotsController(IMapper mapper, IParkopolisRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }
        [HttpGet]
        public IActionResult GetParkingLots(int cityId, int areaId, bool includeParkingSpots)
        {
            if (!_repo.CityExists(cityId)) return NotFound("City not found");

            if (!_repo.AreaExists(areaId)) return NotFound("Area not found");

            if (includeParkingSpots)
            {
                return Ok(_repo.GetParkingLotsIncludingParkingSpacesById(areaId));
            }

            return Ok(_mapper.Map<IEnumerable<ParkingLotForDisplayDto>>(_repo.GetParkingLots(areaId)));
        }

        [HttpGet("{parkingLotId}")]
        public IActionResult GetParkingLot(int cityId, int areaId, int parkingLotId, bool includeParkingSpots)
        {
            if (!_repo.CityExists(cityId)) return NotFound("City not found");

            if (!_repo.AreaExists(areaId)) return NotFound("Area not found");

            if (!_repo.ParkingLotExists(parkingLotId)) return NotFound("Lot not found");


            if (includeParkingSpots)
            {
                return Ok(_repo.GetParkingLotByIdIncludingParkingSpaces(parkingLotId));
            }

            return Ok(_mapper.Map<ParkingLotForDisplayDto>(_repo.GetParkingLotById(parkingLotId)));
        }

        [Route("/users/{userId}/getParkingLots")]
        [HttpGet]
        public IActionResult GetParkingLotsForUser(string userId, bool includeparkingspaces)
        {
            if (!_repo.UserExists(userId)) return NotFound("User does not exist");

            if (includeparkingspaces)
            {
                return Ok(_repo.GetAllParkingLotsForUserIncludingParkingSpots(userId));
            }
            return Ok(_mapper.Map<IEnumerable<ParkingLotForDisplayDto>>(_repo.GetAllParkingLotsForUser(userId)));
        }


        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult CreateParkingLot(int cityId, int areaId, [FromBody] ParkingLot parkingLot)
        {
            if (!_repo.CityExists(cityId)) return NotFound("City not found");

            if (!_repo.AreaExists(areaId)) return NotFound("Area not found");

            //parkingLot.AreaId = areaId;
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _repo.AddParkingLot(parkingLot);
            return NoContent();
        }

        
        [HttpPost]
        [Route("/users/{userId}/addParkingLot")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult AddParkingLotForUser(string userId, [FromBody] ParkingLot parkingLot)
        {
            if (!_repo.UserExists(userId))
            {
                return NotFound("User not found");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }

            parkingLot.ApplicationUserId = userId;
            _repo.AddParkingLot(parkingLot);
            return NoContent();
        }


        [HttpPut("{parkingLotId}")]
        public IActionResult UpdateParkingLot(int cityId, int areaId, int parkingLotId, [FromBody] ParkingLot parkingLot)
        {
            if (!_repo.CityExists(cityId)) return NotFound("City not found");

            if (!_repo.AreaExists(areaId)) return NotFound("Area not found");

            if (!_repo.ParkingLotExists(parkingLotId)) return NotFound("Lot not found");

            if (!_repo.AreaExists(parkingLot.AreaId)) return NotFound("AreaId From Query is Invalid");

            parkingLot.Id = parkingLotId;
            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            _repo.UpdateParkingLot(parkingLotId, parkingLot);

            return NoContent();
        }

       
        [HttpPut("/users/{userId}/editparkinglot/{parkingLotId}")]
        public IActionResult UpdateParkingLotForUser(string userId, int cityId, int areaId, int parkingLotId, [FromBody] ParkingLot parkingLot)
        {
            if (!_repo.UserExists(userId))
            {
                return NotFound("User not found");
            }

            if(!_repo.ParkingLotExists(parkingLotId))
            {
                return NotFound("Lot not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }

            parkingLot.Id = parkingLotId;
            parkingLot.ApplicationUserId = userId;
            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            _repo.UpdateParkingLot(parkingLotId, parkingLot);

            return NoContent();
        }

        [HttpPatch("{parkingLotId}")]
        public IActionResult PartiallyUpdateParkingLot(int cityId, int areaId, int parkingLotId, [FromBody] JsonPatchDocument<ParkingLotForUpdateDto> patchDoc)
        {
            if (!_repo.CityExists(cityId)) return NotFound("City not found");

            if (!_repo.AreaExists(areaId)) return NotFound("Area not found");

            if (!_repo.ParkingLotExists(parkingLotId)) return NotFound("Lot not found");


            var parkingLotFromStore = _repo.GetParkingLotById(parkingLotId);

            var parkingLotToPatch = new ParkingLotForUpdateDto()
            {
                Name = parkingLotFromStore.Name,
                AreaId = parkingLotFromStore.AreaId,
                HasSecurity = parkingLotFromStore.HasSecurity,
                IsPaid = parkingLotFromStore.IsPaid,
                IsStateOwned = parkingLotFromStore.IsStateOwned,
                Location = parkingLotFromStore.Location,
                TotalParkingSpaces = parkingLotFromStore.TotalParkingSpaces,
                ApplicationUserId = parkingLotFromStore.ApplicationUserId
            };

            if (!_repo.AreaExists(parkingLotToPatch.AreaId)) return NotFound("AreaId From Query is Invalid");

            patchDoc.ApplyTo(parkingLotToPatch);

            parkingLotFromStore.Name = parkingLotToPatch.Name;
            parkingLotFromStore.AreaId = parkingLotToPatch.AreaId;
            parkingLotFromStore.HasSecurity = parkingLotToPatch.HasSecurity;
            parkingLotFromStore.IsPaid = parkingLotToPatch.IsPaid;
            parkingLotFromStore.IsStateOwned = parkingLotToPatch.IsStateOwned;
            parkingLotFromStore.Location = parkingLotToPatch.Location;
            parkingLotFromStore.TotalParkingSpaces = parkingLotToPatch.TotalParkingSpaces;

            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            _repo.PatchParkingLot(parkingLotId, parkingLotFromStore);

            return NoContent();
        }

        [HttpPatch("/users/{userId}/editparkinglot/{parkingLotId}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult PartiallyUpdateParkingLot(string userId, int cityId, int areaId, int parkingLotId, [FromBody] JsonPatchDocument<ParkingLotForUpdateDto> patchDoc)
        {
            if (!_repo.UserExists(userId))
            {
                return NotFound("User not found");
            }
            var parkingLotFromStore = _repo.GetParkingLotById(parkingLotId);

            var parkingLotToPatch = new ParkingLotForUpdateDto()
            {
                Name = parkingLotFromStore.Name,
                AreaId = parkingLotFromStore.AreaId,
                HasSecurity = parkingLotFromStore.HasSecurity,
                IsPaid = parkingLotFromStore.IsPaid,
                IsStateOwned = parkingLotFromStore.IsStateOwned,
                Location = parkingLotFromStore.Location,
                TotalParkingSpaces = parkingLotFromStore.TotalParkingSpaces,
                ApplicationUserId = parkingLotFromStore.ApplicationUserId
            };

            if (!_repo.AreaExists(parkingLotToPatch.AreaId)) return NotFound("AreaId From Query is Invalid");

            patchDoc.ApplyTo(parkingLotToPatch);

            parkingLotFromStore.Name = parkingLotToPatch.Name;
            parkingLotFromStore.AreaId = parkingLotToPatch.AreaId;
            parkingLotFromStore.HasSecurity = parkingLotToPatch.HasSecurity;
            parkingLotFromStore.IsPaid = parkingLotToPatch.IsPaid;
            parkingLotFromStore.IsStateOwned = parkingLotToPatch.IsStateOwned;
            parkingLotFromStore.Location = parkingLotToPatch.Location;
            parkingLotFromStore.TotalParkingSpaces = parkingLotToPatch.TotalParkingSpaces;

            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            _repo.PatchParkingLot(parkingLotId, parkingLotFromStore);
            return NoContent();
        }

        [HttpDelete("{parkingLotId}")]
        public IActionResult DeleteParkingLot(int cityId, int areaId, int parkingLotId)
        {
            if (!_repo.CityExists(cityId)) return NotFound("City not found");

            if (!_repo.AreaExists(areaId)) return NotFound("Area not found");

            if (!_repo.ParkingLotExists(parkingLotId)) return NotFound("Lot not found");

            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }
            _repo.RemoveParkingLot(_repo.GetParkingLotById(parkingLotId));

            return NoContent();
        }


        //[HttpGet("{parkingLotId}")]
        [HttpDelete("/users/{userId}/deleteParkingLot/{parkingLotId}")]
       
        [EnableCors("AllowAnyOrigin")]
        public IActionResult DeleteParkingLotForUser(string userId, int cityId, int areaId, int parkingLotId)
        {
            if (!_repo.UserExists(userId))
            {
                return NotFound("User not found");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest("Your query is badly formatted");
            }

            _repo.RemoveParkingLot(_repo.GetParkingLotById(parkingLotId));

            return NoContent();
        }

        [HttpDelete("/users/{userId}/deleteuser")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult DeleteUser(string userId)
        {
            if (!_repo.UserExists(userId))
            {
                return NotFound("User not found");
            }

            _repo.DeleteUser(userId);
            return NoContent();
        }
    }
}
