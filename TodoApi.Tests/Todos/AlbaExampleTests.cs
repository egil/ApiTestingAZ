using System.Net;
using System.Text.Json;
using TodoApi.AblaExtensions;

namespace TodoApi.Todos;

public class AlbaExampleTests
{
    [Fact(Skip = "Demo only")]
    public async Task AbleExample()
    {
        // # Declarative Syntax
        // Write readable tests your whole team can understand.
        // 
        // # Authorization Stubbing
        // Stop fighting with your authorization system. Modify the shape of your user at the test level.
        // 
        // # Extensible
        // Supports custom reusable assertions and configurations.
        var host = await Alba.AlbaHost.For<global::Program>(
            new UseLocalTestDb(),
            new UseManualtTimeProvider());

        var text = await host.GetAsText("/todos");

        var json = await host.GetAsJson<JsonDocument>("/todos");

        await host.Scenario(s =>
        {
            s.Get.Url("/todos");

            // All assertions is evaluated in a scenario
            s.ContentIsEquivalentTo("[]");
            s.ContentTypeShouldBe("application/json");
        });
    }
}
