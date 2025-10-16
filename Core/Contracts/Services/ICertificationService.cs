using Portfolio.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Core.Contracts.Services
{
    public interface ICertificationService
    {
        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest certificationRequest);
    }
}
