using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        [Authorize]
        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddCertification(
            [FromBody] List<AddCertification> certification
        )
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var certificationIds = await _certificationService.AddCertificationAsync(userId.Value, certification);
            return Created(string.Empty, certificationIds);
        }

        [HttpPost("certifications")]
        [ProducesResponseType(typeof(List<CertificationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCertsByIds([FromBody] ItemListRequest request)
        {
            var experiences = await _certificationService.GetAllCertsByIds(request);
            return Ok(experiences);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchCertifications(
            [FromBody] List<PatchCertification> patches
        )
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _certificationService.PatchCertificationsAsync(
                userId.Value,
                patches
            );
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCertifications(
            [FromBody] List<Guid> certificationIds
        )
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (certificationIds == null || certificationIds.Count == 0)
                return BadRequest("Certification IDs cannot be null or empty.");

            var deleted = await _certificationService.DeleteCertificationsAsync(
                userId.Value,
                certificationIds
            );
            if (!deleted)
                return BadRequest("Failed to delete the specified certifications.");

            return Ok(true);
        }

        [HttpGet("certifications")]
        [ProducesResponseType(typeof(List<CertificationItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCertifications()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var certifications = await _certificationService.GetCertificationsByUserIdAsync(
                userId.Value
            );
            return Ok(certifications);
        }
    }
}
