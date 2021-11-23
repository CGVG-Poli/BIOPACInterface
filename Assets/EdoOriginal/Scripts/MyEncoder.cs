using System.Collections;
using System.Collections.Generic;
using System;

public enum MessageType
{
    NULL,
    VALUES,
    COMMAND
}

public enum CommandType
{
    NULL,
    OPEN_CONNECTION_REQUEST,
    CONNECTION_REQUEST_ACCEPTED,
    START_SYNCING_SCENEOBJECTS,
    SETUP_SYNCRATE,
    GENERIC_MESSAGE,
    STOP_SYNCING_SCENEOBJECTS
}

public static class MyEncoder
{
    private static List<byte> byteBuffer = new List<byte>();

    public static byte[] EncodeMessage(MessageType messageType, CommandType commandType, object obj)
    {

        byteBuffer.Clear();
        
        byte[] tempBytes = BitConverter.GetBytes((int)messageType);
        byteBuffer.AddRange(tempBytes);

        switch (messageType)
        {
            case MessageType.COMMAND:

                tempBytes = BitConverter.GetBytes((int)commandType);
                byteBuffer.AddRange(tempBytes);

                if (commandType == CommandType.SETUP_SYNCRATE || commandType == CommandType.CONNECTION_REQUEST_ACCEPTED) {
                    tempBytes = BitConverter.GetBytes((int)obj);
                    byteBuffer.AddRange(tempBytes);
                }

                if (commandType == CommandType.OPEN_CONNECTION_REQUEST)
                {
                    string clientName = (string)obj;
                    int stringSize = System.Text.Encoding.ASCII.GetByteCount(clientName);
                    tempBytes = BitConverter.GetBytes(stringSize);
                    byteBuffer.AddRange(tempBytes);
                    tempBytes = System.Text.Encoding.ASCII.GetBytes(clientName);
                    byteBuffer.AddRange(tempBytes);
                }

                if (commandType == CommandType.GENERIC_MESSAGE)
                {
                    string genericMessage = (string)obj;
                    int stringSize = System.Text.Encoding.ASCII.GetByteCount(genericMessage);
                    tempBytes = BitConverter.GetBytes(stringSize);
                    byteBuffer.AddRange(tempBytes);
                    tempBytes = System.Text.Encoding.ASCII.GetBytes(genericMessage);
                    byteBuffer.AddRange(tempBytes);
                }

                break;
        }

        return byteBuffer.ToArray();
    }

    public static MessageType DecodeMessageType(byte[] data) {

        MessageType messageType = (MessageType)BitConverter.ToInt32(data, 0); //reads only the first Int32 to decode message type

        int i = (int)messageType;

        return messageType;
    }

    public static CommandType DecodeCommandType(byte[] data) {
        int readIndex = sizeof(int); //reads second Int32 to decode command type
        CommandType commandType = (CommandType)BitConverter.ToInt32(data, readIndex);

        return commandType;
    }

    public static int DecodeInteger(byte[] data) {
        int readIndex = 2 * sizeof(int); //reads the third Int32 to decode int sent (syncrate, clientID, etc.)
        int decodedInteger = BitConverter.ToInt32(data, readIndex);

        return decodedInteger;
    }

    public static string DecodeUTF8String(byte[] data) {
        int readIndex = sizeof(int); //reads the second Int32 that contains the length of the string
        int stringSize = BitConverter.ToInt32(data, readIndex);
        readIndex += sizeof(int);
        string strUTF8 = System.Text.Encoding.UTF8.GetString(data, readIndex, stringSize);

        return strUTF8;
    }

    
}
