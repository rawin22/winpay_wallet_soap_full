using System.Collections.Generic;

namespace WinstantPay.Common.Interfaces
{
    /// <summary>
    /// Interface for CRUD operations
    /// </summary>
    /// <remarks>
    /// Created methods for Create, Update and Delete operations for DB objects
    /// </remarks>
    /// <typeparam name="T">Model Type</typeparam>
    /// <typeparam name="K">Type of key</typeparam>
    public interface ICrud<T, K> : IDelete<K>
    {
        IResult Update(T model);
        IResult<List<T>> GetAll();
    }
}