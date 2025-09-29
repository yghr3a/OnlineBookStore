namespace OnlineBookStore.Models.Entities
{
    public class BookEntityModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Author { get; set; }
        public decimal Price { get; set; }
        public string? Introduction { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
