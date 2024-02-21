using Microsoft.AspNetCore.Mvc;

namespace CompanyManagementSystem.Views.Shared.Components.SearchBar
{
    public class SearchBarViewComponent : ViewComponent
    {
        public SearchBarViewComponent() { }

        public IViewComponentResult Invoke(SPager SearchPager, bool BottomBar = false)
        {
            if (BottomBar)
            {
                return View("BottomBar", SearchPager);
            }
            return View("Default", SearchPager);
        }
    }
}
