using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LaylaApi.Services.MediaStorageProviderServices.Interfaces;
using static LaylaApi.Services.MediaStorageProviderServices.Implementation.CloudinaryStorageProvider;
using System.Security.Claims;
using LaylaApi.Models.GenericResponseModels;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryMediaStorageController :ControllerBase
    {
        private readonly IStorageProvider _storageProvider;
        public CloudinaryMediaStorageController(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        private int CurrentUserId()=>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role != null && role.ToLower() == "admin";
        }

        [HttpPost("signature")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<ActionResult<UploadSignatureDto>> CreateUploadSignature([FromQuery] int apartmentId, CancellationToken ct)
        {
             var signature = await _storageProvider.CreateUploadSignatureAsync(CurrentUserId(), apartmentId, IsAdmin(), ct);
             return Ok(ApiResponse<UploadSignatureDto>.Ok(signature));
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook(CancellationToken ct)
        {
            var result = await _storageProvider.ProcessWebhookAsync(Request,ct);

            return result switch
            {
                WebhookResult.Unauthorized => Unauthorized(),
                WebhookResult.Invalid => BadRequest(),
                _ => Ok()
            };
        }

        [HttpDelete("{mediaId:int}")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> DeleteMedia(int mediaId, CancellationToken ct)
        {
            
            bool result = await _storageProvider.DeleteAsync(mediaId, CurrentUserId(), IsAdmin(),ct);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("can not delete this media"));

            return Ok(ApiResponse<object>.Ok("Deleted"));

        }

    }
}
