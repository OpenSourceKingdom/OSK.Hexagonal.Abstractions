using System;

namespace OSK.Hexagonal.Abstractions
{
    public interface IModel<TId>
        where TId : struct, IEquatable<TId>
    {
        TId Id { get; set; }
    }
}
