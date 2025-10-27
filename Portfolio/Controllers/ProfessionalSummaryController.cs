using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
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
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddSummary([FromBody] AddSummary summary)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            summary.UserId = userId.Value;

            var summaryId = await _summaryService.AddSummaryAsync(summary);
            return Created(string.Empty, summaryId);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchSummaries([FromBody] List<PatchSummary> patches)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _summaryService.PatchSummariesAsync(userId.Value, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }
    }
}
