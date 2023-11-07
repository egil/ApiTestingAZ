using System.Text.Json;
using TodoApi.Tests;

namespace Api.Todos;

[UsesVerify]
public class ApiContractTests : TodoApiTestBase
{
    public ApiContractTests(TodoApiFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Verify_OpenApi_spec()
    {        
        var result = await Host.GetAsJson<JsonDocument>("/swagger/v1/swagger.json");

        await Verify(result);
    }
}
