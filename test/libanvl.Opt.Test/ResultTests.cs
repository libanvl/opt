using Xunit;

namespace libanvl.opt.test;

public class ResultTests
{
    [Fact]
    public void Ok_Result_Should_Be_Success()
    {
        var result = new Result<int, string>(42);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Try_Should_Return_Ok_If_Function_Succeeds()
    {
        var result = Result.Try(() => 42);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Try_Should_Return_Err_If_Function_Throws()
    {
        var result = Result.Try<int>(() => throw new InvalidOperationException("error"));
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public void Try_With_Arg_Should_Return_Ok_If_Function_Succeeds()
    {
        var result = Result.Try(42, arg => arg.ToString());
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal("42", result.Unwrap());
    }

    [Fact]
    public void Try_With_Arg_Should_Return_Err_If_Function_Throws()
    {
        var result = Result.Try<int, string>(42, arg => throw new InvalidOperationException("error"));
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public void Validate_Should_Return_Ok_If_All_Validators_Pass()
    {
        var result = Result.Validate<int, string>(42, Validator);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal(42, result.Unwrap());

        static bool Validator(in int value, out string error)
        {
            error = default!;
            return true;
        }
    }

    [Fact]
    public void Validate_Should_Return_Err_If_Any_Validator_Fails()
    {
        var result = Result.Validate<int, string>(42, ValidatorOK, ValidatorError);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Equal("error", result.Error.Unwrap()[0]);

        static bool ValidatorOK(in int value, out string error)
        {
            error = default!;
            return true;
        }

        static bool ValidatorError(in int value, out string error)
        {
            error = "error";
            return false;
        }
    }

    [Fact]
    public void OkOr_Should_Return_Value_If_Success()
    {
        var result = new Result<int, string>(42);
        Assert.Equal(42, result.OkOr(0));
    }

    [Fact]
    public void OkOr_Should_Return_Default_If_Error()
    {
        var result = new Result<int, string>("error");
        Assert.Equal(0, result.OkOr(0));
    }

    [Fact]
    public void Match_Should_Invoke_Ok_Action_If_Success()
    {
        var result = new Result<int, string>(42);
        bool okCalled = false;
        bool errCalled = false;
        result.Match(
            ok => okCalled = true,
            err => errCalled = true
        );
        Assert.True(okCalled);
        Assert.False(errCalled);
    }

    [Fact]
    public void Match_Should_Invoke_Err_Action_If_Error()
    {
        var result = new Result<int, string>("error");
        bool okCalled = false;
        bool errCalled = false;
        result.Match(
            ok => okCalled = true,
            err => errCalled = true
        );
        Assert.False(okCalled);
        Assert.True(errCalled);
    }

    [Fact]
    public void Match_Function_Should_Return_Ok_Result_If_Success()
    {
        var result = new Result<int, string>(42);
        var matchResult = result.Match(
            ok => ok.ToString(),
            err => err
        );
        Assert.Equal("42", matchResult);
    }

    [Fact]
    public void Match_Function_Should_Return_Err_Result_If_Error()
    {
        var result = new Result<int, string>("error");
        var matchResult = result.Match(
            ok => ok.ToString(),
            err => err
        );
        Assert.Equal("error", matchResult);
    }
}
