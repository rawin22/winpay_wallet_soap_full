namespace WinstantPay.Common.Interfaces
{
    /// <summary>
    /// Интерфейс проверки объекта на исспользование в других объектах
    /// </summary>
    /// <typeparam name="K">Тип Id записи</typeparam>
    public interface IUsed<K>
    {
        /// <summary>
        /// Метод проверки исспользуется ли объект в БД или нет
        /// </summary>
        /// <param name="id">Тип Id записи</param>
        /// <returns></returns>
        bool IsUsed(K id);
    }
}