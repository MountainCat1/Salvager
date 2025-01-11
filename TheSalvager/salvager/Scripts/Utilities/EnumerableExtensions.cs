namespace Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Gets a random element from an IEnumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The IEnumerable source collection.</param>
    /// <returns>A randomly selected element.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source is empty.</exception>
    public static T GetRandomElement<T>(this IEnumerable<T> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), "Source collection cannot be null.");
        }

        var list = source as IList<T> ?? source.ToList();

        if (list.Count == 0)
        {
            throw new InvalidOperationException("Cannot select a random element from an empty collection.");
        }

        return list[_random.Next(list.Count)];
    }
}
