using WebScrappingLearn.Model;
using HtmlAgilityPack;
using System.Globalization;
using CsvHelper;

// defining a custom class to store 
// the scraped data 
// initializing HAP 
var web = new HtmlWeb();

// creating the list that will keep the scraped data 
var products = new List<Product>();
// the URL of the first pagination web page 
var firstPageToScrape = "https://www.scrapingcourse.com/ecommerce/page/1/";
// the list of pages discovered during the crawling task 
var pagesDiscovered = new List<string> { firstPageToScrape };
// the list of pages that remains to be scraped 
var pagesToScrape = new Queue<string>();
// initializing the list with firstPageToScrape 
pagesToScrape.Enqueue(firstPageToScrape);
// current crawling iteration 
int i = 1;
// the maximum number of pages to scrape before stopping 
int limit = 12;
// until there is a page to scrape or limit is hit 
while (pagesToScrape.Count != 0 && i < limit)
{
    // getting the current page to scrape from the queue 
    var currentPage = pagesToScrape.Dequeue();
    // loading the page 
    var currentDocument = web.Load(currentPage);
    // selecting the list of pagination HTML elements 
    var paginationHTMLElements = currentDocument.DocumentNode.SelectNodes("//a[contains(@class, 'page-numbers')]") ?? new HtmlNodeCollection(null);
    // to avoid visiting a page twice 
    foreach (var paginationHTMLElement in paginationHTMLElements)
    {
        // extracting the current pagination URL 
        var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;
        // if the page discovered is new 
        if (!pagesDiscovered.Contains(newPaginationLink))
        {
            // if the page discovered needs to be scraped 
            if (!pagesToScrape.Contains(newPaginationLink))
            {
                pagesToScrape.Enqueue(newPaginationLink);
            }
            pagesDiscovered.Add(newPaginationLink);
        }
    }
    // getting the list of HTML product nodes 
    var productHTMLElements = currentDocument.DocumentNode.SelectNodes("//li[contains(@class, 'product')]") ?? new HtmlNodeCollection(null);
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
    // incrementing the crawling counter 
    i++;
}
// opening the CSV stream reader 
using (var writer = new StreamWriter("./ProdutCSV/products.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    // populating the CSV file 
    csv.WriteRecords(products);
}