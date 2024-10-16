using OSK.Functions.Outputs.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Hexagonal.Abstractions
{
    public interface IModelService<TId, TModel>
        where TId: struct, IEquatable<TId>
        where TModel: IModel<TId>
    {
        Task<IOutput<TModel>> CreateAsync(TModel model, CancellationToken cancellationToken = default);

        Task<IOutput<TModel>> UpdateAsync(TModel model, CancellationToken cancellationToken = default);

        Task<IOutput> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    }
}
