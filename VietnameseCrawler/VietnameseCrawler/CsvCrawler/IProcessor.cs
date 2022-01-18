using System.Threading.Tasks;

namespace VietnameseCrawler.CsvCrawler
{
    public interface IProcessor
    {
        Task ProcessDataSet(string input, string output);
    }
}
