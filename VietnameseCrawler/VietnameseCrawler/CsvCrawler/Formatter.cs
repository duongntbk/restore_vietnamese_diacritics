using System.Collections.Generic;
using System.Text;

namespace VietnameseCrawler.CsvCrawler
{
    public class Formatter : IFormatter
    {
        private readonly Dictionary<char, char> _diacritics;
        private readonly HashSet<char> _borderChar;
        private readonly HashSet<char> _ignoreChar;
        private readonly StringBuilder _sbOriginal;
        private readonly StringBuilder _sbStripped;
        private const char BLANK_SPACE = ' ';

        public Formatter()
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
            _borderChar = new HashSet<char>
            {
                '.',
                '?',
                '!',
                ';',
            };
            _ignoreChar = new HashSet<char>
            {
                '"',
                ',',
                '“',
                '”',
                ')',
                '(',
                ':',
                '…',
                '*'
            };
            _sbOriginal = new StringBuilder();
            _sbStripped = new StringBuilder();
        }

        public IEnumerable<(string Original, string Stripped)> Format(string text)
        {
            text = text.ToLower();
            var len = text.Length;
            _sbOriginal.Clear();
            _sbStripped.Clear();

            for (var i = 0; i < len; i++)
            {
                if (_ignoreChar.Contains(text[i]))
                {
                    continue;
                }
                if (_diacritics.TryGetValue(text[i], out char removedChar))
                {
                    _sbStripped.Append(removedChar);
                    _sbOriginal.Append(text[i]);
                }
                else if (_borderChar.Contains(text[i]))
                {
                    while ((i < len - 1) && text[i + 1] == BLANK_SPACE)
                    {
                        i++;
                    }

                    if (_sbOriginal.Length > 0 && _sbStripped.Length > 0)
                    {
                        yield return (_sbOriginal.ToString(), _sbStripped.ToString());
                    }

                    _sbOriginal.Clear();
                    _sbStripped.Clear();
                }
                else
                {
                    _sbStripped.Append(text[i]);
                    _sbOriginal.Append(text[i]);
                }
            }

            if (_sbOriginal.Length > 0 && _sbStripped.Length > 0)
            {
                yield return (_sbOriginal.ToString(), _sbStripped.ToString());
            }
        }
    }
}
