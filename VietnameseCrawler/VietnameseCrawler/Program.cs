using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VietnameseCrawler.CsvCrawler;

namespace VietnameseCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await Peek();
            //Remove();
            await Test();
        }

        static async Task Test()
        {
            var inputPath = @"C:\Learn\Python\restore_diacritic\dataset\old-newspaper.tsv";
            //var inputPath = @"C:\Learn\Python\restore_diacritic\dataset\old-newspaper-dummy.csv";
            var outputPath = @"C:\Learn\Python\restore_diacritic\dataset\old-newspaper-vietnamese.txt";

            Func<Newspaper, bool> verifyNewspaper = (line) => line.Language == "Vietnamese";
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8,
                Delimiter = "\t"
            };
            ICrawler crawler = new Crawler<Newspaper, NewspaperMapByName>(csvConfig, verifyNewspaper);
            IExporter exporter = new Exporter();
            IProcessor processor = new Processor(crawler, exporter);

            await processor.ProcessDataSet(inputPath, outputPath);
        }

        static async Task Peek()
        {
            var inputPath = @"C:\Learn\Python\restore_diacritic\dataset\old-newspaper.tsv";
            using (StreamReader reader = File.OpenText(inputPath))
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                }
            }
        }

        static void Remove()
        {
            var input = "Tôi tên là Dương, tôi đến từ Việt Nam. Năm nay tôi 32 tuổi.";
            var writer = new Exporter();
        }
    }
}
