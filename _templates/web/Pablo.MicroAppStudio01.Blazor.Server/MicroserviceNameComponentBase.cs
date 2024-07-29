using Pablo.MicroAppStudio01.MicroserviceName.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Pablo.MicroAppStudio01.MicroserviceName;

public abstract class MicroserviceNameComponentBase : AbpComponentBase
{
    protected MicroserviceNameComponentBase()
    {
        LocalizationResource = typeof(MicroserviceNameResource);
    }
}
