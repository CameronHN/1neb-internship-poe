using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.SavedResume;
using Portfolio.WebApi.Extensions;
using Portfolio.WebApi.Helper;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SavedResumeController : ControllerBase
    {
        private readonly ISavedResumeService _savedResumeService;

        public SavedResumeController(ISavedResumeService savedResumeService)
        {
            _savedResumeService = savedResumeService;
        }

        /// <summary>
        /// Save a resume snapshot with all its data as JSON
        /// </summary>
        [HttpPost("save")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> SaveResume([FromBody] SaveResumeDataRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var savedResumeId = await _savedResumeService.SaveResumeAsync(userId.Value, request);

            return Created(string.Empty, savedResumeId);
        }

        /// <summary>
        /// Get all saved resumes for the current user
        /// </summary>
        [HttpGet("list")]
        [ProducesResponseType(typeof(List<SavedResumeListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllSavedResumes()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var savedResumes = await _savedResumeService.GetAllSavedResumesByUserIdAsync(
                userId.Value
            );

            return Ok(savedResumes);
        }

        /// <summary>
        /// Generate PDF from a saved resume snapshot
        /// </summary>
        [HttpGet("{id}/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSavedResumePdfById(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var pdf = await _savedResumeService.GetSavedResumePdfFromId(id, userId.Value);

            var savedResume = await _savedResumeService.GetSavedResumeByIdAsync(id, userId.Value);
            string fileDownloadName =
                savedResume != null
                    ? FileNameHelper.FileNameFormatter(savedResume.Name)
                    : "resume.pdf";

            return File(pdf, "application/pdf", fileDownloadName);
        }

        /// <summary>
        /// Delete a saved resume
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSavedResume(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var deleted = await _savedResumeService.DeleteSavedResumeAsync(id, userId.Value);

            if (!deleted)
                return BadRequest("Failed to delete the saved resume.");

            return Ok(true);
        }
    }
}
