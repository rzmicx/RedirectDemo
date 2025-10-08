using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class FrontendRedirectModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Url { get; set; }

    public string TargetUrl => Url ?? "https://10.255.255.1"; 
    public void OnGet()
    {
       
    }
}
