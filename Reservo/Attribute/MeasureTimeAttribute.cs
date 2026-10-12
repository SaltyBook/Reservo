using Serilog;
using System.Diagnostics;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MeasureTimeAttribute : Attribute
{
    public static object? Invoke(
        object instance,
        string methodName,
        params object?[]? args)
    {
        var method = instance.GetType().GetMethod(
            methodName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic);

        if (method == null)
            throw new MissingMethodException(methodName);

        // Ohne Attribut: Methode normal ausführen
        if (method.GetCustomAttribute<MeasureTimeAttribute>() == null)
            return method.Invoke(instance, args);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            return method.Invoke(instance, args);
        }
        finally
        {
            stopwatch.Stop();

            Log.Information(
                $"[MeasureTime] {method.Name}: " +
                $"{stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        }
    }
}
