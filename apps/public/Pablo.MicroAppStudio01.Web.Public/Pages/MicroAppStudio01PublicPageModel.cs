using Pablo.MicroAppStudio01.Web.Public.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Pablo.MicroAppStudio01.Web.Public.Pages;

/* Inherit your Page Model classes from this class.
 */
public abstract class MicroAppStudio01PublicPageModel : AbpPageModel
{
    protected MicroAppStudio01PublicPageModel()
    {
        LocalizationResourceType = typeof(MicroAppStudio01WebPublicResource);
    }
}
