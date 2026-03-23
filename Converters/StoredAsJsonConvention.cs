using System.Reflection;
using Genzy.Base.Attributes;
using Genzy.Base.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Genzy.Base.Conventers;

/// <summary>
/// Convention that applies StringArrayValueConverter to properties marked with [StoredAsJson].
/// Register via configurationBuilder.AddStoredAsJsonConvention() in ConfigureConventions.
/// </summary>
public class StoredAsJsonConvention : IPropertyAddedConvention
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

        if (propertyInfo.GetCustomAttribute<StoredAsJsonAttribute>() is null)
            return;

        propertyBuilder.HasConversion(new StringArrayValueConverter());
    }
}
