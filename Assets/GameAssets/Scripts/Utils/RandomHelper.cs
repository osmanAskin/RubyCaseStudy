using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RubyCase.Utils
{
    public static class RandomHelper
    {
        public static T GetRandomElement<T>(this T[] array)
        {
            if (array == null || array.Length == 0) return default;

            return array[Random.Range(0, array.Length)];
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;

            return list[Random.Range(0, list.Count)];
        }

        public static T GetRandomElementWithinFirst<T>(this T[] array, int topLimit)
        {
            if (array == null || array.Length == 0) return default;

            if (topLimit <= 0) return default;

            return array[Random.Range(0, Math.Min(topLimit, array.Length))];
        }

        public static T GetRandomElementWithinFirst<T>(this List<T> list, int topLimit)
        {
            if (list == null || list.Count == 0) return default;
            if (topLimit <= 0) return default;
            return list[Random.Range(0, Math.Min(topLimit, list.Count))];
        }

        public static T[] GetRandomElements<T>(this T[] array, int elementCount)
        {
            if (array == null || array.Length == 0) return Array.Empty<T>();

            elementCount = Mathf.Clamp(elementCount, 0, array.Length);

            T[] shuffled = (T[])array.Clone();

            shuffled.Shuffle();

            T[] picked = new T[elementCount];
            Array.Copy(shuffled, picked, elementCount);
            return picked;
        }

        public static List<T> GetRandomElements<T>(this List<T> list, int elementCount)
        {
            if (list == null || list.Count == 0)
                return new List<T>();

            elementCount = Mathf.Clamp(elementCount, 0, list.Count);

            var shuffled = new List<T>(list);
            shuffled.Shuffle();

            return shuffled.GetRange(0, elementCount);
        }

        public static void Shuffle<T>(this T[] array)
        {
            if (array == null || array.Length < 2)
                return;

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (list == null || list.Count < 2)
                return;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
