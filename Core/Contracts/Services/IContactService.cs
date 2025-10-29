using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;

namespace Portfolio.Core.Contracts.Services
{
    public interface IContactService
    {
        Task<List<Guid>> AddContactsAsync(Guid userId, List<AddContact> contacts);

        Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request);

    }
}