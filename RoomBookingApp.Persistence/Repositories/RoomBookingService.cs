using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoomBookingApp.Persistence.Repositories
{
    public class RoomBookingService : IRoomBookingService
    {
        private readonly RoomBookingAppDbContext _dbcontext;

        public RoomBookingService(RoomBookingAppDbContext dbcontext)
        {
            this._dbcontext = dbcontext;
        }

        public IEnumerable<Room> GetAvailableRooms(DateTime date)
        {


            //var unAvailableRooms = _dbcontext.RoomBookings.Where(q => q.Date == date)
            //                                                .Select(q => q.RoomId)
            //                                                .ToList();

            //var availableRooms = _dbcontext.Rooms.Where(q => unAvailableRooms.Contains(q.Id) == false);


            var availableRooms = _dbcontext.Rooms
                                            .Where(q=> q.RoomBooking.Any(a=> a.Date == date)).ToList();

            return availableRooms;
        }

        public void Save(RoomBooking roomBooking)
        {
            _dbcontext.RoomBookings.Add(roomBooking);
            _dbcontext.SaveChanges();
        }
    }
}
