using System.Reflection;

namespace Bannerlord.SaveSystem.Tests.Extensions
{
    public static class MethodInfoExtensions
    {
        public static bool IsOverridden(this MethodInfo methodInfo) => methodInfo.GetBaseDefinition() != methodInfo;
    }
}