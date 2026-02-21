using BuildingBlocks.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace BuildingBlocks.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    [JsonConstructor]
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TResult> Success<TResult>(TResult value) => new(true, value, Error.None);

    public static Result<TResult> Failure<TResult>(Error error) => new(false, default, error);

    public static Result<TResult> Create<TResult>(TResult? value) => value is not null ? Success(value) : Failure<TResult>(Error.NullValue);

}

public class Result<TResult> : Result
{

    private readonly TResult? _value;
    [JsonConstructor]
    protected internal Result(bool isSuccess, TResult? value, Error error) : base(isSuccess, error)
    {
        _value = value;
    }
    [NotNull]
    public TResult Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of failure result can not be accessed");
    public static implicit operator Result<TResult>(TResult value) => Create(value);

}
