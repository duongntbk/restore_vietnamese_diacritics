using System.Collections.Generic;

namespace VietnameseCrawler.CsvCrawler
{
    public interface ICrawler
    {
        IAsyncEnumerable<string> ReadAsync(string inputPath);
    }
}
