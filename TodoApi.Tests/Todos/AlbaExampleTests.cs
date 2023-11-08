namespace TodoApi.Tests.Todos;

public class AlbaExampleTests
{
    [Fact(Skip = "Example only")]
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
        var host = await AlbaHost.For<Program>(
            new LocalTestDatabaseAlbaExtension(),
            new TimeProviderAlbaExtension());

        var text = await host.GetAsText("/todos");

        var json = await host.GetAsJson<JsonDocument>("/todos");

        // declarative scenarios
        await host.Scenario(s =>
        {
            s.Get.Url("/todos");

            // All assertions is evaluated in a scenario
            s.ContentTypeShouldBe("application/json");
            s.ContentShouldBeJson("[]");
        });
    }
}
