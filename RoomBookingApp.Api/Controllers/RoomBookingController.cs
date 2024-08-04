using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using System;
using System.Threading.Tasks;

namespace RoomBookingApp.Api.Controllers
{
    [Route("api/roomBooking")]
    [ApiController]
    public class RoomBookingController : ControllerBase
    {
        private IRoomBookingRequestProcessor roomBookingService;

        public RoomBookingController(IRoomBookingRequestProcessor roomBookingServiceMock)
        {
            this.roomBookingService = roomBookingServiceMock;
        }

        [HttpPost]
        public async Task<IActionResult> BookRoom(RoomBookingRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = roomBookingService.BookRoom(request);

                if (result.Flag == BookingResultFlag.Success)
                {
                    return Ok(result);
                }


                ModelState.AddModelError(nameof(RoomBookingRequest.Date), "No Rooms Available for given date");
            }

            return BadRequest(ModelState);
        }
    }
}
