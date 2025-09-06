using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Application.Features.Billing.Interfaces;
using Hotel.Api.Features.Billing.DTOs;
using Hotel.Infrastructure.Features.Billing.Services;

namespace Hotel.Api.Features.Billing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoices;

        public InvoicesController(IInvoiceService invoices)
        {
            _invoices = invoices;
        }

        // POST /api/invoices/from-booking/{bookingId}
        [HttpPost("from-booking/{bookingId:int}")]
        public async Task<IActionResult> CreateFromBooking(int bookingId)
        {
            var id = await _invoices.CreateForBookingAsync(bookingId);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var inv = await _invoices.GetAsync(id);
            if (inv is null) return NotFound();
            return Ok(new InvoiceResponse {
                Id = inv.Id,
                InvoiceNumber = inv.InvoiceNumber,
                BookingId = inv.BookingId,
                Nights = inv.Nights,
                NightlyRate = inv.NightlyRate,
                Subtotal = inv.Subtotal,
                Status = inv.Status,
                CreatedAt = inv.CreatedAt,
                PaidAt = inv.PaidAt
            });
        }

        // GET /api/invoices?bookingId=&status=&page=&pageSize=
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] int? bookingId = null, [FromQuery] string? status = null)
        {
            var items = await _invoices.ListAsync(page, pageSize, bookingId, status);
            var dtos = items.Select(inv => new InvoiceResponse {
                Id = inv.Id,
                InvoiceNumber = inv.InvoiceNumber,
                BookingId = inv. BookingId,
                Nights = inv.Nights,
                NightlyRate = inv.NightlyRate,
                Subtotal = inv.Subtotal,
                Status = inv.Status,
                CreatedAt = inv.CreatedAt,
                PaidAt = inv.PaidAt
            });
            return Ok(dtos);
        }

        // PUT /api/invoices/{id}/pay
        [HttpPut("{id:int}/pay")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            await _invoices.MarkPaidAsync(id);
            return NoContent();
        }

        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> GetPdf(int id)
        {
            var inv = await _invoices.GetAsync(id);
            if (inv is null) return NotFound();

            var pdfBytes = InvoicePdfService.Generate(inv);
            return File(pdfBytes, "application/pdf", $"{inv.InvoiceNumber}.pdf");
        }
    }
}
