using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Application.Services
{
    public class ProfessionalSummaryService : IProfessionalSummaryService
    {
        private readonly IProfessionalSummaryRepository _professionalSummaryRepository;

        public ProfessionalSummaryService(IProfessionalSummaryRepository professionalSummaryRepository)
        {
            _professionalSummaryRepository = professionalSummaryRepository;
        }

        public async Task<Guid> AddSummaryAsync(AddSummary summary)
        {
            return await _professionalSummaryRepository.AddSummaryAsync(summary);
        }
    }
}
