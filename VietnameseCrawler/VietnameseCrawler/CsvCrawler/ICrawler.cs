using System.Collections.Generic;

namespace VietnameseCrawler.CsvCrawler
{
    public interface ICrawler
    {
        IAsyncEnumerable<(string Original, string Stripped)> ReadAsync(string inputPath);
    }
}
