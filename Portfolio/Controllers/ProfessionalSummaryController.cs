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
    }
}
