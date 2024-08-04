using Microsoft.Extensions.Logging;
using Moq;
using RoomBookingApp.Api.Controllers;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace RoomBookingApp.Api.Tests
{
    public class UnitTest1
    {
        

        [Fact]
        public void Should_Return_Forecast_Result()
        {
            //Arrage
            var logger = new Mock<ILogger<WeatherForecastController>>();
            var controller = new WeatherForecastController(logger.Object);

            //Act
            var result = controller.Get();

            //Assert            
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(0);
        }
    }
}
