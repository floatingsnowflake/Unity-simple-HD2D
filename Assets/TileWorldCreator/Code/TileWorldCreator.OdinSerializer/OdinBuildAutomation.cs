#if UNITY_EDITOR
namespace TWC.OdinSerializer.Utilities.Editor
{
	using TWC.OdinSerializer.Editor;
	using TWC.OdinSerializer.Utilities.Editor;
	using TWC.OdinSerializer.Utilities;
	
    using UnityEditor;
    using UnityEditor.Build;
    using System.IO;
	using System;
	using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
#endif
	
	
    public static class OdinBuildAutomation
    {
        private static readonly string EditorAssemblyPath;
        private static readonly string JITAssemblyPath;
        private static readonly string AOTAssemblyPath;
	    private static readonly string GenerateAssembliesDir;
        
	    private static Dictionary<string, string> pathCache = new Dictionary<string, string>(10);

	    private static string GetFilePath(string file)
	    {
		 

		    string filename = Directory.EnumerateFiles("Assets", file, SearchOption.AllDirectories).FirstOrDefault();

		    if (string.IsNullOrEmpty(filename))
		    {
			    return string.Empty;
		    }

		    return $"{Path.GetDirectoryName(filename).Replace("\\", "/")}/";


	    }



        static OdinBuildAutomation()
	    {
		    GenerateAssembliesDir = Path.Combine(GetFilePath("OdinBuildAutomation.cs"), "Generated");
		    
        }

        private static string GetAssemblyDirectory(this Assembly assembly)
        {
            string filePath = new Uri(assembly.CodeBase).LocalPath;
            return Path.GetDirectoryName(filePath);
        }

        public static void OnPreprocessBuild()
        {
            BuildTarget platform = EditorUserBuildSettings.activeBuildTarget;

            try
            {
                // The EditorOnly dll should aways have the same import settings. But lets just make sure.
                //AssemblyImportSettingsUtilities.SetAssemblyImportSettings(platform, EditorAssemblyPath, OdinAssemblyImportSettings.IncludeInEditorOnly);

                if (AssemblyImportSettingsUtilities.IsJITSupported(
                    platform,
                    AssemblyImportSettingsUtilities.GetCurrentScriptingBackend(),
                    AssemblyImportSettingsUtilities.GetCurrentApiCompatibilityLevel()))
                {
                //    AssemblyImportSettingsUtilities.SetAssemblyImportSettings(platform, AOTAssemblyPath, OdinAssemblyImportSettings.ExcludeFromAll);
                //    AssemblyImportSettingsUtilities.SetAssemblyImportSettings(platform, JITAssemblyPath, OdinAssemblyImportSettings.IncludeInBuildOnly);
                }
                else
                {
                //    AssemblyImportSettingsUtilities.SetAssemblyImportSettings(platform, AOTAssemblyPath, OdinAssemblyImportSettings.IncludeInBuildOnly);
                //    AssemblyImportSettingsUtilities.SetAssemblyImportSettings(platform, JITAssemblyPath, OdinAssemblyImportSettings.ExcludeFromAll);
	               
                    // Generates dll that contains all serialized generic type variants needed at runtime.
	                List<Type> types = new List<Type>();
                    
	                //UnityEngine.Debug.Log(GenerateAssembliesDir);
                    
	                if (TWC.OdinSerializer.Editor.AOTSupportUtilities.ScanProjectForSerializedTypes(out types))
                    {
                    }   
                   
	                types.Add(typeof(System.Guid));
	            	types.Add(typeof(System.Boolean[,]));
	                types.Add(typeof(UnityEngine.Color));	
	              
	                    
                	TWC.OdinSerializer.Editor.AOTSupportUtilities.GenerateDLL(GenerateAssembliesDir, "TWCOdinAOTSupport", types);

                }
            }
            finally
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        public static void OnPostprocessBuild()
        {
            // Delete Generated AOT support dll after build so it doesn't pollute the project.
            if (Directory.Exists(GenerateAssembliesDir))
            {
                Directory.Delete(GenerateAssembliesDir, true);
                File.Delete(GenerateAssembliesDir + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }

#if UNITY_2018_1_OR_NEWER
    public class OdinPreBuildAutomation : IPreprocessBuildWithReport
#else
    public class OdinPreBuildAutomation : IPreprocessBuild
#endif
    {
        public int callbackOrder { get { return -1000; } }

#if UNITY_2018_1_OR_NEWER
	    public void OnPreprocessBuild(BuildReport report)
	    {
            OdinBuildAutomation.OnPreprocessBuild();
	    }
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            OdinBuildAutomation.OnPreprocessBuild();
        }
#endif
    }

#if UNITY_2018_1_OR_NEWER
    public class OdinPostBuildAutomation : IPostprocessBuildWithReport
#else
    public class OdinPostBuildAutomation : IPostprocessBuild
#endif
    {
        public int callbackOrder { get { return -1000; } }

#if UNITY_2018_1_OR_NEWER
	    public void OnPostprocessBuild(BuildReport report)
	    {
            OdinBuildAutomation.OnPostprocessBuild();
	    }
#else
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            OdinBuildAutomation.OnPostprocessBuild();

        }
#endif
    }
}
#endif