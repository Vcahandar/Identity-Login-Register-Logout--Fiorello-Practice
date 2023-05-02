using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlowerController : Controller
    {
        private readonly AppDbContext _contex;
        private readonly IFlowerService _flowerService;
        public FlowerController(AppDbContext contex, IFlowerService flowerService)
        {
            _contex = contex;
            _flowerService = flowerService;
        }

        public async Task<IActionResult>Index()
        {
            return View(await _flowerService.GetInfos());
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertHeader expertHeader)
        {
           
                var existData = await _contex.ExpertHeaders.FirstOrDefaultAsync(m => m.Title.Trim().ToLower() == expertHeader.Title.Trim().ToLower());
                if (existData is not null)
                {
                    ModelState.AddModelError("Name", "This Data already exist");
                    return View();
                }



                await _contex.ExpertHeaders.AddAsync(expertHeader);
                await _contex.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            ExpertHeader expert = await _contex.ExpertHeaders.FindAsync(id);

            if (expert == null) return NotFound();

            _contex.ExpertHeaders.Remove(expert);    
            await _contex.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            ExpertHeader expert = await _contex.ExpertHeaders.FindAsync(id);

            if (expert is null) return NotFound();

            return View(expert);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertHeader expert)
        {
            if (id is null) return BadRequest();



            ExpertHeader dbExpert = await _contex.ExpertHeaders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (expert is null) return NotFound();

            if (dbExpert.Title.Trim().ToLower() == expert.Title.Trim().ToLower())
            {
                return RedirectToAction(nameof(Index));
            }

            //dbCategory.Name = category.Name;
            _contex.ExpertHeaders.Update(expert);

            await _contex.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            ExpertHeader expert = await _contex.ExpertHeaders.FindAsync(id);

            if (expert is null) return NotFound();


            return View(expert);

        }

    }
}
