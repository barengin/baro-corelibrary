using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.AAA
{
    public enum AErrorCode
    {
        OK = 0,
        UserNotFound = -1,
        AlreadyExists = -2,
        KeyNotFound = -3,
        TokenExpired = -4
    }

    public interface IAAAServer
    {
        UserData[] UserQuery(string username, string password, string queryString);
        UserData[] UserQuery(Token userToken, string queryString);
        
        AErrorCode AddUserData(string username, string password, UserData[] kvp);
        AErrorCode RemoveUserData(string username, string password, string[] key);

        AErrorCode AddUserData(Token userToken, UserData[] kvp);
        AErrorCode RemoveUserData(Token userToken, string[] key);

        AErrorCode AddUser(string username, string password, UserData[] kvp);
        AErrorCode DeleteUser(string username, string password);

        AErrorCode CreateToken(string username, string password, out Token userToken);

        void DeleteAllData();
    }
}
