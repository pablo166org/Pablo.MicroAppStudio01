# MicroAppStudio01

This is a startup template to create microservice based solutions.

## Before Running the Solution

### Generate Signing-Certificate for AuthServer 

Navigate to `/apps/auth-server/Pablo.MicroAppStudio01.AuthServer` folder and run:

```bash
dotnet dev-certs https -v -ep ./authserver.pfx -p b5178901-a5df-43e1-b812-de05eba15fab
```

to generate pfx file for signing tokens by AuthServer.

> This should be done by every developer.

### Install Client-Side Libraries

Run the following command in this folder:

````bash
abp install-libs
````

### Running on a Kubernetes Cluster Environment

To run the application(s) on your Kubernetes cluster environment, follow these steps:

- Navigate to the [/etc/helm](./etc/helm) directory within your terminal or command prompt.
- Execute the [create-tls-secrets.ps1](./etc/helm/create-tls-secrets.ps1) PowerShell command.
- Open the Kubernetes menu in ABP Studio, then within the Helm tab:
  - Build Docker Images: In the `Charts` tree context menu, click on the `Build Docker Image(s)` option. This will start the process of building your Docker images.
  - Install Charts: After the Docker images have been built, you can install your Helm charts. To do this, go to `Charts->Commands` in the context menu and click on `Install Chart(s)`.

Also, make sure to review the [pre-requirements](./etc/helm/README.md#Pre-requirements) before proceeding.

> This should be done by every developer.

### Additional resources

You can see the following resources to learn more about your solution and the ABP Framework:

* [LeptonX Theme Module](https://docs.abp.io/en/commercial/latest/themes/lepton-x/index)
* [LeptonX Blazor UI](https://docs.abp.io/en/commercial/latest/themes/lepton-x/blazor?UI=Blazor)
