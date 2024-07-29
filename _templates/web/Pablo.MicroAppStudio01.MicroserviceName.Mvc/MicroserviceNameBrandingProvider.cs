using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Pablo.MicroAppStudio01.MicroserviceName;

[Dependency(ReplaceServices = true)]
public class MicroserviceNameBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MicroserviceName";
}
