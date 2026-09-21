namespace CKN.Sdk.Core.Domain;

/// <summary>
/// A marker class representing an Aggregate Root in Domain-Driven Design.
/// Aggregate Roots are the only entities that should be loaded or saved directly by repositories.
/// </summary>
/// <typeparam name="TId">The type of the primary key.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
{
}
