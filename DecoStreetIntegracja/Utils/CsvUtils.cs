using CsvHelper;
using CsvHelper.Configuration;
using DecoStreetIntegracja.Integrator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DecoStreetIntegracja.Utils
{
    public class CsvUtils
    {
        public List<string> GetExistingSymbols(string fileName)
        {
            var result = new List<string>();

            using (var reader = new StreamReader(@"C:\Users\mariu\Downloads\" + fileName + ".csv"))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = (headerNames, index, context) =>
                {
                },
            }))
            {
                var records = csv.GetRecords<CsvRowProducerAndName>().ToList();
                var regex = new Regex("^[Gg][0-9]{1,10}?");
                foreach (var row in records)
                {
                    if (row.producer == "Obrazy GB")
                    {
                        var symbol = row.name.Split(' ').FirstOrDefault(x => regex.IsMatch(x));
                        if (!string.IsNullOrWhiteSpace(symbol))
                        {
                            result.Add(symbol);
                        }
                    }
                }
            }

            return result;
        }

        public List<string> GetBazarBizarDecostreetSkus(string fileName)
        {
            var result = new List<string>();

            using (var reader = new StreamReader(@"Files/" + fileName + ".csv"))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = (headerNames, index, context) =>
                {
                },
            }))
            {
                var records = csv.GetRecords<CsvRowSku>().ToList();
                foreach (var row in records)
                {
                    result.Add(row.Sku.Trim());
                }
            }

            return result;
        }
    }
}
