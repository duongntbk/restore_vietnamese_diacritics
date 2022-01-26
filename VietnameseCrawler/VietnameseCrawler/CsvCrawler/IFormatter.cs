using System.Collections.Generic;

namespace VietnameseCrawler.CsvCrawler
{
    public interface IFormatter
    {
        IEnumerable<(string Original, string Stripped)> Format(string text);
    }
}
