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
        private readonly IApartmentService _ApartmentService;
        private IMapper _Mapper;
        public MediaFilesController(IMediaFileService mediaService, IApartmentService apartmentService, IMapper mapper)
        {
            _mediaService = mediaService;
            _ApartmentService = apartmentService;
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

        private bool HasApartmentFilesAccess(Apartment apartment)
        {
            var userId = CurrentUserId();
            return apartment.OwnerId == userId || IsAdmin();
        }

        // 🟦 رفع الصور والفيديوهات لشقة
        [HttpPost("upload/{apartmentId}")]
        [Authorize]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<IActionResult> Upload(int apartmentId, List<IFormFile> files)
        {

            var result = await _mediaService.UploadFilesAsync(apartmentId, files, CurrentUserId(), IsAdmin());

            return Ok(ApiResponse<IEnumerable<MediaFile>>.Ok(result, "Files uploaded successfully."));
        }

        // 🗑️ حذف ملف
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> Delete(int id)
        {
           
            var media = await _mediaService.GetByIdAsync(id);
            if (media == null)
                return NotFound();

            // التحقق أن المستخدم هو مالك الشقة
            if (media.Apartment == null)
                return BadRequest("Apartment not found for this media.");

            if (HasApartmentFilesAccess(media.Apartment) == false)
                return Forbid("You are not allowed to delete this file.");

            // الحذف
            var rootPath = Directory.GetCurrentDirectory() + "/wwwroot";
            var result = await _mediaService.DeleteAsync(id, rootPath);

            if (!result)
                return BadRequest("Could not delete file.");

            return Ok(new { message = "File deleted" });
        }

        // 🔍 عرض ملفات شقة
        [HttpGet("mediafiles/apartment/{apartmentId}")]
        [Authorize]
        public async Task<IActionResult> GetByApartment(int apartmentId)
        {
            var result = await _mediaService.GetByApartmentIdAsync(apartmentId);
            return Ok(_Mapper.Map<IEnumerable<MediaFileDto>>(result));
        }
    }
}
