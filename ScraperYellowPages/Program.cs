using CsvHelper;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.ComponentModel;
using System.Globalization;

namespace ScraperYellowPages
{

    class Program
    {
        static ScrapingBrowser _scrapingbrowser = new ScrapingBrowser();
        static void Main(string[] args)
        {
            List<Business> result = new List<Business>();
            var pageNum = 1;
            while (pageNum < 20)
            {
                var scrapedData = GetBusinesses("https://www.yellowpages.com/search?search_terms=Coffee%20Shops&geo_location_terms=Tallahassee%2C%20FL&page= " + pageNum++);
                result.AddRange(scrapedData);
            }
            ExportBusinessesToCsv(result);
        }

        static HtmlNode GetHtml(string url)
        {
            WebPage webPage = _scrapingbrowser.NavigateToPage(new Uri(url));
            return webPage.Html;
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

                    /*HtmlWeb web = new HtmlWeb();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc = web.Load(url);

                    foreach(HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                    {
                        HtmlAttribute att = link.Attributes["href"];

                        if (att.Value.Contains("href"))
                        {
                            Console.WriteLine(att.Value);
                        }
                    }*/
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
        static void ExportBusinessesToCsv(List<Business> result)
        {
            using (var writer = new StreamWriter($@"C:\Users\Xin\Dropbox\My PC (DESKTOP-E4HK43S)\Documents\WebScraper\Business Info_{DateTime.Now.ToFileTime()}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {

                csv.WriteRecords(result);
            }
        }
    }
}