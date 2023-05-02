using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class FlowerService : IFlowerService
    {
        private readonly AppDbContext _context;
        public FlowerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expert>> GetAll()
        {
            return await _context.Experts.ToListAsync();
        }

        public async Task<ExpertHeader> GetInfo()
        {
            return  await _context.ExpertHeaders.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExpertHeader>> GetInfos()
        {
            return await _context.ExpertHeaders.ToListAsync();
        }
    }
}
