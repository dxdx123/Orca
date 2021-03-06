/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.12
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace SLNet {

using System;
using System.Runtime.InteropServices;

public class SLikeNet {
 
	    public static readonly SystemAddress UNASSIGNED_SYSTEM_ADDRESS = new SystemAddress();
	    public static readonly RakNetGUID UNASSIGNED_RAKNET_GUID = new RakNetGUID(ulong.MaxValue);

	    public static void StatisticsToString(RakNetStatistics s, out string buffer, int verbosityLevel) 
 	   {
		String tmp = new String('c', 9999);
		buffer=StatisticsToStringHelper(s,tmp,verbosityLevel);
 	   }
	
  public static string StatisticsToStringHelper(RakNetStatistics s, string buffer, int verbosityLevel) {
    string ret = SLikeNetPINVOKE.StatisticsToStringHelper(RakNetStatistics.getCPtr(s), buffer, verbosityLevel);
    return ret;
  }

  public static int MAX_RPC_MAP_SIZE {
    get {
      int ret = SLikeNetPINVOKE.MAX_RPC_MAP_SIZE_get();
      return ret;
    } 
  }

  public static int UNDEFINED_RPC_INDEX {
    get {
      int ret = SLikeNetPINVOKE.UNDEFINED_RPC_INDEX_get();
      return ret;
    } 
  }

  public static bool NonNumericHostString(string host) {
    bool ret = SLikeNetPINVOKE.NonNumericHostString(host);
    return ret;
  }

  public static ushort UNASSIGNED_PLAYER_INDEX {
    get {
      ushort ret = SLikeNetPINVOKE.UNASSIGNED_PLAYER_INDEX_get();
      return ret;
    } 
  }

  public static ulong UNASSIGNED_NETWORK_ID {
    get {
      ulong ret = SLikeNetPINVOKE.UNASSIGNED_NETWORK_ID_get();
      return ret;
    } 
  }

  public static int PING_TIMES_ARRAY_SIZE {
    get {
      int ret = SLikeNetPINVOKE.PING_TIMES_ARRAY_SIZE_get();
      return ret;
    } 
  }

  public static RakString OpPlus(RakString lhs, RakString rhs) {
    RakString ret = new RakString(SLikeNetPINVOKE.OpPlus__SWIG_0(RakString.getCPtr(lhs), RakString.getCPtr(rhs)), true);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private static RakWString OpPlus(RakWString lhs, RakWString rhs) {
    RakWString ret = new RakWString(SLikeNetPINVOKE.OpPlus__SWIG_1(RakWString.getCPtr(lhs), RakWString.getCPtr(rhs)), true);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public static uint MAX_UNSIGNED_LONG {
    get {
      uint ret = SLikeNetPINVOKE.MAX_UNSIGNED_LONG_get();
      return ret;
    } 
  }

  public static ulong GetTime() {
    ulong ret = SLikeNetPINVOKE.GetTime();
    return ret;
  }

  public static uint GetTimeMS() {
    uint ret = SLikeNetPINVOKE.GetTimeMS();
    return ret;
  }

  public static ulong GetTimeUS() {
    ulong ret = SLikeNetPINVOKE.GetTimeUS();
    return ret;
  }

  public static bool GreaterThan(ulong a, ulong b) {
    bool ret = SLikeNetPINVOKE.GreaterThan(a, b);
    return ret;
  }

  public static bool LessThan(ulong a, ulong b) {
    bool ret = SLikeNetPINVOKE.LessThan(a, b);
    return ret;
  }

  public static readonly int SWIG_CSHARP_NO_IMCLASS_STATIC_CONSTRUCTOR = SLikeNetPINVOKE.SWIG_CSHARP_NO_IMCLASS_STATIC_CONSTRUCTOR_get();
  public static readonly int __GET_TIME_64BIT = SLikeNetPINVOKE.__GET_TIME_64BIT_get();
  public static readonly int MAX_ALLOCA_STACK_ALLOCATION = SLikeNetPINVOKE.MAX_ALLOCA_STACK_ALLOCATION_get();
  public static readonly int _USE_RAK_MEMORY_OVERRIDE = SLikeNetPINVOKE._USE_RAK_MEMORY_OVERRIDE_get();
  public static readonly int OPEN_SSL_CLIENT_SUPPORT = SLikeNetPINVOKE.OPEN_SSL_CLIENT_SUPPORT_get();
  public static readonly int BITSTREAM_STACK_ALLOCATION_SIZE = SLikeNetPINVOKE.BITSTREAM_STACK_ALLOCATION_SIZE_get();
  public static readonly int MAXIMUM_NUMBER_OF_INTERNAL_IDS = SLikeNetPINVOKE.MAXIMUM_NUMBER_OF_INTERNAL_IDS_get();
  public static readonly int DATAGRAM_MESSAGE_ID_ARRAY_LENGTH = SLikeNetPINVOKE.DATAGRAM_MESSAGE_ID_ARRAY_LENGTH_get();
  public static readonly int RESEND_BUFFER_ARRAY_LENGTH = SLikeNetPINVOKE.RESEND_BUFFER_ARRAY_LENGTH_get();
  public static readonly int RESEND_BUFFER_ARRAY_MASK = SLikeNetPINVOKE.RESEND_BUFFER_ARRAY_MASK_get();
  public static readonly int GET_TIME_SPIKE_LIMIT = SLikeNetPINVOKE.GET_TIME_SPIKE_LIMIT_get();
  public static readonly int USE_SLIDING_WINDOW_CONGESTION_CONTROL = SLikeNetPINVOKE.USE_SLIDING_WINDOW_CONGESTION_CONTROL_get();
  public static readonly int PREALLOCATE_LARGE_MESSAGES = SLikeNetPINVOKE.PREALLOCATE_LARGE_MESSAGES_get();
  public static readonly int RAKNET_SUPPORT_IPV6 = SLikeNetPINVOKE.RAKNET_SUPPORT_IPV6_get();
  public static readonly int RAKSTRING_TYPE_IS_UNICODE = SLikeNetPINVOKE.RAKSTRING_TYPE_IS_UNICODE_get();
  public static readonly int RPC4_GLOBAL_REGISTRATION_MAX_FUNCTIONS = SLikeNetPINVOKE.RPC4_GLOBAL_REGISTRATION_MAX_FUNCTIONS_get();
  public static readonly int RPC4_GLOBAL_REGISTRATION_MAX_FUNCTION_NAME_LENGTH = SLikeNetPINVOKE.RPC4_GLOBAL_REGISTRATION_MAX_FUNCTION_NAME_LENGTH_get();
  public static readonly int XBOX_BYPASS_SECURITY = SLikeNetPINVOKE.XBOX_BYPASS_SECURITY_get();
  public static readonly int BUFFERED_PACKETS_PAGE_SIZE = SLikeNetPINVOKE.BUFFERED_PACKETS_PAGE_SIZE_get();
  public static readonly int INTERNAL_PACKET_PAGE_SIZE = SLikeNetPINVOKE.INTERNAL_PACKET_PAGE_SIZE_get();
  public static readonly int RAKPEER_USER_THREADED = SLikeNetPINVOKE.RAKPEER_USER_THREADED_get();
  public static readonly int USE_ALLOCA = SLikeNetPINVOKE.USE_ALLOCA_get();
  public static readonly int SLNET_MAX_RETRIEVABLE_FILESIZE = SLikeNetPINVOKE.SLNET_MAX_RETRIEVABLE_FILESIZE_get();
  public static readonly int LIBCAT_SECURITY = SLikeNetPINVOKE.LIBCAT_SECURITY_get();
  public static readonly int _RAKNET_SUPPORT_ConnectionGraph2 = SLikeNetPINVOKE._RAKNET_SUPPORT_ConnectionGraph2_get();
  public static readonly int _RAKNET_SUPPORT_DirectoryDeltaTransfer = SLikeNetPINVOKE._RAKNET_SUPPORT_DirectoryDeltaTransfer_get();
  public static readonly int _RAKNET_SUPPORT_FileListTransfer = SLikeNetPINVOKE._RAKNET_SUPPORT_FileListTransfer_get();
  public static readonly int _RAKNET_SUPPORT_FullyConnectedMesh = SLikeNetPINVOKE._RAKNET_SUPPORT_FullyConnectedMesh_get();
  public static readonly int _RAKNET_SUPPORT_FullyConnectedMesh2 = SLikeNetPINVOKE._RAKNET_SUPPORT_FullyConnectedMesh2_get();
  public static readonly int _RAKNET_SUPPORT_MessageFilter = SLikeNetPINVOKE._RAKNET_SUPPORT_MessageFilter_get();
  public static readonly int _RAKNET_SUPPORT_NatPunchthroughClient = SLikeNetPINVOKE._RAKNET_SUPPORT_NatPunchthroughClient_get();
  public static readonly int _RAKNET_SUPPORT_NatPunchthroughServer = SLikeNetPINVOKE._RAKNET_SUPPORT_NatPunchthroughServer_get();
  public static readonly int _RAKNET_SUPPORT_NatTypeDetectionClient = SLikeNetPINVOKE._RAKNET_SUPPORT_NatTypeDetectionClient_get();
  public static readonly int _RAKNET_SUPPORT_NatTypeDetectionServer = SLikeNetPINVOKE._RAKNET_SUPPORT_NatTypeDetectionServer_get();
  public static readonly int _RAKNET_SUPPORT_PacketLogger = SLikeNetPINVOKE._RAKNET_SUPPORT_PacketLogger_get();
  public static readonly int _RAKNET_SUPPORT_ReadyEvent = SLikeNetPINVOKE._RAKNET_SUPPORT_ReadyEvent_get();
  public static readonly int _RAKNET_SUPPORT_ReplicaManager3 = SLikeNetPINVOKE._RAKNET_SUPPORT_ReplicaManager3_get();
  public static readonly int _RAKNET_SUPPORT_Router2 = SLikeNetPINVOKE._RAKNET_SUPPORT_Router2_get();
  public static readonly int _RAKNET_SUPPORT_RPC4Plugin = SLikeNetPINVOKE._RAKNET_SUPPORT_RPC4Plugin_get();
  public static readonly int _RAKNET_SUPPORT_TeamBalancer = SLikeNetPINVOKE._RAKNET_SUPPORT_TeamBalancer_get();
  public static readonly int _RAKNET_SUPPORT_TeamManager = SLikeNetPINVOKE._RAKNET_SUPPORT_TeamManager_get();
  public static readonly int _RAKNET_SUPPORT_UDPProxyClient = SLikeNetPINVOKE._RAKNET_SUPPORT_UDPProxyClient_get();
  public static readonly int _RAKNET_SUPPORT_UDPProxyCoordinator = SLikeNetPINVOKE._RAKNET_SUPPORT_UDPProxyCoordinator_get();
  public static readonly int _RAKNET_SUPPORT_UDPProxyServer = SLikeNetPINVOKE._RAKNET_SUPPORT_UDPProxyServer_get();
  public static readonly int _RAKNET_SUPPORT_ConsoleServer = SLikeNetPINVOKE._RAKNET_SUPPORT_ConsoleServer_get();
  public static readonly int _RAKNET_SUPPORT_RakNetTransport = SLikeNetPINVOKE._RAKNET_SUPPORT_RakNetTransport_get();
  public static readonly int _RAKNET_SUPPORT_TelnetTransport = SLikeNetPINVOKE._RAKNET_SUPPORT_TelnetTransport_get();
  public static readonly int _RAKNET_SUPPORT_TCPInterface = SLikeNetPINVOKE._RAKNET_SUPPORT_TCPInterface_get();
  public static readonly int _RAKNET_SUPPORT_LogCommandParser = SLikeNetPINVOKE._RAKNET_SUPPORT_LogCommandParser_get();
  public static readonly int _RAKNET_SUPPORT_RakNetCommandParser = SLikeNetPINVOKE._RAKNET_SUPPORT_RakNetCommandParser_get();
  public static readonly int _RAKNET_SUPPORT_EmailSender = SLikeNetPINVOKE._RAKNET_SUPPORT_EmailSender_get();
  public static readonly int _RAKNET_SUPPORT_HTTPConnection = SLikeNetPINVOKE._RAKNET_SUPPORT_HTTPConnection_get();
  public static readonly int _RAKNET_SUPPORT_HTTPConnection2 = SLikeNetPINVOKE._RAKNET_SUPPORT_HTTPConnection2_get();
  public static readonly int _RAKNET_SUPPORT_PacketizedTCP = SLikeNetPINVOKE._RAKNET_SUPPORT_PacketizedTCP_get();
  public static readonly int _RAKNET_SUPPORT_TwoWayAuthentication = SLikeNetPINVOKE._RAKNET_SUPPORT_TwoWayAuthentication_get();
  public static readonly int _RAKNET_SUPPORT_CloudClient = SLikeNetPINVOKE._RAKNET_SUPPORT_CloudClient_get();
  public static readonly int _RAKNET_SUPPORT_CloudServer = SLikeNetPINVOKE._RAKNET_SUPPORT_CloudServer_get();
  public static readonly int _RAKNET_SUPPORT_DynDNS = SLikeNetPINVOKE._RAKNET_SUPPORT_DynDNS_get();
  public static readonly int _RAKNET_SUPPORT_Rackspace = SLikeNetPINVOKE._RAKNET_SUPPORT_Rackspace_get();
  public static readonly int _RAKNET_SUPPORT_FileOperations = SLikeNetPINVOKE._RAKNET_SUPPORT_FileOperations_get();
  public static readonly int _RAKNET_SUPPORT_UDPForwarder = SLikeNetPINVOKE._RAKNET_SUPPORT_UDPForwarder_get();
  public static readonly int _RAKNET_SUPPORT_StatisticsHistory = SLikeNetPINVOKE._RAKNET_SUPPORT_StatisticsHistory_get();
  public static readonly int _RAKNET_SUPPORT_LibVoice = SLikeNetPINVOKE._RAKNET_SUPPORT_LibVoice_get();
  public static readonly int _RAKNET_SUPPORT_RelayPlugin = SLikeNetPINVOKE._RAKNET_SUPPORT_RelayPlugin_get();
  public static readonly string PRINTF_64_BIT_MODIFIER = SLikeNetPINVOKE.PRINTF_64_BIT_MODIFIER_get();
  public static readonly int NETWORK_ID_MANAGER_HASH_LENGTH = SLikeNetPINVOKE.NETWORK_ID_MANAGER_HASH_LENGTH_get();
  public static readonly string RAK_TIME_FORMAT_STRING = SLikeNetPINVOKE.RAK_TIME_FORMAT_STRING_get();
  public static readonly int ALLOW_JOIN_ANY_AVAILABLE_TEAM = SLikeNetPINVOKE.ALLOW_JOIN_ANY_AVAILABLE_TEAM_get();
  public static readonly int ALLOW_JOIN_SPECIFIC_TEAM = SLikeNetPINVOKE.ALLOW_JOIN_SPECIFIC_TEAM_get();
  public static readonly int ALLOW_JOIN_REBALANCING = SLikeNetPINVOKE.ALLOW_JOIN_REBALANCING_get();
  public static readonly int _TABLE_BPLUS_TREE_ORDER = SLikeNetPINVOKE._TABLE_BPLUS_TREE_ORDER_get();
  public static readonly int _TABLE_MAX_COLUMN_NAME_LENGTH = SLikeNetPINVOKE._TABLE_MAX_COLUMN_NAME_LENGTH_get();
  public static readonly int REMOTE_MAX_TEXT_INPUT = SLikeNetPINVOKE.REMOTE_MAX_TEXT_INPUT_get();
  public static readonly int MESSAGE_FILTER_MAX_MESSAGE_ID = SLikeNetPINVOKE.MESSAGE_FILTER_MAX_MESSAGE_ID_get();
}

}
