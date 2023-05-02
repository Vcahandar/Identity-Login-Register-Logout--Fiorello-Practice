using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
   
        public ProductController(IProductService productService, 
                                 AppDbContext context,
                                 ICategoryService categoryService,
                                  IWebHostEnvironment env)
        {
            _productService= productService;
            _context = context;
            _categoryService = categoryService;
            _env = env;
        }

        public async Task<IActionResult>  Index(int  page = 1,int take = 4)
        {
            List<Product> products = await _productService.GetPaginatedDatas(page,take);

            List<ProductListVM> mappedDatas = GetMappedDatas(products);

            int pageCount = await GetPageCountAsync(take);

            Paginate<ProductListVM> paginatedDatas = new(mappedDatas, page, pageCount);

            ViewBag.take = take;

            return View(paginatedDatas);
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            var productCount = await _productService.GetCountAsync();
            return (int)Math.Ceiling((decimal)productCount / take);
        }
        private List<ProductListVM> GetMappedDatas(List<Product> products)
        {
            List<ProductListVM> mappedDatas = new();

            foreach (var product in products)
            {
                ProductListVM productVM = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Count = product.Count,
                    CategoryName = product.Category.Name,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault().Image
                };

                mappedDatas.Add(productVM);
            }

            return mappedDatas;
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.categories = await GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            try
            {
                ViewBag.categories = await GetCategoriesAsync();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                foreach (var photo in model.Photos)
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


                List<ProductImage> productImages = new();

                foreach (var photo in model.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    ProductImage productImage = new()
                    {
                        Image = fileName,
                    };

                    productImages.Add(productImage);
                }

                productImages.FirstOrDefault().IsMain = true;
                
                decimal convertedPrice = decimal.Parse(model.Price);

                Product newProduct = new()
                {
                    Name = model.Name,
                    Price = convertedPrice,
                    Count = model.Count,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Images = productImages
                };

                await _context.ProductImages.AddRangeAsync(productImages);
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll();
            return new SelectList(categories, "Id", "Name");
        }


        [HttpGet]
        public async  Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _productService.GetFullDataById((int)id);

            if(product == null) return NotFound();

            ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);

            return View(product);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            Product product = await _productService.GetFullDataById((int)id);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }



        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null) return BadRequest();
            Product product = await _productService.GetFullDataById((int)id);

            if (product == null) return NotFound();

            ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);

            return View(product);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null) return BadRequest();

            Product product = await _productService.GetFullDataById((int)id);

            if (product == null) return NotFound();

            ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);
            ViewBag.category = await GetCategoriesAsync();

            ProductEditVM model = new ProductEditVM()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryID = product.CategoryId,
                productImages = product.Images,
            };
            
            return View(model);

            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,ProductEditVM model)
        {
            if(id == null) return BadRequest();

            Product dbProduct = await _productService.GetFullDataById((int)id);
            if (dbProduct == null) return NotFound();

            List<ProductImage> productImage = new();

            foreach (var item in dbProduct.Images)
            {
                ProductImage image = new()
                {
                    Image = item.Image,
                };

                productImage.Add(image);
            }

            ProductEditVM productEdit = new()
            {
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price,
                CategoryID = dbProduct.CategoryId,
                productImages = dbProduct.Images,
            };

            ViewBag.desc = Regex.Replace(productEdit.Description, "<.*?>", String.Empty);
            ViewBag.category = await GetCategoriesAsync();

            if (model.Photos is not null)
            {
               
           

                foreach (var photo in model.Photos)
                {
                    ProductImage productImg = new()
                    {
                        Image = photo.CreateFile(_env, "img")
                    };

                    dbProduct.Images.Add(productImg);
                }
                dbProduct.Images.FirstOrDefault().IsMain = true;
            }
            else
            {
                foreach (var item in dbProduct.Images)
                {
                    ProductImage newProductImage = new()
                    {
                        Image = item.Image
                    };
                }
            }

            dbProduct.Name = model.Name;
            dbProduct.Description = model.Description;
            dbProduct.Price = model.Price;
            dbProduct.CategoryId = model.CategoryID;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
    
}
