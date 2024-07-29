using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Pablo.MicroAppStudio01.Web.Public.Pages;

public class IndexModel : MicroAppStudio01PublicPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
