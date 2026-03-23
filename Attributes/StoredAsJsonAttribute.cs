namespace Genzy.Base.Attributes;

/// <summary>
/// Marks a string[] property to be stored as JSON string in the database.
/// Requires StoredAsJsonConvention to be registered in ConfigureConventions.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class StoredAsJsonAttribute : Attribute;
