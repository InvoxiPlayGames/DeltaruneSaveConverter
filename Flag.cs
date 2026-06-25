using System;

namespace DeltaruneSaveConverter;

public class Flag
{
 
    public static void ParseFlagArray(string listhex, ref double[] output, int length)
    {
        byte[] rawlist = new byte[listhex.Length / 2];
        for (int i = 0; i < listhex.Length; i += 2)
        {
            rawlist[i / 2] = Convert.ToByte(listhex.Substring(i, 2), 16);
        }

        int byteIndex = 8;
    
        for (int i = 0; i < length; i++)
        {
            if (byteIndex + 8 > rawlist.Length) break;

            output[i] = BitConverter.ToDouble(rawlist, byteIndex);
        
            byteIndex += 8;
        }
    }
}
