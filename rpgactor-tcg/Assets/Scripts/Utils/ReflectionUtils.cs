using System;
using System.Reflection;

public static class ReflectionUtils
{
    public static Type FindReflectedType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;

        if (typeName.Contains("."))
        {
            var assemblyName = typeName[..typeName.IndexOf('.')];
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null) return null;
            type = assembly.GetType(typeName);
            if ( type!=null ) return type;
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }
        }
        return null;
    }
}
