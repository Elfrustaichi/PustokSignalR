using PustokBackTask.Models;

namespace PustokBackTask.ViewModels
{
    public class HomeViewModel
    {
        public List<Feature> features = new List<Feature>();

        public List<Slider> sliders = new List<Slider>();

        public List<Book> FeaturedBooks = new List<Book>();

        public List<Book> NewBooks = new List<Book>();

        public List<Book> DiscountedBooks = new List<Book>();

    }
}
