namespace Ilin_LR5.Models;

public class TodoItemDTO
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    // ЛР5, п.2: нові поля також передаються через DTO
    public DateOnly? DueDate { get; set; }
    public int Priority { get; set; }
}
