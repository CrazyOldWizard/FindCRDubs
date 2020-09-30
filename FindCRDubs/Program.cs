using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FindCRDubs
{
    class Program
    {
        private const string SEARCHURL = @"https://www.crunchyroll.com/videos/anime/alpha?group=all";
        private const string crURL = @"https://www.crunchyroll.com";
        static void Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var txtFile = Path.Combine(currentDir, "DubbedShows.txt");
            File.WriteAllText(txtFile, "Shows on CR that are dubbed");
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent: Other");
            var source = webClient.DownloadString(SEARCHURL);
            var doc = new HtmlDocument();
            doc.LoadHtml(source);
            var links = doc.DocumentNode.Descendants("a");
           
            foreach (var link in links)
            {
                var atts = link.GetAttributes();
                var allAtts = atts.ToArray();
                foreach(var att in allAtts)
                {
                    if(att.Name == "token" && att.Value == "shows-portraits")
                    {
                        var lnk = atts.Where(a => a.Name == "href").Select(a => a.Value).FirstOrDefault();
                        var showURL = crURL + lnk;
                        if(CheckForDub(showURL))
                        {
                            Console.WriteLine($"{showURL} is dubbed");
                            File.AppendAllText(txtFile, Environment.NewLine + showURL);
                        }
                    }
                }
            }
            Console.WriteLine("FINISHED SEARCH");
            Console.WriteLine($"Txt file with results can be found at: {txtFile}");
            Process.Start(txtFile);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }

        private static bool CheckForDub(string url)
        {
            Console.WriteLine($"Checking {url} ...");
            var wc = new WebClient();
            wc.Headers.Add("User-Agent: Other");
            var source = wc.DownloadString(url);
            if (source.ToLower().Contains("english dub"))
                return true;
            else
                return false;
        }


    }
}
