using CsvHelper.Configuration;

namespace VietnameseCrawler.CsvCrawler
{
    public class NewspaperMapByName : ClassMap<Newspaper>
    {
        public NewspaperMapByName()
        {
            Map(p => p.Language).Name("Language");
            Map(p => p.Text).Name("Text");
        }
    }
}
