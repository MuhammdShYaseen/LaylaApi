using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IBookingService _bookingService;
        private readonly IApartmentService _apartmentService;
        private readonly IUserService _userService;
        public ContractController(IContractService contractService, IBookingService bookingService, IApartmentService apartmentService, IUserService userService)
        {
            _contractService = contractService;
            _bookingService = bookingService;
            _apartmentService = apartmentService;
            _userService = userService;

        }
        private int CurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 0;
        }
        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role != null && role.ToLower() == "admin";
        }
        private bool HasContractAccess(Booking booking, Apartment apartment)
        {
            var userId = CurrentUserId();
            return booking.UserId == userId || apartment.OwnerId == userId || IsAdmin();
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            var userId = CurrentUserId();

            // Only renter, owner, or admin can access it
            var booking = await _bookingService.GetEntityByIdAsync(contract.BookingId);
            if (booking == null) return NotFound();

            var apartment = await _apartmentService.GetEntityByIdAsync(booking.ApartmentId);
            if (apartment == null) return NotFound();

            var hasAccess = HasContractAccess(booking, apartment);
            if (!hasAccess)
            {
                return Forbid("You do not have access to this contract.");
            }

            return Ok(contract);
        }

        [HttpGet("booking/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetByBooking(int bookingId)
        {
            var contract = await _contractService.GetByBookingIdAsync(bookingId);
            if (contract == null) return NotFound();

            var booking = await _bookingService.GetEntityByIdAsync(bookingId);
            if (booking == null) return NotFound();

            var apartment = await _apartmentService.GetEntityByIdAsync(booking.ApartmentId);
            if (apartment == null) return NotFound();

            var userId = CurrentUserId();

            var hasAccess = HasContractAccess(booking, apartment);
            if (!hasAccess)
            {
                return Forbid("You do not have access to this contract.");
            }

            return Ok(contract);
        }

        [HttpPut("{id}/sign-owner")]
        [Authorize]
        public async Task<IActionResult> SignByOwner(int id)
        {
            var userId = CurrentUserId();

            var contract = await _contractService.GetEntityByIdAsync(id);
            if (contract == null) return NotFound("contract");

            var booking = await _bookingService.GetEntityByIdAsync(contract.BookingId);
            if (booking == null) return NotFound("booking");
               
            var apartment = await _apartmentService.GetEntityByIdAsync(booking.ApartmentId);
            if (apartment == null) return NotFound("apartment");

            if (apartment?.OwnerId != userId && !IsAdmin())
                return Forbid("Only the apartment owner can sign this contract.");

            contract.IsSignedByOwner = true;
            
            var signed = await _contractService.SignContractAsync(id, userId);
            return Ok(signed);
        }
        [HttpPut("{id}/sign-renter")]
        [Authorize]
        public async Task<IActionResult> SignByRenter(int id)
        {
            var contract = await _contractService.GetEntityByIdAsync(id);
            if (contract == null) return NotFound();

            var booking = await _bookingService.GetEntityByIdAsync(contract.BookingId);
            if (booking == null) return NotFound();

            var apartment = await _apartmentService.GetEntityByIdAsync(booking.ApartmentId);
            if (apartment == null) return NotFound();

            var userId = CurrentUserId();

            if (booking.UserId != userId && !IsAdmin())
                return Forbid("Only the renter can sign this contract.");

            contract.IsSignedByRenter = true;

            var signed = await _contractService.UpdateEntityAsync(contract);
            return Ok(signed);
        }
        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> GenerateContract([FromBody] ContractCreateDto model)
        {
            var userId = CurrentUserId();

            var booking = await _bookingService.GetEntityByIdAsync(model.BookingId);
            if (booking == null) return NotFound();

            var apartment = await _apartmentService.GetEntityByIdAsync(booking.ApartmentId);
            var renter = await _userService.GetByIdAsync(booking.UserId);

            if (apartment == null) return NotFound();
            if (renter == null) return NotFound();
            var owner = await _userService.GetByIdAsync(apartment.OwnerId);

            // إنشاء عقد DB
            if (owner == null) return NotFound();
            var contract = new Contract
            {
                BookingId = booking.Id,
                SpecialTerms = model.SpecialTerms ?? ""
            };

            contract = await _contractService.AddEntityAsync(contract);

            // إنشاء PDF وحفظه في wwwroot
            
            string pdfUrl = _contractService.GenerateContractPdf(contract, booking, apartment, renter, owner , 
                                                                 model.SpecialTerms ?? "");
            // حفظ الرابط DB
            contract.ContractUrl = pdfUrl;
            await _contractService.UpdateEntityAsync(contract);

            return Ok(contract);
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            var booking = await _bookingService.GetByIdAsync(contract.BookingId);
            if (booking == null) return NotFound();

            var apartment = await _apartmentService.GetByIdAsync(booking.ApartmentId);
            if (apartment == null) return NotFound();
            var userId = CurrentUserId();

            if (apartment.OwnerId != userId && !IsAdmin())
                return Forbid("Only the owner can delete this contract.");

            var success = await _contractService.DeleteAsync(id);
            if (!success) return BadRequest(new { message = "Could not delete contract." });

            return Ok(new { message = "Contract deleted." });
        }

    }
}
