using EntityFramework_Slider.Data;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {

        private readonly IFooterService _footerService;
        public FooterViewComponent(IFooterService footerService)
        {
            _footerService = footerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult(View( await _footerService.GetAll()));
        }
       
    }
}
