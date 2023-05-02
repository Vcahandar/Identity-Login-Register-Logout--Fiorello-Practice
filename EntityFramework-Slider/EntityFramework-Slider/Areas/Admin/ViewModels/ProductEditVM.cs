using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ProductEditVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        public ICollection<ProductImage>  productImages { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
