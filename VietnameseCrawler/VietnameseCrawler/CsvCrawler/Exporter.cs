using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VietnameseCrawler.CsvCrawler
{
    public class Exporter : IExporter
    {
        private readonly Dictionary<char, char> _diacritics;

        public Exporter()
        {
            _diacritics = new Dictionary<char, char>
            {
                { 'à', 'a' },
                { 'ả', 'a' },
                { 'ã', 'a' },
                { 'á', 'a' },
                { 'ạ', 'a' },
                { 'ă', 'a' },
                { 'ằ', 'a' },
                { 'ẳ', 'a' },
                { 'ẵ', 'a' },
                { 'ắ', 'a' },
                { 'ặ', 'a' },
                { 'â', 'a' },
                { 'ầ', 'a' },
                { 'ẩ', 'a' },
                { 'ẫ', 'a' },
                { 'ấ', 'a' },
                { 'ậ', 'a' },
                { 'đ', 'd' },
                { 'è', 'e' },
                { 'ẻ', 'e' },
                { 'ẽ', 'e' },
                { 'é', 'e' },
                { 'ẹ', 'e' },
                { 'ê', 'e' },
                { 'ề', 'e' },
                { 'ể', 'e' },
                { 'ễ', 'e' },
                { 'ế', 'e' },
                { 'ệ', 'e' },
                { 'ì', 'i' },
                { 'ỉ', 'i' },
                { 'ĩ', 'i' },
                { 'í', 'i' },
                { 'ị', 'i' },
                { 'ò', 'o' },
                { 'ỏ', 'o' },
                { 'õ', 'o' },
                { 'ó', 'o' },
                { 'ọ', 'o' },
                { 'ô', 'o' },
                { 'ồ', 'o' },
                { 'ổ', 'o' },
                { 'ỗ', 'o' },
                { 'ố', 'o' },
                { 'ộ', 'o' },
                { 'ơ', 'o' },
                { 'ờ', 'o' },
                { 'ở', 'o' },
                { 'ỡ', 'o' },
                { 'ớ', 'o' },
                { 'ợ', 'o' },
                { 'ù', 'u' },
                { 'ủ', 'u' },
                { 'ũ', 'u' },
                { 'ú', 'u' },
                { 'ụ', 'u' },
                { 'ư', 'u' },
                { 'ừ', 'u' },
                { 'ử', 'u' },
                { 'ữ', 'u' },
                { 'ứ', 'u' },
                { 'ự', 'u' },
                { 'ỳ', 'y' },
                { 'ỷ', 'y' },
                { 'ỹ', 'y' },
                { 'ý', 'y' },
                { 'ỵ', 'y' }
            };
        }

        public async Task WriteAsync(IAsyncEnumerable<string> data, string outputPath)
        {
            using (var fs = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                using (var textWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    await foreach (var line in data)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        var convertedLine = Convert(line);
                        await textWriter.WriteLineAsync($"{line}\t{convertedLine}");
                    }
                }
            }
        }

        private string Convert(string original)
        {
            original = original.ToLower();
            var len = original.Length;
            var sb = new StringBuilder();

            for (var i = 0; i < len; i++)
            {
                if (_diacritics.TryGetValue(original[i], out char removedChar))
                {
                    sb.Append(removedChar);
                }
                else
                {
                    sb.Append(original[i]);
                }
            }

            return sb.ToString();
        }
    }
}
