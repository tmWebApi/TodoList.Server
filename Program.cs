using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();

// CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();
app.MapGet("/", () => "TodoList is runing!");
app.MapGet("/items", ItemService.GetAllItemsAsync);
app.MapPost("/items", ItemService.AddItemAsync);
app.MapPut("items/{itemId}/status", ItemService.UpdateItemStatusAsync);
app.MapDelete("items/{itemId}", ItemService.DeleteItemAsync);
app.Run();

class ItemService
{
    public static async Task<IResult> DeleteItemAsync(ToDoDbContext db, int itemId)
    {
        var existingItem = await db.Items.FindAsync(itemId);

        if (existingItem == null)
        {
            return Results.NotFound($"Task with ID {itemId} not found");
        }

        db.Items.Remove(existingItem);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    public static async Task<IResult> UpdateItemStatusAsync(ToDoDbContext db, int itemId, Item updateStatus)
    {
        var existingItem = await db.Items.FindAsync(itemId);

        if (existingItem == null)
        {
            return Results.NotFound($"Task with ID {itemId} not found");
        }
        existingItem.IsComplete = updateStatus.IsComplete;

        await db.SaveChangesAsync();

        return Results.Ok(existingItem);
    }
    public static async Task<IResult> GetAllItemsAsync(ToDoDbContext db)
    {        
        Console.WriteLine("get items", db);
        var itmes = await db.Items.ToListAsync();
        return Results.Ok(itmes);
    }
    public static async Task<IResult> AddItemAsync(ToDoDbContext db, Item newItem)
    {

        await db.Items.AddAsync(newItem);
        await db.SaveChangesAsync();
        return Results.Created($"/items/{newItem.Id}", newItem);
    }
}
