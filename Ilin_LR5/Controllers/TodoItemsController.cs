using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilin_LR5.Models;

namespace Ilin_LR5.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
        _context = context;
    }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        return await _context.TodoItems
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    // ЛР5, п.4: пошук за двома та більше Id
    // GET: api/TodoItems/search?ids=1&ids=2&ids=3
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems([FromQuery] long[] ids)
    {
        var uniqueIds = ids.Distinct().ToArray();
        if (uniqueIds.Length < 2)
        {
            return BadRequest("Для пошуку потрібно вказати щонайменше два різні Id, наприклад: ?ids=1&ids=2");
        }

        var items = await _context.TodoItems
            .Where(x => uniqueIds.Contains(x.Id))
            .Select(x => ItemToDTO(x))
            .ToListAsync();

        if (items.Count == 0)
        {
            return NotFound("Жодного запису з вказаними Id не знайдено.");
        }

        return items;
    }

    // GET: api/TodoItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return ItemToDTO(todoItem);
    }

    // PUT: api/TodoItems/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO)
    {
        if (id != todoDTO.Id)
        {
            return BadRequest();
        }

        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.Name = todoDTO.Name;
        todoItem.IsComplete = todoDTO.IsComplete;
        todoItem.DueDate = todoDTO.DueDate;
        todoItem.Priority = todoDTO.Priority;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/TodoItems
    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
    {
        var todoItem = DTOToItem(todoDTO);

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }

    // ЛР5, п.5: масове додавання записів
    // POST: api/TodoItems/bulk
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> PostTodoItems(List<TodoItemDTO> todoDTOs)
    {
        if (todoDTOs == null || todoDTOs.Count == 0)
        {
            return BadRequest("Масив записів не може бути порожнім.");
        }

        var todoItems = todoDTOs.Select(DTOToItem).ToList();

        _context.TodoItems.AddRange(todoItems);
        await _context.SaveChangesAsync();

        var result = todoItems.Select(ItemToDTO).ToList();
        return StatusCode(StatusCodes.Status201Created, result);
    }

    // DELETE: api/TodoItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id)
    {
        return _context.TodoItems.Any(e => e.Id == id);
    }

    private static TodoItem DTOToItem(TodoItemDTO dto) =>
        new TodoItem
        {
            Name = dto.Name,
            IsComplete = dto.IsComplete,
            DueDate = dto.DueDate,
            Priority = dto.Priority
        };

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete,
            DueDate = todoItem.DueDate,
            Priority = todoItem.Priority
        };
}
