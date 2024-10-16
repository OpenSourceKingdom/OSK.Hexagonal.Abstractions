﻿using OSK.Functions.Outputs.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace OSK.Hexagonal.Abstractions
{
    public interface IModelRepository<TId, TModel>
        where TId : struct, IEquatable<TId>
        where TModel : IModel<TId>
    {
        Task<IOutput<TModel>> CreateAsync(TModel model, CancellationToken cancellationToken = default);

        Task<IOutput<TModel>> UpdateAsync(TModel model, CancellationToken cancellationToken = default);

        Task<IOutput<TModel>> GetAsync(TId id, CancellationToken cancellationToken = default);

        Task<IOutput> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    }
}