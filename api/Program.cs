using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//adicionando servido ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

//adiconar produto
app.MapPost("/products", (Product product) => {
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapGet("/products/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);
    if(product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

//edit method
app.MapPut("/products", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();
});

//delete method
app.MapDelete("/products/{code}", ([FromRoute] string code) => {
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok();
});


if (app.Environment.IsStaging())
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });

app.Run();


//lista é um array com mais funções, excluir, add, put, delete, etc...
public static class ProductRepository{
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    //pega a lista produtos do appsettings.json e adiciona a listagem
    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    //method add
    public static void Add(Product product)
    {
        if (Products == null)
            Products = new List<Product>();

        Products.Add(product); 
    }  

    //method GetBy
    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove (Product product)
    {
        Products.Remove(product);
    }

}

//classe categoria do produto
public class Category {
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Tag {
    public int Id { get; set; }
    public string Name { get; set; }

    public int ProductId { get; set; }
}

public class Product{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; }
}

public class ApplicationDbContext : DbContext {

    //fluentAPI
    protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        builder.Entity<Product>()
            .Property(p => p.Name).HasMaxLength(120).IsRequired();
        builder.Entity<Product>()
            .Property(p => p.Code).HasMaxLength(20).IsRequired();
    }
    public DbSet<Product> Products { get; set; }

    //configurando conexão com banco
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(
            "connection string"
        );
}