# WebScrappingLearn

Simple .NET web-scraping sample that uses Selenium to collect product data and exports it to CSV.

## Features
- Scrapes product Url, Image, Name and Price from a demo e‑commerce page
- Exports results to `ProductsCSV/products.csv`
- Headless Chrome (Selenium) usage

## Prerequisites (Windows)
- .NET 9 SDK
- Google Chrome (matching ChromeDriver used by Selenium)
- ChromeDriver (Selenium.WebDriver package may provide runtime support; ensure `selenium-manager` or a compatible driver is available)

## Setup
1. Open a terminal in the repository root:
   ```powershell
   cd "d:\Learning .net\WebScrappingLearn"
   ```
2. Restore packages:
   ```powershell
   dotnet restore
   ```

## Run
From the project folder:
```powershell
cd "d:\Learning .net\WebScrappingLearn\WebScrappingLearn"
dotnet run
```
The application scrapes the target site and writes results to `ProductsCSV/products.csv`. If the app starts an HTTP endpoint (when modified to a web app), open the URL shown in the console.

## Output
- CSV: `ProductsCSV/products.csv`
- You can open it in Excel or any text editor.

## Git / .gitignore
This repository includes a `.gitignore` that ignores build artifacts and NuGet package files. To ensure you do not commit binaries, nupkg files, or build outputs, run (repo root):
```powershell
git add .gitignore
# Untrack common build outputs and packages already committed
git rm -r --cached WebScrappingLearn/bin WebScrappingLearn/obj WebScrappingLearn/**/bin WebScrappingLearn/**/obj packages .nuget 2>$null
git rm --cached **/*.nupkg **/*.snupkg 2>$null
git rm --cached project.assets.json project.nuget.cache 2>$null
git add -A
git commit -m "Add .gitignore and stop tracking build artifacts and packages"
```
Adjust paths if your files are in different locations.

## Troubleshooting
- NullReferenceException when scraping: ensure CSS selectors match the target page and add null checks before accessing elements.
- ChromeDriver errors: ensure Chrome version matches the driver or use selenium-manager to obtain a compatible driver.

## Notes
- Do not commit `bin/`, `obj/`, or NuGet package files to source control.
- If you want to persist `ProductsCSV` in the repo, remove it from `.gitignore`; otherwise it is ignored by default here.

Reference: [Web Scraping in C# - ZenRows](https://www.zenrows.com/blog/web-scraping-c-sharp#csharp-scraping-libraries)
