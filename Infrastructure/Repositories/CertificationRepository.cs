using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Models;
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

        public async Task<List<Guid>> AddCertificationsAsync(
            Guid userId,
            List<AddCertification> certifications
        )
        {
            var entities = certifications
                .Select(cert => new Certification
                {
                    CertificationName = cert.CertificationName,
                    IssuingOrganisation = cert.IssuingOrganisation,
                    CredentialUrl = cert.CredentialUrl,
                    IssuedDate = !string.IsNullOrWhiteSpace(cert.IssuedDate)
                        ? DateOnly.Parse(cert.IssuedDate)
                        : null,
                    ExpiryDate = !string.IsNullOrWhiteSpace(cert.ExpiryDate)
                        ? DateOnly.Parse(cert.ExpiryDate)
                        : null,
                    UserId = userId,
                })
                .ToList();

            await _dbContext.Certification.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> DeleteCertificationsAsync(Guid userId, List<Guid> certificationIds)
        {
            if (certificationIds.IsNullOrEmpty())
                return false;

            var certificationsToDelete = await _dbContext
                .Certification.Where(cert =>
                    cert.UserId == userId && certificationIds.Contains(cert.Id)
                )
                .ToListAsync();

            if (certificationsToDelete.Count != certificationIds.Count)
                return false;

            _dbContext.Certification.RemoveRange(certificationsToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
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

        public async Task<CertificationItem> GetCertificationByIdAsync(Guid id)
        {
            var certification =
                await _dbContext
                    .Certification.Where(cert => cert.Id == id)
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
                    .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Certification does not exist");

            return certification;
        }

        public async Task<List<CertificationModel>> GetCertificationsByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .Certification.Where(cert => cert.UserId == userId)
                .Select(cert => new CertificationModel
                {
                    CertificationName = cert.CertificationName,
                    IssuingOrganisation = cert.IssuingOrganisation,
                    CredentialUrl = cert.CredentialUrl,
                    IssuedDate = cert.IssuedDate,
                    ExpiryDate = cert.ExpiryDate,
                })
                .ToListAsync();
        }

        public async Task<bool> PatchCertificationsAsync(
            Guid userId,
            List<PatchCertification> patches
        )
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            var certs = await _dbContext
                .Set<Certification>()
                .Where(c => c.UserId == userId && ids.Contains(c.Id))
                .ToListAsync();

            if (certs.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            foreach (var cert in certs)
            {
                var patch = patchMap[cert.Id];

                if (
                    patch.CertificationName != null
                    && patch.CertificationName != cert.CertificationName
                )
                {
                    cert.CertificationName = patch.CertificationName;
                    anyChange = true;
                }

                if (patch.IssuingOrganisation != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.IssuingOrganisation)
                        ? null
                        : patch.IssuingOrganisation;
                    if (newValue != cert.IssuingOrganisation)
                    {
                        cert.IssuingOrganisation = newValue;
                        anyChange = true;
                    }
                }

                if (patch.CredentialUrl != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.CredentialUrl)
                        ? null
                        : patch.CredentialUrl;
                    if (newValue != cert.CredentialUrl)
                    {
                        cert.CredentialUrl = newValue;
                        anyChange = true;
                    }
                }

                if (patch.IssuedDate != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.IssuedDate)
                        ? null
                        : patch.IssuedDate;
                    DateOnly? newIssuedDate = !string.IsNullOrWhiteSpace(newValue)
                        ? DateOnly.Parse(newValue)
                        : null;
                    if (newIssuedDate != cert.IssuedDate)
                    {
                        cert.IssuedDate = newIssuedDate;
                        anyChange = true;
                    }
                }

                if (patch.ExpiryDate != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.ExpiryDate)
                        ? null
                        : patch.ExpiryDate;
                    DateOnly? newExpiryDate = !string.IsNullOrWhiteSpace(newValue)
                        ? DateOnly.Parse(newValue)
                        : null;
                    if (newExpiryDate != cert.ExpiryDate)
                    {
                        cert.ExpiryDate = newExpiryDate;
                        anyChange = true;
                    }
                }
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}
