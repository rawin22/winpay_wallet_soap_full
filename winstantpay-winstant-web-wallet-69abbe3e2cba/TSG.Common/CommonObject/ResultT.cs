namespace WinstantPay.Common.Object
{
    public class Result<T> : Result, Interfaces.IResult<T>
    {
        public Result() : base()
        {
        }

        public Result(T obj) : base()
        {
            Obj = obj;
        }
        public Result(T obj, string message)
            : base(message)
        {
            Obj = obj;
        }
        public Result(T obj, string message, int code)
            : base(message, code)
        {
            Obj = obj;
        }
        public Result(T obj, bool success, string message)
            : base(success, message, 0)
        {
            Obj = obj;
        }
        public Result(T obj, bool success, string message, int code)
            : base(success, message, code)
        {
            Obj = obj;
        }

        public Result(string message)
            : base(message)
        { }

        public T Obj { get; }
    }
}