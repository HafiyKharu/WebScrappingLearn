using WebScrappingLearn.Model;
using HtmlAgilityPack;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var web = new HtmlWeb();
var products = new List<Product>();

// Load the HTML document from a URL or file
var document = web.Load("https://www.scrapingcourse.com/ecommerce/");

// safe-get the product elements (SelectNodes returns null if nothing matches)
var productHTMLElements = document?.DocumentNode?.SelectNodes("//li[contains(@class, 'product')]");

// if nothing found, continue with an empty list
if (productHTMLElements != null)
{
    // iterating over the list of product elements 
    foreach (var productHTMLElement in productHTMLElements)
    {
        // use safe accessors to avoid null refs
        var urlRaw = productHTMLElement.SelectSingleNode(".//a")?.GetAttributeValue("href", string.Empty) ?? string.Empty;
        var imageRaw = productHTMLElement.SelectSingleNode(".//img")?.GetAttributeValue("src", string.Empty) ?? string.Empty;
        var nameRaw = productHTMLElement.SelectSingleNode(".//h2")?.InnerText?.Trim() ?? string.Empty;
        var priceRaw = productHTMLElement.SelectSingleNode(".//*[contains(@class, 'price')]")?.InnerText?.Trim() ?? string.Empty;

        // decode HTML entities and skip empty/invalid entries if desired
        var url = HtmlEntity.DeEntitize(urlRaw);
        var image = HtmlEntity.DeEntitize(imageRaw);
        var name = HtmlEntity.DeEntitize(nameRaw);
        var price = HtmlEntity.DeEntitize(priceRaw);

        if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(image) && string.IsNullOrEmpty(price))
            continue;

        var product = new Product() { Url = url, Image = image, Name = name, Price = price };
        products.Add(product);
    }
}

// Render HTML when a browser requests the root, otherwise return JSON
app.MapGet("/", (HttpContext http) =>
{
    var accept = http.Request.Headers["Accept"].ToString();
    if (accept.Contains("text/html"))
    {
        var html = RenderHtml(products);
        return Results.Content(html, "text/html; charset=utf-8");
    }

    return Results.Json(products);
});

app.Run();

static string RenderHtml(List<Product> products)
{
    var sb = new StringBuilder();
    var enc = HtmlEncoder.Default;
    sb.AppendLine("<!doctype html>");
    sb.AppendLine("<html><head><meta charset='utf-8'><title>Products</title>");
    sb.AppendLine("<style>body{font-family:Segoe UI,Arial;margin:20px}ul{list-style:none;padding:0}li{margin:12px 0;padding:12px;border:1px solid #ddd;border-radius:6px}img{max-width:150px;display:block;margin:6px 0}</style>");
    sb.AppendLine("</head><body>");
    sb.AppendLine("<h1>Product List</h1>");
    sb.AppendLine("<ul>");
    foreach (var p in products)
    {
        sb.AppendLine("<li>");
        sb.AppendLine($"<h2>{enc.Encode(p.Name ?? "")}</h2>");
        if (!string.IsNullOrEmpty(p.Image))
            sb.AppendLine($"<img src=\"{enc.Encode(p.Image)}\" alt=\"{enc.Encode(p.Name ?? "")}\" />");
        sb.AppendLine($"<p>Price: {enc.Encode(p.Price ?? "")}</p>");
        if (!string.IsNullOrEmpty(p.Url))
            sb.AppendLine($"<p><a href=\"{enc.Encode(p.Url)}\" target=\"_blank\">View product</a></p>");
        sb.AppendLine("</li>");
    }
    sb.AppendLine("</ul>");
    sb.AppendLine("</body></html>");
    return sb.ToString();
}
