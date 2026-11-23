namespace GymManagementSystem.BLL.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorMessage => Error;
    public string? Status { get; }

    protected Result(bool isSuccess, string? error = null, string? status = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Status = status;
    }

    public static Result Ok() => new(true);
    public static Result Fail(string error, string? status = null) => new(false, error, status);

    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
    public static Result<T> Fail<T>(string error, string? status = null) => Result<T>.Fail(error, status);
}

public class Result<T> : Result
{
    public T? Value { get; }

    internal Result(bool isSuccess, T? value, string? error = null, string? status = null)
        : base(isSuccess, error, status)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value);
    public new static Result<T> Fail(string error, string? status = null) => new(false, default, error, status);

    public static implicit operator Result<T>(T value) => Ok(value);
}
