using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieCatalogApi.Entities;
using MovieCatalogApi.Services;
using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Web.Pages
{
    public class TitleModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IMovieCatalogDataService _movieCatalogDataService;

        [BindProperty]
        
        [Required, StringLength(500, ErrorMessage = "Title can't be more than 500 characters")]
        public string? PrimaryTitle { get; set; }

        [BindProperty]
        [ Required,StringLength(500, ErrorMessage = "Title can't be more than 500 characters")]
        public string? OriginalTitle { get; set; }

        [BindProperty]
        public TitleType TitleType { get; set; }

        [BindProperty]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int? StartYear { get; set; }

        [BindProperty]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
        public int? EndYear { get; set; }

        [BindProperty]
        [Range(1, 9999, ErrorMessage = "Runtime must be between 1 and 9999 minutes.")]
        public int? RuntimeMinutes { get; set; }


        
        [MaxLength(3,ErrorMessage = "Select maximum 3 genres"), BindProperty, Required]
        public List<int>? Genres { get; set; } = new List<int>();

        [BindProperty(SupportsGet =true)]
        public int? Id { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }

        

        public TitleModel(ILogger<IndexModel> logger, IMovieCatalogDataService movieCatalogDataService)
        {
            _logger = logger;
            _movieCatalogDataService = movieCatalogDataService;


        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                
                return Page();
            }

            if (Id.HasValue)
            {             
                await _movieCatalogDataService.InsertOrUpdateTitleAsync(
                
                     Id,
                    PrimaryTitle,
                    OriginalTitle,
                    TitleType,
                    StartYear,
                    EndYear,
                   RuntimeMinutes,
                    Genres.ToArray()
                );
                SuccessMessage = "Title updated successfully!";
            }
            else
            {
                var title = await _movieCatalogDataService.InsertOrUpdateTitleAsync(
                    null, PrimaryTitle, OriginalTitle,
                    TitleType, StartYear, EndYear, RuntimeMinutes, Genres.ToArray());
                    
                SuccessMessage = "New title created successfully!";
                Id = title.Id;
            }

            return RedirectToPage("/Title", new { Id });

        }


        public async Task OnGet(int? id)
        {
            
            if (id != null)
            {

                var title = await _movieCatalogDataService.GetTitleByIdAsync(id.Value);
                if (title != null)
                {
                    PrimaryTitle = title.PrimaryTitle;
                    OriginalTitle = title.OriginalTitle;
                    TitleType = title.TitleType;
                    StartYear = title.StartYear;
                    EndYear = title.EndYear;
                    RuntimeMinutes = title.RuntimeMinutes;
                    Genres = title.TitleGenres.Select(x => x.GenreId).ToList();
                }
                

            }
            
        }
        public async Task<IReadOnlyCollection<SelectListItem>> GetGenreOptionsAsync()
        {
            var genres = await _movieCatalogDataService.GetGenresAsync();

            var selectedGenres = Genres;

            var selectList = genres.Select(genre => new SelectListItem
            {
                Value = genre.Id.ToString(),
                Text = genre.Name,
                Selected = selectedGenres.Contains(genre.Id)


            }).ToList();
            
            
            return selectList.AsReadOnly();

        }
    }
}
