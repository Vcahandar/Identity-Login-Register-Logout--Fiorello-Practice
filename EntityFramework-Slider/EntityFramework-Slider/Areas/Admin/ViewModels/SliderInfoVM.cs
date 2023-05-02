using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class SliderInfoVM
    {
        [Required(ErrorMessage = "Don`t be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public string Description { get; set; }

        public string SignatureImage { get; set; }
        public IFormFile SignaturePhoto { get; set; }
    }
}
