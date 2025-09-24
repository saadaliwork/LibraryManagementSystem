using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}