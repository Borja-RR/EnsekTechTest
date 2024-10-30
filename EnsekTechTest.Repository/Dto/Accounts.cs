namespace EnsekTechTest.Repository.Dto
{
    public class Accounts
    {
        public long AccountId { get; set; }
        public required string FirstName { get; set; }
        public required string Lastname { get; set; }

        // Navigation property for the related Meters
        public ICollection<Meters>? Meters { get; set; }
    }
}
