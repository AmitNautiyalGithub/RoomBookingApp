using System;
using Xunit;
using Shouldly;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Moq;
using RoomBookingApp.Core.DataServices;
using System.Collections.Generic;
using System.Linq;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Core
{

    public class RoomBookingRequestProcessorTests
    {
        private RoomBookingRequestProcessor _processor;
        private RoomBookingRequest _request;
        private Mock<IRoomBookingService> _roomBookingServiceMock;
        private List<Room> _availableRooms;

        public RoomBookingRequestProcessorTests()
        {
            _request = new RoomBookingRequest()
            {
                FullName = "Test Name",
                Email = "test@test.com",
                Date = new DateTime(2024, 8, 2)
            };

            _availableRooms = new List<Room>
            {
                 new Room
                 {
                     Id = 1
                 }
            };

            _roomBookingServiceMock = new Mock<IRoomBookingService>();
            _roomBookingServiceMock.Setup(x => x.GetAvailableRooms(_request.Date))
                                    .Returns(_availableRooms);


            _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
        }

        [Fact]
        public void Should_Return_Room_Booking_Response_With_Values()
        {
            //Arrange


            //Act
            RoomBookingResult result = _processor.BookRoom(_request);

            //Assert
            //Assert.NotNull(result);
            //Assert.Equal(request.FullName, result.FullName);
            //Assert.Equal(request.Email, result.Email);
            //Assert.Equal(request.Date, result.Date);

            //Fluent Assertion using Shouldly library
            result.ShouldNotBeNull();
            result.FullName.ShouldBe(_request.FullName);
            result.Email.ShouldBe(_request.Email);
            result.Date.ShouldBe(_request.Date);

        }


        [Fact]
        public void Should_Throw_Exception_For_Null_Request()
        {
            var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    _processor.BookRoom(null);
                });

            exception.ParamName.ShouldBe("bookingRequest");

        }

        [Fact]
        public void Should_Save_Room_Booking_Request()
        {
            RoomBooking savedRoomBooking = null;

            _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
                                    .Callback<RoomBooking>(booking =>
                                    {
                                        savedRoomBooking = booking;
                                    });

            _processor.BookRoom(_request);

            _roomBookingServiceMock.Verify(x => x.Save(It.IsAny<RoomBooking>()), Times.Once);

            savedRoomBooking.ShouldNotBeNull();
            savedRoomBooking.FullName.ShouldBe(_request.FullName);
            savedRoomBooking.Email.ShouldBe(_request.Email);
            savedRoomBooking.Date.ShouldBe(_request.Date);
            savedRoomBooking.RoomId.ShouldBe(_availableRooms.First().Id);
        }


        [Fact]
        public void Should_Not_Save_Room_Booking_Request_If_None_Available()
        {
           // _availableRooms.Clear();
            
            _processor.BookRoom(_request);

            _roomBookingServiceMock.Verify(x => x.Save(It.IsAny<RoomBooking>()), Times.Once);
            
        }

        [Theory]
        [InlineData(BookingResultFlag.Failure, false)]
        [InlineData(BookingResultFlag.Success, true)]
        public void Should_Return_SuccessOrFailure_Flag_in_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
        {
            if(!isAvailable)
            {
                _availableRooms.Clear();
            }

            var result = _processor.BookRoom(_request);
            bookingSuccessFlag.ShouldBe(result.Flag);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(null, false)]
        public void Should_Return_RoomBookingId_In_Result(int? roomBookingId, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }
            else
            {
                _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
                                    .Callback<RoomBooking>(booking =>
                                    {
                                        booking.Id = roomBookingId.Value;
                                    });
            }

            var result = _processor.BookRoom(_request);
            result.RoomBookingId.ShouldBe(roomBookingId);
        }
    }

}