namespace WinstantPay.Common.Object
{
    public class Result : Interfaces.IResult
    {
        public bool Success { get; }
        public string Message { get; }
        public int Code { get; }

        public Result()
        {
            Success = string.IsNullOrWhiteSpace(Message) || string.IsNullOrEmpty(Message);
        }

        public Result(string message)
        {
            Message = message;
            Success = string.IsNullOrWhiteSpace(Message) || string.IsNullOrEmpty(Message);
        }

        public Result(string message, int code)
        {
            Message = message;
            Code = code;
            Success = string.IsNullOrWhiteSpace(Message) || string.IsNullOrEmpty(Message);
        }
        
        public Result(bool success, string message, int code)
        {
            Success = success;
            Message = message;
            Code = code;
        }

    }
}
