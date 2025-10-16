using Portfolio.Application.Documents;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using QuestPDF.Fluent;

namespace Portfolio.Application.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ISkillRepository _skillRepository;

        public ResumeService(
            IUserRepository userRepository,
            ICertificationRepository certificationRepository,
            IEducationRepository educationRepository,
            IExperienceRepository experienceRepository,
            ISkillRepository skillRepository
        )
        {
            _userRepository = userRepository;
            _certificationRepository = certificationRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _skillRepository = skillRepository;
        }

        public async Task<ResumeDto> GetResume(ResumeRequest resumeRequest)
        {
            var resumeDto = new ResumeDto();

            var user = await _userRepository.GetUserEntityDetailsByUserId(resumeRequest.UserId);

            resumeDto.Name = $"{user.FirstName} {user.LastName}";

            user.PhoneNumber ??= "";

            resumeDto.Contact = new ContactInfo();
            resumeDto.Contact.Phone = user.PhoneNumber;
            resumeDto.Contact.Email = user.Email;

            if (resumeRequest.SkillsIds != null)
            {
                var skills = await _skillRepository.GetAllSkillsByIds(resumeRequest.SkillsIds);
                resumeDto.Skills = skills;
            }

            if (resumeRequest.EducationIds != null)
            {
                var educationItems = await _educationRepository.GetAllEducationsByIds(
                    resumeRequest.EducationIds
                );
                resumeDto.Education = educationItems;
            }

            if (resumeRequest.ExperienceIds != null)
            {
                var experienceItems = await _experienceRepository.GetAllExperiencesByIds(
                    resumeRequest.ExperienceIds
                );
                resumeDto.Experience = experienceItems;
            }

            if (resumeRequest.CertificationIds != null)
            {
                var certificationItems = await _certificationRepository.GetAllCertsByIds(
                    resumeRequest.CertificationIds
                );
                resumeDto.Certification = certificationItems;
            }

            return resumeDto;
        }

        public async Task<ResumeDto> GetResumeByUserId(Guid userId)
        {
            return await _userRepository.GetResumeDtoByUserId(userId);
        }

        public byte[] RenderPdf(ResumeDto dto)
        {
            try
            {
                var document = new ResumeBuilder(dto ?? new());
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"PDF generation failed: {ex.Message}", ex);
            }
        }
    }
}
