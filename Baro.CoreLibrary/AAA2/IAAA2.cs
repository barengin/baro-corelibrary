using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.AAA2
{
    public enum AAA2ErrorCode
    {
        OK = 0,
        UserNotFound = -1,
        AlreadyExists = -2,
        KeyNotFound = -3,
        PermissionRequired = -4
    }

    public interface IAAA2
    {
        AAA2ErrorCode UserQuery(AAA2Credential userCredential, string queryString, out UserData2[] data);
        AAA2ErrorCode UserQueryByKey(AAA2Credential loginCredential, string keyOfUser, string valueOfKey, string queryStringOfKey, out UserData2[] data);

        AAA2ErrorCode AddUserData(AAA2Credential userCredential, UserData2[] data);
        AAA2ErrorCode RemoveUserData(AAA2Credential userCredential, string[] keys);

        AAA2ErrorCode AddUser(AAA2Credential loginCredential, AAA2Credential userToAdd, UserData2[] data);
        AAA2ErrorCode DeleteUser(AAA2Credential loginCredential, AAA2Credential userToDelete);

        AAA2ErrorCode RemoveAllUsers(AAA2Credential loginCredential);

        AAA2ErrorCode CheckUser(AAA2Credential userCredential);

        AAA2ErrorCode GetAllUsers(AAA2Credential userCredential, out User2[] users);
    }
}
