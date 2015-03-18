using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Collections.Generics
{
    public class StaticStack<T>
    {
        private T[] items;
        private int count;

        public int Count
        {
            get { return count; }
        }

        public StaticStack(int capacity)
        {
            items = new T[capacity];
            count = 0;
        }

        public void Clear()
        {
            count = 0;
        }

        public void Push(T value)
        {
            items[count++] = value;
        }

        public T Pop()
        {
            return items[--count];
        }
    }
}
