namespace EntityFramework_Slider.Models
{
    public class Advantage : BaseEntity
    {
        public string? Description { get; set; }
        public int AboutId { get; set; }
        public About About { get; set; }
    }
}
