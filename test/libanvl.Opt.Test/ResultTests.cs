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
    public void OkOr_Should_Return_Value_From_Function_If_Success()
    {
        var result = new Result<int, string>(42);
        Assert.Equal(42, result.OkOr(() => 0));
    }

    [Fact]
    public void OkOr_Should_Return_Value_From_Function_If_Error()
    {
        var result = new Result<int, string>("error");
        Assert.Equal(0, result.OkOr(() => 0));
    }

    [Fact]
    public void OkOr_Should_Return_Value_From_Function_With_Error_If_Error()
    {
        var result = new Result<int, string>("error");
        Assert.Equal(0, result.OkOr(err => 0));
    }

    [Fact]
    public void OkOr_Should_Return_Value_From_Function_With_Error_If_Success()
    {
        var result = new Result<int, string>(42);
        Assert.Equal(42, result.OkOr(err => 0));
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
    [Fact]
    public void Err_Result_Should_Be_Error()
    {
        var result = new Result<int, string>("error");
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Equal("error", result.Error.Unwrap());
    }

    [Fact]
    public void From_Should_Create_Result_From_Opt()
    {
        var value = Opt.Some(42);
        var error = Opt<string>.None;
        var result = Result.From(value, error);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void From_Should_Create_Result_From_Opt_Error()
    {
        var value = Opt<int>.None;
        var error = Opt.Some("error");
        var result = Result.From(value, error);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Equal("error", result.Error.Unwrap());
    }

    [Fact]
    public void OkOrDefault_Should_Return_Value_If_Success()
    {
        var result = new Result<int, string>(42);
        Assert.Equal(42, result.OkOrDefault());
    }

    [Fact]
    public void OkOrDefault_Should_Return_Default_If_Error()
    {
        var result = new Result<int, string>("error");
        Assert.Equal(default(int), result.OkOrDefault());
    }

    [Fact]
    public void Implicit_Conversion_To_Result_Should_Create_Success()
    {
        Result<int, string> result = 42;
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Implicit_Conversion_To_Result_Should_Create_Error()
    {
        Result<int, string> result = "error";
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Equal("error", result.Error.Unwrap());
    }

    [Fact]
    public void Implicit_Conversion_To_Value_Should_Return_Value_If_Success()
    {
        var result = new Result<int, string>(42);
        int value = result;
        Assert.Equal(42, value);
    }

    [Fact]
    public void Implicit_Conversion_To_Value_Should_Return_Default_If_Error()
    {
        var result = new Result<int, string>("error");
        int value = result;
        Assert.Equal(default(int), value);
    }

    [Fact]
    public void Implicit_Conversion_To_Error_Should_Return_Error_If_Error()
    {
        var result = new Result<int, string>("error");
        string error = result!;
        Assert.Equal("error", error);
    }

    [Fact]
    public void Implicit_Conversion_To_Error_Should_Return_Default_If_Success()
    {
        var result = new Result<int, string>(42);
        string error = result!;
        Assert.Equal(default(string), error);
    }

    [Fact]
    public unsafe void Validate_Delegate_Should_Return_Ok_If_All_Validators_Pass()
    {
        delegate*<int, out string, bool> validator = &Validator;
        var result = Result.Validate<int, string>(42, validator);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal(42, result.Unwrap());

        static bool Validator(int value, out string error)
        {
            error = default!;
            return true;
        }
    }

    [Fact]
    public unsafe void Validate_Delegate_Should_Return_Err_If_Any_Validator_Fails()
    {
        delegate*<int, out string, bool> validatorOK = &ValidatorOK;
        delegate*<int, out string, bool> validatorError = &ValidatorError;
        var result = Result.Validate<int, string>(42, validatorOK, validatorError);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Equal("error", result.Error.Unwrap()[0]);

        static bool ValidatorOK(int value, out string error)
        {
            error = default!;
            return true;
        }

        static bool ValidatorError(int value, out string error)
        {
            error = "error";
            return false;
        }
    }

    [Fact]
    public unsafe void Try_With_Arg_Delegate_Should_Return_Err_If_Function_Throws()
    {
        delegate*<int, string> func = &ThrowExceptionWithArg;
        var result = Result.Try(42, func);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public unsafe void Try_With_Two_Args_Delegate_Should_Return_Ok_If_Function_Succeeds()
    {
        delegate*<int, int, string> func = &TwoIntsToString;
        var result = Result.Try(42, 24, func);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal("42, 24", result.Unwrap());
    }

    [Fact]
    public unsafe void Try_With_Two_Args_Delegate_Should_Return_Err_If_Function_Throws()
    {
        delegate*<int, int, string> func = &ThrowExceptionWithTwoArgs;
        var result = Result.Try(42, 24, func);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public unsafe void Try_With_Three_Args_Delegate_Should_Return_Ok_If_Function_Succeeds()
    {
        delegate*<int, int, int, string> func = &ThreeIntsToString;
        var result = Result.Try(42, 24, 12, func);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal("42, 24, 12", result.Unwrap());
    }

    [Fact]
    public unsafe void Try_With_Three_Args_Delegate_Should_Return_Err_If_Function_Throws()
    {
        delegate*<int, int, int, string> func = &ThrowExceptionWithThreeArgs;
        var result = Result.Try(42, 24, 12, func);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public unsafe void Try_With_Four_Args_Delegate_Should_Return_Ok_If_Function_Succeeds()
    {
        delegate*<int, int, int, int, string> func = &FourIntsToString;
        var result = Result.Try(42, 24, 12, 6, func);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal("42, 24, 12, 6", result.Unwrap());
    }

    [Fact]
    public unsafe void Try_With_Four_Args_Delegate_Should_Return_Err_If_Function_Throws()
    {
        delegate*<int, int, int, int, string> func = &ThrowExceptionWithFourArgs;
        var result = Result.Try(42, 24, 12, 6, func);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    private static string ThrowExceptionWithArg(int value) => throw new InvalidOperationException("error");

    private static string TwoIntsToString(int value1, int value2) => $"{value1}, {value2}";

    private static string ThrowExceptionWithTwoArgs(int value1, int value2) => throw new InvalidOperationException("error");

    private static string ThreeIntsToString(int value1, int value2, int value3) => $"{value1}, {value2}, {value3}";

    private static string ThrowExceptionWithThreeArgs(int value1, int value2, int value3) => throw new InvalidOperationException("error");

    private static string FourIntsToString(int value1, int value2, int value3, int value4) => $"{value1}, {value2}, {value3}, {value4}";

    private static string ThrowExceptionWithFourArgs(int value1, int value2, int value3, int value4) => throw new InvalidOperationException("error");

    [Fact]
    public unsafe void Try_Delegate_Should_Return_Err_If_Function_Throws()
    {
        delegate*<int> func = &ThrowException;
        var result = Result.Try(func);
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    private static int ThrowException() => throw new InvalidOperationException("error");
}
