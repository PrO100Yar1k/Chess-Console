namespace Chess_Console.Others
{
    internal class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T Value { get; private set; }
        public string Error { get; private set; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = null;
        }

        private Result(string error)
        {
            IsSuccess = false;
            Error = error;
            Value = default;
        }

        public static Result<T> Success(T value)
            => new Result<T>(value);

        public static Result<T> Failure(string error)
            => new Result<T>(error);
    }
}
