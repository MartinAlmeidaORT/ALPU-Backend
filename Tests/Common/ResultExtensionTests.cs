using FluentAssertions;
using FluentResults;
using GraphQL.Common;
using HotChocolate;

namespace Tests.Common;

public class ResultExtensionsTests
{
    [Fact]
    public void UnwrapOrThrow_WhenSuccess_ReturnsValue()
    {
        var result = Result.Ok("value");
        var unwrapped = result.UnwrapOrThrow();
        unwrapped.Should().Be("value");
    }

    [Fact]
    public void UnwrapOrThrow_WhenFailed_ThrowsGraphQLException()
    {
        Result<string> result = Result.Fail("something went wrong");
        var act = () => result.UnwrapOrThrow();
        act.Should().Throw<GraphQLException>();
    }
}
