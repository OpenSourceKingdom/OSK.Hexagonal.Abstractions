using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace OSK.Hexagonal.Abstractions
{
    public interface IModelRepository<TId, TModel, TFilter>: IModelRepository<TId, TModel>
        where TId : struct, IEquatable<TId>
        where TModel : IModel<TId>
        where TFilter : GetListFilter
    {
        Task<IPaginatedOutput<TModel>> GetListAsync(TFilter filter, CancellationToken cancellationToken = default);
    }
}
