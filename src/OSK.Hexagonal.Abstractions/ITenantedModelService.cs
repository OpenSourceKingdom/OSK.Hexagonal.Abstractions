using OSK.Functions.Outputs.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace OSK.Hexagonal.Abstractions
{
    public interface ITenantedModelService<TTenantId, TId, TModel>
        where TTenantId : struct, IEquatable<TTenantId>
        where TId : struct, IEquatable<TId>
        where TModel : ITenantedModel<TTenantId, TId>
    {
        Task<IOutput<TModel>> CreateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken = default);

        Task<IOutput<TModel>> UpdateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken = default);

        Task<IOutput> DeleteAsync(TTenantId tenantId, TId id, CancellationToken cancellationToken = default);
    }
}
