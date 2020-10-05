using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using SLNet;
using UnityEngine;

public class NetworkConnect
{
    public const int MAX_CONNECTION = 1;

    private AddressOrGUID _serverGUID;
    private RakPeerInterface _rakPeerInterface;
    private bool _connectSuccess;

    private List<Action> _delayActionList = new List<Action>();

    private RakPeerInterface RakPeerInterface
    {
        get { return _rakPeerInterface; }
        set => _rakPeerInterface = value;
    }

    public void Connect(string host, int port, string password)
    {
        Initialize(host, port);
        
        ConnectionAttemptResult connectResult = RakPeerInterface.Connect(host, Convert.ToUInt16(port), 
            password, password.Length);
        
        if (connectResult == ConnectionAttemptResult.CONNECTION_ATTEMPT_STARTED)
        {
            // Debug.LogFormat("Connect {0}:{1} Successfully", host, port);
            _connectSuccess = true;
        }
        else
        {
            Debug.LogErrorFormat("Couldn't connect RakNet ip: {0} port: {1} reason: {2}", host, port, connectResult);
        }
    }

    private void Initialize(string host, int port)
    {
        _serverGUID = new SystemAddress(host, Convert.ToUInt16(port));
        RakPeerInterface = RakPeerInterface.GetInstance();
        
        SocketDescriptor socketDesc = new SocketDescriptor();
        StartupResult result = RakPeerInterface.Startup(MAX_CONNECTION, socketDesc, 1);

        if(result == StartupResult.RAKNET_STARTED)
        {
            // Debug.LogFormat("!!! Server Start Successfully");
        }
        else
        {
            Debug.LogErrorFormat("!!! Server Start Failed: {0}", result);
        }
        
        RakPeerInterface.SetOccasionalPing(true);
    }

    public void Update()
    {
        if (_connectSuccess)
        {
            for (Packet packet = RakPeerInterface.Receive();
                packet != null;
                RakPeerInterface.DeallocatePacket(packet), packet = RakPeerInterface.Receive())
            {
                DefaultMessageIDTypes identifier = GetPacketIdentifier(packet);
                // Debug.LogFormat("!!! identifier: {0}", identifier);

                Dispatch(identifier, packet);
            }
        }

        ProcessDelayAction();
    }

    private void ProcessDelayAction()
    {
        if (_delayActionList.Count > 0)
        {
            foreach (var action in _delayActionList)
            {
                action();
            }
            _delayActionList.Clear();
        }
    }


    private void Dispatch(DefaultMessageIDTypes identifier, Packet packet)
    {
        switch (identifier)
        {
            case DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
                Console.WriteLine("ID_DISCONNECTION_NOTIFICATION");
                break;
            case DefaultMessageIDTypes.ID_ALREADY_CONNECTED:
                Console.WriteLine("ID_ALREADY_CONNECTED with guid " + packet.guid);
                break;
            case DefaultMessageIDTypes.ID_INCOMPATIBLE_PROTOCOL_VERSION:
                Console.WriteLine("ID_INCOMPATIBLE_PROTOCOL_VERSION ");
                break;
            case DefaultMessageIDTypes.ID_REMOTE_DISCONNECTION_NOTIFICATION:
                Console.WriteLine("ID_REMOTE_DISCONNECTION_NOTIFICATION ");
                break;
            case DefaultMessageIDTypes.ID_REMOTE_CONNECTION_LOST: // Server telling the clients of another client disconnecting forcefully.  You can manually broadcast this in a peer to peer enviroment if you want.
                Console.WriteLine("ID_REMOTE_CONNECTION_LOST");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_BANNED: // Banned from this server
                Console.WriteLine("We are banned from this server.");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED:
                Console.WriteLine("Connection attempt failed ");
                break;
            case DefaultMessageIDTypes.ID_NO_FREE_INCOMING_CONNECTIONS:
                Console.WriteLine("Server is full ");
                break;
            case DefaultMessageIDTypes.ID_INVALID_PASSWORD:
                Console.WriteLine("ID_INVALID_PASSWORD");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_LOST:
                // Couldn't deliver a reliable packet - i.e. the other system was abnormally
                // terminated
                Console.WriteLine("ID_CONNECTION_LOST");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
                // This tells the client they have connected
                Console.WriteLine("ID_CONNECTION_REQUEST_ACCEPTED to %s " + packet.systemAddress.ToString() + "with GUID " + packet.guid.ToString());
                Console.WriteLine("My external address is:" + RakPeerInterface.GetExternalID(packet.systemAddress).ToString());
                break;
            case DefaultMessageIDTypes.ID_CONNECTED_PING:
            case DefaultMessageIDTypes.ID_UNCONNECTED_PING:
                Console.WriteLine("Ping from " + packet.systemAddress.ToString(true));
                break;

            case DefaultMessageIDTypes.ID_USER_LOGIN_SUCCESS_RES:
            {
                var res = DeserializeObj<LoginRes>(packet);
                OnLoginSuccess(res);
            }
                break;
            
            case DefaultMessageIDTypes.ID_USER_PLAYER_JOIN_RES:
            {
                var res = DeserializeObj<PlayerJoinRes>(packet);
                OnPlayerJoin(res);
            }
                break;
            
            case DefaultMessageIDTypes.ID_USER_PLAYER_LEAVE_RES:
            {
                var res = DeserializeObj<PlayerLeaveRes>(packet);
                OnPlayerLeave(res);
            }
                break;
            
            case DefaultMessageIDTypes.ID_USER_GAME_START_RES:
            {
                var res = DeserializeObj<GameStartRes>(packet);
                OnGameStart(res);
            }
                break;
            
            case DefaultMessageIDTypes.ID_USER_SEND_ACTION_RES:
            {
                var res = DeserializeObj<LockstepPacketRes>(packet);
                OnOtherUserSendActionRes(res);
            }
                break;
            
            case DefaultMessageIDTypes.ID_USER_CONFIRM_ACTION_RES:
            {
                var res = DeserializeObj<LockstepConformRes>(packet);
                OnServerConfirmAction(res);
            }
                break;

            default:
                break;
        }
    }

    private void OnLoginSuccess(LoginRes res)
    {
        Debug.Log($"OnLoginSuccess {res}");
        NetworkController.Instance.PlayerId = res.PlayerID;
        NetworkController.Instance.SeatId = res.SeatID;
    }

    private void OnPlayerJoin(PlayerJoinRes res)
    {
        Debug.Log($"OnPlayerJoin {res}");
    }

    private void OnPlayerLeave(PlayerLeaveRes res)
    {
        Debug.Log($"OnPlayerLeave {res}");
    }

    private void OnGameStart(GameStartRes res)
    {
        Debug.Log($"OnGameStart {res}");

        GameContext gameContext = Contexts.sharedInstance.game;
        gameContext.SetScene("DefaultScene");

        LockStepController.Instance.GameStart = true;
    }

    private void OnOtherUserSendActionRes(LockstepPacketRes res)
    {
        Debug.Log($"OnOtherUserSendActionRes {res}");

        LockStepController.Instance.OnOtherUserSendActionRes(res);
    }

    private void OnServerConfirmAction(LockstepConformRes res)
    {
        Debug.Log($"OnServerConfirmAction {res}");

        LockStepController.Instance.OnServerConfirmAction(res);
    }

    private T DeserializeObj<T>(Packet packet)
    {
        byte[] bytes = packet.data;
        
        var memory = new ReadOnlyMemory<byte>(bytes, 1, bytes.Length - 1);
        var res = MessagePackSerializer.Deserialize<T>(memory);

        return res;
    }
    
    private DefaultMessageIDTypes GetPacketIdentifier(Packet p)
    {
        if (p == null)
            return (DefaultMessageIDTypes)255;

        byte headByte = p.data[0];
        if (headByte == (char)DefaultMessageIDTypes.ID_TIMESTAMP)
        {
            return (DefaultMessageIDTypes)p.data[5];
        }
        else
        {
            return (DefaultMessageIDTypes)headByte;
        }
    } 

    public void Destroy()
    {
        Action action = () =>
        {
            DestroyImmediate();
        };
        
        _delayActionList.Add(action);
    }

    public void DestroyImmediate()
    {
        if (RakPeerInterface != null)
        {
            RakPeerInterface.Shutdown(300);
            RakPeerInterface.DestroyInstance(RakPeerInterface);

            RakPeerInterface = null;
        }
    }

    // public void Disconnect()
    // {
    //     var req = new PlayerLeaveReq();
    //     
    //     SendObjectRaw((byte)DefaultMessageIDTypes.ID_USER_PLAYER_LEAVE_REQ, req);
    // }
    
    public void SendObjectRaw<T>(byte identifier, T obj)
    {
        byte[] bytes = MessagePackSerializer.Serialize(obj);
        int start = 0;
        int sendLength = bytes.Length;
        
        int bufferLength = sendLength + 1;

        // rent buffer
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLength);

        buffer[0] = identifier;
        Array.Copy(bytes, start, buffer, 1, sendLength);

        RakPeerInterface.Send(buffer, bufferLength, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE_ORDERED, (char)0,
            _serverGUID, false);

        // return buffer
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
