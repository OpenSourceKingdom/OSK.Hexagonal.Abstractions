using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Hexagonal.Abstractions.Repositories
{
    public abstract class ModelRepositoryBase<TId, TModel> : IModelRepository<TId, TModel>
        where TId : struct, IEquatable<TId>
        where TModel : IModel<TId>
    {
        #region IModelRepository

        public async Task<IOutput<TModel>> CreateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            var nextIdOutput = await GetNextIdAsync(cancellationToken);
            if (!nextIdOutput.IsSuccessful)
            {
                return nextIdOutput.AsType<TModel>();
            }

            model.Id = nextIdOutput.Value;
            return await CreateNewAsync(model, cancellationToken);
        }

        public abstract Task<IOutput> DeleteAsync(TId id, CancellationToken cancellationToken = default);
        public abstract Task<IOutput<TModel>> GetAsync(TId id, CancellationToken cancellationToken = default);
        public abstract Task<IOutput<TModel>> UpdateAsync(TModel model, CancellationToken cancellationToken = default);

        #endregion

        #region Helpers

        protected abstract Task<IOutput<TModel>> CreateNewAsync(TModel model, CancellationToken cancellationToken);
        protected abstract ValueTask<IOutput<TId>> GetNextIdAsync(CancellationToken cancellationToken); 

        #endregion
    }
}
