﻿using Microsoft.AspNetCore.Mvc;
using Parkopolis.API.MockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parkopolis.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Parkopolis.API.MockData;

namespace Parkopolis.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/areas/{areaId}/parkinglots")]
    public class ParkingLotsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetParkingLots(int cityId, int areaId)
        {
            if (!Validation.CityExists(cityId) || !Validation.AreaExists(cityId, areaId)) return NotFound();

            if (!Validation.ParkingLotExists(areaId)) return NotFound();

            return Ok(ParkingLotsDataStore.CurrentParkingLots.ParkingLots.FindAll(p => p.AreaId == areaId));
        }

        [HttpGet("{parkingLotId}")]
        public IActionResult GetParkingLot(int cityId, int areaId, int parkingLotId)
        {
            if (!Validation.CityExists(cityId) || !Validation.AreaExists(cityId, areaId) || !Validation.SingleParkingLotExists(parkingLotId)) return NotFound();

            if (!Validation.ParkingLotExists(areaId)) return NotFound();

            return Ok(ParkingLotsDataStore.CurrentParkingLots.ParkingLots.FirstOrDefault(p => p.Id == parkingLotId));
        }

        [HttpPost]
        public void CreateParkingLot(int areaid, [FromBody] ParkingLotForCreationDto parkingLot)
        {
            var maxParkingLotId = ParkingLotsDataStore.CurrentParkingLots.ParkingLots.Max(p => p.Id);

            var newParkingLot = new ParkingLotDto()
            {
                Id = maxParkingLotId + 1,
                Name = parkingLot.Name,
                AreaId = parkingLot.AreaId,
                HasSecurity = parkingLot.HasSecurity,
                IsPaid = parkingLot.IsPaid,
                IsStateOwned = parkingLot.IsStateOwned,
                Location = parkingLot.Location,
                TotalParkingSpaces = parkingLot.TotalParkingSpaces
            };

            ParkingLotsDataStore.CurrentParkingLots.ParkingLots.Add(newParkingLot);
        }

        public void UpdateParkingLot(int areaId, [FromBody] ParkingLotForCreationDto parkingLot)
        {

        }

        public void PartiallyUpdateParkingLot(int areaId, int id, [FromBody] JsonPatchDocument<ParkingLotForUpdateDto> patchDoc)
        {

        }

        public void DeleteParkingLot(int areaId, int id)
        {

        }
    }

}
