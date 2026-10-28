namespace Ilin_LR5.Models;

public class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    // ЛР5, п.2: дата виконання та пріоритет
    public DateOnly? DueDate { get; set; }
    public int Priority { get; set; }

    public string? Secret { get; set; }
}
