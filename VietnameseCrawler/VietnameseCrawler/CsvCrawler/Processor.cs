using System.Threading.Tasks;

namespace VietnameseCrawler.CsvCrawler
{
    public class Processor : IProcessor
    {
        private readonly ICrawler _crawler;
        private readonly IExporter _exporter;

        public Processor(ICrawler crawler, IExporter exporter)
        {
            _crawler = crawler;
            _exporter = exporter;
        }

        public async Task ProcessDataSet(string input, string output)
        {
            var data = _crawler.ReadAsync(input);
            await _exporter.WriteAsync(data, output);
        }
    }
}
