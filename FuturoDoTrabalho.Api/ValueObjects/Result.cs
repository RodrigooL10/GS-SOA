namespace FuturoDoTrabalho.Api.ValueObjects
{
    // ====================================================================================
    // VALUE OBJECT: Result<T> (Generic)
    // ====================================================================================
    // Represents the result of an operation that returns a value
    // Encapsulates success/failure state, returned value, and error message
    // Provides a type-safe way to handle operation outcomes without exceptions
    // ====================================================================================
    public class Result<T>
    {
        // ====================
        // PROPERTIES
        // ====================
        // Indicates whether the operation succeeded
        public bool IsSuccess { get; }

        // The returned value if operation succeeded, null/default if failed
        public T? Value { get; }

        // Error message if operation failed, null if succeeded
        public string? Error { get; }

        // ====================
        // CONSTRUCTOR (Private)
        // Prevents direct instantiation - use Success() or Failure() factory methods
        // ====================
        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        // ====================
        // FACTORY METHOD: Success
        // Creates a result representing successful operation with returned value
        // ====================
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        // ====================
        // FACTORY METHOD: Failure
        // Creates a result representing failed operation with error message
        // ====================
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error);
        }
    }

    // ====================================================================================
    // VALUE OBJECT: Result (Non-Generic)
    // ====================================================================================
    // Represents the result of an operation that doesn't return a value
    // Used for operations like Delete, Update, or other actions without return values
    // Encapsulates success/failure state and optional error message
    // ====================================================================================
    public class Result
    {
        // ====================
        // PROPERTIES
        // ====================
        // Indicates whether the operation succeeded
        public bool IsSuccess { get; }

        // Error message if operation failed, null if succeeded
        public string? Error { get; }

        // ====================
        // CONSTRUCTOR (Private)
        // Prevents direct instantiation - use Success() or Failure() factory methods
        // ====================
        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        // ====================
        // FACTORY METHOD: Success
        // Creates a result representing successful operation without return value
        // ====================
        public static Result Success()
        {
            return new Result(true, null);
        }

        // ====================
        // FACTORY METHOD: Failure
        // Creates a result representing failed operation with error message
        // ====================
        public static Result Failure(string error)
        {
            return new Result(false, error);
        }
    }
}
