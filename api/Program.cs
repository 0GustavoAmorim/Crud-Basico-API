using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
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

public class Product{
    public string Code { get; set; }
    public string Name { get; set; }
}