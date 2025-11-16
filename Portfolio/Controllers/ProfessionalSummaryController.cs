using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalSummaryController : ControllerBase
    {
        private readonly IProfessionalSummaryService _summaryService;

        public ProfessionalSummaryController(IProfessionalSummaryService summaryService)
        {
            _summaryService = summaryService;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddSummaries([FromBody] List<AddSummary> summaries)
        {
            var userId = User.GetUserId()!.Value;

            if (summaries is null)
                throw new ValidationException("Request body cannot be null.");

            var summaryIds = await _summaryService.AddSummariesAsync(userId, summaries);
            return Created(string.Empty, summaryIds);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchSummary([FromBody] PatchSummary patch)
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                throw new Portfolio.Core.Exceptions.ValidationException(
                    "Request body cannot be null."
                );

            var updated = await _summaryService.PatchSummaryAsync(userId, patch);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSummaryById([FromRoute] Guid id)
        {
            var summary = await _summaryService.GetProfessionalSummaryById(id);
            if (summary is null)
                return BadRequest();

            return Ok(summary);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProfessionalSummaries(
            [FromBody] List<Guid> summaryIds
        )
        {
            var userId = User.GetUserId()!.Value;

            if (summaryIds == null || summaryIds.Count == 0)
                return BadRequest("Summary IDs cannot be null or empty.");

            var deleted = await _summaryService.DeleteProfessionalSummariesAsync(
                userId,
                summaryIds
            );
            if (!deleted)
                return BadRequest("Failed to delete the specified professional summaries.");

            return Ok(true);
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfessionalSummary()
        {
            var userId = User.GetUserId()!.Value;

            var summary = await _summaryService.GetSummariesByUserId(userId);
            return Ok(summary);
        }
    }
}
