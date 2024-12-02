using System.Text.RegularExpressions;
using System.Web;

namespace InfoTrack.Services;

public interface IWebScrapingService
{
    Task<List<int>> GetKeywordResults(string url, string keyword);
}

public class WebScrapingService : IWebScrapingService
{
    private readonly ILogger _logger;

    private const string SearchUrl = "https://www.google.co.uk/search?num=100";
    private const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.79 Safari/537.36";
    private const string SearchResultsStartTag = "<div id=\"search\">";
    private const string SearchResultsEndTag = "<div id=\"bottomads\">";

    public WebScrapingService(ILogger<WebScrapingService> logger)
    {
        _logger = logger;
    }

    public async Task<List<int>> GetKeywordResults(string url, string keyword)
    {
        _logger.LogInformation($"Getting search result positions for url: '{url}' and keyword: '{keyword}'");

        var searchString = await GetSearchResultsString(keyword);

        if (string.IsNullOrEmpty(searchString))
            return new List<int>();

        var results = GetAllLinks(searchString);
        var keywordResults = GetUrlPositions(results, url);

        return keywordResults;
    }

    private async Task<string> GetSearchResultsString(string keyword)
    {
        var searchResultsStart = -1;
        var searchResultsEnd = -1;
        var page  = string.Empty;

        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            var searchUrlWithQuery = GetSearchUrlWithQuery(keyword);
            page = await client.GetStringAsync(searchUrlWithQuery);

            searchResultsStart = page.IndexOf(SearchResultsStartTag);
            searchResultsEnd = page.IndexOf(SearchResultsEndTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search results");
        }

        if (string.IsNullOrEmpty(page) || searchResultsStart == -1 || searchResultsEnd == -1)
            return string.Empty;

        return page.Substring(searchResultsStart, searchResultsEnd - searchResultsStart);
    }

    private string GetSearchUrlWithQuery(string keyword)
    {
        var uri = new UriBuilder(SearchUrl);
        var query = HttpUtility.ParseQueryString(uri.Query);
        query["q"] = keyword;
        uri.Query = query.ToString();
        return uri.ToString();
    }

    private List<string> GetAllLinks(string resultsPage)
    {
        var links = new List<string>();
        var linkRegex = @"<a\b[^>]*>.*?<h3\b[^>]*>.*?<\/h3>.*?<\/a>";

        var matches = Regex.Matches(resultsPage, linkRegex).Cast<Match>().ToList();
        var results = matches.Select(x => x.Value).ToList();

        return results;
    }

    private List<int> GetUrlPositions(List<string> resultUrls, string searchUrl)
    {
        var positions = new List<int>();

        for (var i = 0; i < resultUrls.Count; i++)
        {
            var resultUrl = resultUrls[i];
            
            if (resultUrl.Contains(searchUrl))
            {
                positions.Add(i);
            }
        }

        return positions;
    }
}
