using Localization.Resources.AbpUi;
using Volo.Abp.Localization;
using Volo.Abp.Validation.Localization;

namespace Pablo.MicroAppStudio01.Web.Public.Localization;

[LocalizationResourceName("MicroAppStudio01WebPublic")]
[InheritResource(
    typeof(AbpValidationResource),
    typeof(AbpUiResource)
    )]
public class MicroAppStudio01WebPublicResource
{
    
}