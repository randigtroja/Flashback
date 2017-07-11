using System.Collections.Generic;

namespace Flashback.Model
{
    public class FlashbackCacheList<T> : List<T>
    {
        public new void Add(T item)
        {
            if (Count >= 20)
            {
                RemoveAt(0);
            }

            base.Add(item);
        }
    }
}
