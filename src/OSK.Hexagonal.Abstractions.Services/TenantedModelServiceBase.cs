using OSK.Functions.Outputs.Abstractions;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace OSK.Hexagonal.Abstractions.Services
{
    public abstract class TenantedModelServiceBase<TTenantId, TId, TModel, TTenantRepository, TRepository> : ITenantedModelService<TTenantId, TId, TModel>
        where TTenantId : struct, IEquatable<TTenantId>
        where TId : struct, IEquatable<TId>
        where TModel : ITenantedModel<TTenantId, TId>
        where TTenantRepository : ITenantRepository<TTenantId>
        where TRepository : ITenantedModelRepository<TTenantId, TId, TModel>
    {
        #region Variables

        protected IOutputFactory OutputFactory { get; }
        protected TTenantRepository TenantRepository { get; }
        protected TRepository Repository { get; }

        protected virtual string? ApplicationId { get; }

        #endregion

        #region Constructors

        protected TenantedModelServiceBase(TTenantRepository tenantRepository, TRepository repository, IOutputFactory outputFactory)
        {
            TenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            OutputFactory = outputFactory ?? throw new ArgumentNullException(nameof(outputFactory));
        }

        #endregion

        #region IModelService

        public async Task<IOutput<TModel>> CreateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (!model.TenantId.Equals(tenantId))
            {
                return OutputFactory.BadRequest<TModel>("Tenant id mismatch.", ApplicationId);
            }

            var output = await TenantRepository.ExistsAsync(tenantId, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsType<TModel>();
            }

            output = await MutateModelAsync(tenantId, model, true, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsType<TModel>();
            }

            output = await ValidateForCreateAsync(tenantId, model, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsType<TModel>();
            }

            var createdOutput = await Repository.CreateAsync(tenantId, model, cancellationToken);
            if (!createdOutput.IsSuccessful)
            {
                return createdOutput;
            }

            await ProcessResourceCreationAsync(model, cancellationToken);
            return createdOutput;
        }

        public async Task<IOutput> DeleteAsync(TTenantId tenantId, TId id, CancellationToken cancellationToken = default)
        {
            var output = await TenantRepository.ExistsAsync(tenantId, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output;
            }

            output = await ValidateForDeleteAsync(tenantId, id, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output;
            }

            output = await Repository.DeleteAsync(tenantId, id, cancellationToken);
            if (output.IsSuccessful)
            {
                await ProcessResourceRemovalAsync(tenantId, id, cancellationToken);
                return output;
            }

            return output.Code.StatusCode == HttpStatusCode.NotFound
                ? OutputFactory.Success() // if the id was not found, it was already successfully deleted
                : output;
        }
        
        public async Task<IOutput<TModel>> UpdateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (!model.TenantId.Equals(tenantId))
            {
                return OutputFactory.BadRequest<TModel>("Tenant id mismatch.", ApplicationId);
            }

            var output = await TenantRepository.ExistsAsync(tenantId, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsType<TModel>();
            }

            output = await MutateModelAsync(tenantId, model, false, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsType<TModel>();
            }

            output = await ValidateForUpdateAsync(tenantId, model, cancellationToken);
            if (!output.IsSuccessful)
            {
                return output.AsType<TModel>();
            }

            var updatedOutput = await Repository.UpdateAsync(tenantId, model, cancellationToken);
            if (updatedOutput.IsSuccessful)
            {
                return updatedOutput;
            }

            await ProcessResourceChangeAsync(model, cancellationToken);
            return updatedOutput;
        }

        #endregion

        #region Helpers

        protected virtual ValueTask<IOutput> MutateModelAsync(TTenantId tenantId, TModel model, bool isNewModel, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Success());
        }

        protected virtual ValueTask<IOutput> ValidateForCreateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Success());
        }

        protected virtual ValueTask<IOutput> ValidateForUpdateAsync(TTenantId tenantId, TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Success());
        }

        protected virtual ValueTask<IOutput> ValidateForDeleteAsync(TTenantId tenantId, TId id, CancellationToken cancellationToken)
        {
            return new ValueTask<IOutput>(OutputFactory.Success());
        }

        protected virtual ValueTask ProcessResourceCreationAsync(TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        protected virtual ValueTask ProcessResourceChangeAsync(TModel model, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        protected virtual ValueTask ProcessResourceRemovalAsync(TTenantId tenantId, TId id, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        #endregion
    }
}