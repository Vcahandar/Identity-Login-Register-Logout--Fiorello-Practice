using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public HomeController(AppDbContext context,
               IBasketService basketService,
               IProductService productService,
               ICategoryService categoryService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
 
            List<Slider> sliders = await _context.Sliders.Where(m=>!m.SoftDelete).ToListAsync();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            IEnumerable<Category> categories = await _categoryService.GetAll();

            IEnumerable<Product> products = await _productService.GetAll();

            About abouts = await _context.Abouts.Include(m=>m.Advantages).FirstOrDefaultAsync();

            IEnumerable<Expert> experts = await _context.Experts.Where(m => !m.SoftDelete).ToListAsync();

            Subscribe subscribes = await _context.Subscribes.FirstOrDefaultAsync();

            IEnumerable<Say> says = await _context.Says.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Instagram> instagrams = await _context.Instagrams.Where(m => m.SoftDelete == false).ToListAsync();

            HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,
                Categories = categories,
                Products = products,
                Abouts = abouts,
                Experts = experts,
                Subscribes = subscribes, 
                Says = says,
                Instagrams = instagrams
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null) return BadRequest();

            Product? dbProduct = await _productService.GetById((int)id);

            if (dbProduct == null) return NotFound();

            List<BasketVM> basket = _basketService.GetBasketDatas();

            BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbProduct.Id);

            _basketService.AddProductToBasket(existProduct, dbProduct, basket);

            int basketCount = basket.Sum(m=>m.Count);

            return Ok(basketCount);
        }


        



        //public IActionResult Test()
        //{
        //    var sessionData = HttpContext.Session.GetString("name");
        //    var cookieData = Request.Cookies["surname"];
        //    var objectData = JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);


        //    return Json(objectData);
        //}

        //class Book
        //{
        //    public int Id { get; set; }
        //    public string Name { get; set; }
        //}
    }
}