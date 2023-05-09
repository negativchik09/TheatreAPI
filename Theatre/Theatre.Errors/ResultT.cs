﻿namespace Theatre.Errors;

public class Result<TValue> : Result
{
    private readonly TValue? _value;
    protected internal Result(TValue? value, bool isSuccess, Error error) 
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess && _value != null
        ? _value
        : throw new InvalidOperationException();

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}