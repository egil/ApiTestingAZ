using Xunit.Abstractions;

namespace TodoApi.Tests.Todos.OpenApiSpecTests;

public class ApiContractTests : TodoApiTestBase
{
    public ApiContractTests(
        ITestOutputHelper testOutputHelper, 
        TodoApiFixture fixture) : base(fixture, testOutputHelper)
    {
    }

    [Fact]
    public async Task Verify_OpenApi_spec()
    {
        // Act - download the OpenApi specification
        var result = await Host.GetAsJson<JsonDocument>(
            "/swagger/v1/swagger.json");

        // Assert - use (semantic) snapshot testing to verify 
        // that the OpenApi spec matches the verified spec
        await Verify(result);
    }
}
