namespace Saba.Application.Extensions;

public static class ObjectExtensions
{
    public static TTarget CopyFrom<TSource, TTarget>(this TTarget target, TSource source)
    {
        var sourceProps = typeof(TSource).GetProperties();
        var targetProps = typeof(TTarget).GetProperties();

        foreach (var targetProp in targetProps)
        {
            var sourceProp = sourceProps.FirstOrDefault(p => p.Name == targetProp.Name && p.PropertyType == targetProp.PropertyType);
            if (sourceProp != null && targetProp.CanWrite)
            {
                var value = sourceProp.GetValue(source);
                targetProp.SetValue(target, value);
            }
        }

        return target;
    }
}
