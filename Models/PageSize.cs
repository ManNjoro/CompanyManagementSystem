using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class PageSize
    {
        /// <summary>
        /// Get a list of page sizes
        /// </summary>
        /// <param name="selectedPageSize">Selected page size</param>
        /// <returns>List of SelectListItem for page sizes</returns>
        public List<SelectListItem> GetSize(int selectedPageSize = 10)
        {
            return GetPageSizes(selectedPageSize);
        }
        private List<SelectListItem> GetPageSizes(int selectedPageSize = 10)
        {
            var pagesSizes = new List<SelectListItem>();
            if (selectedPageSize == 5)
                pagesSizes.Add(new SelectListItem("5", "5", true));
            else
                pagesSizes.Add(new SelectListItem("5", "5"));

            for (int lp = 10; lp <= 100; lp += 10)
            {
                if (lp == selectedPageSize)
                {
                    pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString(), true));
                }
                else
                    pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString()));
            }
            return pagesSizes;
        }
    }
}
