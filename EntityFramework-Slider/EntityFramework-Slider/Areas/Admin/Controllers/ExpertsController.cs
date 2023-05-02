using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ExpertsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFlowerService _flowerService;
        private readonly IWebHostEnvironment _env;

        public ExpertsController(AppDbContext context, IFlowerService flowerService, IWebHostEnvironment env)
        {
            _context = context;
            _flowerService = flowerService;
            _env = env;
        }


        [HttpGet]
        public async Task<IActionResult>  Index()
        {
            return View(await _flowerService.GetAll());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expert expert)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!expert.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size Must Be Max 500kb");
                }

                if (!expert.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                string fileName = Guid.NewGuid().ToString() + "_" + expert.Photo.FileName;
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await expert.Photo.CopyToAsync(stream);
                }

                expert.Image = fileName;
                await _context.Experts.AddAsync(expert);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Expert expert = await _context.Experts.FindAsync(id);

                if (expert == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", expert.Image);
                FileHelper.DeleteFile(path);


                _context.Experts.Remove(expert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Expert expert = await _context.Experts.FindAsync(id);
            if (expert is null) return NotFound();
            return View(expert);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Expert expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);
            if (expert is null) return NotFound();
            return View(expert);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Expert expert)
        {
            try
            {
                if (id == null) return BadRequest();
                Expert dbExpert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);
                if (expert is null) return NotFound();

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!expert.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File Type must be image");
                    return View(dbExpert);
                }


                if (expert.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size must be max 200kb");
                    return View(dbExpert);
                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbExpert.Image);

                FileHelper.DeleteFile(oldPath);


                string fileName = Guid.NewGuid().ToString() + "_" + expert.Photo.FileName;
                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(newPath, FileMode.Create))
                {
                    await expert.Photo.CopyToAsync(stream);
                }

                dbExpert.Image = fileName;


                dbExpert.Name = expert.Name;
                dbExpert.Profession = expert.Profession;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));



            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}
