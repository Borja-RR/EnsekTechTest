using CsvHelper;
using EnsekTechTest.Services.CsvHelper.Interfaces;
using System.Globalization;

namespace EnsekTechTest.Services.CsvHelper
{
    public class CsvService : ICsvService
    {
        public List<T> ReadCsv<T>(Stream file)
        {
            var reader = new StreamReader(file);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<T>();
            return records.ToList();
        }
    }
}
