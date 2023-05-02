using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBlogService _blog;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context,
                              IBlogService blog,
                              IWebHostEnvironment env)
        {
            _context = context;
            _blog = blog;
            _env = env;
        }

        public async Task<IActionResult>  Index()
        {
            return View(await _blog.GetAll());
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!blog.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File Type must be image");
                    return View();
                }

                if (blog.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size must be max 200kb");
                    return View();
                }

                string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await blog.Photo.CopyToAsync(stream);
                }

                blog.Image = fileName;
                await _context.Blogs.AddAsync(blog);
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

                Blog blog = await _context.Blogs.FindAsync(id);

                if (blog == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", blog.Image);
                FileHelper.DeleteFile(path);


                _context.Blogs.Remove(blog);
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
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog is null) return NotFound();
            return View(blog);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (int? id,Blog blog)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!blog.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File Type must be image");
                    return View();
                }


                if (!blog.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Image Size must be max 200kb");
                    return View();
                }

                if (id == null) return BadRequest();
                Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
                if (blog is null) return NotFound();
                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbBlog.Image);

                FileHelper.DeleteFile(oldPath);


                string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;
                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(newPath, FileMode.Create))
                {
                    await blog.Photo.CopyToAsync(stream);
                }

                dbBlog.Image = fileName;


                dbBlog.Header = blog.Header;
                dbBlog.Description = blog.Description;
                dbBlog.Date = blog.Date;

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
            if(id == null) return BadRequest();
            Blog blog = await _context.Blogs.FindAsync(id);
            if (blog is null) return NotFound();
            return View(blog);
        }




    }
}
