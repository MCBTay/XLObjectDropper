namespace Dreamteck
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    public static class FindDerivedClasses
    {
        public static List<Type> GetAllDerivedClasses(this Type aBaseClass, string[] aExcludeAssemblies)
        {
            List<Type> result = new List<Type>();
            foreach (Assembly A in AppDomain.CurrentDomain.GetAssemblies())
            {
                bool exclude = false;
                foreach (string S in aExcludeAssemblies)
                {
                    if (A.GetName().FullName.StartsWith(S))
                    {
                        exclude = true;
                        break;
                    }
                }
                if (exclude)
                    continue;
                if (aBaseClass.IsInterface)
                {
                    foreach (Type C in A.GetExportedTypes())
                        foreach (Type I in C.GetInterfaces())
                            if (aBaseClass == I)
                            {
                                result.Add(C);
                                break;
                            }
                }
                else
                {
                    foreach (Type C in A.GetExportedTypes())
                        if (C.IsSubclassOf(aBaseClass))
                            result.Add(C);
                }
            }
            return result;
        }

        public static List<Type> GetAllDerivedClasses(this Type aBaseClass)
        {
            return GetAllDerivedClasses(aBaseClass, new string[0]);
        }
    }
}
