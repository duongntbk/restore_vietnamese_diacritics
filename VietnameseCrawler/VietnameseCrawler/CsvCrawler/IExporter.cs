using System.Collections.Generic;
using System.Threading.Tasks;

namespace VietnameseCrawler.CsvCrawler
{
    public interface IExporter
    {
        Task WriteAsync(IAsyncEnumerable<string> data, string outputPath);
    }
}
