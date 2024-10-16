using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Hexagonal.Abstractions
{
    public interface ITenantRepository<TTenantId>
        where TTenantId : struct, IEquatable<TTenantId>
    {
        Task<IOutput> ExistsAsync(TTenantId tenantId, CancellationToken cancellationToken = default);
    }
}
