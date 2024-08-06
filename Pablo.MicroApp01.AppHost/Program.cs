var builder = DistributedApplication.CreateBuilder(args);

//services
builder.AddProject<Projects.Pablo_MicroAppStudio01_AdministrationService>("pablo-microappstudio01-administrationservice", launchProfileName: "Pablo.MicroAppStudio01.AdministrationService");
builder.AddProject<Projects.Pablo_MicroAppStudio01_IdentityService>("pablo-microappstudio01-identityservice", launchProfileName: "Pablo.MicroAppStudio01.IdentityService");
builder.AddProject<Projects.Pablo_MicroAppStudio01_SaasService>("pablo-microappstudio01-saasservice", launchProfileName: "Pablo.MicroAppStudio01.SaasService");
builder.AddProject<Projects.Pablo_MicroAppStudio01_AuthServer>("pablo-microappstudio01-authserver", launchProfileName: "Pablo.MicroAppStudio01.AuthServer");
builder.AddProject<Projects.Pablo_MicroAppStudio01_AuditLoggingService>("pablo-microappstudio01-auditloggingservice", launchProfileName: "Pablo.MicroAppStudio01.AuditLoggingService");
builder.AddProject<Projects.Pablo_MicroAppStudio01_ChatService>("pablo-microappstudio01-chatservice", launchProfileName: "Pablo.MicroAppStudio01.ChatService");
builder.AddProject<Projects.Pablo_MicroAppStudio01_FileManagementService>("pablo-microappstudio01-filemanagementservice", launchProfileName: "Pablo.MicroAppStudio01.FileManagementService");

//gateways
builder.AddProject<Projects.Pablo_MicroAppStudio01_PublicGateway>("pablo-microappstudio01-publicgateway", launchProfileName: "Pablo.MicroAppStudio01.WebGateway");
builder.AddProject<Projects.Pablo_MicroAppStudio01_WebGateway>("pablo-microappstudio01-webgateway", launchProfileName: "Pablo.MicroAppStudio01.WebGateway");


//apps
builder.AddProject<Projects.Pablo_MicroAppStudio01_Web_Public>("pablo-microappstudio01-web-public", launchProfileName: "Pablo.MicroAppStudio01.Web.Public.Host");
builder.AddProject<Projects.Pablo_MicroAppStudio01_Blazor>("pablo-microappstudio01-blazor", launchProfileName: "Pablo.MicroAppStudio01.Blazor");

builder.Build().Run();
