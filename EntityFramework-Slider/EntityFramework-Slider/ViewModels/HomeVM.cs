using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public SliderInfo SliderInfo { get; set; }
        public About Abouts { get; set; }
        public IEnumerable<Expert>  Experts { get; set; }
        public Subscribe Subscribes { get; set; }
        public IEnumerable<Say> Says { get; set; }
        public IEnumerable<Instagram> Instagrams { get; set; }
    }
}
