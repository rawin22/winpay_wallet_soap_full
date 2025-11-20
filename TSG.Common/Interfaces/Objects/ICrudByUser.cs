using System;
using System.Collections.Generic;

namespace WinstantPay.Common.Interfaces
{
    public interface ICrudByUser<T>
    {
        IResult<List<T>> GetByUser(Guid userId);
    }
}