using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domain.Common;

public abstract class Enumeration : IComparable
{
    public int Id { get; }
    public string Name { get; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Id == value);
        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid value in {typeof(T)}");
        return matchingItem;
    }

    public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);
}