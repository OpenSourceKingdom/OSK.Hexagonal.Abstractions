using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Hexagonal.Abstractions.Repositories
{
    public abstract class ModelRepositoryBase<TId, TModel, TFilter> : ModelRepositoryBase<TId, TModel>, IModelRepository<TId, TModel, TFilter>
        where TId : struct, IEquatable<TId>
        where TModel : IModel<TId>
        where TFilter : GetListFilter
    {
        public abstract Task<PaginatedOutput<TModel>> GetListAsync(TFilter filter, CancellationToken cancellationToken = default);
    }
}
