using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Pablo.MicroAppStudio01.AuthServer;

[Dependency(ReplaceServices = true)]
public class BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MicroAppStudio01 Authentication Server";
}