using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Collections.Generic;

namespace EntityFramework_Slider.Controllers
{
    public class BasketController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;

        public BasketController(AppDbContext context,
                                IBasketService basketService,
                                IProductService productService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketVM> basketProducts = _basketService.GetBasketDatas();
            List<BasketDetailVM> basketDetails = new();

            
            


            foreach (var product in basketProducts)
            {
                var dbProduct = await _productService.GetFullDataById(product.Id);
                basketDetails.Add(new BasketDetailVM
                {
                    Id = dbProduct.Id,
                    Name = dbProduct.Name,
                    CategoryName = dbProduct.Category.Name,
                    Description = dbProduct.Description,
                    Price = dbProduct.Price,
                    Image = dbProduct.Images.Where(m=>m.IsMain).FirstOrDefault().Image,
                    Count = product.Count,
                    Total = dbProduct.Price * product.Count
                });
            }

            return View(basketDetails);






            //List<BasketVM> basket;

            //if (Request.Cookies["basket"] != null)
            //{
            //    basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            //}
            //else
            //{
            //    basket = new List<BasketVM>();
            //}

            //foreach (var baskets in basket)
            //{
            //    Product dbproduct = _context.Products.Include(m => m.Images).FirstOrDefault(m => m.Id == baskets.Id);
            //    baskets.Product = dbproduct;
            //}


            //return View(basket);


            
        }


        [ActionName("Delete")]
        public IActionResult DeleteProductFromBasket(int? id)
        {
            if (id is null) return BadRequest();
            _basketService.DeleteProductFromBasket((int)id);
            return RedirectToAction("Index");
        }
    }
}
