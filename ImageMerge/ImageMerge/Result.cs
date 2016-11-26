namespace ImageMerge
{
    public class Result<T, TError>
    {
        public Result(TError error)
        {
            Error = error;
            Success = false;
        }

        public Result(T data)
        {
            Data = data;
            Success = true;
        }

        public bool Success { get; }
        public bool Failure => !Success;
        public T Data { get; }
        public TError Error { get; }
    }
}