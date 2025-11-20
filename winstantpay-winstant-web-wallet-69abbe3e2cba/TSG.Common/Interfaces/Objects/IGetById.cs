namespace WinstantPay.Common.Interfaces
{
    /// <summary>
    /// Interface get element by Id in Db table
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <typeparam name="K">Type of key</typeparam>
    public interface IGetById<T, K>
    {
        /// <summary>
        /// Method for geting element by Id in Db table
        /// </summary>
        /// <param name="id">Id number</param>
        /// <returns>Single object by model</returns>
        IResult<T> GetById(K id);
    }
}