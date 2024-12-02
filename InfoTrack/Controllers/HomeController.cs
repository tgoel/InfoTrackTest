using InfoTrack.Models;
using InfoTrack.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InfoTrack.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebScrapingService _webScrapingService;

    public HomeController(ILogger<HomeController> logger, IWebScrapingService webScrapingService)
    {
        _logger = logger;
        _webScrapingService = webScrapingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(new KeywordSearchResultModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(KeywordSearchResultModel model)
    {
        if (ModelState.IsValid)
        {
            var results = await _webScrapingService.GetKeywordResults(model.Url, model.Keyword);
            model.Result = results.Count > 0 ? string.Join(", ", results) : "No results";

            return View(model);
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
