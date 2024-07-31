using System.Collections.Generic;

namespace CCL.Types
{
    public static class SerializationUtility
    {
        public static void SerializeJaggedArray<T>(T[][] input, out List<T> values, out List<int> lengths)
        {
            values = new List<T>();
            lengths = new List<int>();

            foreach (var item in input)
            {
                values.AddRange(item);
                lengths.Add(item.Length);
            }
        }

        public static void DeserializeJaggedArray<T>(List<T> values, List<int> lengths, out T[][] array)
        {
            int count = lengths.Count;
            int current = 0;
            array = new T[count][];

            for (int i = 0; i < lengths.Count; i++)
            {
                int l = lengths[i];
                array[i] = new T[l];
                values.CopyTo(current, array[i], 0, l);
                current += l;
            }
        }
    }
}
