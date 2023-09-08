using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
namespace TWC.Utilities
{
	public static class ReflectionHelpers
	{
		public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
		{
			var result = new List<System.Type>();
			var assemblies = aAppDomain.GetAssemblies();
			
			//try {
				foreach (var assembly in assemblies)
				{
					var types = assembly.GetTypes();
					foreach (var type in types)
					{
						if (type.IsSubclassOf(aType))
							result.Add(type);
						
					}
				}
			//}
			//catch(System.Reflection.ReflectionTypeLoadException ex)
			//{
			//	foreach(System.Exception inner in ex.LoaderExceptions) 
			//	{
			//		// write details of "inner", in particular inner.Message
			//		Debug.Log("TWC Reflection Type Load Exception: " + inner.Message);
			//	}
			//}
			return result.ToArray();
		}
	}
}