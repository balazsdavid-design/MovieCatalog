using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalog.Web.Utils;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;

namespace MovieCatalog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IMovieCatalogDataService _movieCatalogDataService;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } 

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } 

        [BindProperty(SupportsGet = true)]
        public TitleSort TitleSort { get; set; } 

        [BindProperty(SupportsGet = true)] 
        public bool SortDescending { get; set; }

        [BindProperty(SupportsGet = true)]
        public TitleFilter Filter { get; set; }

        public IReadOnlyList<SelectListItem> PageSizeOptions { get; set; } = new[]
       {
            new SelectListItem("10", "10"),
            new SelectListItem("20", "20", true),  // alapértelmezett kiválasztott elem
            new SelectListItem("30", "30"),
            new SelectListItem("60", "60"),
            new SelectListItem("120", "120"),
        };

        public IReadOnlyList<SelectListItem> TitleSortOptions { get; set; } = new[]
        {
            new SelectListItem("Release Year", TitleSort.ReleaseYear.ToString()),
            new SelectListItem("Primary Title", TitleSort.PrimaryTitle.ToString()),
            new SelectListItem("Original Title", TitleSort.Runtime.ToString()),
        };

        public IReadOnlyList<SelectListItem> SortDirectionOptions { get; set; } = new[]
        {
            new SelectListItem("Ascending", "false"),
            new SelectListItem("Descending", "true", true) // alapértelmezett kiválasztott elem
        };

        



        public Dictionary<Genre, int> Genres { get; set; }

        public PagedResult<Title> Titles { get; set; }

        public IReadOnlyList<int> PageNumberOptions =>
    new[]{
            1, 2, 3, PageNumber - 1, PageNumber,    PageNumber + 1, Titles.LastPageNumber -    1,
            Titles.LastPageNumber, Titles.  LastPageNumber + 1
        }
        .Where(i => i > 0 && i <= Titles.LastPageNumber + 1)
        .Distinct()
        .OrderBy(i => i)
        .ToArray();

        public IndexModel(ILogger<IndexModel> logger,IMovieCatalogDataService movieCatalogDataService)
        {
            _logger = logger;
            _movieCatalogDataService = movieCatalogDataService;
            Filter = TitleFilter.Empty;

            
        }

        public async Task<ActionResult> OnGetAsync()
        {
            var genres = await _movieCatalogDataService.GetGenresWithTitleCountsAsync();

            if(!Request.Query.Any())
            {
                return RedirectToPage(new { PageSize = 20, PageNumber = 1, TitleSort = TitleSort.ReleaseYear, SortDescending = true });

            }

            

            Genres = genres.OrderBy(g => g.Key.Name).ToDictionary(g=> g.Key,g=> g.Value);

            



            var titles = _movieCatalogDataService.GetTitlesAsync(PageSize, PageNumber, Filter, TitleSort, SortDescending)
                .Result;
            Titles = titles;

            return Page();

        }
    }
}