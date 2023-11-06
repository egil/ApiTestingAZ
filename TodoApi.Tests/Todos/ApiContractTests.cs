using System.Text.Json;

namespace Api.Todos;

[UsesVerify]
public class ApiContractTests
{
    [Fact]
    public async Task Verify_OpenApi_spec()
    {
        await using var host = await AlbaHost.For<Program>();
        
        var result = await host.GetAsJson<JsonDocument>("/swagger/v1/swagger.json");

        await Verify(result);
    }
}
