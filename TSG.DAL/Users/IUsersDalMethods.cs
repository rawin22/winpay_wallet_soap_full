using System;
using System.Collections.Generic;
using TSG.Models.APIModels;
using TSG.Models.DTO;
using TSG.Models.DTO.UsersDataBlock;
using TSG.Models.ServiceModels;
using WinstantPay.Common.Interfaces;

namespace TSG.Dal.Users
{
    public interface IUsersDalMethods : IGetById<User, string>
    {
        IResult<List<User>> GetAllUsers();
        IResult<User> InsertOrUpdateInfo(UserInfo ui);

        IResult<UserAliasesDto> GetUserAliasesByUserId(Guid userId);
        IResult<UserAliasesDto> GetUserAliasesByUserName(string userName);
        IResult SaveUserAliases(UserAliasesDto userAliases);
        IResult<UserAliasesDto> ExistedUserByAliasName(string aliasName);
        IResult Update(UserDto user);
        IResult UpdateAdmin(string firstName, string lastName, string email, string userName);
    }
}