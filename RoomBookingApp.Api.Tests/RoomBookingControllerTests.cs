using Microsoft.AspNetCore.Mvc;
using Moq;
using RoomBookingApp.Api.Controllers;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;
using System;
using Xunit;

namespace RoomBookingApp.Api.Tests
{
    public class RoomBookingControllerTests
    {
        private readonly Mock<IRoomBookingRequestProcessor> _roomBookingServiceMock;
        private readonly RoomBookingController _roomBookingController;
        private RoomBookingRequest _request;
        private RoomBookingResult _result;

        public RoomBookingControllerTests()
        {
            _roomBookingServiceMock = new Mock<IRoomBookingRequestProcessor>();
            _roomBookingController = new RoomBookingController(_roomBookingServiceMock.Object);
            _request = new RoomBookingRequest();
            _result = new RoomBookingResult();

            _roomBookingServiceMock.Setup(x => x.BookRoom(_request))
                                    .Returns(_result);


        }

        [Theory]
        [InlineData(1, true, typeof(OkObjectResult), BookingResultFlag.Success)]
        [InlineData(0, false, typeof(BadRequestObjectResult), BookingResultFlag.Failure)]
        public async void Should_Call_Booking_Method_when_Valid(
            int expectedMethodCalls,
            bool isModelValid,
            Type expectedActiveResultType,
            BookingResultFlag bookingResultFlag)
        {

              //Arrange
            if (!isModelValid)
            {
                _roomBookingController.ModelState.AddModelError("test", "ErrorMessage");
            }

            _result.Flag = bookingResultFlag;

            //Act
            var result = await _roomBookingController.BookRoom(_request);

            //Assert
            result.ShouldBeOfType(expectedActiveResultType);
            _roomBookingServiceMock.Verify(x => x.BookRoom(_request), Times.Exactly(expectedMethodCalls));
        }

    }
}
