using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace ShaderGraphEnhancer
{
	public class PackageCacheOverrider
	{
		const string pre =
			"[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"Seonghwan.ShaderGraph.Enhancer.Editor\")]";

		[InitializeOnLoadMethod]
		private static void Override()
		{
			string packageCachePath = Application.dataPath.Replace("/Assets", "/Library/PackageCache");
			DirectoryInfo packageCache = new DirectoryInfo(packageCachePath);

			DirectoryInfo shaderGraphPackage = null;
			foreach (DirectoryInfo directoryInfo in packageCache.GetDirectories())
			{
				if (directoryInfo.Name.Contains("com.unity.shadergraph@"))
				{
					shaderGraphPackage = directoryInfo;
					break;
				}
				else
				{
					// Debug.Log(directoryInfo.Name);
				}
			}

			Assert.IsNotNull(shaderGraphPackage);

			string filePath = shaderGraphPackage.FullName + "/Editor/Drawing/Views/GraphEditorView.cs";
			FileInfo file = new FileInfo(filePath);

			Assert.IsTrue(file.Exists);
			
			var defines =
				PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings
					.selectedBuildTargetGroup);

			string text = File.ReadAllText(filePath);
			string[] lines = File.ReadAllLines(filePath);
			if (text.Contains(pre))
			{
				Run();
			}
			else
			{
				var removed = defines.Replace("SGE_INSTALLED", "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
					removed);
				
				Install();
			}
			
			
			if(!defines.Contains("SGE_INSTALLED"))
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
					defines + ";SGE_INSTALLED");
			
			EditorApplication.wantsToQuit += () =>
			{
				var defines =
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings
						.selectedBuildTargetGroup);
				var removed = defines.Replace("SGE_INSTALLED", "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
					removed);

				return true;
			};

			void Install()
			{
				StringBuilder sb = new StringBuilder(text.Length + 256);
				sb.AppendLine("#define SG_HOOKED");
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].StartsWith("using")) { }
				}

				text = text.Replace("namespace UnityEditor.ShaderGraph.Drawing",
							$"{pre}\n\nnamespace UnityEditor.ShaderGraph.Drawing")
						.Replace("MaterialGraphView m_GraphView;",
							"public static Action<MaterialGraphView, GraphData> installedCallback;\n\t\t\t\tMaterialGraphView m_GraphView;")
						.Replace("m_InspectorView.InitializeGraphSettings();",
							"m_InspectorView.InitializeGraphSettings();\n\t\t\t\tinstalledCallback?.Invoke(graphView, graph);")
					;
				File.WriteAllText(filePath, text);

				var defines =
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings
						.selectedBuildTargetGroup);
				
				if(!defines.Contains("SGE_INSTALLED"))
					PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
					defines + ";SGE_INSTALLED");
			}

			void Run()
			{
#if SGE_INSTALLED
				UnityEditor.ShaderGraph.Drawing.GraphEditorView.installedCallback += (v, g) =>
				{
					KeyboardShortcutHelper keyboardShortcutHelper = new KeyboardShortcutHelper(v, g);
				};
#endif
			}
		}

		[MenuItem("Tools/ShaderGraph Key2Node/Force Resolve")]
		static void ForceInstall()
		{
			bool yes = EditorUtility.DisplayDialog("Force Resolve", "Resolve ShaderGraph Enhancer",
				"Resolve", "Cancel");

			if (!yes) return;
			
			string packageCachePath = Application.dataPath.Replace("/Assets", "/Library/PackageCache");
			DirectoryInfo packageCache = new DirectoryInfo(packageCachePath);

			DirectoryInfo shaderGraphPackage = null;
			foreach (DirectoryInfo directoryInfo in packageCache.GetDirectories())
			{
				if (directoryInfo.Name.Contains("com.unity.shadergraph@"))
				{
					shaderGraphPackage = directoryInfo;
					break;
				}
				else
				{
					Debug.Log(directoryInfo.Name);
				}
			}

			Assert.IsNotNull(shaderGraphPackage);

			string filePath = shaderGraphPackage.FullName + "/Editor/Drawing/Views/GraphEditorView.cs";

			string text = File.ReadAllText(filePath);
			string[] lines = File.ReadAllLines(filePath);

			StringBuilder sb = new StringBuilder(text.Length + 256);
			sb.AppendLine("#define SG_HOOKED");
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith("using")) { }
			}

			text = text.Replace("namespace UnityEditor.ShaderGraph.Drawing",
						$"{pre}\n\nnamespace UnityEditor.ShaderGraph.Drawing")
					.Replace("MaterialGraphView m_GraphView;",
						"public static Action<MaterialGraphView, GraphData> installedCallback;\n\t\t\t\tMaterialGraphView m_GraphView;")
					.Replace("m_InspectorView.InitializeGraphSettings();",
						"m_InspectorView.InitializeGraphSettings();\n\t\t\t\tinstalledCallback?.Invoke(graphView, graph);")
				;
			File.WriteAllText(filePath, text);

			var defines =
				PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings
					.selectedBuildTargetGroup);
			if(!defines.Contains("SGE_INSTALLED"))
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
				defines + ";SGE_INSTALLED");

			EditorApplication.wantsToQuit += () =>
			{
				var defines =
					PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings
						.selectedBuildTargetGroup);
				var removed = defines.Replace("SGE_INSTALLED", "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
					removed);

				return true;
			};
		}

		[MenuItem("Tools/ShaderGraph Key2Node/Clean Uninstall")]
		static void CleanRemove()
		{
			bool yes = EditorUtility.DisplayDialog("Uninstall", "Uninstalling ShaderGraph Enhancer package",
				"Uninstall", "Cancel");

			if (!yes) return;
			
			var defines =
				PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings
					.selectedBuildTargetGroup);
			var removed = defines.Replace("SGE_INSTALLED", "");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
				removed);
			UnityEditor.PackageManager.Client.Remove("com.unity.seonghwan.shadergraph-enhancer");
		}
	}
}