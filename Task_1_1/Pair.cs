namespace Task_1_1
{
        public struct Pair<TKey, TValue>
    {
        public Pair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}