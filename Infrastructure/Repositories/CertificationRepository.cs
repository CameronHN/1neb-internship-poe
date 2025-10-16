using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Infrastructure.Repositories
{
    public class CertificationRepository : ICertificationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CertificationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0) return [];

            var certs = await _dbContext.Certification
                .Where(cert => ids.Contains(cert.Id))
                .Select(ce => new
                {
                    ce.Id,
                    ce.CertificationName,
                    ce.IssuingOrganisation,
                    ce.CredentialUrl,
                    ce.IssuedDate,
                    ce.ExpiryDate
                })
                .ToListAsync();

            if (request.IsDescending)
            {
                // Order by IssuedDate descending
                certs = certs
                    .OrderByDescending(c => c.IssuedDate)
                    .ToList();
            }
            else
            {
                // Preserve the exact order of the incoming Ids
                var order = ids
                    .Select((id, index) => new { id, index })
                    .ToDictionary(x => x.id, x => x.index);

                certs = certs
                    .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                    .ToList();
            }

            return certs.Select(ce => new CertificationItem
            {
                Name = ce.CertificationName,
                Organisation = ce.IssuingOrganisation,
                CredentialUrl = ce.CredentialUrl,
                IssuedDate = ce.IssuedDate.HasValue ? ce.IssuedDate.Value.ToString("MMMM yyyy") : null,
                ExpirationDate = ce.ExpiryDate.HasValue ? ce.ExpiryDate.Value.ToString("MMMM yyyy") : null
            }).ToList();
        }
    }
}
