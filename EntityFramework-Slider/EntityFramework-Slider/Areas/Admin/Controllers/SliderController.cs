using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context, 
                                IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.ToListAsync();
            return View(sliders);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            if (slider is null) return NotFound();
            return View(slider);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM slider)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                foreach (var photo in slider.Photos)
                {

                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File Type must be image");
                        return View();
                    }

                    if (photo.CheckFileSize(500))
                    {
                        ModelState.AddModelError("Photo", "Image Size must be max 200kb");
                        return View();
                    }
                }



                foreach (var photo in slider.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    Slider newSlider = new()
                    {
                        Image = fileName
                    };

                    await _context.Sliders.AddAsync(newSlider);

                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));



            }
            catch (Exception)
            {

                throw;
            }
        }


        #region ---------TEK-TEK ADD ELEMEK FILE(PHOTO)-------


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Slider slider)
        //{

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return View();
        //        }

        //        if (!slider.Photo.CheckFileType("image/"))
        //        {
        //            ModelState.AddModelError("Photo", "File Type must be image");
        //            return View();
        //        }

        //        if(slider.Photo.CheckFileSize(500))
        //        {
        //            ModelState.AddModelError("Photo", "Image Size must be max 200kb");
        //            return View();
        //        }

        //        string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;
        //        string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

        //        using (FileStream stream = new FileStream(path, FileMode.Create))
        //        {
        //            await slider.Photo.CopyToAsync(stream);
        //        };

        //        await _context.Sliders.AddAsync(slider);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));



        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }



        //}

        #endregion



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

                if (slider is null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", slider.Image);

                FileHelper.DeleteFile(path);

                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            if (slider is null) return NotFound();
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Slider slider)
        {
            try
            {

                if (id == null) return BadRequest();
                Slider dbSlider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
                if (slider is null) return NotFound();

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!slider.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File Type must be image");
                    return View(dbSlider);
                }


                if (slider.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size must be max 200kb");
                    return View(dbSlider);
                }


                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSlider.Image);

                FileHelper.DeleteFile(oldPath);


                string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;
                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(newPath, FileMode.Create))
                {
                    await slider.Photo.CopyToAsync(stream);
                }

                dbSlider.Image = fileName;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int? id)
        {
            if(id == null) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            if (slider is null) return NotFound();

            slider.SoftDelete = !slider.SoftDelete;

            //if (slider.SoftDelete)
            //{
            //    slider.SoftDelete = false;
            //}
            //else
            //{
            //    slider.SoftDelete = true;
            //}
            await _context.SaveChangesAsync();

            return Ok(slider.SoftDelete);
        }




    }
}
