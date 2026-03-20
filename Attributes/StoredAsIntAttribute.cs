namespace Genzy.Base.Attributes;

/// <summary>
/// Marks an enum property to be stored as int in the database.
/// Apply to property or to the enum type itself.
/// Requires StoredAsIntConvention to be registered in ConfigureConventions.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
public sealed class StoredAsIntAttribute : Attribute;
