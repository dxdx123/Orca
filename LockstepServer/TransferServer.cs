using SLNet;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using MessagePack;
using MessagePack.Resolvers;

public class TransferServer
{
    public const int FPS = 60;
    public const int TimePerFrame = (int)(1000 * 1.0f / FPS);

    public const string HOST = "0.0.0.0";
    public const int PORT = 8888;

    public const string PASSWORD = "SLikeNet";
    public const int MAX_CONNECTION = 16;
    public const int TIMEOUT = 5000;

    public const int MAX_ROOM_PLAYER = 1;

    private RakPeerInterface _rakPeerInterface;

    private Dictionary<string, AddressOrGUID> _playerDict = new Dictionary<string, AddressOrGUID>();
    private Dictionary<string, int> _playerSeatDict = new Dictionary<string, int>();
    private Dictionary<string, bool> _playerReadyDict = new Dictionary<string, bool>();

    public RakPeerInterface GetRankPeerInterface()
    {
        if (_rakPeerInterface == null)
        {
            _rakPeerInterface = RakPeerInterface.GetInstance();
        }

        return _rakPeerInterface;
    }

    private static bool serializerRegistered = false;
    private static void Initialize()
    {
        if (!serializerRegistered)
        {
            StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }
    }

    public void Init()
    {
        Initialize();

        Console.WriteLine("!!! TransferServer::Init");

        GetRankPeerInterface().SetIncomingPassword(PASSWORD, PASSWORD.Length);
        GetRankPeerInterface().SetTimeoutTime(TIMEOUT, SLikeNet.UNASSIGNED_SYSTEM_ADDRESS);
        GetRankPeerInterface().SetOccasionalPing(true);

        SocketDescriptor socketDesc = new SocketDescriptor(PORT, HOST);
        StartupResult result = GetRankPeerInterface().Startup(MAX_CONNECTION, socketDesc, 1);
        GetRankPeerInterface().SetMaximumIncomingConnections(MAX_CONNECTION);

        if (result == StartupResult.RAKNET_STARTED)
        {
            Console.WriteLine("!!! Server Start Successfully");
        }
        else
        {
            Console.WriteLine("!!! Server Start Failed: {0}", result);
        }
    }

    public void Run()
    {
        Console.WriteLine("!!! TransferServer::Run");

        var sw = Stopwatch.StartNew();

        int ticks = 0;

        while (true)
        {
            Thread.Sleep(TimePerFrame - ticks);

            ticks += (int)(sw.ElapsedMilliseconds);

            while (ticks >= TimePerFrame)
            {
                Update(TimePerFrame);

                ticks -= TimePerFrame;
            }

            sw.Restart();
        }
    }

    private void Update(int dt)
    {
        // Console.WriteLine("!!! Update dt: {0}", dt);

        for (Packet packet = GetRankPeerInterface().Receive(); packet != null; GetRankPeerInterface().DeallocatePacket(packet), packet = GetRankPeerInterface().Receive())
        {
            DefaultMessageIDTypes identifier = GetPacketIdentifier(packet);
            Console.WriteLine("!!! identifier: {0}", identifier);

            Dispatch(identifier, packet);
        }
    }

    private void Dispatch(DefaultMessageIDTypes identifier, Packet packet)
    {
        switch (identifier)
        {
            case DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION:
                Console.WriteLine("ID_NEW_INCOMING_CONNECTION to: " + packet.systemAddress.ToString() + " with GUID: " + packet.guid.ToString());
                // address must be guid
                AddPlayer(packet.guid.ToString(), packet.guid);
                break;

            case DefaultMessageIDTypes.ID_USER_PLAYER_LEAVE_REQ:
                Console.WriteLine("ID_USER_PLAYER_LEAVE_REQ");
                RemovePlayer(packet.guid.ToString());
            break;

            case DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
                Console.WriteLine("ID_DISCONNECTION_NOTIFICATION");
                RemovePlayer(packet.guid.ToString());
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
                Console.WriteLine("We are banned from this server.\n");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED:
                Console.WriteLine("Connection attempt failed ");
                break;
            case DefaultMessageIDTypes.ID_NO_FREE_INCOMING_CONNECTIONS:
                Console.WriteLine("Server is full ");
                break;
            case DefaultMessageIDTypes.ID_INVALID_PASSWORD:
                Console.WriteLine("ID_INVALID_PASSWORD\n");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_LOST:
                // Couldn't deliver a reliable packet - i.e. the other system was abnormally
                // terminated
                Console.WriteLine("ID_CONNECTION_LOST\n");
                break;
            case DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
                // This tells the client they have connected
                Console.WriteLine("ID_CONNECTION_REQUEST_ACCEPTED to %s " + packet.systemAddress.ToString() + "with GUID " + packet.guid.ToString());
                break;
            case DefaultMessageIDTypes.ID_CONNECTED_PING:
            case DefaultMessageIDTypes.ID_UNCONNECTED_PING:
                Console.WriteLine("Ping from " + packet.systemAddress.ToString(true));
                break;

            case DefaultMessageIDTypes.ID_USER_PLAYER_READY_REQ:
                SetPlayerReady(packet.guid.ToString());
                break;

            case DefaultMessageIDTypes.ID_USER_SEND_ACTION_REQ:
                OnUserSendActionReq(packet.guid.ToString(), packet);
                break;

            case DefaultMessageIDTypes.ID_USER_CONFIRM_ACTION_REQ:
                OnUserConfirmAction(packet.guid.ToString(), packet);
                break;

            default:
                Console.WriteLine("!!! identifier: {0}", identifier);
                break;
        }
    }

    private void SetPlayerReady(string guid)
    {
        Console.WriteLine($"SetPlayerReady guid: {guid}");

        _playerReadyDict[guid] = true;

        CheckGameStart();
    }

    private void OnUserConfirmAction(string senderPlayerId, Packet packet)
    {
        var reqObj = DeserializeObj<LockstepConformReq>(packet);

        string playerId = reqObj.PlayerID;

        AddressOrGUID address;
        if(_playerDict.TryGetValue(playerId, out address))
        {
            var resObj = new LockstepConformRes()
            {
                LockstepTurnID = reqObj.LockstepTurnID,
                PlayerID = senderPlayerId,
                SeatID = GetSeatId(playerId),
            };

            SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_CONFIRM_ACTION_RES, resObj);
        }
        else
        {
            throw new Exception($"not found player id: {playerId}");
        }
    }

    private void OnUserSendActionReq(string senderPlayerId, Packet packet)
    {
        var reqObj = DeserializeObj<LockstepPacketReq>(packet);

        foreach(var item in _playerDict)
        {
            string playerId = item.Key;
            AddressOrGUID address = item.Value;

            if(senderPlayerId == playerId)
            {
                // self
            }
            else
            {
                var resObj = new LockstepPacketRes()
                {
                    PlayerID = senderPlayerId,
                    SeatID = GetSeatId(senderPlayerId),
                    LockstepTurnID = reqObj.LockstepTurnID,
                    ActionId = reqObj.ActionId,
                    ActionData = reqObj.ActionData,
                };

                SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_SEND_ACTION_RES, resObj);
            }
        }
    }

    private T DeserializeObj<T>(Packet packet)
    {
        byte[] bytes = packet.data;
        
        var memory = new ReadOnlyMemory<byte>(bytes, 1, bytes.Length - 1);
        var res = MessagePackSerializer.Deserialize<T>(memory);

        return res;
    }

    private void AddPlayer(string guid, AddressOrGUID address)
    {
        if(!_playerDict.ContainsKey(guid))
        {
            int seatId = _playerSeatDict.Count;
            _playerSeatDict.Add(guid, seatId);

            // self
            {
                LoginRes loginObj = new LoginRes()
                {
                    PlayerID = guid,
                    SeatID = seatId,
                };
                SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_LOGIN_SUCCESS_RES, loginObj);

                foreach(var item in _playerDict)
                {
                    PlayerJoinRes obj = new PlayerJoinRes()
                    {
                        PlayerID = item.Key,
                        SeatID = GetSeatId(item.Key),
                    };

                    SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_PLAYER_JOIN_RES, obj);
                }

                PlayerJoinRes joinObj = new PlayerJoinRes()
                {
                    PlayerID = guid,
                    SeatID = seatId,
                };
                SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_PLAYER_JOIN_RES, joinObj);
            }

            // others
            {
                foreach(var item in _playerDict)
                {
                    PlayerJoinRes joinObj = new PlayerJoinRes()
                    {
                        PlayerID = guid,
                        SeatID = seatId,
                    };

                    var otherAddress = item.Value;
                    SendObjectRaw(otherAddress, (byte)DefaultMessageIDTypes.ID_USER_PLAYER_JOIN_RES, joinObj);
                }
            }

            // add it
            _playerDict.Add(guid, address);

        }
        else
        {
            throw new Exception($"Same player: {guid} exists");
        }
    }

    private int GetSeatId(string guid)
    {
        int seatId;

        if(_playerSeatDict.TryGetValue(guid, out seatId))
        {
            return seatId;
        }
        else
        {
            throw new Exception($"GetSeatId not find {guid}");
        }
    }

    private bool GetReadyStatus(string guid)
    {
        bool ready;

        if(_playerReadyDict.TryGetValue(guid, out ready))
        {
            return ready;
        }
        else
        {
            return false;
        }
    }

    private void CheckGameStart()
    {
        bool allReady = IsAllReady();

        if(allReady)
        {
            foreach(var item in _playerDict)
            {
                string playerId = item.Key;
                AddressOrGUID address = item.Value;

                var obj = new GameStartRes();
                SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_GAME_START_RES, obj);
            }
        }
        else
        {
            // nothing
        }
    }

    private bool IsAllReady()
    {
        if(_playerDict.Count < MAX_ROOM_PLAYER)
        {
            return false;
        }
        else
        {
            foreach(var item in _playerDict)
            {
                string playerId = item.Key;

                bool ready = GetReadyStatus(playerId);
                if(ready)
                {
                    // check next
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    private void RemovePlayer(string guid)
    {
        AddressOrGUID address;

        if(_playerDict.TryGetValue(guid, out address))
        {
            // self
            {
                PlayerLeaveRes leaveObj = new PlayerLeaveRes()
                {
                    PlayerID = guid,
                    SeatID = GetSeatId(guid),
                };

                SendObjectRaw(address, (byte)DefaultMessageIDTypes.ID_USER_PLAYER_LEAVE_RES, leaveObj);
            }

            _playerDict.Remove(guid);

            // others
            {
                foreach(var item in _playerDict)
                {
                    PlayerLeaveRes leaveObj = new PlayerLeaveRes()
                    {
                        PlayerID = guid,
                        SeatID = GetSeatId(guid),
                    };

                    AddressOrGUID otherAddress = item.Value;
                    SendObjectRaw(otherAddress, (byte)DefaultMessageIDTypes.ID_USER_PLAYER_LEAVE_RES, leaveObj);
                }
            }

            _playerSeatDict.Remove(guid);
            _playerReadyDict.Remove(guid);
        }
        else
        {
            // nothing
        }
    }

    public void SendObjectRaw<T>(AddressOrGUID address, byte identifier, T obj)
    {
        byte[] bytes = MessagePackSerializer.Serialize(obj);
        int start = 0;
        int sendLength = bytes.Length;
        int bufferLength = sendLength + 1;

        // rent buffer
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLength);

        buffer[0] = identifier;
        Array.Copy(bytes, start, buffer, 1, sendLength);

        GetRankPeerInterface().Send(buffer, bufferLength, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE_ORDERED, (char)0,
            address, false);

        // return buffer
        ArrayPool<byte>.Shared.Return(buffer);
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
}
