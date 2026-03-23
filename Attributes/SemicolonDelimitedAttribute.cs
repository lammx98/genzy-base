namespace Genzy.Base.Attributes;

/// <summary>
/// Marks a string[] property to be stored as delimiter-separated string in the database.
/// Default separator is ";". Requires SemicolonDelimitedConvention to be registered in ConfigureConventions.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SemicolonDelimitedAttribute(string separator = ";") : Attribute
{
    public string Separator { get; } = separator;
}
