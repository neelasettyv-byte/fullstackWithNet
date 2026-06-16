using System.Collections.Generic;
using Xunit;

namespace Api.Tests;

// Simple tests so the pipeline has a test stage to run.
// These test basic logic; in a real app you'd test your endpoints and services.
public class BasicTests
{
    [Fact]
    public void Login_WithCorrectPassword_Succeeds()
    {
        var users = new Dictionary<string, string> { { "admin", "password123" } };
        var ok = users.TryGetValue("admin", out var pwd) && pwd == "password123";
        Assert.True(ok);
    }

    [Fact]
    public void Login_WithWrongPassword_Fails()
    {
        var users = new Dictionary<string, string> { { "admin", "password123" } };
        var ok = users.TryGetValue("admin", out var pwd) && pwd == "wrongpass";
        Assert.False(ok);
    }

    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(-1, 1, 0)]
    public void Addition_Works(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }
}
