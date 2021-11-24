using System.Collections;
using System.Collections.Generic;
using System;

public static class BIOPACEncoder
{
    private static List<byte> byteBuffer = new List<byte>();

    public static byte[] EncodeBIOPACString(string BIOPACString)
    {
        byteBuffer.Clear();

        switch (EncodingController.Instance.EncodingType)
        {
            case EncodingType.ASCII:
                byteBuffer.AddRange(System.Text.Encoding.ASCII.GetBytes(BIOPACString));
                break;
            case EncodingType.Unicode:
                byteBuffer.AddRange(System.Text.Encoding.Unicode.GetBytes(BIOPACString));
                break;
            case EncodingType.UTF7:
                byteBuffer.AddRange(System.Text.Encoding.UTF7.GetBytes(BIOPACString));
                break;
            case EncodingType.UTF8:
                byteBuffer.AddRange(System.Text.Encoding.UTF8.GetBytes(BIOPACString));
                break;
            case EncodingType.UTF32:
                byteBuffer.AddRange(System.Text.Encoding.UTF32.GetBytes(BIOPACString));
                break;
            case EncodingType.BigEndianUnicode:
                byteBuffer.AddRange(System.Text.Encoding.BigEndianUnicode.GetBytes(BIOPACString));
                break;
        }

        return byteBuffer.ToArray();
    }

    public static string DecodeBIOPACString(byte[] data, int dataLength)
    {
        string decodedString;

        switch (EncodingController.Instance.EncodingType)
        {
            case EncodingType.Unicode:
                decodedString = System.Text.Encoding.Unicode.GetString(data, 0, dataLength);
                break;
            case EncodingType.UTF7:
                decodedString = System.Text.Encoding.UTF7.GetString(data, 0, dataLength);
                break;
            case EncodingType.UTF8:
                decodedString = System.Text.Encoding.UTF8.GetString(data, 0, dataLength);
                break;
            case EncodingType.UTF32:
                decodedString = System.Text.Encoding.UTF32.GetString(data, 0, dataLength);
                break;
            case EncodingType.BigEndianUnicode:
                decodedString = System.Text.Encoding.BigEndianUnicode.GetString(data, 0, dataLength);
                break;
            default:
            case EncodingType.ASCII:
                decodedString = System.Text.Encoding.ASCII.GetString(data, 0, dataLength);
                break;
        }

        return decodedString;
    }
}
