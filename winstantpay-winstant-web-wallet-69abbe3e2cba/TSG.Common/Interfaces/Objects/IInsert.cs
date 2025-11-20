namespace WinstantPay.Common.Interfaces
{
    public interface IInsert <T>
    {
        IResult Insert(T model);
    }
}