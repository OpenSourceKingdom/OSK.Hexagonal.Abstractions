using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace OSK.Hexagonal.Abstractions.Repositories
{
    public abstract class TenantedModelRepositoryBase<TTenantId, TId, TModel> : ITenantedModelRepository<TTenantId, TId, TModel>
        where TTenantId : struct, IEquatable<TTenantId>
        where TId : struct, IEquatable<TId>
        where TModel : ITenantedModel<TTenantId, TId>
    {
        #region ITenantedModelRepository

        public async Task<IOutput<TModel>> CreateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken = default)
        {
            var nextIdOutput = await GetNextIdAsync(tenantId, cancellationToken);
            if (!nextIdOutput.IsSuccessful)
            {
                return nextIdOutput.AsOutput<TModel>();
            }

            model.Id = nextIdOutput.Value;
            return await CreateNewAsync(tenantId, model, cancellationToken);
        }

        public abstract Task<IOutput> DeleteAsync(TTenantId tenantId, TId id, CancellationToken cancellationToken = default);
        public abstract Task<IOutput<TModel>> GetAsync(TTenantId tenantId, TId id, CancellationToken cancellationToken = default);
        public abstract Task<IOutput<TModel>> UpdateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken = default);

        #endregion

        #region Helpers

        protected abstract Task<IOutput<TModel>> CreateNewAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken);
        protected abstract ValueTask<IOutput<TId>> GetNextIdAsync(TTenantId tenantId, CancellationToken cancellationToken);

        #endregion
    }
}