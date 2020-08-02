using HarmonyLib;

using System.Reflection;

using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Extensions
{
    public static class DefinitionContextExtensions
    {
        private static MethodInfo Method { get; } = AccessTools.DeclaredMethod(typeof(TSSD.DefinitionContext), "TryGetTypeDefinition");
        public static TSSD.TypeDefinitionBase TryGetTypeDefinition(this TSSD.DefinitionContext context, TSSD.SaveId saveId) => 
            (TSSD.TypeDefinitionBase) Method.Invoke(context, new object[] { saveId } );
    }
}