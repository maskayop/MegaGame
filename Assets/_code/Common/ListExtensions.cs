using System;
using System.Collections.Generic;

namespace Vopere.Common
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                // ����� ���������
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}