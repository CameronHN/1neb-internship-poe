using Portfolio.Core.Entities;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISavedResumeRepository
    {
        // -------------------- Create --------------------
        /// <summary>
        /// Adds a record of a saved resume for a user.
        /// </summary>
        /// <returns>ID for the newly created saved resume.</returns>
        Task<Guid> CreateAsync(SavedResume savedResume);

        // -------------------- Read --------------------
        /// <summary>
        /// Retrieves a single saved resume by its unique identifier.
        /// </summary>
        Task<SavedResume?> GetByIdAsync(Guid id, Guid userId);

        /// <summary>
        /// Gets all saved resumes associated with a specific user.
        /// </summary>
        Task<List<SavedResume>> GetAllByUserIdAsync(Guid userId);

        // -------------------- Delete --------------------
        /// <summary>
        /// Deletes a saved resume for a user by its ID.
        /// </summary>
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
