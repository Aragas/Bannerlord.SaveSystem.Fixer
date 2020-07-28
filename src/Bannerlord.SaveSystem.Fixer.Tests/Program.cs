using Bannerlord.SaveSystem.Patches;

using HarmonyLib;

using System.Diagnostics;

using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Bannerlord.SaveSystem.Tests
{
    public static class Program
    {
        // Broke it at some point, needs fixing
        public static void Main(string[] args)
        {
            var harmony = new Harmony("org.aragas.bannerlord.savesystem.fix");

            harmony.Patch(LoadContextPatch.GetObjectWithIdMethod,
                postfix: new HarmonyMethod(typeof(LoadContextPatch), nameof(LoadContextPatch.GetObjectWithIdPostfix)));

            harmony.Patch(VariableLoadDataPatch.GetDataToUseMethod,
                transpiler: new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(VariableLoadDataPatch.GetDataToUseTranspiler)));
            harmony.Patch(VariableLoadDataPatch.ReadMethod,
                transpiler: new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(VariableLoadDataPatch.ReadTranspiler)));

            harmony.Patch(DefinitionContextPatch.TryGetTypeDefinitionMethod,
                transpiler: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.TryGetTypeDefinitionTranspiler)));
            harmony.Patch(DefinitionContextPatch.GetTypeDefinitionMethod,
                postfix: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetTypeDefinitionPostfix)));
            harmony.Patch(DefinitionContextPatch.GetClassDefinitionMethod,
                postfix: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetClassDefinitionPostfix)));
            harmony.Patch(DefinitionContextPatch.GetStructDefinitionMethod,
                prefix: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetStructDefinitionPrefix)));


            var saveName = "save_0021";

            //string fileName = FilePaths.SavePath + saveName + ".sav";
            var fileName = "D:\\GitHub\\SaveFixer\\" + saveName + ".sav";
            ISaveDriver saveDriver = new FileDriver(fileName);

            var applicationVersion = saveDriver.LoadMetaData().GetApplicationVersion();
            if (applicationVersion.Major <= 1 && applicationVersion.Minor <= 4 && applicationVersion.Revision < 2)
                saveDriver = new OldFileDriver(fileName);

            var loadResult = SaveManager.Load(saveDriver);

            Debug.Assert(loadResult.Successful);
        }
    }
}