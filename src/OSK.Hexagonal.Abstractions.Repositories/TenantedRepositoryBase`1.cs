using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Hexagonal.Abstractions.Repositories
{
    public abstract class TenantedRepositoryBase<TTenantId, TId, TModel, TFilter> : TenantedModelRepositoryBase<TTenantId, TId, TModel>, ITenantedModelRepository<TTenantId, TId, TModel, TFilter>
        where TTenantId : struct, IEquatable<TTenantId>
        where TId : struct, IEquatable<TId>
        where TModel : ITenantedModel<TTenantId, TId>
        where TFilter : GetListFilter
    {
        public abstract Task<PaginatedOutput<TModel>> GetListAsync(TTenantId tenantId, TFilter filter, CancellationToken cancellationToken = default);
    }
}
