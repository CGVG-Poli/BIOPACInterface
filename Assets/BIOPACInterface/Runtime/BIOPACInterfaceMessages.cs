using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

public class BIOPACInterfaceMessages
{
}

#region Custom Messages

public class BaseMessage
{
    public BaseMessage() {}
}

public class ClientInformationMessage : BaseMessage
{
    public string ClientDeviceName { get; set; }
    public string ClientUniqueIdentifier { get; set; }
}

public class ClientServerSyncMessage : BaseMessage
{
    public SimpleTime ClientTime { get; set; }
    public SimpleTime ClientTimeSending { get; set; }
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
        Hours = 0;
        Minutes = 0;
        Seconds = 0;
        Milliseconds = 0;
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
    
    public static TimeSpan operator -(SimpleTime t1, SimpleTime t2)
    {
        DateTime time1 = new DateTime(1989, 6, 17, t1.Hours, t1.Minutes, t1.Seconds, t1.Milliseconds);
        DateTime time2 = new DateTime(1989, 6, 17, t2.Hours, t2.Minutes, t2.Seconds, t2.Milliseconds);

        return (time1 - time2);
    }  
}

#endregion




