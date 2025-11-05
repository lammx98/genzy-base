using System;
using System.Reflection;

namespace Genzy.Base.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class EnumStringAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

public static class EnumExtensions
{
    public static string GetLabel(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<EnumStringAttribute>();
        return attr?.Value ?? value.ToString();
    }

    public static T FromLabel<T>(string label) where T : Enum
    {
        var type = typeof(T);
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<EnumStringAttribute>();
            if (attr != null && attr.Value == label)
                return (T)field.GetValue(null)!;

            if (field.Name == label)
                return (T)field.GetValue(null)!;
        }

        throw new ArgumentException($"No {type.Name} with label or name '{label}' found.", nameof(label));
    }
}