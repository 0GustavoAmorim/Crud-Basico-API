
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