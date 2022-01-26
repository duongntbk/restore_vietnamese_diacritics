using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using VietnameseCrawler.CsvCrawler;

namespace VietnameseCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Must provide path to input file and output file.");
            }

            var inputPath = args[0];
            var outputPath = args[1];

            await Run(inputPath, outputPath);
        }

        static async Task Run(string inputPath, string outputPath)
        {
            Func<Newspaper, bool> verifyNewspaper = (line) => line.Language == "Vietnamese";
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8,
                Delimiter = "\t"
            };
            IFormatter formatter = new Formatter();
            ICrawler crawler = new Crawler<Newspaper, NewspaperMapByName>(csvConfig, verifyNewspaper, formatter);
            IExporter exporter = new Exporter();
            IProcessor processor = new Processor(crawler, exporter);

            await processor.ProcessDataSet(inputPath, outputPath);
        }
    }
}
