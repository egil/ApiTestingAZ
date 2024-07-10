# Testing .NET Web APIs from A to Z

This is the demo solution used in the talk "Testing .NET Web APIs from A to Z".

> You built an API in .NET, put it in production, customers are using it, and now you have promises to keep. Promises related to the API contract and behavior. A good test suite will help you keep those promises by protecting against regressions. It will allow you to quickly iterate, add new features and expand the API if it is resistant to refactoring and is giving fast feedback.
> 
> This session is about how to build such a test suite. We will discuss and showcase how to verify the API contract, the API’s behavior, and cross-cutting concerns like authentication and authorization. We will cover the pros/cons of various approaches regarding test runtime, coverage, and usage of test doubles vs. end-to-end testing using real data stores, among other things.
> 
> -- https://sessionize.com/s/egil/testing-.net-web-apis-from-a-to-z/68735

There is a recording available of this talk from Øredev 2023: https://youtu.be/AA6zaQ1gKv8?si=3b92RZsDKByEwzI4

## The demo solution showcases the following:

1. Using [Alba](https://jasperfx.github.io/alba/), a wrapper around `WebApplicationFactory`, to configure and run the API under test.

    1. How to create custom assertions that integrates nicely with Albas existing assertions. The custom assertions leverage semantic comparison of JSON returned from the API endpoints, resulting in more maintainable tests.

    2. How to create custom reusable extensions which customizes the API. In particular, it includes a `LocalTestDatabaseAlbaExtension` and `TimeProviderAlbaExtension` extensions.

2. Using [Verify](https://github.com/VerifyTests/Verify) for snapshot testing of content returned the API under test.

    1. How to customize Verify to perform capture and comparison differently. In particular, Verify is customzied to perform semantic comparison of JSON instead of string based comparison for more maintainable tests.

    2. How to create custom file converter that takes Alba's `IScenarioResult` type and generate a JSON document that can be verified using snapshot testing.

3. How to create xUnit test collections that span multiple test classes and custom test fixtures that are shared by the collection (`ApiTestCollection`, `TodoApiFixture`).

4. How to create a base class that encapsulates pre- and post-test run operations (`TodoApiTestBase`).

5. How to capture `ILogger` statements into test output via the `CaptureLoggingAlbaExtension` extension.

## Requirements

To run the demo solution locally, you will need the following:

- .NET 8 SDK
- SQL Server LocalDB instance
