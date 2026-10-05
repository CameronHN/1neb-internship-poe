using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Application.Services
{
    public class ResumeDataService : IResumeDataService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IProfessionalSummaryRepository _professionalSummaryRepository;
        private readonly IProfessionalLinkRepository _professionalLinkRepository;
        private readonly ITitleRepository _titleRepository;

        public ResumeDataService(
            IUserRepository userRepository,
            ICertificationRepository certificationRepository,
            IEducationRepository educationRepository,
            IExperienceRepository experienceRepository,
            ISkillRepository skillRepository,
            IProfessionalSummaryRepository professionalSummaryRepository,
            IProfessionalLinkRepository professionalLinkRepository,
            ITitleRepository titleRepository
        )
        {
            _userRepository = userRepository;
            _certificationRepository = certificationRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _skillRepository = skillRepository;
            _professionalSummaryRepository = professionalSummaryRepository;
            _professionalLinkRepository = professionalLinkRepository;
            _titleRepository = titleRepository;
        }

        public async Task<ResumeDTO> GetResumeDetailsAsync(Guid userId, ResumeRequest resumeRequest)
        {
            var resumeDto = new ResumeDTO();

            var user = await _userRepository.GetUserDetailsByUserIdAsync(userId);

            resumeDto.Name = $"{user.FirstName} {user.LastName}";

            resumeDto.Email = user.Email;
            resumeDto.PhoneNumber = user.PhoneNumber;

            if (resumeRequest.TitleId.HasValue)
            {
                var title = await _titleRepository.GetTitleByIdAsync(
                    id: resumeRequest.TitleId.Value,
                    userId: userId
                );
                resumeDto.Title = title;
            }

            if (resumeRequest.ProfessionalSummaryId.HasValue)
            {
                var summary = await _professionalSummaryRepository.GetSummaryByIdAsync(
                    id: resumeRequest.ProfessionalSummaryId.Value,
                    userId: userId
                );
                resumeDto.Summary = summary;
            }

            if (resumeRequest.SocialMediaIds != null)
            {
                var socials = await _professionalLinkRepository.GetProfessionalLinksByIdsAsync(
                    userId,
                    resumeRequest.SocialMediaIds
                );
                resumeDto.ProfessionalLinks = socials;
            }

            if (resumeRequest.SkillsIds != null)
            {
                var skills = await _skillRepository.GetAllSkillsByIdsAsync(
                    userId,
                    resumeRequest.SkillsIds
                );
                resumeDto.Skills = skills;
            }

            if (resumeRequest.EducationIds != null)
            {
                var educationItems = await _educationRepository.GetAllEducationsByIdsAsync(
                    userId,
                    resumeRequest.EducationIds
                );
                resumeDto.Education = educationItems;
            }

            if (resumeRequest.ExperienceIds != null)
            {
                var experienceItems = await _experienceRepository.GetAllExperiencesByIdsAsync(
                    userId,
                    resumeRequest.ExperienceIds
                );
                resumeDto.Experience = experienceItems;
            }

            if (resumeRequest.CertificationIds != null)
            {
                var certificationItems =
                    await _certificationRepository.GetAllCertificationsByTheirIdsAsync(
                        userId,
                        resumeRequest.CertificationIds
                    );
                resumeDto.Certification = certificationItems;
            }

            return resumeDto;
        }

        public async Task<GetAllResumeDetails?> GetResumeByUserIdAsync(Guid userId)
        {
            return await _userRepository.GetAllResumeDetailsByUserIdAsync(userId);
        }
    }
}
