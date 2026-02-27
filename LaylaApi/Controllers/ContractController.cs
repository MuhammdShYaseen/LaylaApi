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
    [Authorize(Policy = "ConfirmedEmail")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
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
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.GetByIdAsync(id, userId, isAdmin, ct);

            if (contract == null) 
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract));
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetByBooking(int bookingId, CancellationToken ct)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.GetByBookingIdAsync(bookingId, userId, isAdmin, ct);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract));
        }

        [HttpPut("{id}/sign-owner")]
        public async Task<IActionResult> SignByOwner(int id, CancellationToken ct)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.SignContractAsync(id, userId, isAdmin, ContractSigner.Owner, ct);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract signed by owner."));
        }

        [HttpPut("{id}/sign-renter")]
        public async Task<IActionResult> SignByRenter(int id, CancellationToken ct)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.SignContractAsync(id, userId, isAdmin, ContractSigner.Renter, ct);
            if (contract == null)
                throw new KeyNotFoundException("Contract not found or access denied.");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract signed by renter."));
        }


        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContract([FromBody] ContractCreateDto model, CancellationToken ct)
        {
            var userId = CurrentUserId();
            var isAdmin = IsAdmin();

            var contract = await _contractService.GenerateContractAsync(userId, model, isAdmin, ct);
            if (contract == null)
                throw new BadHttpRequestException("Contract is not Generated");

            return Ok(ApiResponse<ContractDto>.Ok(contract, "Contract Generated"));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var isDeleted = await _contractService.DeleteAsync(id, CurrentUserId(), IsAdmin(), ct);

            if (!isDeleted)
                throw new BadHttpRequestException("Could not delete contract.");

            return Ok(ApiResponse<object>.Ok("Contract deleted successfully."));
        }

    }
}
