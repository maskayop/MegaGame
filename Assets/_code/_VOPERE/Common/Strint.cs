using UnityEngine;

namespace Vopere.Common
{
    public static class Strint
    {
        static readonly uint[] uints = new uint []
        {
            0, // à
            1, // á
            2, // â
            4, // ã
            8, // ä
            16, // å
            32, // ¸
            64, // æ
            128, // ç
            256, // è
            512, // é
            1024, // ê
            2048, // ë
            4096, // ì
            8192, // í
            16384, // î
            32768, // ï
            65536, // ð
            131072, // ñ
            262144, // ò
            524288, // ó
            1048576, // ô
            2097152, // õ
            4194304, // ö
            8388608, // ÷
            16777216, // ø
            33554432, // ù
            67108864, // ú
            134217728, // û
            268435456, // ü
            536870912, // ý
            1073741824, // þ
            2147483648, // ÿ
        };

        static readonly char[] chars = new char[]
        {
            'à', 'á', 'â', 'ã', 'ä', 'å', '¸', 'æ', 'ç', 'è', 'é',
            'ê', 'ë', 'ì', 'í', 'î', 'ï', 'ð', 'ñ', 'ò', 'ó', 'ô',
            'õ', 'ö', '÷', 'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ'
        };


        public static int GetInt(string IN)
        {
            int value = 0;

            if (IN == null)
                return 0;

            char[] textBuffer = IN.ToCharArray();

            for (int i = 0; i < textBuffer.Length; i++)
            {
                if (GetCharId(textBuffer[i]) >= 32)
                {
                    value = int.MaxValue;
                    break;
                }

                value += (int)uints[GetCharId(textBuffer[i])];
            }

            return value;
        }

        public static string GetString(int IN)
        {
            string value = "";
            int temp = IN;

            for (int i = 0; i < 32; i++)
            {
                value += chars[GetUintId(temp)];
                temp -= (int)uints[GetUintId(temp)];

                if (temp <= 0)
                    break;
            }

            return value;
        }

        static int GetCharId(char c)
        {
            int value = -1;

            for (int i = 0; i < chars.Length; i++)
            {
                if (c == chars[i])
                {
                    value = i;
                    break;
                }
            }

            return value;
        }

        static int GetUintId(int IN)
        {
            int value = -1;

            for (int i = 0; i < uints.Length; i++)
            {
                if (IN < uints[i])
                {
                    value = i - 1;
                    break;
                }
            }

            return value;
        }

        public static int Summation(string a, string b)
        {
            return GetInt(a) + GetInt(b);
        }

        public static int Subtraction(string a, string b)
        {
            return GetInt(a) - GetInt(b);
        }

        public static int Multiplication(string a, string b)
        {
            return GetInt(a) * GetInt(b);
        }

        public static int Division(string a, string b)
        {
            return GetInt(a) / GetInt(b);
        }
    }
}
