﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parkopolis.API.Models
{
    public class ParkingLot
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsPaid { get; set; }
        public bool IsStateOwned { get; set; }
        public int TotalParkingSpaces { get; set; }
        public bool HasSecurity { get; set; }
        public int UserId { get; set; }
        //public DateTime FreeInterval { get; set; }

        //Navigation Properties
        //public Area Area { get; set; }
        //public User User { get; set; }
        public ICollection<ParkingSpace> ParkingSpaces { get; set; }

    }
}