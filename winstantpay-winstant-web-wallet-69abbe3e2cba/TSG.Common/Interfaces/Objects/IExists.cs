namespace WinstantPay.Common.Interfaces
{
    /// <summary>
    /// Интерфейс проверки на существование объекта в БД
    /// </summary>
    /// <typeparam name="K">Тип Id записи объекта</typeparam>
    public interface IExists<K>
    {
        bool Exists(K id);
    }
}