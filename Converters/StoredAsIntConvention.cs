using System.Reflection;
using Genzy.Base.Attributes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Genzy.Base.Conventers;

/// <summary>
/// Convention that applies int conversion to properties marked with [StoredAsInt].
/// Register via configurationBuilder.AddStoredAsIntConvention() in ConfigureConventions.
/// </summary>
public class StoredAsIntConvention : IPropertyAddedConvention
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

        if (!underlyingType.IsEnum)
            return;

        var hasAttribute = propertyInfo.GetCustomAttribute<StoredAsIntAttribute>() is not null
            || underlyingType.GetCustomAttribute<StoredAsIntAttribute>() is not null;

        if (!hasAttribute)
            return;

        propertyBuilder.HasConversion(typeof(int));
    }
}
