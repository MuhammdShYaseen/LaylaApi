using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Implementations;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaFilesController : ControllerBase
    {
        private readonly IMediaFileService _mediaService;
        private readonly IMapper _Mapper;
        public MediaFilesController(IMediaFileService mediaService,  IMapper mapper)
        {
            _mediaService = mediaService;
            _Mapper = mapper;
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


        // 🟦 رفع الصور والفيديوهات لشقة
        [HttpPost("upload/{apartmentId}")]
        [Authorize(Policy = "ConfirmedEmail")]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<IActionResult> Upload(int apartmentId, List<IFormFile> files)
        {

            var result = await _mediaService.UploadFilesAsync(apartmentId, files, CurrentUserId(), IsAdmin());

            return Ok(ApiResponse<IEnumerable<MediaFile>>.Ok(result, "Files uploaded successfully."));
        }

        // 🗑️ حذف ملف
        [HttpDelete("{id}")]
        [Authorize(Policy = "ConfirmedEmail")]

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediaService.DeleteAsync( id, CurrentUserId(), IsAdmin());

            return Ok(ApiResponse<object>.Ok("File deleted successfully."));
        }

        // 🔍 عرض ملفات شقة
        [HttpGet("mediafiles/apartment/{apartmentId}")]
        public async Task<IActionResult> GetByApartment(int apartmentId)
        {
            var result = await _mediaService.GetByApartmentIdAsync(apartmentId);
            return Ok(_Mapper.Map<IEnumerable<MediaFileDto>>(result));
        }
    }
}
