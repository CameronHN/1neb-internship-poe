using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddContacts([FromBody] List<AddContact> contacts)
        {
            var userId = User.GetUserId()!.Value;

            if (contacts is null)
                throw new ValidationException("Request body cannot be null.");

            var addedIds = await _contactService.AddContactsAsync(userId, contacts);
            return Created(string.Empty, addedIds);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SocialMediaItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContactById([FromRoute] Guid id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        [HttpGet("contacts")]
        [ProducesResponseType(typeof(List<ContactModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetContacts()
        {
            var userId = User.GetUserId()!.Value;

            var contacts = await _contactService.GetContactsByUserIdAsync(userId);
            return Ok(contacts);
        }

        [HttpPost("contacts")]
        [ProducesResponseType(typeof(List<SocialMediaItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllContactsByIds([FromBody] ItemListRequest request)
        {
            var contacts = await _contactService.GetContactsByIdsAsync(request);
            return Ok(contacts);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchContacts([FromBody] List<PatchContact> patches)
        {
            var userId = User.GetUserId()!.Value;

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _contactService.PatchContactsAsync(userId, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteContacts([FromBody] List<Guid> contactIds)
        {
            var userId = User.GetUserId()!.Value;

            if (contactIds == null || !contactIds.Any())
                return BadRequest("Contact IDs cannot be null or empty.");

            var deleted = await _contactService.DeleteContactsAsync(userId, contactIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified contacts.");

            return Ok(true);
        }
    }
}
