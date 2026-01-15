using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LaylaApi.Services.DataCRUD.Implementations.ContractService;

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

            var contract = await _contractService.SignContractAsync(id, userId, isAdmin, ContractSigner.Owner);
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

            var contract = await _contractService.SignContractAsync(id, userId, isAdmin, ContractSigner.Renter);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract signed by renter."));
        }


        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> GenerateContract([FromBody] ContractCreateDto model)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.GenerateContractAsync(userId, model, isAdmin);
            if (contract == null)
                throw new BadHttpRequestException("Contract is not Generated");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract Generated"));
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _contractService.DeleteAsync(id, CurrentUserId(), IsAdmin());

            if (!isDeleted)
                throw new BadHttpRequestException("Could not delete contract.");

            return Ok(ApiResponse<object>.Ok("Contract deleted successfully."));
        }

    }
}
