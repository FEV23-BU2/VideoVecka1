namespace VideoVecka1;

using Microsoft.AspNetCore.Mvc;

/*
Todo:
  id: int
  title: string
  description: string
  completed: bool
  creationDate: DateTime

Skapa todos:
POST /api/todo
title, description

Radera todos:
DELETE /api/todo/{id}

Uppdatera todos:
PUT /api/todo/{id}
completed

Hämta todos:
GET /api/todos

*/


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddSingleton<TodoService, TodoService>();

        var app = builder.Build();

        app.MapControllers();
        app.UseHttpsRedirection();

        app.Run();
    }
}

public class Todo
{
    private static int ID_COUNTER = 0;

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public DateTime CreationDate { get; set; }

    public Todo(string title, string description)
    {
        this.Title = title;
        this.Description = description;
        this.Completed = false;
        this.CreationDate = DateTime.Now;
        this.Id = ID_COUNTER++;
    }
}

public class CreateTodoDto
{
    public string Title { get; set; }
    public string Description { get; set; }

    public CreateTodoDto(string title, string description)
    {
        this.Title = title;
        this.Description = description;
    }
}

[ApiController]
[Route("api")]
public class TodoController : ControllerBase
{
    private TodoService todoService;

    public TodoController(TodoService todoService)
    {
        this.todoService = todoService;
    }

    [HttpPost("todo")]
    public IActionResult CreateTodo([FromBody] CreateTodoDto dto)
    {
        try
        {
            Todo todo = todoService.CreateTodo(dto.Title, dto.Description);
            return Ok(todo);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpDelete("todo/{id}")]
    public IActionResult RemoveTodo(int id)
    {
        Todo? todo = todoService.RemoveTodo(id);
        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpPut("todo/{id}")]
    public IActionResult UpdateTodo(int id, [FromQuery] bool completed)
    {
        Todo? todo = todoService.UpdateTodo(id, completed);
        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpGet("todos")]
    public List<Todo> GetAllTodos()
    {
        return todoService.GetAllTodos();
    }
}

public class TodoService
{
    private List<Todo> todos = new List<Todo>();

    public Todo CreateTodo(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must not be null or empty");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be null or empty");
        }

        Todo todo = new Todo(title, description);
        todos.Add(todo);
        return todo;
    }

    public Todo? RemoveTodo(int id)
    {
        for (int i = 0; i < todos.Count; i++)
        {
            if (todos[i].Id == id)
            {
                Todo todo = todos[i];
                todos.RemoveAt(i);
                return todo;
            }
        }

        return null;
    }

    public Todo? UpdateTodo(int id, bool completed)
    {
        for (int i = 0; i < todos.Count; i++)
        {
            if (todos[i].Id == id)
            {
                Todo todo = todos[i];
                todo.Completed = completed;
                return todo;
            }
        }

        return null;
    }

    public List<Todo> GetAllTodos()
    {
        return todos;
    }
}
