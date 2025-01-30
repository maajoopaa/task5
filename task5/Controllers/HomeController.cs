using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using task5.Models;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using SkiaSharp;
using System.Globalization;

namespace task5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ISettingsService _settings;

        public HomeController(ILogger<HomeController> logger, ISettingsService settings)
        {
            _logger = logger;
            _settings = settings;
        }
        public IActionResult MainPage(string language="ru", int seed=0, string likes="1",string reviews="1",int lastId=1)
        {
            ViewBag.Language = language;
            ViewBag.Seed = seed;
            ViewBag.Likes = likes;
            ViewBag.Reviews = reviews;
            if(_settings.Language != language || _settings.Seed != seed || _settings.Likes != likes || _settings.Reviews != reviews)
            {
                _settings.Language = language;
                _settings.Seed = seed;
                _settings.Likes = likes;
                _settings.Reviews = reviews;
            }
            var books = GenerateBooks(language, seed, double.Parse(likes), double.Parse(reviews), 20,lastId);
            return View(books);
        }
        public IActionResult LoadMoreBooks(string language, int seed, string likes, string reviews,int lastId)
        {
            return Json(GenerateBooks(language, seed, double.Parse(likes), double.Parse(reviews), 10, lastId));
        }
        private List<Book> GenerateBooks(string language, int seed, double likes, double reviews, int count,int lastId)
        {
            Randomizer.Seed = new Random(seed+lastId);
            double variationForLikes = likes < 1.0 ? 0.0 : 1.0;
            double variationForReviews = reviews < 1.0 ? 0.0 : 1.0;
            var bookFaker = new Faker<Book>(language)
                .RuleFor(b => b.Id, f =>
                {
                    return lastId+f.IndexFaker;
                })
                .RuleFor(b => b.ISBN, f => f.Commerce.Ean13())
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(1, 4))
                .RuleFor(b => b.Authors, f => GenerateAuthors(language, 1, 3))
                .RuleFor(b => b.Publisher, f => f.Company.CompanyName() + $", {f.Random.Int(DateTime.Now.Year - 5, DateTime.Now.Year)}")
                .RuleFor(b => b.Rating, f => (int)Math.Round(f.Random.Double(likes - variationForLikes, Math.Max(Math.Min(likes + variationForLikes, 10),1))))
                .RuleFor(b => b.Reviews, f => GenerateReviews(language, reviews - variationForReviews, Math.Max(reviews + variationForReviews,1)))
                .RuleFor(b => b.Image, (f, b) => GenerateImage(b.Title, b.Authors));
            return bookFaker.Generate(count);
        }
        private List<Author> GenerateAuthors(string language, int min, int max)
        {
            var authorsFaker = new Faker<Author>(language)
                .RuleFor(a => a.Name, f => f.Name.FullName());
            return authorsFaker.GenerateBetween(min, max);
        }
        private List<Review> GenerateReviews(string language, double min, double max)
        {
            var reviewsFaker = new Faker<Review>(language)
                .RuleFor(r => r.Text, f => f.Lorem.Paragraph())
                .RuleFor(r => r.Author, f => new Author
                {
                    Name = f.Name.FullName()
                });
            return reviewsFaker.GenerateBetween((int)Math.Round(min),(int)Math.Round(max));
        }
        private string GenerateImage(string title, List<Author> authorsList)
        {
            string authors = string.Join(", ", authorsList);
            int width = 600;
            int height = 800;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.Clear(SKColor.Parse("#2F4F4F")); 

            var titlePaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 36,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            var titleText = title;
            var titleBounds = new SKRect();
            titlePaint.MeasureText(titleText, ref titleBounds);
            canvas.DrawText(titleText, width / 2f, height / 3f - titleBounds.Height / 2f, titlePaint);

            var authorsPaint = new SKPaint
            {
                Color = SKColors.LightGray,
                TextSize = 18,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal),
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            var authorsText = authors;
            var authorsBounds = new SKRect();
            authorsPaint.MeasureText(authorsText, ref authorsBounds);
            canvas.DrawText(authorsText, width / 2f, height / 2f + 20, authorsPaint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using var ms = new MemoryStream();
            data.SaveTo(ms);
            byte[] byteImage = ms.ToArray();

            return Convert.ToBase64String(byteImage);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
