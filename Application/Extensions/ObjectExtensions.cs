using System.Linq.Expressions;

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

    public static TTarget CopyFrom<TSource, TTarget>(TTarget target, TSource source, Expression<Func<TSource, object>> expression)
    {
        var sourceProps = typeof(TSource).GetProperties();
        var targetProps = typeof(TTarget).GetProperties();

        var memberExpression = expression.Body as MemberExpression;
        if (memberExpression == null)
            throw new ArgumentException("Invalid expression");

        var sourceProp = sourceProps.FirstOrDefault(p => p.Name == memberExpression.Member.Name && p.PropertyType == targetProps.First().PropertyType);
        if (sourceProp != null)
        {
            var value = sourceProp.GetValue(source);
            targetProps.First().SetValue(target, value);
        }

        return target;
        
    }     
    
}
