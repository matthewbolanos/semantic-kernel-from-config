
using System.Reflection;

namespace PowerMatt.SKFromConfig.Extensions;

#pragma warning disable CS8600
#pragma warning disable CS8601  
#pragma warning disable CS8602 
#pragma warning disable CS8614 
#pragma warning disable CS8714 

public class MergeUtilities
{
    public static T MergeObjects<T>(T defaultObj, T environmentObj)
    {
        if (defaultObj == null)
        {
            throw new ArgumentNullException(nameof(defaultObj));
        }

        if (environmentObj == null)
        {
            throw new ArgumentNullException(nameof(environmentObj));
        }

        Type objType = typeof(T);
        T mergedObj = Activator.CreateInstance<T>();

        foreach (PropertyInfo property in objType.GetProperties())
        {
            object defaultValue = property.GetValue(defaultObj);
            object environmentValue = property.GetValue(environmentObj);

            if (environmentValue != null)
            {
                if (IsAssignableToGenericType(property.PropertyType, typeof(Dictionary<,>)))
                {
                    MethodInfo mergeMethod = typeof(MergeUtilities).GetMethod(nameof(MergeDictionaries), BindingFlags.Static | BindingFlags.NonPublic);
                    MethodInfo genericMergeMethod = mergeMethod.MakeGenericMethod(property.PropertyType.GenericTypeArguments);
                    object mergedValue = genericMergeMethod.Invoke(null, new object[] { defaultValue, environmentValue });
                    property.SetValue(mergedObj, mergedValue);
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    object mergedValue = MergeObjects(defaultValue, environmentValue);
                    property.SetValue(mergedObj, mergedValue);
                }
                else
                {
                    property.SetValue(mergedObj, environmentValue);
                }
            }
            else
            {
                property.SetValue(mergedObj, defaultValue);
            }
        }

        return mergedObj;
    }

    private static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type baseType = givenType.BaseType;
        if (baseType == null)
            return false;

        return IsAssignableToGenericType(baseType, genericType);
    }

    private static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> defaultDict, Dictionary<TKey, TValue> environmentDict)
    {
        var mergedDict = new Dictionary<TKey, TValue>(defaultDict);

        foreach (var kvp in environmentDict)
        {
            if (defaultDict[kvp.Key] == null)
            {
                mergedDict[kvp.Key] = kvp.Value;
            }
            mergedDict[kvp.Key] = MergeObjects(defaultDict[kvp.Key], kvp.Value);
        }

        return mergedDict;
    }
}