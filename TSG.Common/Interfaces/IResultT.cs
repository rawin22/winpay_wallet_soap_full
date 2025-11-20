namespace WinstantPay.Common.Interfaces
{
    public interface IResult<T> : IResult
    {
        T Obj { get; }
    }
}