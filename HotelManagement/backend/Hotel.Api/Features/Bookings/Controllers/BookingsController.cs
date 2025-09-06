using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Application.Features.Bookings.Interfaces;
using Hotel.Application.Features.Guests.Interfaces;
using Hotel.Api.Features.Bookings.DTOs;

namespace Hotel.Api.Features.Bookings.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookings;
        private readonly IGuestService _guests;

        public BookingsController(IBookingService bookings, IGuestService guests)
        {
            _bookings = bookings;
            _guests = guests;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateRequest req)
        {
            if (req.StartDate.Date >= req.EndDate.Date) return BadRequest("StartDate must be before EndDate");
            int guestId;
            if (req.GuestId.HasValue)
            {
                guestId = req.GuestId.Value;
            }
            else if (req.Guest != null && !string.IsNullOrWhiteSpace(req.Guest.FullName))
            {
                guestId = await _guests.CreateAsync(req.Guest.FullName, req.Guest.Phone, req.Guest.Email, req.Guest.IdProof);
            }
            else
            {
                return BadRequest("Provide guestId or guest payload");
            }

            var createdBy = int.Parse(User.FindFirst("sub")!.Value);
            var id = await _bookings.CreateAsync(req.RoomId, guestId, req.StartDate, req.EndDate, req.NightlyRate, req.Notes, createdBy);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var b = await _bookings.GetAsync(id);
            if (b is null) return NotFound();

            // Minimal response (without joins); client can fetch room/guest separately if needed.
            var dto = new BookingResponse {
                Id = b.Id,
                RoomId = b.RoomId,
                GuestId = b.GuestId,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = b.Status,
                NightlyRate = b.NightlyRate,
                Nights = b.Nights,
                Notes = b.Notes,
                CreatedByUserId = b.CreatedByUserId,
                CreatedAt = b.CreatedAt
            };
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] int? roomId = null, [FromQuery] int? guestId = null, [FromQuery] string? status = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var items = await _bookings.ListAsync(page, pageSize, roomId, guestId, status, from, to);
            var dtos = items.Select(b => new BookingResponse {
                Id = b.Id, RoomId = b.RoomId, GuestId = b.GuestId, StartDate = b.StartDate, EndDate = b.EndDate,
                Status = b.Status, NightlyRate = b.NightlyRate, Nights = b.Nights, Notes = b.Notes,
                CreatedByUserId = b.CreatedByUserId, CreatedAt = b.CreatedAt
            });
            return Ok(dtos);
        }

        [HttpPut("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _bookings.ConfirmAsync(id);
            return NoContent();
        }

        [HttpPut("{id:int}/checkin")]
        public async Task<IActionResult> CheckIn(int id)
        {
            await _bookings.CheckInAsync(id, DateTime.UtcNow);
            return NoContent();
        }

        [HttpPut("{id:int}/checkout")]
        public async Task<IActionResult> CheckOut(int id)
        {
            await _bookings.CheckOutAsync(id, DateTime.UtcNow);
            return NoContent();
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookings.CancelAsync(id);
            return NoContent();
        }
    }
}
