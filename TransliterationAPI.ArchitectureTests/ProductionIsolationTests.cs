using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using NUnit.Framework;

namespace TransliterationAPI.ArchitectureTests
{
    [TestFixture]
    public sealed class ProductionIsolationTests
    {
        [Test]
        public void GivenProductionAssemblies_WhenInspectingReferences_ThenTestAssembliesAreExcluded()
        {
            AssemblyName[] references = ProductionArchitecture.Assembly.GetReferencedAssemblies();

            Assert.That(references, Is.Not.Empty);
            Assert.That(
                references.Where(reference => IsTestDependency(reference.Name ?? string.Empty)),
                Is.Empty,
                "The production assembly must not reference test projects or testing frameworks.");
        }

        [Test]
        public void GivenProductionAssemblies_WhenInspectingVisibility_ThenInternalsAreNotExposed()
            => Assert.That(
                ProductionArchitecture.Assembly.GetCustomAttributes<InternalsVisibleToAttribute>(),
                Is.Empty,
                "Production internals must not be exposed through InternalsVisibleTo.");

        [Test]
        public void GivenProductionProjects_WhenInspectingReferences_ThenTestDependenciesAreExcluded()
        {
            string[] projectPaths = Directory.GetFiles(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "ProductionProjects"),
                "*.csproj");

            Assert.That(projectPaths, Has.Length.EqualTo(1), "Load every production project.");

            foreach (string projectPath in projectPaths)
            {
                AssertProjectIsolation(projectPath);
            }
        }

        private static void AssertProjectIsolation(string projectPath)
        {
            XDocument project = XDocument.Load(projectPath);
            XElement[] references = project.Descendants()
                .Where(element => element.Name.LocalName is
                    "ProjectReference" or "PackageReference" or "Reference")
                .ToArray();

            Assert.That(references, Is.Not.Empty, $"{projectPath} must declare its dependencies.");
            Assert.That(
                references.Where(reference => IsTestDependency(
                    reference.Attribute("Include")?.Value ?? string.Empty)),
                Is.Empty,
                $"{projectPath} must not declare test project or framework dependencies.");
            Assert.That(
                project.Descendants().Where(IsInternalsVisibilityDeclaration),
                Is.Empty,
                $"{projectPath} must not declare InternalsVisibleTo.");
        }

        private static bool IsTestDependency(string dependencyName)
            => dependencyName.Contains("Tests", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("NUnit", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("xunit", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("Moq", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("TngTech.ArchUnitNET", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("ArchUnitNET", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("Microsoft.NET.Test.Sdk", StringComparison.OrdinalIgnoreCase) ||
                dependencyName.StartsWith("Microsoft.TestPlatform", StringComparison.OrdinalIgnoreCase);

        private static bool IsInternalsVisibilityDeclaration(XElement element)
            => string.Equals(element.Name.LocalName, "InternalsVisibleTo", StringComparison.Ordinal) ||
                string.Equals(
                    element.Attribute("Include")?.Value,
                    typeof(InternalsVisibleToAttribute).FullName,
                    StringComparison.Ordinal);
    }
}