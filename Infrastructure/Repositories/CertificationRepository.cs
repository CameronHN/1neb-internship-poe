using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class CertificationRepository : ICertificationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CertificationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Guid>> AddCertificationsAsync(List<AddCertification> certifications)
        {
            var entities = certifications
                .Select(cert => new Certification
                {
                    Id = Guid.NewGuid(),
                    CertificationName = cert.CertificationName,
                    IssuingOrganisation = cert.IssuingOrganisation,
                    CredentialUrl = cert.CredentialUrl,
                    IssuedDate = !string.IsNullOrWhiteSpace(cert.IssuedDate)
                        ? DateOnly.Parse(cert.IssuedDate)
                        : null,
                    ExpiryDate = !string.IsNullOrWhiteSpace(cert.ExpiryDate)
                        ? DateOnly.Parse(cert.ExpiryDate)
                        : null,
                    UserId = cert.UserId,
                })
                .ToList();

            await _dbContext.Certification.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public Task DeleteCertificationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CertificationItem>> GetAllCertificationsByTheirIdsAsync(
            ItemListRequest request
        )
        {
            var ids = request.Ids;
            if (ids.Count == 0)
                return [];

            var certs = await _dbContext
                .Certification.Where(cert => ids.Contains(cert.Id))
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

            switch (request.Order)
            {
                case SortOrder.Descending:
                    certs = certs.OrderByDescending(c => c.IssuedDate).ToList();
                    break;
                case SortOrder.Ascending:
                    certs = certs.OrderBy(c => c.IssuedDate).ToList();
                    break;
                case SortOrder.None:
                default:
                    // Preserve the order of IDs
                    var order = ids.Select((id, idx) => new { id, idx })
                        .ToDictionary(x => x.id, x => x.idx);
                    certs = certs
                        .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                        .ToList();
                    break;
            }

            return certs
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
                .ToList();
        }

        public Task<CertificationItem?> GetCertificationByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCertificationAsync(Guid userId, UpdateCertification certification)
        {
            throw new NotImplementedException();
        }
    }
}
