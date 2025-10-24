using Portfolio.Core.DTOs.Contact;

namespace Portfolio.Core.Contracts.Services
{
    public interface IContactService
    {
        Task<List<Guid>> AddContactsAsync(List<AddContact> contacts);
    }
}