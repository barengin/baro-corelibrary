using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Core
{
    public enum ErrorCodes : int
    {
        OK = 0,
        DialogResultOK = 0,
        InvalidInstance = -1,
        NotFoundInTolerance = -2,
        TabFileNotFound = -3,
        CantCreateXlsx = -4,
        NoMapCatalogExists = -5,
        TableNotRegistered = -6,
        CollectionNotFound = -7,
        CantCreateTab = -8,
        TableIsEmpty = -9,
        DbPrinxNotFound = -10,
        CantCreateKml = -11,
        CantOpenEmailConfigFile = -12,
        CantOpenAttachment = -13,
        MailNotRegistered = -14,
        CantOpenConfigFile = -15,
        CantParseConfigFile = -16,
        CantOpenTabFile = -17,
        DialogResultCancel = -18
    }
}
