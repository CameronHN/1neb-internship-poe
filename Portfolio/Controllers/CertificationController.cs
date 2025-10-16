using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

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

        [HttpPost("certifications")]
        [ProducesResponseType(typeof(List<CertificationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCertsByIds([FromBody] ItemListRequest request)
        {
            var experiences = await _certificationService.GetAllCertsByIds(request);
            return Ok(experiences);
        }
    }
}
