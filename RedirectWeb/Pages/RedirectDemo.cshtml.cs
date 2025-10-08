using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RedirectDemoModel : PageModel
{
    [BindProperty]
    public string? TargetUrl { get; set; }

    public IActionResult OnPostBackend()
    {
        if (string.IsNullOrWhiteSpace(TargetUrl))
        {
            // balik ke halaman dengan pesan validasi
            ModelState.AddModelError(nameof(TargetUrl), "Masukkan URL terlebih dahulu!");
            return Page();
        }

        // nanti di sini akan dipisahkan ke page backend
        // sementara arahkan langsung untuk demo sederhana
        return RedirectToPage("/BackendRedirect", new { url = TargetUrl });
    }

    public IActionResult OnPostFrontend()
    {
        if (string.IsNullOrWhiteSpace(TargetUrl))
        {
            ModelState.AddModelError(nameof(TargetUrl), "Masukkan URL terlebih dahulu!");
            return Page();
        }

        // arahkan ke page frontend
        return RedirectToPage("/FrontendRedirect", new { url = TargetUrl });
    }
}
