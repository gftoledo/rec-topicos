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
app.MapPut("/api/categorias/{id}", ([FromRoute] int id, [FromBody] Categoria categoriaAlterado, [FromServices] BibliotecaDbContext ctx) =>
{
    var categoria = ctx.Categoria.Find(id);
    if (categoria == null) return Results.NotFound();

    categoria.Nome = categoriaAlterado.Nome;
    ctx.Categoria.Update(categoria);
    ctx.SaveChanges();
    return Results.Ok(categoria);
});




// GET /api/livros
app.MapGet("/api/livros", ([FromServices] BibliotecaDbContext ctx) =>
{
    var livros = ctx.Livro.Include(p => p.Categoria).ToList();
    return livros.Any() ? Results.Ok(livros) : Results.NotFound();
});

// GET /api/livros/{id}
app.MapGet("/api/livros/{id}", ([FromRoute] int id, [FromServices] BibliotecaDbContext ctx) =>
{
    var livro = ctx.Livro.Find(id);
    return livro == null ? Results.NotFound() : Results.Ok(livro);
});





//POST/api/livro
app.MapPost("/api/livros", ([FromBody] Livro livro, [FromServices] BibliotecaDbContext ctx) =>
{
    if (string.IsNullOrWhiteSpace(livro.Titulo) || livro.Titulo.Length < 3)
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");

    if (string.IsNullOrWhiteSpace(livro.Autor) || livro.Autor.Length < 3)
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");

    var categoria = ctx.Categoria.Find(livro.CategoriaId);
    if (categoria == null)
        return Results.BadRequest("Categoria inválida. O ID da categoria fornecido não existe.");

    ctx.Livro.Add(livro);
    ctx.SaveChanges();
    return Results.Created($"/api/livros/{livro.Id}", livro);
});


//PUT/api/livros/{id}
app.MapPut("/api/livros/{id}", ([FromRoute] int id, [FromBody] Livro livroAlterado, [FromServices] BibliotecaDbContext ctx) =>
{
    var livro = ctx.Livro.Find(id);
    if (livro == null)
        return Results.NotFound(new { erro = "Livro não encontrado." });

    
    if (string.IsNullOrWhiteSpace(livroAlterado.Titulo) || livroAlterado.Titulo.Length < 3)
    {
        return Results.BadRequest(new { erro = "Título deve ter no mínimo 3 caracteres." });
    }

    if (string.IsNullOrWhiteSpace(livroAlterado.Autor) || livroAlterado.Autor.Length < 3)
    {
        return Results.BadRequest(new { erro = "Autor deve ter no mínimo 3 caracteres." });
    }

    var categoria = ctx.Categoria.Find(livroAlterado.CategoriaId);
    if (categoria == null)
    {
        return Results.BadRequest(new { erro = "Categoria inválida. O ID da categoria fornecido não existe." });
    }
    
    livro.Titulo = livroAlterado.Titulo;
    livro.Autor = livroAlterado.Autor;
    livro.Categoria = categoria;

    ctx.Livro.Update(livro);
    ctx.SaveChanges();

    return Results.Ok(livro);
});

//DELETE/api/livro/{id}
app.MapDelete("/api/livros/{id}", ([FromRoute] int id, [FromServices] BibliotecaDbContext ctx) =>
{
    var livro = ctx.Livro.Find(id);
    if (livro == null) return Results.NotFound();

    ctx.Livro.Remove(livro);
    ctx.SaveChanges();
    return Results.Ok(livro);
});


app.Run();
