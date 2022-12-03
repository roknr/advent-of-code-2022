namespace AdventOfCode2022.Day1
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source, Func<T, bool> splitByPredicate)
        {
            using var sourceEnumerator = source.GetEnumerator();

            while (sourceEnumerator.MoveNext())
            {
                yield return SplitGroup(sourceEnumerator);
            }

            IEnumerable<T> SplitGroup(IEnumerator<T> enumerator)
            {
                yield return enumerator.Current;

                while (enumerator.MoveNext() && !splitByPredicate(enumerator.Current))
                {
                    yield return enumerator.Current;
                }
            }
        }

        public static List<TSource> MaxNBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            int n,
            Func<TSource?, TKey> keySelector,
            IComparer<TKey>? keyComparer = null)
        {
            keyComparer ??= Comparer<TKey>.Default;
            var sourceComparerBy = new ReverseComparerBy<TSource, TKey>(keySelector, keyComparer);

            var result = new List<TSource>(n);

            using var enumerator = source.GetEnumerator();

            for (var i = 0; i < n; i++)
            {
                if (enumerator.MoveNext())
                {
                    result.Add(enumerator.Current);
                }
                else
                {
                    throw new InvalidOperationException("Not enough elements in the collection.");
                }
            }

            result.Sort(sourceComparerBy);

            while (enumerator.MoveNext())
            {
                var index = result.BinarySearch(enumerator.Current, sourceComparerBy);

                if (index < 0)
                {
                    index = ~index;
                }

                if (index < n)
                {
                    result.Insert(index, enumerator.Current);
                    result.RemoveAt(n);
                }
            }

            return result;
        }

        public class ComparerBy<TSource, TKey> : IComparer<TSource>
        {
            private readonly Func<TSource?, TKey> _keySelector;
            private readonly IComparer<TKey> _keyComparer;

            public ComparerBy(Func<TSource?, TKey> keySelector, IComparer<TKey>? keyComparer = null)
            {
                _keySelector = keySelector;
                _keyComparer = keyComparer ?? Comparer<TKey>.Default;
            }

            public virtual int Compare(TSource? x, TSource? y)
            {
                var keyX = _keySelector(x);
                var keyY = _keySelector(y);

                if (keyX == null)
                {
                    return keyY == null
                        ? 0 : -1;
                }

                return _keyComparer.Compare(keyX, keyY);
            }
        }

        public class ReverseComparerBy<TSource, TKey> : ComparerBy<TSource, TKey>
        {
            public ReverseComparerBy(Func<TSource?, TKey> keySelector, IComparer<TKey>? keyComparer = null) : base(keySelector, keyComparer) { }

            public override int Compare(TSource? x, TSource? y)
            {
                return base.Compare(y, x);
            }
        }
    }
}
