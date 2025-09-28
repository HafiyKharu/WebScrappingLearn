using WebScrappingLearn.Model;
using CsvHelper; 
using System.Globalization; 
using OpenQA.Selenium; 
using OpenQA.Selenium.Chrome; 

var products = new List<Product>();

// to open Chrome in headless mode 
var chromeOptions = new ChromeOptions();
chromeOptions.AddArguments("headless");

// starting a Selenium instance 
using (var driver = new ChromeDriver(chromeOptions))
{
    // navigating to the target page in the browser 
    driver.Navigate().GoToUrl("https://www.scrapingcourse.com/ecommerce/");

    // getting the HTML product elements 
    var productHTMLElements = driver.FindElements(By.CssSelector("li.product"));
    // iterating over them to scrape the data of interest 
    foreach (var productHTMLElement in productHTMLElements)
    {
        // scraping logic 
        var url = productHTMLElement.FindElement(By.CssSelector("a")).GetAttribute("href");
        var image = productHTMLElement.FindElement(By.CssSelector("img")).GetAttribute("src");
        var name = productHTMLElement.FindElement(By.CssSelector("h2")).Text;
        var price = productHTMLElement.FindElement(By.CssSelector(".price")).Text;

        var product = new Product() { Url = url, Image = image, Name = name, Price = price };

        products.Add(product);
    }
}

// export logic 
using (var writer = new StreamWriter("./ProductsCSV/products.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(products);
}