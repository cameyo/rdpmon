using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Cameyo
{
    public class WTS
    {
        #region Constants
        public const Int32 WTS_CURRENT_SESSION = -1;
        static public IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        [Flags]
        public enum WaitSystemEventFlags : UInt32
        {
            /* ===================================================================== 
             == EVENT - Event flags for WTSWaitSystemEvent
             ===================================================================== */
            None = 0x00000000, // return no event
            CreatedWinstation = 0x00000001, // new WinStation created
            DeletedWinstation = 0x00000002, // existing WinStation deleted
            RenamedWinstation = 0x00000004, // existing WinStation renamed
            ConnectedWinstation = 0x00000008, // WinStation connect to client
            DisconnectedWinstation = 0x00000010, // WinStation logged on without client           
            LogonUser = 0x00000020, // user logged on to existing WinStation
            LogoffUser = 0x00000040, // user logged off from existing WinStation
            WinstationStateChange = 0x00000080, // WinStation state change
            LicenseChange = 0x00000100, // license state change
            AllEvents = 0x7fffffff, // wait for all event types
            // Unfortunately cannot express this as an unsigned long...
            //FlushEvent = 0x80000000 // unblock all waiters
        }
        #endregion

        static private void Log(string msg)
        {
            RdpMon.Utils.Log(msg);
        }

        #region Dll Imports
        [DllImport("wtsapi32.dll")]
        static extern int WTSEnumerateSessions(
            IntPtr pServer,
            [MarshalAs(UnmanagedType.U4)] int iReserved,
            [MarshalAs(UnmanagedType.U4)] int iVersion,
            ref IntPtr pSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref int iCount);

        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(
            System.IntPtr pServer,
            Int32 iSessionID,
            WTS_INFO_CLASS oInfoClass,
            out System.IntPtr pBuffer,
            out uint iBytesReturned);

        [DllImport("wtsapi32.dll")]
        static extern void WTSFreeMemory(
            IntPtr pMemory);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static public extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static public extern bool WTSLogoffSession(IntPtr hServer, int sessionId, bool bWait);

        // WTSVirtualChannel*
        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        static public extern IntPtr WTSVirtualChannelOpen(IntPtr hServer,
            Int32 dwSessionID, [MarshalAs(UnmanagedType.LPStr)] string virtualName);

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSVirtualChannelWrite(IntPtr channelHandle,
           byte[] buffer, int length, ref int bytesWritten);

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSVirtualChannelRead(IntPtr сhannelHandle, uint timeout,
            byte[] buffer, uint length, ref uint bytesReaded);

        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSVirtualChannelClose(IntPtr channelHandle);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSSendMessage(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.I4)] int SessionId,
            String pTitle,
            [MarshalAs(UnmanagedType.U4)] int TitleLength,
            String pMessage,
            [MarshalAs(UnmanagedType.U4)] int MessageLength,
            [MarshalAs(UnmanagedType.U4)] int Style,
            [MarshalAs(UnmanagedType.U4)] int Timeout,
            [MarshalAs(UnmanagedType.U4)] out int pResponse,
            bool bWait);

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSWaitSystemEvent(
            IntPtr hServer,
            UInt32 EventMask,
            out UInt32 EventFlags);
        #endregion

        #region Structures
        //Structure for Terminal Service Client IP Address
        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_CLIENT_ADDRESS
        {
            public int iAddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] bAddress;
        }
        
        public enum AddressFamilyType
        {
            AF_INET = 2,
            AF_INET6 = 10, 
            AF_IPX = 4, 
        }

        //Structure for Terminal Service Session Info
        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_SESSION_INFO
        {
            public Int32 iSessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public string sWinsWorkstationName;
            public WTS_CONNECTSTATE_CLASS oState;
        }
        public class SessionInfo
        {
            public Int32 ID;
            public WTS_CONNECTSTATE_CLASS State;
            public string UserName;
            public DateTime LogonTime;
            public DateTime DisconnectTime;
            public DateTime LastInputTime;
            //public string IP;
            public struct DisplayInfo
            {
                public int Width, Height, ColorDepth;
            }
            public DisplayInfo Display;

            public string StateStr()
            {
                if (State == WTS_CONNECTSTATE_CLASS.WTSActive)
                    return "Active";
                else if (State == WTS_CONNECTSTATE_CLASS.WTSDisconnected)
                    return "Disconnected (" + Cameyo.RdpMon.Utils.DurationStr(DateTime.UtcNow.Subtract(DisconnectTime)) + ")";
                else if (State == WTS_CONNECTSTATE_CLASS.WTSDown)
                    return "Down (" + Cameyo.RdpMon.Utils.DurationStr(DateTime.UtcNow.Subtract(DisconnectTime)) + ")";
                else
                {
                    var str = State.ToString();
                    if (str.StartsWith("WTS"))
                        str = str.Remove(0, 3);
                    return str;
                }
            }

            long _sessionUID = 0;
            public long SessionUID()
            {
                if (_sessionUID == 0)
                    _sessionUID = Cameyo.RdpMon.Session.GetSessionUid(this.ID, this.LogonTime);
                return _sessionUID;
            }
        }

        //Structure for Terminal Service Session Client Display
        [StructLayout(LayoutKind.Sequential)]
        private struct WTS_CLIENT_DISPLAY
        {
            public int iHorizontalResolution;
            public int iVerticalResolution;
            //1 = The display uses 4 bits per pixel for a maximum of 16 colors.
            //2 = The display uses 8 bits per pixel for a maximum of 256 colors.
            //4 = The display uses 16 bits per pixel for a maximum of 2^16 colors.
            //8 = The display uses 3-byte RGB values for a maximum of 2^24 colors.
            //16 = The display uses 15 bits per pixel for a maximum of 2^15 colors.
            public int iColorDepth;
        }

        public const int WINSTATIONNAME_LENGTH = 32;
        public const int DOMAIN_LENGTH = 17;
        public const int USERNAME_LENGTH = 20;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct WTSINFO
        {
            public WTS_CONNECTSTATE_CLASS State;
            public UInt32 SessionId;
            public UInt32 IncomingBytes;
            public UInt32 OutgoingBytes;
            public UInt32 IncomingFrames;
            public UInt32 OutgoingFrames;
            public UInt32 IncomingCompressedBytes;
            public UInt32 OutgoingCompressedBytes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WINSTATIONNAME_LENGTH)]
            public String WinStationName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DOMAIN_LENGTH)]
            public String Domain;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = USERNAME_LENGTH + 1)]
            public String UserName;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 ConnectTime;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 DisconnectTime;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 LastInputTime;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 LogonTime;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 CurrentTime;
        }
        #endregion

        #region Enumurations
        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo,
            WTSConfigInfo,
            WTSValidationInfo,
            WTSSessionAddressV4,
            WTSIsRemoteSession
        }
        #endregion

        public static List<SessionInfo> ListSessions()
        {
            IntPtr server = IntPtr.Zero;
            var ret = new List<SessionInfo>();
            server = WTS_CURRENT_SERVER_HANDLE;

            IntPtr ppSessionInfo = IntPtr.Zero;
            Int32 count = 0;
            Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
            Int32 dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            var current = (Int64)ppSessionInfo;

            if (retval != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += dataSize;
                    var sessionInfo = QuerySessionInfo(si.iSessionID);
                    if (sessionInfo != null)
                        ret.Add(sessionInfo);
                    else
                        ret.Add(new SessionInfo { ID = si.iSessionID, State = si.oState });
                }
                WTSFreeMemory(ppSessionInfo);
            }

            return ret;
        }

        static public SessionInfo QuerySessionInfo(Int32 SessionID)
        {
            var sessionInfo = new SessionInfo();
            sessionInfo.ID = SessionID;
            IntPtr pName = IntPtr.Zero;
            uint iReturned = 0;
            if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, SessionID, WTS_INFO_CLASS.WTSSessionInfo, out pName, out iReturned) && iReturned > 1)
            {
                var wtsinfo = (WTSINFO)Marshal.PtrToStructure(pName, typeof(WTSINFO));
                sessionInfo.UserName = wtsinfo.UserName;
                sessionInfo.State = wtsinfo.State;
                if (wtsinfo.DisconnectTime != 0)
                    sessionInfo.DisconnectTime = DateTime.FromFileTimeUtc(wtsinfo.DisconnectTime);
                else
                    sessionInfo.DisconnectTime = DateTime.MinValue;
                if (wtsinfo.LogonTime != 0)
                    sessionInfo.LogonTime = DateTime.FromFileTimeUtc(wtsinfo.LogonTime);
                else
                    sessionInfo.LogonTime = DateTime.MinValue;
                if (wtsinfo.LastInputTime != 0)
                    sessionInfo.LastInputTime = DateTime.FromFileTimeUtc(wtsinfo.LastInputTime);
                else
                    sessionInfo.LastInputTime = DateTime.MinValue;
                
                /*// Get IP
                var pBuf = IntPtr.Zero;
                if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, SessionID, WTS_INFO_CLASS.WTSClientAddress, out pBuf, out iReturned) && iReturned > 1)
                {
                    var oClientAddress = (WTS_CLIENT_ADDRESS)Marshal.PtrToStructure(pBuf, typeof(WTS_CLIENT_ADDRESS));
                    if (oClientAddress.iAddressFamily == (int)AddressFamilyType.AF_INET)
                    {
                        sessionInfo.IP = oClientAddress.bAddress[2] + "." + oClientAddress.bAddress[3] + "." + 
                            oClientAddress.bAddress[4] + "." + oClientAddress.bAddress[5];
                    }
                    else
                        sessionInfo.IP = (oClientAddress.iAddressFamily).ToString() + ":" + oClientAddress.bAddress.Length;
                    WTSFreeMemory(pBuf);
                }
                else
                    sessionInfo.IP = "Len=" + iReturned.ToString();*/

                // Get display info
                var pBuf = IntPtr.Zero;
                if (WTSQuerySessionInformation(WTS_CURRENT_SERVER_HANDLE, SessionID, WTS_INFO_CLASS.WTSClientDisplay, out pBuf, out iReturned) && iReturned > 1)
                {
                    var oClientDisplay = (WTS_CLIENT_DISPLAY)Marshal.PtrToStructure(pBuf, typeof(WTS_CLIENT_DISPLAY));
                    sessionInfo.Display = new SessionInfo.DisplayInfo
                    {
                        Width = oClientDisplay.iHorizontalResolution,
                        Height = oClientDisplay.iVerticalResolution,
                        ColorDepth = oClientDisplay.iColorDepth,
                    };
                    WTSFreeMemory(pBuf);
                }

                WTSFreeMemory(pName);
                return sessionInfo;
            }
            return null;
        }
    }
}
