using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IFlowerService
    {
        Task<IEnumerable<Expert>> GetAll();
        Task<ExpertHeader> GetInfo();
        Task<IEnumerable<ExpertHeader>> GetInfos();
    }
}
