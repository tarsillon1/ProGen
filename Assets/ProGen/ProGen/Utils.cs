using System;

namespace ProGen
{
    public static class Utils
    {
        public static string EncodePosition(params int[] pos)
        {
            string key = "";
            for (int i = 0; i < pos.Length; i++)
            {
                if (i == pos.Length - 1)
                {
                    key += pos[i];
                }
                else
                {
                    key += pos[i] + ":";
                }
            }
            return key;
        }

        public static int[] DecodePosition(string key)
        {
            string[] parts = key.Split(':');
            int[] pos = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                pos[i] = Int32.Parse(parts[i]);
            }
            return pos;
        }
    }
}