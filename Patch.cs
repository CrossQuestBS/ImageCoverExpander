using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Patch.ImageCoverExpander
{
    public class ImageCoverExpanderPatch : IPostBuildPlayerScriptDLLs
    {
        public int callbackOrder => 0;

        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            Debug.Log("ImageCoverExpander: Post-processing build...");
            var assemblyPath = report.GetFiles();

            var imageCoverModDll = assemblyPath.FirstOrDefault(t => t.path.Contains("Mod.ImageCoverExpander.dll"));
            var mainDll = assemblyPath.FirstOrDefault(t => t.path.Contains("Main.dll"));
        
            Patch(mainDll.path, imageCoverModDll.path);
        }

        private static void Patch(string mainAssemblyPath, string modAssemblyPath)
        {
            try
            {
                if (!File.Exists(mainAssemblyPath))
                {
                    Debug.LogError("Assembly file does not exist at path: " + mainAssemblyPath);
                    return;
                }
                var resolver = new DefaultAssemblyResolver();
                var assemblyParentPath = Directory.GetParent(mainAssemblyPath);

                if (assemblyParentPath is null)
                {
                    Debug.LogError("Failed to get parent path for: " + mainAssemblyPath);
                    return;
                }
                
                resolver.AddSearchDirectory(assemblyParentPath.FullName);
            
                using (var modAssembly = AssemblyDefinition.ReadAssembly(modAssemblyPath,
                           new ReaderParameters { ReadWrite = true, InMemory = true, AssemblyResolver = resolver }))
                {
                    using (var assembly = AssemblyDefinition.ReadAssembly(mainAssemblyPath, new ReaderParameters { ReadWrite = true, InMemory = true, AssemblyResolver = resolver}))
                    {
                        var modMethod = PatchUtils.GetMethod(modAssembly, "ArtworkManager", "UpdateArtwork");
        
                        if (modMethod is null)
                            return;
        
                        var patchType = PatchUtils.GetType(assembly, "StandardLevelDetailView");
        
                        if (patchType is null)
                            return;
        
                        var patchMethod = PatchUtils.GetMethod(patchType, "OnEnable");
        
                        if (patchMethod is null)
                            return;

                        var levelBar = PatchUtils.GetField(patchType, "_levelBar");
                    
                        if (levelBar is null)
                            return;
                    
                        var processor = patchMethod.Body.GetILProcessor();
                        var reference = assembly.MainModule.ImportReference(modMethod);

                        // Remove IL return 
                        processor.Remove(patchMethod.Body.Instructions.Last());
                    
                        // this._levelBar
                        processor.Emit(OpCodes.Ldarg_0);
                        processor.Emit(OpCodes.Ldfld, levelBar);
                    
                        // ArtworkManager.UpdateArtwork(this._levelBar)
                        processor.Emit(OpCodes.Call, reference);
                    
                        // Add IL return
                        processor.Emit(OpCodes.Ret);
                    
                        assembly.Write(mainAssemblyPath);
                    }
                }
                
                Debug.Log("ImageCoverExpander: Modification completed.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error modifying assembly: " + ex.Message);
            }
        }
    }
}
