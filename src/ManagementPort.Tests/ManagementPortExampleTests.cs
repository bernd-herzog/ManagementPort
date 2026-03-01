using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ManagementPort.Tests;

public class ManagementPortExampleTests
{
  [Test]
  public async Task ManagementPortTest()
  {
    var client = new WebApplicationFactory<Program>().CreateClient();
    client.DefaultRequestHeaders.Add("X-API-KEY", "test");

    var defaultPage = await client.GetAsync("/weatherforecast");
    Assert.That(defaultPage.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

    await SetApiKeyViaManagementPort("test");

    defaultPage = await client.GetAsync("/weatherforecast");
    Assert.That(defaultPage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
  }

  private async Task SetApiKeyViaManagementPort(string apiKey)
  {
    var processInfo = new ProcessStartInfo
    {
      FileName = "bash",
      Arguments = $"-c \"curl -s --http2-prior-knowledge --unix-socket ~/management_port.sock http://localhost/management --data '\\\"{apiKey}\\\"' -H 'Content-Type: application/json'\"",
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    using var process = Process.Start(processInfo);

    await process!.WaitForExitAsync();

    string output = await process.StandardOutput.ReadToEndAsync();
    string error = await process.StandardError.ReadToEndAsync();

    Assert.That(process.ExitCode, Is.EqualTo(0), $"StdErr: {error} StdOut: {output}");
    Assert.That(error, Is.EqualTo(""));
    Assert.That(output, Is.EqualTo("OK"));
  }
}
