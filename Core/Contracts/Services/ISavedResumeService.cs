using Portfolio.Core.DTOs.SavedResume;

namespace Portfolio.Core.Contracts.Services
{
    public interface ISavedResumeService
    {
        /// <summary>
        /// Saves a new resume for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="request">The resume data to save, including name, template type, and resume content.</param>
        /// <returns>The unique identifier of the newly created saved resume.</returns>
        Task<Guid> SaveResumeAsync(Guid userId, SaveResumeDataRequest request);

        /// <summary>
        /// Retrieves a specific saved resume by its identifier for a user.
        /// </summary>
        /// <returns>The saved resume details if found and owned by the user; otherwise, null.</returns>
        Task<SavedResumeDetail?> GetSavedResumeByIdAsync(Guid id, Guid userId);

        /// <summary>
        /// Retrieves all saved resumes for a specific user.
        /// </summary>
        /// <returns>A list of saved resume details (excluding the data) for the user.</returns>
        Task<List<SavedResumeListItem>> GetAllSavedResumesByUserIdAsync(Guid userId);

        /// <summary>
        /// Deletes a saved resume for a user.
        /// </summary>
        /// <returns>True if the resume was successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteSavedResumeAsync(Guid id, Guid userId);

        /// <summary>
        /// Generates a PDF document from a saved resume.
        /// </summary>
        /// <returns>A byte array containing the generated PDF document.</returns>
        Task<byte[]> GeneratePdfFromSavedResumeAsync(Guid id, Guid userId);
    }
}
