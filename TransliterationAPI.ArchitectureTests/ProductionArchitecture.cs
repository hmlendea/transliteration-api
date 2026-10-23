using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

using Assembly = System.Reflection.Assembly;

namespace TransliterationAPI.ArchitectureTests
{
    internal static class ProductionArchitecture
    {
        internal static readonly Assembly Assembly = typeof(Program).Assembly;

        internal static readonly Architecture Model = new ArchLoader()
            .LoadAssemblies(Assembly)
            .Build();
    }
}