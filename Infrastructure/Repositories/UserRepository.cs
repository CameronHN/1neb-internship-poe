using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
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

        public async Task<User?> GetUserById(Guid id)
        {
            User? user = await _dbContext.User.FindAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            return user;
        }

        public async Task AddUser(User user)
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

            existingUser.Gender = updateUserDTO.Gender;
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
            var resume = await _dbContext.User.Where(u => u.Id == userId)
                .Select(u => new ResumeDto
                {
                    Name = u.FirstName + " " + u.LastName,
                    Title = u.ProfessionalSummaries.FirstOrDefault().Summary,
                    Contact = new ContactInfo
                    {
                        Email = u.Email,
                        Phone = u.PhoneNumber,
                        LinkedIn = u.Contacts.FirstOrDefault().LinkedIn,
                        Github = u.Contacts.FirstOrDefault().GitHub
                    },
                    Summary = u.ProfessionalSummaries.FirstOrDefault().Summary,
                    Skills = u.Skills.Select(s => s.SkillName).ToList(),
                    Experience = u.Experiences.Select(e => new ExperienceItem
                    {
                        Company = e.CompanyName,
                        Role = e.JobTitle,
                        Start = e.StartDate.ToString("yyyy-MM"),
                        End = e.EndDate.ToString("yyyy-MM"),
                        Responsibilities = e.Responsibilities.Select(r => r.Responsibility).ToList()
                    }).ToList(),
                    Education = u.Educations.Select(ed => new EducationItem
                    {
                        Institution = ed.InstitutionName,
                        Degree = ed.Qualification,
                        Start = ed.StartDate.ToString("yyyy"),
                        End = ed.EndDate.ToString("yyyy"),
                        Major = ed.Major
                    }).ToList(),
                    Certification = u.Certifications.Select(ce => new CertificationItem
                    {
                        Name = ce.CertificationName,
                        Organisation = ce.IssuingOrganisation,
                        CredentialUrl = ce.CredentialUrl,
                        IssuedDate = ce.IssuedDate.HasValue ? ce.IssuedDate.Value.ToString("yyyy-MM") : null,
                        ExpirationDate = ce.ExpiryDate.HasValue ? ce.ExpiryDate.Value.ToString("yyyy-MM") : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return resume;
        }

        public async Task<List<string>> GetAllSkillsByUserId(Guid userId)
        {
            return await _dbContext.Skill
                .Where(s => s.UserId == userId)
                .Select(s => s.SkillName)
                .ToListAsync();
        }

        public async Task<List<EducationItem>> GetAllEducationItemsByUserId(Guid userId)
        {
            return await _dbContext.Education
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.EndDate)
                .Select(e => new EducationItem
                {
                    Institution = e.InstitutionName,
                    Degree = e.Qualification,
                    Start = e.StartDate.ToString("MMMM yyyy"),
                    End = e.EndDate == default ? "Present" : e.EndDate.ToString("MMMM yyyy"),
                    Major = e.Major
                })
                .ToListAsync();
        }
    }
}
