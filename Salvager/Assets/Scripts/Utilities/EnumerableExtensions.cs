using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Utilities
{
    public static class EnumerableExtensions
    {
        // TODO: this is really fancy but it's not the best solution for performance or my sanity
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            
            var count = 0;
            T selectedElement = default(T);

            foreach (var element in enumerable)
            {
                count++;
                if (Random.Range(0, count) == 0)
                {
                    selectedElement = element;
                }
            }

            if (count == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            return selectedElement;
        }
    }
}