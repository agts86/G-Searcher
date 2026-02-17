using System.Reflection;

namespace Shared.Utilities;

/// <summary>
/// リフレクションを使ってポリモーフィズムを実現するクラス
/// </summary>
public class Polymorphism
{
    /// <summary>
    /// ポリモーフィズムの配列を生成する
    /// </summary>
    /// <param name="assembly">探索対象アセンブリ</param>
    /// <param name="obj">インスタンス引数</param>
    /// <typeparam name="T">基底型</typeparam>
    /// <returns>基底型配列</returns>
    public static T[] CreatePolymorphismArray<T>(Assembly assembly, params object[] obj)
    {
        return [.. assembly.GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
            .Select(x => obj.Length == 0 ? (T)Activator.CreateInstance(x) : (T)Activator.CreateInstance(x, obj))];
    }
}
