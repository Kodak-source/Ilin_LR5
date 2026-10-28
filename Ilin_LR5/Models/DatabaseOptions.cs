using System.ComponentModel.DataAnnotations;

namespace Ilin_LR5.Models;

// ЛР5, п.3: налаштування бази даних з appsettings.json з валідацією
public class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(ErrorMessage = "Ім'я бази даних (Database:Name) не може бути null або порожнім.")]
    [MinLength(4, ErrorMessage = "Ім'я бази даних (Database:Name) має містити не менше 4 символів.")]
    public string? Name { get; set; }
}
