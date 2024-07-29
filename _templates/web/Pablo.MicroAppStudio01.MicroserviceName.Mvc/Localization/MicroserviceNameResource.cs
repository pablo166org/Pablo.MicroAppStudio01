using Localization.Resources.AbpUi;
using Volo.Abp.Localization;
using Volo.Abp.Validation.Localization;

namespace Pablo.MicroAppStudio01.MicroserviceName.Localization;

[LocalizationResourceName("MicroserviceName")]
[InheritResource(
    typeof(AbpValidationResource),
    typeof(AbpUiResource)
    )]
public class MicroserviceNameResource
{
    
}