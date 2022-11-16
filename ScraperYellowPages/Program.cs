using CsvHelper;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.Globalization;

namespace ScraperYellowPages
{

    class Program
    {
        static ScrapingBrowser _scrapingbrowser = new ScrapingBrowser();

        static void Main(string[] args)
        {
            var scrapedData = GetBusinesses("https://api.rocketscrape.com/?apiKey=fed04a8c-59c3-4412-9173-90585a6df7d9&url=https://www.yellowpages.com/search?search_terms=cabinet&geo_location_terms=Orlando%2C%20FL&page=6");
            ExportBusinessesToCsv(scrapedData);
        }
        static List<Business> GetBusinesses(string url)
        {
            List<Business> result = new List<Business>();
            var html = GetHtml(url);
            var businesses = html.CssSelect(".v-card");

            foreach (var business in businesses)
            {
                try
                {
                    var name = business.CssSelect(".business-name");
                    var number = business.CssSelect(".phone");
                    var address = business.CssSelect(".adr");

                    if (name.Count() == 1 && number.Count() == 1 && address.Count() == 1)
                    {
                        var newBusiness = new Business();
                        newBusiness.Name = name.Single().InnerText;
                        newBusiness.Number = number.Single().InnerText;
                        newBusiness.Address = address.Single().InnerText;

                        result.Add(newBusiness);
                    }
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine("error");
                }
            }
            return result;
        }
        static void ExportBusinessesToCsv(List<Business> firstBusinessDetails)
        {
            using (var writer = new StreamWriter($@"C:\Users\Xin\Dropbox\My PC (DESKTOP-E4HK43S)\Documents\WebScraper\Business Info_{DateTime.Now.ToFileTime()}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(firstBusinessDetails);

            }
        }
        static HtmlNode GetHtml(string url)
        {
            WebPage webPage = _scrapingbrowser.NavigateToPage(new Uri(url));
            return webPage.Html;
        }
    }
}