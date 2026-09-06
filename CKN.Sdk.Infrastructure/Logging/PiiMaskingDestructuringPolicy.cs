using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace CKN.Sdk.Infrastructure.Logging;

/// <summary>
/// A Serilog destructuring policy that automatically masks sensitive properties (PII Data).
/// </summary>
public class PiiMaskingDestructuringPolicy : IDestructuringPolicy
{
    private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "PasswordHash",
        "Token",
        "AccessToken",
        "RefreshToken",
        "Secret",
        "ClientSecret",
        "Email",
        "CreditCard",
        "CardNumber",
        "Cvv"
    };

    private const string MaskedValue = "****";

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        var type = value.GetType();

        // If it's a built-in type or string, let Serilog handle it
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(Guid))
        {
            result = null!;
            return false;
        }

        // We only destructure objects that have properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (properties.Length == 0)
        {
            result = null!;
            return false;
        }

        var logProperties = new List<LogEventProperty>();

        foreach (var property in properties)
        {
            if (!property.CanRead) continue;

            string propertyName = property.Name;

            if (SensitivePropertyNames.Contains(propertyName))
            {
                // Mask sensitive property
                logProperties.Add(new LogEventProperty(propertyName, new ScalarValue(MaskedValue)));
            }
            else
            {
                // Retrieve the value normally
                object? propertyValue = null;
                try
                {
                    propertyValue = property.GetValue(value);
                }
                catch
                {
                    // Ignore exceptions during property access
                }

                logProperties.Add(new LogEventProperty(propertyName, propertyValueFactory.CreatePropertyValue(propertyValue, destructureObjects: true)));
            }
        }

        result = new StructureValue(logProperties, type.Name);
        return true;
    }
}
