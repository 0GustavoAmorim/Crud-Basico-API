using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//adicionando serviço de maneira correta com connection string no appsettings
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["database:SqlServer"]);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

//adiconar produto //instancia produto, preenche tudo que necessário, salva no banco
app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product { 
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags){
            product.Tags.Add(new Tag{ Name = item});
        }
    }

    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});

//consulta produto
app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {
    var product = context.Products
    .Include(p => p.Category)
    .Include(p => p.Tags)
    .Where(p => p.Id == id).First();
    if(product != null){
        Console.WriteLine("Product found");
        return Results.Ok(product);
    }
    return Results.NotFound();
});

//edit method
app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) => {
    var product = context.Products
    .Include(p => p.Tags)
    .Where(p => p.Id == id).First();
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();



    product.Code = productRequest.Code;
    product.Name = productRequest.Name;
    product.Description = productRequest.Description;
    product.Category = category;
    product.Tags = new List<Tag>();
    if (productRequest.Tags != null){
        product.Tags = new List<Tag>();
        foreach(var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item});
        }
    }
    context.SaveChanges();
    return Results.Ok();
});

//delete method
app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {
    var product = context.Products.Where(p => p.Id == id).First();
    context.Products.Remove(product);
    context.SaveChanges();
    return Results.Ok();
});


if (app.Environment.IsStaging())
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });

app.Run();
