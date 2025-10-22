using Portfolio.Core.DTOs.Contact;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IContactRepository
    {
        Task AddContactsAsync(List<AddContact> contacts);
    }
}
