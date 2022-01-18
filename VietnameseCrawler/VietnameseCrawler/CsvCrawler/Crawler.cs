using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VietnameseCrawler.CsvCrawler
{
    public class Crawler<TRecord, TMapping> : ICrawler
        where TRecord : IText
        where TMapping : ClassMap
    {
        private readonly CsvConfiguration _csvConfig;
        private readonly Func<TRecord, bool> _verifyLineFunc;
        private readonly HashSet<char> _borderChar;
        private readonly char BLANK_SPACE = ' ';

        public Crawler(CsvConfiguration csvConfig, Func<TRecord, bool> verifyLineFunc)
        {
            _csvConfig = csvConfig;
            _verifyLineFunc = verifyLineFunc;
            _borderChar = new HashSet<char>
            {
                '.',
                '?',
                '!',
                ';'
            };
        }

        public async IAsyncEnumerable<string> ReadAsync(string inputPath)
        {
            using (var fs = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var textRead = new StreamReader(fs, Encoding.UTF8))
                using (var csv = new CsvReader(textRead, _csvConfig))
                {
                    csv.Context.RegisterClassMap<TMapping>();
                    var data = csv.GetRecordsAsync<TRecord>();

                    await foreach (var row in data)
                    {
                        if (_verifyLineFunc(row))
                        {
                            var lines = Split(row.Text);
                            foreach (var line in lines)
                            {
                                yield return line;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<string> Split(string text)
        {
            var len = text.Length;
            var sb = new StringBuilder();

            for (var i = 0; i < len; i++)
            {
                if (_borderChar.Contains(text[i]))
                {
                    while((i < len - 1) && text[i + 1] == BLANK_SPACE)
                    {
                        i++;
                    }
                    yield return sb.ToString();
                    sb.Clear();
                }
                else
                {
                    sb.Append(text[i]);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }
    }
}
