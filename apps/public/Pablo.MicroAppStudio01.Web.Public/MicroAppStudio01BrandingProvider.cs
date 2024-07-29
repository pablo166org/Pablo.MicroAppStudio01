using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Pablo.MicroAppStudio01.Web.Public;

[Dependency(ReplaceServices = true)]
public class MicroAppStudio01BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MicroAppStudio01";
}
