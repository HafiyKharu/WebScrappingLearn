using WebScrappingLearn.Model;
using HtmlAgilityPack;
using System.Globalization;
using CsvHelper;

// creating the list that will keep the scraped data 
var products = new List<Product>();

// creating the HAP object 
var web = new HtmlWeb();

// visiting the target web page 
var document = web.Load("https://www.scrapingcourse.com/ecommerce/");

// getting the list of HTML product nodes 
var productHTMLElements = document.DocumentNode.SelectNodes("//li[contains(@class, 'product')]");
// iterating over the list of product HTML elements 
foreach (var productHTMLElement in productHTMLElements)
{
    // scraping logic 
    var url = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//a").Attributes["href"].Value);
    var image = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//img").Attributes["src"].Value);
    var name = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//h2").InnerText);
    var price = HtmlEntity.DeEntitize(productHTMLElement.SelectSingleNode(".//*[contains(@class, 'price')]").InnerText);

    var product = new Product() { Url = url, Image = image, Name = name, Price = price };
    products.Add(product);
}

// creating the CSV output file 
using var writer = new StreamWriter("./ProductsCSV/products.csv");
using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
// populating the CSV file 
csv.WriteRecords(products);
