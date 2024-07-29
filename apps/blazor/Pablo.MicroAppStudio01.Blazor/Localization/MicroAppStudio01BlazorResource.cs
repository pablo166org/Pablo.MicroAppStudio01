using Localization.Resources.AbpUi;
using Volo.Abp.Localization;
using Volo.Abp.Validation.Localization;

namespace Pablo.MicroAppStudio01.Blazor.Localization;

[LocalizationResourceName("MicroAppStudio01Blazor")]
[InheritResource(
    typeof(AbpValidationResource),
    typeof(AbpUiResource)
    )]
public class MicroAppStudio01BlazorResource
{
    
}