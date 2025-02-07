using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace OSK.Hexagonal.Abstractions
{
    public interface ITenantedModelRepository<TTenantId, TId, TModel, TFilter>: ITenantedModelRepository<TTenantId, TId, TModel>
        where TTenantId : struct, IEquatable<TTenantId>
        where TId : struct, IEquatable<TId>
        where TModel : ITenantedModel<TTenantId, TId>
        where TFilter: GetListFilter
    {
        Task<IPaginatedOutput<TModel>> GetListAsync(TTenantId tenantId, TFilter filter, CancellationToken cancellationToken = default);
    }
}
