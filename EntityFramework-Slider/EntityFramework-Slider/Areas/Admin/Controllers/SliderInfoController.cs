using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SliderInfoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
       

        public SliderInfoController(AppDbContext context,
                                    IWebHostEnvironment env)
                                    
        {
            _context = context;
            _env = env;
   
        }


        public async Task<IActionResult>  Index()
        {
            IEnumerable<SliderInfo> sliderInfos = await _context.SliderInfos.ToListAsync();
            return View(sliderInfos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderInfo sliderInfo)
        {
            try
            {
                if (!ModelState.IsValid) return View();
                if (!sliderInfo.SignaturePhoto.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size Must Be Max 500kb");
                }

                if (!sliderInfo.SignaturePhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }


                //sliderInfo.SignatureImage = sliderInfo.SignaturePhoto.CreateFile(_env, "img");

                //await _context.AddAsync(sliderInfo);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

                //Hazir method da cixardib yazmaqda olur sadece elim cox koda oyressin
                //deye uzun kod yazdim.



                string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.SignaturePhoto.FileName;
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await sliderInfo.SignaturePhoto.CopyToAsync(stream);
                }

                sliderInfo.SignatureImage = fileName;
                await _context.SliderInfos.AddAsync(sliderInfo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));



            }
            catch (Exception)
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

                SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (sliderInfo == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", sliderInfo.SignatureImage);
                FileHelper.DeleteFile(path);


                _context.SliderInfos.Remove(sliderInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));




            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult>  Edit(int? id)
        {
            if (id is null) return BadRequest();
            SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (dbSliderInfo is null) return NotFound();
            SliderInfoVM sliderInfo = new()
            {
                SignatureImage = dbSliderInfo.SignatureImage,
                Title = dbSliderInfo.Title,
                Description = dbSliderInfo.Description
            };
            return View(sliderInfo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,SliderInfoVM infoVM)
        {
            try
            {
                if (id == null) return BadRequest();
                SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m=>m.Id == id);
                if(dbSliderInfo is null) return NotFound();
                SliderInfoVM sliderInfoVM = new()
                {
                    SignatureImage = dbSliderInfo.SignatureImage,
                    Title = dbSliderInfo.Title,
                    Description = dbSliderInfo.Description
                };

                if (!ModelState.IsValid)
                {
                    return View();
                }


                if (!infoVM.SignaturePhoto.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File Type must be image");
                    return View();
                }


                if (infoVM.SignaturePhoto.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size must be max 200kb");
                    return View();
                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderInfo.SignatureImage);
                FileHelper.DeleteFile(oldPath);


                string fileName = Guid.NewGuid().ToString() + "_" + infoVM.SignaturePhoto.FileName;
                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(newPath, FileMode.Create))
                {
                    await infoVM.SignaturePhoto.CopyToAsync(stream);
                }


                infoVM.SignatureImage = fileName;

                dbSliderInfo.Title = infoVM.Title;
                dbSliderInfo.Description = infoVM.Description;
                

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex )
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();
            var dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (dbSliderInfo is null) return NotFound();
            return View(dbSliderInfo);
        }   

    }
}
