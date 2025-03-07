using System.Reflection;

namespace G_Searcher.Utilities;

public class Polymorphism
{
    public static T[] CreatePolymorphismArray<T>(params object[] obj)
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
            .Select(x => obj.Length == 0 ? (T)Activator.CreateInstance(x) : (T)Activator.CreateInstance(x, obj))
            .ToArray();
    }
}
