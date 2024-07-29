using Volo.Abp.Bundling;

namespace Pablo.MicroAppStudio01.Blazor;

public class MicroAppStudio01BundleContributor : IBundleContributor
{
    public void AddScripts(BundleContext context)
    {
    }

    public void AddStyles(BundleContext context)
    {
        context.Add("main.css");
    }
}
