using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.Resume;
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

        public async Task<GetAllResumeDetails?> GetAllResumeDetailsByUserIdAsync(Guid userId)
        {
            var user = await _dbContext
                .User.Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            // One projected query per collection instead of a single query that joins them all.
            // Joining sibling collections multiplies the rows (titles x links x skills x ...),
            // so each list is fetched on its own, scoped to the user, and sorted in SQL.
            // Dates are read raw and formatted afterwards, so no formatting is sent to SQL.
            var titles = await _dbContext
                .Title.Where(t => t.UserId == userId)
                .Select(t => new TitleItems { Id = t.Id, Title = t.ResumeTitle })
                .ToListAsync();

            var summaries = await _dbContext
                .ProfessionalSummary.Where(s => s.UserId == userId)
                .Select(s => new SummaryItems { Id = s.Id, Summary = s.Summary })
                .ToListAsync();

            var socials = await _dbContext
                .ProfessionalLink.Where(c => c.UserId == userId)
                .OrderBy(c => c.Link)
                .ThenBy(c => c.Id)
                .Select(c => new SocialMediaItems
                {
                    Id = c.Id,
                    SocialMediaType = c.LinkType,
                    SocialMediaUrl = c.Link,
                })
                .ToListAsync();

            var skills = await _dbContext
                .Skill.Where(s => s.UserId == userId)
                .OrderBy(s => s.SkillName)
                .ThenBy(s => s.Id)
                .Select(s => new SkillsItems
                {
                    Id = s.Id,
                    Skill = s.SkillName,
                    SkillLevel = s.ProficiencyLevel,
                })
                .ToListAsync();

            var experiences = await _dbContext
                .Experience.Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EndDate)
                .ThenBy(e => e.Id)
                .Select(e => new
                {
                    e.Id,
                    e.CompanyName,
                    e.JobTitle,
                    e.StartDate,
                    e.EndDate,
                    Responsibilities = e.Responsibilities.Select(r => r.Responsibility).ToList(),
                })
                .ToListAsync();

            var educations = await _dbContext
                .Education.Where(ed => ed.UserId == userId)
                .OrderByDescending(ed => ed.EndDate)
                .ThenBy(ed => ed.Id)
                .Select(ed => new
                {
                    ed.Id,
                    ed.InstitutionName,
                    ed.Qualification,
                    ed.StartDate,
                    ed.EndDate,
                    ed.Major,
                    ed.Achievement,
                })
                .ToListAsync();

            var certifications = await _dbContext
                .Certification.Where(ce => ce.UserId == userId)
                .OrderByDescending(ce => ce.IssuedDate)
                .ThenBy(ce => ce.Id)
                .Select(ce => new
                {
                    ce.Id,
                    ce.CertificationName,
                    ce.IssuingOrganisation,
                    ce.CredentialUrl,
                    ce.IssuedDate,
                    ce.ExpiryDate,
                })
                .ToListAsync();

            return new GetAllResumeDetails
            {
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Title = titles,
                Summaries = summaries,
                Socials = socials,
                Skills = skills,
                Experience = experiences
                    .Select(e => new ExperienceItems
                    {
                        Id = e.Id,
                        Company = e.CompanyName,
                        JobTitle = e.JobTitle,
                        StartDate = e.StartDate.ToString("MMMM yyyy"),
                        EndDate = e.EndDate.ToString("MMMM yyyy"),
                        Responsibilities = e.Responsibilities,
                    })
                    .ToList(),
                Education = educations
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
                    .ToList(),
                Certification = certifications
                    .Select(ce => new CertificationItems
                    {
                        Id = ce.Id,
                        Name = ce.CertificationName,
                        Organisation = ce.IssuingOrganisation,
                        CredentialUrl = ce.CredentialUrl,
                        IssuedDate = ce.IssuedDate?.ToString("MMMM yyyy"),
                        ExpirationDate = ce.ExpiryDate?.ToString("MMMM yyyy"),
                    })
                    .ToList(),
            };
        }

        public async Task<UserModel> GetUserDetailsByUserIdAsync(Guid id)
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
