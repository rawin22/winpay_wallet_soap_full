namespace WinstantPay.Common.Interfaces
{
    /// <summary>
    /// Interface for DELETE operations
    /// </summary>
    /// <remarks>
    /// Delete methods for DB objects
    /// </remarks>
    /// <typeparam name="T">Type of key</typeparam>
    public interface IDelete<T>
    {
        /// <summary>
        /// Method for delete
        /// </summary>
        /// <param name="id">Id value in table</param>
        /// <returns>IResult type contains bool prop IsSuccess, string prop Message, and custom integer prop Code [for error detalization]</returns>
        IResult Delete(T id);
    }
}