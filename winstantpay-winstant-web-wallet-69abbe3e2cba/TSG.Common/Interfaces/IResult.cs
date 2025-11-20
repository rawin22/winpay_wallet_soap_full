namespace WinstantPay.Common.Interfaces
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
        int Code { get; }
    }

    
}