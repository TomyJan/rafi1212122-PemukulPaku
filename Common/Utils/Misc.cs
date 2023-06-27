using System;
using System.Collections.Generic;

namespace Common.Utils
{
    public static class Misc 
    {
        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static double CalculateEntropy(string input)
        {
            int length = input.Length;
            Dictionary<char, int> frequencies = new Dictionary<char, int>();

            // 统计字符出现次数
            foreach (char c in input)
            {
                if (frequencies.ContainsKey(c))
                    frequencies[c]++;
                else
                    frequencies[c] = 1;
            }

            // 计算熵
            double entropy = 0.0;
            foreach (int count in frequencies.Values)
            {
                double probability = (double)count / length;
                entropy -= probability * (Math.Log(probability) / Math.Log(2));
            }

            return entropy;
        }

    }
}
