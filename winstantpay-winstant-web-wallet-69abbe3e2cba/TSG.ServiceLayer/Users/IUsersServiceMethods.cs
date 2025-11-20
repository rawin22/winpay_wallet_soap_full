using System;
using System.Collections.Generic;
using TSG.Models.APIModels;
using TSG.Models.ServiceModels;
using TSG.Models.ServiceModels.UsersDataBlock;
using WinstantPay.Common.Interfaces;

namespace TSG.ServiceLayer.Users
{
    public interface IUsersServiceMethods : IGetById<Models.ServiceModels.User, string>
    {
        IResult<List<Models.ServiceModels.User>> GetAllUsers();
        IResult<User> InsertOrUpdateInfo(UserInfo ui);
        
        IResult SaveUserAliases(UserAliasesSo userAliases);
        IResult<UserAliasesSo> ExistedUserByAliasName(string aliasName);
        bool IfAliasExistInSystem(string aliasName, string userName);
        IResult<UserAliasesSo> GetUserAliasesByUserId(Guid userId);
        IResult<UserAliasesSo> GetUserAliasesByUserName(string userName);
        IResult Update(User user);
        IResult UpdateAdmin(string firstName, string lastName, string email, string userName);
    }
}