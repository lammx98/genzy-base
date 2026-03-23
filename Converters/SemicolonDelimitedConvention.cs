using System.Reflection;
using Genzy.Base.Attributes;
using Genzy.Base.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Genzy.Base.Conventers;

/// <summary>
/// Convention that applies SemicolonDelimitedStringArrayConverter to properties marked with [SemicolonDelimited].
/// Register via configurationBuilder.AddSemicolonDelimitedConvention() in ConfigureConventions.
/// </summary>
public class SemicolonDelimitedConvention : IPropertyAddedConvention
{
    public void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        IConventionContext<IConventionPropertyBuilder> context)
    {
        var propertyInfo = propertyBuilder.Metadata.PropertyInfo;
        if (propertyInfo is null)
            return;

        var clrType = propertyBuilder.Metadata.ClrType;
        var underlyingType = Nullable.GetUnderlyingType(clrType) ?? clrType;
        if (underlyingType != typeof(string[]))
            return;

        var attr = propertyInfo.GetCustomAttribute<SemicolonDelimitedAttribute>();
        if (attr is null)
            return;

        propertyBuilder.HasConversion(new SemicolonDelimitedStringArrayConverter(attr.Separator));
    }
}
