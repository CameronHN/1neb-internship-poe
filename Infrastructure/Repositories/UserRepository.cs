using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.User;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
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

        public async Task AddUser(ApplicationUser user)
        {
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();
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

        public async Task<ResumeDto?> GetResumeDtoByUserId(Guid userId)
        {
            var resume = await _dbContext
                .User.Where(u => u.Id == userId)
                .Select(u => new ResumeDto
                {
                    Name = u.FirstName + " " + u.LastName,
                    Title = u.ProfessionalSummaries.FirstOrDefault().Summary,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Socials = u
                        .Contacts.OrderBy(c => c.ContactUrl)
                        .Select(c => new SocialMediaItem { SocialMediaUrl = c.ContactUrl })
                        .ToList(),
                    Summary = u.ProfessionalSummaries.FirstOrDefault().Summary,
                    Skills = u
                        .Skills.OrderBy(s => s.SkillName)
                        .Select(s => new SkillsItem
                        {
                            Skill = s.SkillName,
                            SkillLevel = s.ProficiencyLevel,
                        })
                        .ToList(),
                    Experience = u
                        .Experiences.OrderByDescending(e => e.EndDate)
                        .Select(e => new ExperienceItem
                        {
                            Company = e.CompanyName,
                            JobTitle = e.JobTitle,
                            StartDate = e.StartDate.ToString("MMMM yyyy"),
                            EndDate = e.EndDate.ToString("MMMM yyyy"),
                            Responsibilities = e
                                .Responsibilities.Select(r => r.Responsibility)
                                .ToList(),
                        })
                        .ToList(),
                    Education = u
                        .Educations.OrderByDescending(ed => ed.EndDate)
                        .Select(ed => new EducationItem
                        {
                            Institution = ed.InstitutionName,
                            Qualification = ed.Qualification,
                            StartDate = ed.StartDate.ToString("MMMM yyyy"),
                            EndDate = ed.EndDate.ToString("MMMM yyyy"),
                            Major = ed.Major,
                        })
                        .ToList(),
                    Certification = u
                        .Certifications.OrderByDescending(ce => ce.IssuedDate)
                        .Select(ce => new CertificationItem
                        {
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
                        .ToList(),
                })
                .FirstOrDefaultAsync();

            return resume;
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

        public async Task<UserEntityDetailsDto> GetUserEntityDetailsByUserId(Guid id)
        {
            UserEntityDetailsDto? user = await _dbContext
                .User.Where(u => u.Id == id)
                .Select(u => new UserEntityDetailsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
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
