using System;
using System.Text.RegularExpressions;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.NUnit;

using NUnit.Framework;

using TransliterationAPI.Service;
using TransliterationAPI.Service.Transliterators;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace TransliterationAPI.ArchitectureTests
{
    [TestFixture]
    public sealed class DependencyBoundaryTests
    {
        [TestCase("TransliterationAPI.API", "TransliterationAPI.Service.Transliterators")]
        [TestCase("TransliterationAPI.API", "NuciDAL")]
        [TestCase("TransliterationAPI.API", "System.Net")]
        [TestCase("TransliterationAPI.API", "Microsoft.EntityFrameworkCore")]
        [TestCase("TransliterationAPI.API", "Amazon.DynamoDBv2")]
        [TestCase("TransliterationAPI.Service", "TransliterationAPI.API")]
        [TestCase("TransliterationAPI.Service", "Microsoft.AspNetCore")]
        [TestCase("TransliterationAPI.Configuration", "TransliterationAPI.API")]
        [TestCase("TransliterationAPI.Service.Entities", "TransliterationAPI.API")]
        [TestCase("TransliterationAPI.API.Requests", "TransliterationAPI.Service.Transliterators")]
        [TestCase("TransliterationAPI.API.Responses", "NuciDAL")]
        public void GivenAnArchitecturalArea_WhenInspectingDependencies_ThenItsBoundaryIsRespected(
            string sourceNamespace,
            string forbiddenNamespace)
        {
            IObjectProvider<IType> sourceTypes = Types().That()
                .ResideInAssembly(ProductionArchitecture.Assembly)
                .And().ResideInNamespaceMatching(BuildNamespacePattern(sourceNamespace));

            Assert.That(
                sourceTypes.GetObjects(ProductionArchitecture.Model),
                Is.Not.Empty,
                $"The {sourceNamespace} boundary must select production types.");

            Types().That().Are(sourceTypes).Should().NotDependOnAny(
                Types().That().ResideInNamespaceMatching(BuildNamespacePattern(forbiddenNamespace)))
                .Because($"{sourceNamespace} must remain independent of {forbiddenNamespace}")
                .Check(ProductionArchitecture.Model);
        }

        [TestCase("TransliterationAPI.Service.TransliterationService")]
        [TestCase("TransliterationAPI.Service.HttpRequestManager")]
        public void GivenControllers_WhenInspectingDependencies_ThenConcreteServicesAreNotUsed(
            string implementationName)
        {
            IObjectProvider<IType> controllers = Types().That()
                .ResideInNamespace("TransliterationAPI.API.Controllers");

            Assert.That(controllers.GetObjects(ProductionArchitecture.Model), Is.Not.Empty);

            Types().That().Are(controllers).Should().NotDependOnAny(
                Types().That().HaveFullName(implementationName))
                .Because("controllers must delegate through application interfaces")
                .Check(ProductionArchitecture.Model);
        }

        [TestCase("System.Net")]
        [TestCase("System.IO")]
        [TestCase("NuciDAL")]
        public void GivenLocalTransliterators_WhenInspectingDependencies_ThenTheyRemainFreeOfIo(
            string forbiddenNamespace)
        {
            IObjectProvider<IType> localTransliterators = Types().That()
                .ResideInAssembly(ProductionArchitecture.Assembly)
                .And().AreAssignableTo(typeof(Transliterator));

            Assert.That(localTransliterators.GetObjects(ProductionArchitecture.Model), Is.Not.Empty);

            Types().That().Are(localTransliterators).Should().NotDependOnAny(
                Types().That().ResideInNamespaceMatching(BuildNamespacePattern(forbiddenNamespace)))
                .Because("local algorithms must not perform HTTP or cache persistence")
                .Check(ProductionArchitecture.Model);
        }

        [TestCase(typeof(TransliterationService))]
        [TestCase(typeof(ExternalTransliterator))]
        public void GivenOrchestrationAndProviders_WhenInspectingDependencies_ThenHttpUsesThePort(
            Type sourceContract)
        {
            IObjectProvider<IType> sourceTypes = Types().That()
                .ResideInAssembly(ProductionArchitecture.Assembly)
                .And().AreAssignableTo(sourceContract);

            Assert.That(sourceTypes.GetObjects(ProductionArchitecture.Model), Is.Not.Empty);

            Types().That().Are(sourceTypes).Should().NotDependOnAny(
                Types().That().ResideInNamespaceMatching(BuildNamespacePattern("System.Net")))
                .Because("provider transport belongs behind IHttpRequestManager")
                .Check(ProductionArchitecture.Model);
        }

        private static string BuildNamespacePattern(string namespaceName)
            => $"^{Regex.Escape(namespaceName)}(\\.|$)";
    }
}