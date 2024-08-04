using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Domain;
using RoomBookingApp.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RoomBookingApp.Persistence.Tests
{
    public class RoomBookingServiceTests
    {

        public RoomBookingServiceTests()
        {

        }

        [Fact]
        public void Should_Return_Available_Room()
        {
            //Arrage
            var date = DateTime.Now;

            var options = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("AvailableRoomTest").Options;

            using (var context = new RoomBookingAppDbContext(options))
            {
                context.Rooms.AddRange(
                        new Room { Id = 1, Name = "Conference Room A" },
                        new Room { Id = 2, Name = "Conference Room B" },
                        new Room { Id = 3, Name = "Conference Room C" });

                context.RoomBookings.Add(new RoomBooking { RoomId = 1, Date = date });
                context.RoomBookings.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1) });

                context.SaveChanges();

                RoomBookingService roomBookingService = new RoomBookingService(context);

                //Act
                var availableRooms = roomBookingService.GetAvailableRooms(date);


                //Assert
                Assert.Equal(2, availableRooms.Count());
                Assert.Contains(availableRooms, q => q.Id == 2);
                Assert.Contains(availableRooms, q => q.Id == 3);
                Assert.DoesNotContain(availableRooms, q => q.Id == 1);

            }
        }

        [Fact]
        public void Should_Save_Room_Booking()
        {
            var options = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                           .UseInMemoryDatabase("ShouldSaveTest").Options;

            var roomBooking = new RoomBooking { RoomId = 1, Date = DateTime.Now, FullName = "Amit Nauityal" };
                       

            using (var context = new RoomBookingAppDbContext(options))
            {
                RoomBookingService roomBookingService = new RoomBookingService(context);
                roomBookingService.Save(roomBooking);

                var bookings = context.RoomBookings.ToList();

                var booking = Assert.Single(bookings);

                Assert.Equal(roomBooking.Date, booking.Date);
                Assert.Equal(roomBooking.RoomId, booking.RoomId);
            }
        }
    }
}
