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
        private readonly IProfessionalSummaryRepository _professionalSummaryRepository;
        private readonly IContactRepository _contactRepository;
        private readonly ITitleRepository _titleRepository;

        public ResumeService(
            IUserRepository userRepository,
            ICertificationRepository certificationRepository,
            IEducationRepository educationRepository,
            IExperienceRepository experienceRepository,
            ISkillRepository skillRepository,
            IProfessionalSummaryRepository professionalSummaryRepository,
            IContactRepository contactRepository,
            ITitleRepository titleRepository
        )
        {
            _userRepository = userRepository;
            _certificationRepository = certificationRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _skillRepository = skillRepository;
            _professionalSummaryRepository = professionalSummaryRepository;
            _contactRepository = contactRepository;
            _titleRepository = titleRepository;
        }

        public async Task<ResumeDto> GetResumeDetailsAsync(Guid userId, ResumeRequest resumeRequest)
        {
            var resumeDto = new ResumeDto();

            var user = await _userRepository.GetUserDetailsByUserId(userId);

            resumeDto.Name = $"{user.FirstName} {user.LastName}";

            resumeDto.Email = user.Email;
            resumeDto.PhoneNumber = user.PhoneNumber;

            if (resumeRequest.TitleId.HasValue)
            {
                var title = await _titleRepository.GetTitleById(resumeRequest.TitleId.Value);
                resumeDto.Title = title;
            }

            if (resumeRequest.ProfessionalSummaryId.HasValue)
            {
                var summary = await _professionalSummaryRepository.GetSummaryById(
                    resumeRequest.ProfessionalSummaryId.Value
                );
                resumeDto.Summary = summary;
            }

            if (resumeRequest.SocialMediaIds != null)
            {
                var socials = await _contactRepository.GetContactsByIdsAsync(
                    resumeRequest.SocialMediaIds
                );
                resumeDto.Socials = socials;
            }

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
                var certificationItems =
                    await _certificationRepository.GetAllCertificationsByTheirIdsAsync(
                        resumeRequest.CertificationIds
                    );
                resumeDto.Certification = certificationItems;
            }

            return resumeDto;
        }

        public async Task<ResumeDto?> GetResumeByUserId(Guid userId)
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
