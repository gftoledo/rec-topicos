using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BibliotecaDbContext>();
var app = builder.Build();

// GET /api/categoria
app.MapGet("/api/categorias", ([FromServices] BibliotecaDbContext ctx) =>
{
    var categoria = ctx.Categoria.ToList();
    return categoria.Any() ? Results.Ok(categoria) : Results.NotFound();
});

// POST /api/categoria
app.MapPost("/api/categorias", ([FromBody] Categoria categoria, [FromServices] BibliotecaDbContext ctx) =>
{
    ctx.Categoria.Add(categoria);
    ctx.SaveChanges();
    return Results.Created($"/api/categorias/{categoria.Id}", categoria);
});

// PUT /api/categoria/{id}
app.MapPut("/api/categorias/{id}", ([FromRoute] int id, [FromBody] Status categoriaAlterado, [FromServices] BibliotecaDbContext ctx) =>
{
    var categoria = ctx.Categoria.Find(id);
    if (categoria == null) return Results.NotFound();

    categoria.Nome = categoriaAlterado.Nome;
    ctx.Categoria.Update(categoria);
    ctx.SaveChanges();
    return Results.Ok(categoria);
});








app.Run();
