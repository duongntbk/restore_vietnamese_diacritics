using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VietnameseCrawler.CsvCrawler
{
    public class Exporter : IExporter
    {
        public async Task WriteAsync(IAsyncEnumerable<(string Original, string Stripped)> data, string outputPath)
        {
            using (var fs = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                using (var textWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    await foreach (var (original, stripped) in data)
                    {
                        await textWriter.WriteLineAsync($"{stripped}\t{original}");
                    }
                }
            }
        }
    }
}
