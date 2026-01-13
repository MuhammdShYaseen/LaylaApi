using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
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
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.GetByIdAsync(id, userId, isAdmin);

            if (contract == null) 
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract));
        }

        [HttpGet("booking/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetByBooking(int bookingId)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.GetByBookingIdAsync(bookingId, userId, isAdmin);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract));
        }

        [HttpPut("{id}/sign-owner")]
        [Authorize]
        public async Task<IActionResult> SignByOwner(int id)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.SignContractAsync(id, userId, isAdmin);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract signed by owner."));
        }

        [HttpPut("{id}/sign-renter")]
        [Authorize]
        public async Task<IActionResult> SignByRenter(int id)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.SignContractAsync(id, userId, isAdmin);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract signed by renter."));
        }


        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> GenerateContract([FromBody] ContractCreateDto model)
        {
            var userId = CurrentUserId();

            var booking = await _bookingService.GetEntityByIdAsync(model.BookingId);
            if (booking == null) 
                throw new KeyNotFoundException();

            var apartment = await _apartmentService.GetEntityByIdAsync(booking.ApartmentId);
            var renter = await _userService.GetByIdAsync(booking.UserId);

            if (apartment == null) 
                throw new KeyNotFoundException();

            if (renter == null) 
                throw new KeyNotFoundException();

            var owner = await _userService.GetByIdAsync(apartment.OwnerId);

            // إنشاء عقد DB
            if (owner == null) 
                throw new KeyNotFoundException();

            var contract = await _contractService.AddEntityAsync(booking.Id, model.SpecialTerms ?? "");

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
            if (contract == null) 
                throw new KeyNotFoundException();

            var booking = await _bookingService.GetByIdAsync(contract.BookingId);
            if (booking == null) 
                throw new KeyNotFoundException();

            var apartment = await _apartmentService.GetByIdAsync(booking.ApartmentId);
            if (apartment == null) 
                throw new KeyNotFoundException();

            var userId = CurrentUserId();

            if (apartment.OwnerId != userId && !IsAdmin())
                throw new UnauthorizedAccessException("Only the owner can delete this contract.");

            var success = await _contractService.DeleteAsync(id);
            if (!success) throw new BadHttpRequestException("Could not delete contract.");

            return Ok(new { message = "Contract deleted." });
        }

    }
}
