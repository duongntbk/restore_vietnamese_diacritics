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
        
        private readonly IFormatter _formatter;

        public Crawler(CsvConfiguration csvConfig, Func<TRecord, bool> verifyLineFunc, IFormatter formatter)
        {
            _csvConfig = csvConfig;
            _verifyLineFunc = verifyLineFunc;
            _formatter = formatter;
        }

        public async IAsyncEnumerable<(string Original, string Stripped)> ReadAsync(string inputPath)
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
                            var formatted = _formatter.Format(row.Text);
                            foreach (var (original, stripped) in formatted)
                            {
                                yield return (original, stripped);
                            }
                        }
                    }
                }
            }
        }
    }
}
