using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using LaylaApi.Services.MediaStorageProviderServices.Interfaces;
using static LaylaApi.Services.MediaStorageProviderServices.Implementation.CloudinaryStorageProvider;
using System.Security.Claims;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using System.Security.Cryptography.Xml;

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
        [Authorize]
        public async Task<ActionResult<UploadSignatureDto>> CreateUploadSignature([FromQuery] int apartmentId)
        {
             var signature = await _storageProvider.CreateUploadSignatureAsync(CurrentUserId(), apartmentId, IsAdmin());
             return Ok(ApiResponse<UploadSignatureDto>.Ok(signature));
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var result = await _storageProvider.ProcessWebhookAsync(Request);

            return result switch
            {
                WebhookResult.Unauthorized => Unauthorized(),
                WebhookResult.Invalid => BadRequest(),
                _ => Ok()
            };
        }

        [HttpDelete("{mediaId:int}")]
        public async Task<IActionResult> DeleteMedia(int mediaId)
        {
            
                bool result = await _storageProvider.DeleteAsync(mediaId, CurrentUserId(), IsAdmin());
                return Ok(ApiResponse<object>.Ok("Deleted"));

        }

    }
}
