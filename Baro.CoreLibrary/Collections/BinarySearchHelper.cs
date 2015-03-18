using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Collections
{
    public interface IBinarySearchable<T>
    {
        T this[int index] { get; }
    }

    public static class BinarySearchHelper
    {
        public static int BinarySearch<T>(IList<T> array, int index, int length, T value, Comparison<T> comparer)
        {
            int low = index;
            int high = (index + length) - 1;

            while (low <= high)
            {
                int mid = ((high + low) >> 1);

                int compareValue = comparer(array[mid], value);

                if (compareValue == 0)
                {
                    return mid;
                }

                if (compareValue < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return ~low;
        }

        public static int BinarySearch<T>(IBinarySearchable<T> array, int index, int length, T value, Comparison<T> comparer)
        {
            int low = index;
            int high = (index + length) - 1;

            while (low <= high)
            {
                int mid = ((high + low) >> 1);

                int compareValue = comparer(array[mid], value);

                if (compareValue == 0)
                {
                    return mid;
                }

                if (compareValue < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return ~low;
        }

        public static int BinarySearch<T>(T[] array, int index, int length, T value, Comparison<T> comparer)
        {
            int low = index;
            int high = (index + length) - 1;

            while (low <= high)
            {
                int mid = ((high + low) >> 1);

                int compareValue = comparer(array[mid], value);

                if (compareValue == 0)
                {
                    return mid;
                }

                if (compareValue < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return ~low;
        }
    }
}
