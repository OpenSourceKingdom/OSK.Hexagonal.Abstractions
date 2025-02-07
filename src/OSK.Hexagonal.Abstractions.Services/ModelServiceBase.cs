using OSK.Functions.Outputs.Abstractions;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Hexagonal.Abstractions.Services
{
    public abstract class ModelServiceBase<TId, TModel, TRepository> : IModelService<TId, TModel>
        where TId : struct, IEquatable<TId>
        where TModel : IModel<TId>
        where TRepository : IModelRepository<TId, TModel>
    {
        #region Variables

        protected IOutputFactory OutputFactory { get; }
        protected TRepository Repository { get; }

        #endregion

        #region Constructors

        protected ModelServiceBase(TRepository repository, IOutputFactory outputFactory)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            OutputFactory = outputFactory ?? throw new ArgumentNullException(nameof(outputFactory));
        }

        #endregion

        #region IModelService

        public async Task<IOutput<TModel>> CreateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var output = await MutateModelAsync(model, true, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsOutput<TModel>();
            }

            output = await ValidateForCreateAsync(model, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsOutput<TModel>();
            }

            var createdOutput = await Repository.CreateAsync(model, cancellationToken);
            if (!createdOutput.IsSuccessful)
            {
                return createdOutput;
            }

            await ProcessResourceCreationAsync(model, cancellationToken);
            return createdOutput;
        }

        public async Task<IOutput> DeleteAsync(TId id, CancellationToken cancellationToken = default)
        {
            var output = await ValidateForDeleteAsync(id, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output;
            }

            output = await Repository.DeleteAsync(id, cancellationToken);
            if (output.IsSuccessful)
            {
                await ProcessResourceRemovalAsync(id, cancellationToken);
                return output;
            }

            return output.StatusCode.SpecificityCode == OutputSpecificityCode.DataNotFound
                ? OutputFactory.Succeed() // if the id was not found, it was already successfully deleted
                : output;
        }

        public async Task<IOutput<TModel>> UpdateAsync(TModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var output = await MutateModelAsync(model, false, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsOutput<TModel>();
            }

            output = await ValidateForUpdateAsync(model, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsOutput<TModel>();
            }

            var updatedOutput = await Repository.UpdateAsync(model, cancellationToken);
            if (updatedOutput.IsSuccessful)
            {
                return updatedOutput;
            }

            await ProcessResourceChangeAsync(model, cancellationToken);
            return updatedOutput;
        }

        #endregion

        #region Helpers

        protected virtual ValueTask<IOutput> MutateModelAsync(TModel model, bool isNewModel, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Succeed());
        }
        
        protected virtual ValueTask<IOutput> ValidateForCreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Succeed());
        }

        protected virtual ValueTask<IOutput> ValidateForUpdateAsync(TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Succeed());
        }

        protected virtual ValueTask<IOutput> ValidateForDeleteAsync(TId id, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Succeed());
        }

        protected virtual ValueTask ProcessResourceCreationAsync(TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        protected virtual ValueTask ProcessResourceChangeAsync(TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        protected virtual ValueTask ProcessResourceRemovalAsync(TId id, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        #endregion
    }
}
