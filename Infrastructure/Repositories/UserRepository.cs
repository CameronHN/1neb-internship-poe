using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.User;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Models;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser?> GetUserById(Guid id)
        {
            ApplicationUser? user = await _dbContext.User.FindAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            return user;
        }

        public async Task UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var existingUser = await _dbContext.User.FindAsync(updateUserDTO.UserId);
            if (existingUser == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            existingUser.PhoneNumber = updateUserDTO.PhoneNumber;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _dbContext.User.FindAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            _dbContext.User.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GetAllResumeDetails?> GetAllResumeDetailsByUserId(Guid userId)
        {
            var user = await _dbContext
                .User.Where(u => u.Id == userId)
                .AsSplitQuery()
                .Include(u => u.Titles)
                .Include(u => u.ProfessionalSummaries)
                .Include(u => u.Contacts)
                .Include(u => u.Skills)
                .Include(u => u.Experiences)
                .ThenInclude(e => e.Responsibilities)
                .Include(u => u.Educations)
                .Include(u => u.Certifications)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            return new GetAllResumeDetails
            {
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Title =
                    user.Titles?.Select(t => new TitleItems { Id = t.Id, Title = t.ResumeTitle })
                        .ToList() ?? new List<TitleItems>(),
                Summaries =
                    user.ProfessionalSummaries?.Select(s => new SummaryItems
                        {
                            Id = s.Id,
                            Summary = s.Summary,
                        })
                        .ToList() ?? new List<SummaryItems>(),
                Socials =
                    user.Contacts?.OrderBy(c => c.ContactUrl)
                        .Select(c => new SocialMediaItems
                        {
                            Id = c.Id,
                            SocialMediaUrl = c.ContactUrl,
                        })
                        .ToList() ?? new List<SocialMediaItems>(),
                Skills =
                    user.Skills?.OrderBy(s => s.SkillName)
                        .Select(s => new SkillsItems
                        {
                            Id = s.Id,
                            Skill = s.SkillName,
                            SkillLevel = s.ProficiencyLevel,
                        })
                        .ToList() ?? new List<SkillsItems>(),
                Experience =
                    user.Experiences?.OrderByDescending(e => e.EndDate)
                        .Select(e => new ExperienceItems
                        {
                            Id = e.Id,
                            Company = e.CompanyName,
                            JobTitle = e.JobTitle,
                            StartDate = e.StartDate.ToString("MMMM yyyy"),
                            EndDate = e.EndDate.ToString("MMMM yyyy"),
                            Responsibilities =
                                e.Responsibilities?.Select(r => r.Responsibility).ToList()
                                ?? new List<string>(),
                        })
                        .ToList() ?? new List<ExperienceItems>(),
                Education =
                    user.Educations?.OrderByDescending(ed => ed.EndDate)
                        .Select(ed => new EducationItems
                        {
                            Id = ed.Id,
                            Institution = ed.InstitutionName,
                            Qualification = ed.Qualification,
                            StartDate = ed.StartDate.ToString("MMMM yyyy"),
                            EndDate = ed.EndDate.ToString("MMMM yyyy"),
                            Major = ed.Major,
                            Achievement = ed.Achievement,
                        })
                        .ToList() ?? new List<EducationItems>(),
                Certification =
                    user.Certifications?.OrderByDescending(ce => ce.IssuedDate)
                        .Select(ce => new CertificationItems
                        {
                            Id = ce.Id,
                            Name = ce.CertificationName,
                            Organisation = ce.IssuingOrganisation,
                            CredentialUrl = ce.CredentialUrl,
                            IssuedDate = ce.IssuedDate.HasValue
                                ? ce.IssuedDate.Value.ToString("MMMM yyyy")
                                : null,
                            ExpirationDate = ce.ExpiryDate.HasValue
                                ? ce.ExpiryDate.Value.ToString("MMMM yyyy")
                                : null,
                        })
                        .ToList() ?? new List<CertificationItems>(),
            };
        }

        public async Task<List<string>> GetAllSkillsByUserId(Guid userId)
        {
            return await _dbContext
                .Skill.Where(s => s.UserId == userId)
                .Select(s => s.SkillName)
                .ToListAsync();
        }

        public async Task<List<EducationItem>> GetAllEducationItemsByUserId(Guid userId)
        {
            return await _dbContext
                .Education.Where(e => e.UserId == userId)
                .OrderBy(e => e.EndDate)
                .Select(e => new EducationItem
                {
                    Institution = e.InstitutionName,
                    Qualification = e.Qualification,
                    StartDate = e.StartDate.ToString("MMMM yyyy"),
                    EndDate = e.EndDate == default ? "Present" : e.EndDate.ToString("MMMM yyyy"),
                    Major = e.Major,
                })
                .ToListAsync();
        }

        public async Task<UserModel> GetUserDetailsByUserId(Guid id)
        {
            UserModel? user = await _dbContext
                .User.Where(u => u.Id == id)
                .Select(u => new UserModel
                {
                    FirstName = u.FirstName ?? string.Empty,
                    LastName = u.LastName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            return user;
        }
    }
}
