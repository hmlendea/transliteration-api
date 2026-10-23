using System;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.NUnit;

using NuciAPI.Controllers;
using NuciAPI.Requests;
using NuciAPI.Responses;

using NUnit.Framework;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace TransliterationAPI.ArchitectureTests
{
    [TestFixture]
    public sealed class TransportPlacementTests
    {
        [TestCase(typeof(NuciApiController), "TransliterationAPI.API.Controllers")]
        [TestCase(typeof(NuciApiRequest), "TransliterationAPI.API.Requests")]
        [TestCase(typeof(NuciApiSuccessResponse), "TransliterationAPI.API.Responses")]
        public void GivenTransportTypes_WhenInspectingPlacement_ThenTheyResideAtTheApiBoundary(
            Type contractType,
            string expectedNamespace)
        {
            IObjectProvider<IType> transportTypes = Types().That()
                .ResideInAssembly(ProductionArchitecture.Assembly)
                .And().AreAssignableTo(contractType);

            Assert.That(
                transportTypes.GetObjects(ProductionArchitecture.Model),
                Is.Not.Empty,
                $"The {contractType.Name} selector must match production transport types.");

            Types().That().Are(transportTypes).Should().ResideInNamespace(expectedNamespace)
                .Because("controllers and transport contracts belong in their documented API namespaces")
                .Check(ProductionArchitecture.Model);
        }
    }
}