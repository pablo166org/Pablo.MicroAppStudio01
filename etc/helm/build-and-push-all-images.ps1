param ($Version="1.0.0")

$Version = Get-Date -Format "yyyyMMdd.HHmmss"

#construir imagenes
./build-image.ps1 -ProjectPath "../../services/administration/Pablo.MicroAppStudio01.AdministrationService/Pablo.MicroAppStudio01.AdministrationService.csproj" -ImageName microappstudio01/administration -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/administration -RemoteTagName ppelle/administration -Version $Version

./build-image.ps1 -ProjectPath "../../services/identity/Pablo.MicroAppStudio01.IdentityService/Pablo.MicroAppStudio01.IdentityService.csproj" -ImageName microappstudio01/identity -Version $Version
./build-image.ps1 -ProjectPath "../../services/saas/Pablo.MicroAppStudio01.SaasService/Pablo.MicroAppStudio01.SaasService.csproj" -ImageName microappstudio01/saas -Version $Version
./build-image.ps1 -ProjectPath "../../services/audit-logging/Pablo.MicroAppStudio01.AuditLoggingService/Pablo.MicroAppStudio01.AuditLoggingService.csproj" -ImageName microappstudio01/auditlogging -Version $Version
./build-image.ps1 -ProjectPath "../../services/file-management/Pablo.MicroAppStudio01.FileManagementService/Pablo.MicroAppStudio01.FileManagementService.csproj" -ImageName microappstudio01/filemanagement -Version $Version
./build-image.ps1 -ProjectPath "../../services/chat/Pablo.MicroAppStudio01.ChatService/Pablo.MicroAppStudio01.ChatService.csproj" -ImageName microappstudio01/chat -Version $Version
./build-image.ps1 -ProjectPath "../../gateways/web/Pablo.MicroAppStudio01.WebGateway/Pablo.MicroAppStudio01.WebGateway.csproj" -ImageName microappstudio01/webgateway -Version $Version
./build-image.ps1 -ProjectPath "../../apps/auth-server/Pablo.MicroAppStudio01.AuthServer/Pablo.MicroAppStudio01.AuthServer.csproj" -ImageName microappstudio01/authserver -Version $Version
./build-image.ps1 -ProjectPath "../../apps/blazor/Pablo.MicroAppStudio01.Blazor/Pablo.MicroAppStudio01.Blazor.csproj" -ImageName microappstudio01/blazor -Version $Version
./build-image.ps1 -ProjectPath "../../public/web/Pablo.MicroAppStudio01.Web.Public/Pablo.MicroAppStudio01.Web.Public.csproj" -ImageName microappstudio01/webpublic -Version $Version
./build-image.ps1 -ProjectPath "../../gateways/public/Pablo.MicroAppStudio01.PublicGateway/Pablo.MicroAppStudio01.PublicGateway.csproj" -ImageName microappstudio01/publicgateway -Version $Version


 #subir imagenes a docker hub

./push-image.ps1 -LocalTagName microappstudio01/identity -RemoteTagName ppelle/identity -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/saas -RemoteTagName ppelle/saas -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/auditlogging -RemoteTagName ppelle/auditlogging -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/filemanagement -RemoteTagName ppelle/filemanagement -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/chat -RemoteTagName ppelle/chat -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/webgateway -RemoteTagName ppelle/webgateway -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/authserver -RemoteTagName ppelle/authserver -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/blazor -RemoteTagName ppelle/blazor -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/webpublic -RemoteTagName ppelle/webpublic -Version $Version
./push-image.ps1 -LocalTagName microappstudio01/publicgateway -RemoteTagName ppelle/publicgateway -Version $Version