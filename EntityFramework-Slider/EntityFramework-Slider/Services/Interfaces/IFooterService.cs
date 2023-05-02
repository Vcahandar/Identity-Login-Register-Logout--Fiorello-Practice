using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IFooterService
    {
        Task<IEnumerable<Social>> GetAll();
    }
}
