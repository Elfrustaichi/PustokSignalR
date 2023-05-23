using PustokBackTask.Models;

namespace PustokBackTask.ViewModels
{
    public class HomeViewModel
    {
        public List<Feature> features = new List<Feature>();

        public List<Sliders> sliders = new List<Sliders>();

        public List<Book> FeaturedBooks = new List<Book>();

        public List<Book> NewBooks = new List<Book>();

        public List<Book> DiscountedBooks = new List<Book>();

    }
}
