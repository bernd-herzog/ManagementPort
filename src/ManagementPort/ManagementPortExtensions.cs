using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

public static class ManagementPortExtensions
{
  public static async Task RunWithManagementPortAsync(this WebApplication webApplication, WebApplication managementPort)
  {
    await webApplication.StartAsync();
    await managementPort.StartAsync();

    await webApplication.WaitForShutdownAsync();
    await managementPort.WaitForShutdownAsync();
  }
}
