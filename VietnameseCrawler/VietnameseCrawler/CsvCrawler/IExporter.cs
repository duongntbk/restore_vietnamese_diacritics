using System.Collections.Generic;
using System.Threading.Tasks;

namespace VietnameseCrawler.CsvCrawler
{
    public interface IExporter
    {
        Task WriteAsync(IAsyncEnumerable<(string Original, string Stripped)> data, string outputPath);
    }
}
