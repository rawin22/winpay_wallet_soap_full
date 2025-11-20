namespace WinstantPay.Common.Interfaces
{
    public interface IInsertByT <T>
    {
        IResult<T> Insert(T model);
    }
}