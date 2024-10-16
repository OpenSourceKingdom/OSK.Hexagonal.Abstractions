using System;

namespace OSK.Hexagonal.Abstractions
{
    public interface ITenantedModel<TTenantId, TId>: IModel<TId>
        where TTenantId : struct, IEquatable<TTenantId>
        where TId : struct, IEquatable<TId>
    {
        TTenantId TenantId { get; }
    }
}
