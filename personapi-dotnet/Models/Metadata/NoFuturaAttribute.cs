using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.Metadata;

/// <summary>
/// Valida que una fecha (DateOnly o DateTime) no sea posterior a hoy. Un valor nulo es válido.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NoFuturaAttribute : ValidationAttribute
{
    public NoFuturaAttribute()
        : base("La fecha no puede ser futura.")
    {
    }

    public override bool IsValid(object? value)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return value switch
        {
            null => true,
            DateOnly fecha => fecha <= hoy,
            DateTime fecha => DateOnly.FromDateTime(fecha) <= hoy,
            _ => false
        };
    }
}
