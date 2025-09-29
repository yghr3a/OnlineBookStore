namespace OnlineBookStore.Models.ViewModels
{
    public class BookViewModel
    {
        public int Number { get; set; }
        public required string Name { get; set; }
        public string? Author { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
