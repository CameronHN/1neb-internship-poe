using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.Models;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddCertification(
            [FromBody] List<AddCertification> certification
        )
        {
            var userId = User.GetUserId()!.Value;

            if (certification is null)
                throw new ValidationException("Request body cannot be null.");

            var certificationIds = await _certificationService.AddCertificationAsync(
                userId,
                certification
            );
            return Created(string.Empty, certificationIds);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchCertification([FromBody] PatchCertification patch)
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                throw new ValidationException("Request body cannot be null.");

            var updated = await _certificationService.PatchCertificationAsync(userId, patch);
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
            var userId = User.GetUserId()!.Value;

            if (certificationIds == null || certificationIds.Count == 0)
                return BadRequest("Certification IDs cannot be null or empty.");

            var deleted = await _certificationService.DeleteCertificationsAsync(
                userId,
                certificationIds
            );
            if (!deleted)
                return BadRequest("Failed to delete the specified certifications.");

            return Ok(true);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CertificationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCertificationById([FromRoute] Guid id)
        {
            var userId = User.GetUserId()!.Value;
            var title = await _certificationService.GetCertificationByIdAsync(id, userId);
            if (title is null)
                return BadRequest();

            return Ok(title);
        }
    }
}
