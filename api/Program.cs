using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//adiconar produto
app.MapPost("/saveproduct", (Product product) => {
    ProductRepository.Add(product);
});


app.MapGet("/getproduct/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);

    return product;
});

//edit method
app.MapPut("/editproduct", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
});

//delete method
app.MapDelete("/deleteproduct/{code}", ([FromRoute] string code) => {
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
});

app.Run();


//lista é um array com mais funções, excluir, add, put, delete, etc...
public static class ProductRepository{
    public static List<Product> Products { get; set; }

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