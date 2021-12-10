using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

public class BIOPACInterfaceMessages
{
}

#region Custom Messages
public class ClientServerSyncMessage
{
    public SimpleTime ClientTime { get; set; }
    public SimpleTime ServerTime { get; set; }
}
#endregion

#region Support Classes

public class SimpleTime : INetSerializable
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public int Milliseconds { get; set; }

    public SimpleTime()
    {
        
    }
    public SimpleTime(DateTime time)
    {
        Hours = time.Hour;
        Minutes = time.Minute;
        Seconds = time.Second;
        Milliseconds = time.Millisecond;
    }
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Hours);
        writer.Put(Minutes);
        writer.Put(Seconds);
        writer.Put(Milliseconds);
    }

    public void Deserialize(NetDataReader reader)
    {
        Hours = reader.GetInt();
        Minutes = reader.GetInt();
        Seconds = reader.GetInt();
        Milliseconds = reader.GetInt();
    }

    public override string ToString()
    {
        return $"{Hours}:{Minutes}:{Seconds},{Milliseconds}";
    }
}

#endregion




