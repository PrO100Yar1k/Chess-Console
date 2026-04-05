using Chess_Console.Infrastructure.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldStoreValueAndSetIsSuccessToTrue()
    {
        var expectedValue = "Test Data";
        var result = Result<string>.Success(expectedValue);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedValue, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ShouldStoreMessageAndSetIsSuccessToFalse()
    {
        var expectedError = "Something went wrong";
        var result = Result<int>.Failure(expectedError);

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Error);
        Assert.Equal(default, result.Value);
    }

    [Fact]
    public void SuccessAction_ShouldExecuteWhenCalled()
    {
        bool wasExecuted = false;
        var result = Result<Action>.Success(() => wasExecuted = true);

        result.Value?.Invoke();

        Assert.True(result.IsSuccess);
        Assert.True(wasExecuted);
    }

    [Fact]
    public void Failure_WithNullMessage_ShouldStillBeFailure()
    {
        var result = Result<object>.Failure(null);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Error);
    }
}