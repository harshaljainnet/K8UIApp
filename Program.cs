using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    var json = await client.GetStringAsync(
        "http://k8backendapp-service:8080/api/products");

    var products = JsonSerializer.Deserialize<List<Product>>(json,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    var rows = string.Join("", products!.Select(p => $"""
        <tr>
            <td>{p.ProductId}</td>
            <td>{p.ProductName}</td>
            <td>{p.ProductPrice:N0}</td>
        </tr>
        """));

    var html = $$"""
    <!DOCTYPE html>
    <html>
    <head>
        <title>Products</title>
        <style>
            body {
                font-family: Arial;
                background: #f5f7fa;
                padding: 40px;
            }

            .container {
                max-width: 800px;
                margin: auto;
                background: white;
                padding: 30px;
                border-radius: 10px;
            }

            h1 {
                margin-bottom: 5px;
            }

            .subtitle {
                color: #666;
                margin-bottom: 25px;
            }

            table {
                width: 100%;
                border-collapse: collapse;
            }

            th, td {
                padding: 14px;
                text-align: left;
                border-bottom: 1px solid #ddd;
            }

            th {
                background: #f1f3f5;
            }
        </style>
    </head>

    <body>
        <div class="container">

            <h1>Products</h1>

            <div class="subtitle">
                Products loaded from Backend API running in Kubernetes
            </div>

            <table>
                <tr>
                    <th>Product ID</th>
                    <th>Product Name</th>
                    <th>Price</th>
                </tr>

                {{rows}}

            </table>

        </div>
    </body>
    </html>
    """;

    return Results.Content(html, "text/html");
});

app.Run();

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal ProductPrice { get; set; }
}