using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// Contains values that specify the error codes of the connection.  
    /// </summary> 
    public enum RasError
    {
        Success = 0,
        Pending = 600,                  // RASBASE
        InvalidPortHandle,              //(RASBASE+1)
        PortAlreadyOpen,                //(RASBASE+2)
        BufferTooSmall,                 //(RASBASE+3)
        WrongInfoSpecified,             //(RASBASE+4)
        CannotSetPortInfo,              //(RASBASE+5)
        PortNotConnected,               //(RASBASE+6)
        EventInvalid,                   //(RASBASE+7)
        DeviceDoesNotExist,             //(RASBASE+8)
        DeviceTypeDoesNotExist,         //(RASBASE+9)
        BufferInvalid,                  //(RASBASE+10)
        RouteNotAvailable,              //(RASBASE+11)
        RouteNotAllocated,              //(RASBASE+12)
        InvalidCompressionSpecified,    //(RASBASE+13)
        OutOfBuffers,                   //(RASBASE+14)
        PortNotFound,                   //(RASBASE+15)
        AsyncRequestPending,            //(RASBASE+16)
        AlreadyDisconnecting,           //(RASBASE+17)
        PortNotOpen,                    //(RASBASE+18)
        PortDisconnected,               //(RASBASE+19)
        NoEndpoints,                    //(RASBASE+20)
        CannotOpenPhonebook,            //(RASBASE+21)
        CannotLoadPhonebook,            //(RASBASE+22)
        CannotFindPhonebookEntry,       //(RASBASE+23)
        CannotWritePhonebook,           //(RASBASE+24)
        CorruptPhonebook,               //(RASBASE+25)
        CannotLoadString,               //(RASBASE+26)
        KeyNotFound,                    //(RASBASE+27)
        Disconnection,                  //(RASBASE+28)
        RemoteDisconnection,            //(RASBASE+29)
        HardwareFailure,                //(RASBASE+30)
        UserDisconnection,              //(RASBASE+31)
        InvalidSize,                    //(RASBASE+32)
        PortNotAvailable,               //(RASBASE+33)
        CannotProjectClient,            //(RASBASE+34)
        Unknown,                        //(RASBASE+35)
        WrongDeviceAttached,            //(RASBASE+36)
        BadString,                      //(RASBASE+37)
        RequestTimeout,                 //(RASBASE+38)
        CannotGetLANA,                  //(RASBASE+39)
        NETBIOSError,                   //(RASBASE+40)
        ServerOutOfResources,           //(RASBASE+41)
        NameExistsOnNetwork,            //(RASBASE+42)
        ServerGeneralNetworkError,      //(RASBASE+43)
        MessageAliasNotAdded,           //(RASBASE+44)
        AuthInternal,                   //(RASBASE+45)
        RestrictedLogonHours,           //(RASBASE+46)
        AccountDisabled,                //(RASBASE+47)
        PasswordExpired,                //(RASBASE+48)
        NoDialinPermission,             //(RASBASE+49)
        ServerNotResponding,            //(RASBASE+50)
        FromDevice,                     //(RASBASE+51)
        UnrecognizedResponse,           //(RASBASE+52)
        macroNotFound,                  //(RASBASE+53)
        MacroNotDefined,                //(RASBASE+54)
        MessageMacroNotFound,           //(RASBASE+55)
        DefautOffMacroNotFound,     //(RASBASE+56)
        FileCouldNotBeOpened,       //(RASBASE+57)
        DeviceNameTooLong,            //(RASBASE+58)
        DeviceNameNotFound,           //(RASBASE+59)
        NoResponses,                   //(RASBASE+60)
        NoCommandFound,               //(RASBASE+61)
        wrongKeySpecified,            //(RASBASE+62)
        UnknownDeviceType,            //(RASBASE+63)
        AllocatingMemory,              //(RASBASE+64)
        PortNotConfigured,            //(RASBASE+65)
        DeviceNotReady,               //(RASBASE+66)
        ReadingIniFile,               //(RASBASE+67)
        NoConnection,                  //(RASBASE+68)
        BadUsageInIniFile,          //(RASBASE+69)
        ReadingSectionName,            //(RASBASE+70)
        ReadingDeviceType,             //(RASBASE+71)
        ReadingDeviceName,             //(RASBASE+72)
        ReadingUsage,                  //(RASBASE+73)
        ReadingMaxConnectBps,          //(RASBASE+74)
        ReadingMaxCarrierBps,          //(RASBASE+75)
        LineBusy,                      //(RASBASE+76)
        VoiceAnswer,                   //(RASBASE+77)
        NoAnswer,                      //(RASBASE+78)
        NoCarrier,                     //(RASBASE+79)
        NoDialtone,                    //(RASBASE+80)
        InCommand,                     //(RASBASE+81)
        WritingSectionName,            //(RASBASE+82)
        WritingDeviceType,             //(RASBASE+83)
        WritingDeviceName,             //(RASBASE+84)
        WritingMaxConnectBps,          //(RASBASE+85)
        WritingMaxCarrierBps,          //(RASBASE+86)
        WritingUsage,                  //(RASBASE+87)
        WritingDefaultOff,             //(RASBASE+88)
        ReadingDefaultOff,             //(RASBASE+89)
        EmptyIniFile,                 //(RASBASE+90)
        AuthenticationFailure,         //(RASBASE+91)
        PortOrDevice,                 //(RASBASE+92)
        NotBinaryMacro,               //(RASBASE+93)
        DCBNotFound,                  //(RASBASE+94)
        StateMachinesNotStarted,     //(RASBASE+95)
        StateMachinesAlreadyStarted, //(RASBASE+96)
        PartialResponseLooping,       //(RASBASE+97)
        UnknownResponseKey,           //(RASBASE+98)
        ReceiveBufferFull,                  //(RASBASE+99)
        CommandTooLong,                   //(RASBASE+100)
        UnsupportedBps,                //(RASBASE+101)
        UnexpectedResponse,            //(RASBASE+102)
        InteractiveMode,               //(RASBASE+103)
        BadCallbackNumber,            //(RASBASE+104)
        InvalidAuthState,             //(RASBASE+105)
        WritingInitBps,                //(RASBASE+106)
        X25Diagnostic,                 //(RASBASE+107)
        AccountExpired,                   //(RASBASE+108)
        ChangingPassword,              //(RASBASE+109)
        Overrun,                        //(RASBASE+110)
        RASMANCannotInitialize,	     //(RASBASE+111)
        BiplexPortNotAvailable,      //(RASBASE+112)
        NoActiveISDNLines,           //(RASBASE+113)
        NoISDNChannelsAvailable,     //(RASBASE+114)
        TooManyLineErrors,           //(RASBASE+115)
        IPConfiguration,               //(RASBASE+116)
        NoIPAddresses,                //(RASBASE+117)
        PPP_Timeout,                    //(RASBASE+118)
        PPP_RemoteTerminated,          //(RASBASE+119)
        PPP_NoProtocolsConfigured,    //(RASBASE+120)
        PPP_NoResponse,                //(RASBASE+121)
        PPP_InvalidPacket,             //(RASBASE+122)
        PhoneNumberTooLong,          //(RASBASE+123)
        IPXCP_NoDialoutConfigured,    //(RASBASE+124)
        IPXCP_NoDialinConfigured,     //(RASBASE+125)
        IPXCP_DialoutAlreadyActive,   //(RASBASE+126)
        AccessingTCPCFGDLL,            //(RASBASE+127)
        RASAdapterNoIP,              //(RASBASE+128)
        SLIP_RequiresIP,               //(RASBASE+129)
        ProjectionNotComplete,        //(RASBASE+130)
        ProtocolNotConfigured,        //(RASBASE+131)
        PPP_NotConverging,             //(RASBASE+132)
        PPP_CPRejected,                //(RASBASE+133)
        PPP_LCPTerminated,             //(RASBASE+134)
        PPP_RequiredAddressRejected,  //(RASBASE+135)
        PPP_NCPTerminated,             //(RASBASE+136)
        PPP_LoopbackDetected,          //(RASBASE+137)
        PPP_NoAddressAssigned,        //(RASBASE+138)
        CannotUseLogonCredentials,   //(RASBASE+139)
        TAPIConfiguration,             //(RASBASE+140)
        NoLocalEncryption,            //(RASBASE+141)
        NoRemoteEncryption,           //(RASBASE+142)
        RemoteRequiresEncryption,     //(RASBASE+143)
        IPXCP_NetNumberConflict,      //(RASBASE+144)
        SMMInvalid,                    //(RASBASE+145)
        SMMUninitialized,              //(RASBASE+146)
        NoMACForPort,                //(RASBASE+147)
        SMMTimeout,                    //(RASBASE+148)
        BadPhoneNumber,               //(RASBASE+149)
        WrongModule,                   //(RASBASE+150)
        PPP_MAC,						 //(RASBASE+151)
        PPP_LCP,						 //(RASBASE+152)
        PPP_AUTH,						 //(RASBASE+153)
        PPP_NCP,						 //(RASBASE+154)
        PowerOff,						 //(RASBASE+155)
        PowerOffCD,					 //(RASBASE+156)
        DialAlreadyInProgress,       //(RASBASE+157)
        RASAutoCannotInitialize      //(RASBASE+158)
    }
}