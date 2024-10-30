namespace EnsekTechTest.Services.CsvHelper.Interfaces
{
    public interface ICsvService
    {
        public List<T> ReadCsv<T>(Stream file);
    }
}
