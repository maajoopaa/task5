namespace task5.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Image { get; set; } = null!;
        public int Rating { get; set; }
        public List<Review> Reviews { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public string Title { get; set; } = null!;
        public List<Author> Authors { get; set; } = null!;
        public string Publisher { get; set; } = null!;
    }
}
