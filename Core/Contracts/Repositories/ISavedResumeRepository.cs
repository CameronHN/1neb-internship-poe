using Portfolio.Core.DTOs.SavedResume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISavedResumeRepository
    {
        // -------------------- Create --------------------
        /// <summary>
        /// Adds a record of a saved resume for a user.
        /// </summary>
        /// <returns>ID for the newly created saved resume.</returns>
        Task<Guid> CreateAsync(Guid userId, AddSavedResume savedResume);

        // -------------------- Read --------------------
        /// <summary>
        /// Retrieves a single saved resume by its unique identifier.
        /// </summary>
        Task<SavedResumeModel?> GetByIdAsync(Guid userId, Guid id);

        /// <summary>
        /// Gets all saved resumes associated with a specific user.
        /// </summary>
        Task<List<SavedResumeItem>> GetAllByUserIdAsync(Guid userId);

        // -------------------- Delete --------------------
        /// <summary>
        /// Deletes a saved resume for a user by its ID.
        /// </summary>
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
