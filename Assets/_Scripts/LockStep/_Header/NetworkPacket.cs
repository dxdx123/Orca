using System.Collections;
using System.Collections.Generic;
using MessagePack;

[MessagePackObject]
public class LockstepPacketReq
{
    [Key(0)]
    public int LockstepTurnID { get; set; }

    [Key(1)]
    public int ActionId { get; set; }
    
    [Key(2)]
    public byte[] ActionData { get; set; }

    public override string ToString()
    {
        return $"LockstepTurnID: {LockstepTurnID}, ActionId: {ActionId}, ActionData: {ActionData}";
    }
}

[MessagePackObject]
public class LockstepPacketRes
{
    [Key(0)]
    public string PlayerID { get; set; }
    
    [Key(1)]
    public int SeatID { get; set; }
    
    [Key(2)]
    public int LockstepTurnID { get; set; }

    [Key(3)]
    public int ActionId { get; set; }
    
    [Key(4)]
    public byte[] ActionData { get; set; }
    
    public override string ToString()
    {
        return $"PlayerID: {PlayerID}, SeatID: {SeatID}, LockstepTurnID: {LockstepTurnID}, ActionId: {ActionId}, ActionData: {ActionData}";
    }
}

[MessagePackObject]
public class LockstepConformReq
{
    [Key(0)]
    public int LockstepTurnID { get; set; }
    
    [Key(1)]
    public string PlayerID { get; set; }
    
    [Key(2)]
    public int SeatID { get; set; }
    
    public override string ToString()
    {
        return $"LockstepTurnID: {LockstepTurnID}, PlayerID: {PlayerID}, SeatID: {SeatID}";
    }
}

[MessagePackObject]
public class LockstepConformRes
{
    [Key(0)]
    public int LockstepTurnID { get; set; }
    
    [Key(1)]
    public string PlayerID { get; set; }
    
    [Key(2)]
    public int SeatID { get; set; }
    
    public override string ToString()
    {
        return $"LockstepTurnID: {LockstepTurnID}, PlayerID: {PlayerID}, SeatID: {SeatID}";
    }
}

[MessagePackObject]
public class PlayerJoinRes
{
    [Key(0)]
    public string PlayerID { get; set; }
    
    [Key(1)]
    public int SeatID { get; set; }
    
    public override string ToString()
    {
        return $"PlayerID: {PlayerID}, SeatID: {SeatID}";
    }
}

[MessagePackObject]
public class PlayerLeaveRes
{
    [Key(0)]
    public string PlayerID { get; set; }
    
    [Key(1)]
    public int SeatID { get; set; }
    
    public override string ToString()
    {
        return $"PlayerID: {PlayerID}, SeatID: {SeatID}";
    }
}

[MessagePackObject]
public class LoginRes
{
    [Key(0)]
    public string PlayerID { get; set; }
    
    [Key(1)]
    public int SeatID { get; set; }
    
    public override string ToString()
    {
        return $"PlayerID: {PlayerID}, SeatID: {SeatID}";
    }
}

[MessagePackObject]
public class GameReadyReq
{
    [Key(0)]
    public bool Ready;
    
    public override string ToString()
    {
        return $"Ready: {Ready}";
    }
}

[MessagePackObject]
public class GameReadyRes
{
    [Key(0)]
    public bool Ready;
    
    public override string ToString()
    {
        return $"Ready: {Ready}";
    }
}

[MessagePackObject]
public class GameStartRes
{
}
