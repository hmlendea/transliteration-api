using System;
using System.Text.RegularExpressions;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.NUnit;

using NuciAPI.Controllers;
using NuciAPI.Requests;
using NuciAPI.Responses;

using NUnit.Framework;

using TransliterationAPI.Service;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace TransliterationAPI.ArchitectureTests
{
    [TestFixture]
    public sealed class AbstractionBoundaryTests
    {
        [TestCase(typeof(ITransliterator), typeof(ITransliterationService))]
        [TestCase(typeof(ITransliterator), typeof(TransliterationService))]
        [TestCase(typeof(ITransliterator), typeof(ITransliteratorFactory))]
        [TestCase(typeof(ITransliterator), typeof(TransliteratorFactory))]
        [TestCase(typeof(ITransliterator), typeof(CachedTransliteration))]
        [TestCase(typeof(ITransliterator), typeof(IHttpRequestManager))]
        [TestCase(typeof(ITransliterator), typeof(HttpRequestManager))]
        [TestCase(typeof(IExternalTransliterator), typeof(ITransliterationService))]
        [TestCase(typeof(IExternalTransliterator), typeof(TransliterationService))]
        [TestCase(typeof(IExternalTransliterator), typeof(ITransliteratorFactory))]
        [TestCase(typeof(IExternalTransliterator), typeof(TransliteratorFactory))]
        [TestCase(typeof(IExternalTransliterator), typeof(CachedTransliteration))]
        [TestCase(typeof(IExternalTransliterator), typeof(HttpRequestManager))]
        [TestCase(typeof(TransliterationService), typeof(TransliteratorFactory))]
        [TestCase(typeof(TransliterationService), typeof(Transliterator))]
        [TestCase(typeof(TransliterationService), typeof(ExternalTransliterator))]
        [TestCase(typeof(TransliterationService), typeof(IHttpRequestManager))]
        [TestCase(typeof(TransliterationService), typeof(HttpRequestManager))]
        [TestCase(typeof(NuciApiController), typeof(IHttpRequestManager))]
        [TestCase(typeof(NuciApiRequest), typeof(ITransliterationService))]
        [TestCase(typeof(NuciApiRequest), typeof(IHttpRequestManager))]
        [TestCase(typeof(NuciApiSuccessResponse), typeof(ITransliterationService))]
        [TestCase(typeof(NuciApiSuccessResponse), typeof(IHttpRequestManager))]
        public void GivenAnAbstractionBoundary_WhenInspectingDependencies_ThenBypassesAreProhibited(
            Type sourceContract,
            Type forbiddenContract)
        {
            IObjectProvider<IType> sourceTypes = BuildProductionTypes(sourceContract);

            Assert.That(
                sourceTypes.GetObjects(ProductionArchitecture.Model),
                Is.Not.Empty,
                $"The {sourceContract.Name} boundary must select production types.");

            Types().That().Are(sourceTypes).Should().NotDependOnAny(
                Types().That().AreAssignableTo(forbiddenContract))
                .Because($"{sourceContract.Name} must not bypass its boundary through {forbiddenContract.Name}")
                .Check(ProductionArchitecture.Model);
        }

        [TestCase(typeof(IExternalTransliterator), "System.IO")]
        [TestCase(typeof(IExternalTransliterator), "NuciDAL")]
        [TestCase(typeof(ITransliterator), "Microsoft.Extensions.DependencyInjection")]
        [TestCase(typeof(IExternalTransliterator), "Microsoft.Extensions.DependencyInjection")]
        [TestCase(typeof(TransliterationService), "Microsoft.Extensions.DependencyInjection")]
        public void GivenAlgorithmsAndOrchestration_WhenInspectingDependencies_ThenInfrastructureIsIsolated(
            Type sourceContract,
            string forbiddenNamespace)
        {
            IObjectProvider<IType> sourceTypes = BuildProductionTypes(sourceContract);
            string namespacePattern = $"^{Regex.Escape(forbiddenNamespace)}(\\.|$)";

            Assert.That(
                sourceTypes.GetObjects(ProductionArchitecture.Model),
                Is.Not.Empty,
                $"The {sourceContract.Name} boundary must select production types.");

            Types().That().Are(sourceTypes).Should().NotDependOnAny(
                Types().That().ResideInNamespaceMatching(namespacePattern))
                .Because($"{sourceContract.Name} must remain independent of {forbiddenNamespace}")
                .Check(ProductionArchitecture.Model);
        }

        [TestCase("TransliterationAPI.Service")]
        [TestCase("TransliterationAPI.Configuration")]
        [TestCase("TransliterationAPI.API")]
        public void GivenApplicationAreas_WhenInspectingDependencies_ThenHostCompositionIsNotUsed(
            string sourceNamespace)
        {
            string namespacePattern = $"^{Regex.Escape(sourceNamespace)}(\\.|$)";
            IObjectProvider<IType> sourceTypes = Types().That()
                .ResideInAssembly(ProductionArchitecture.Assembly)
                .And().ResideInNamespaceMatching(namespacePattern);

            Assert.That(
                sourceTypes.GetObjects(ProductionArchitecture.Model),
                Is.Not.Empty,
                $"The {sourceNamespace} boundary must select production types.");

            Types().That().Are(sourceTypes).Should().NotDependOnAny(
                Types().That().ResideInAssembly(ProductionArchitecture.Assembly)
                    .And().ResideInNamespace("TransliterationAPI"))
                .Because("host construction and dependency registration belong only to the composition root")
                .Check(ProductionArchitecture.Model);
        }

        private static IObjectProvider<IType> BuildProductionTypes(Type contractType) =>
            Types().That().ResideInAssembly(ProductionArchitecture.Assembly)
                .And().AreAssignableTo(contractType);
    }
}
