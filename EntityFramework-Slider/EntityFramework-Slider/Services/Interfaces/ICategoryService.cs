using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<List<Category>> GetPaginateDatas(int page, int take);
        Task<int> GetCountAsync();

    }
}
