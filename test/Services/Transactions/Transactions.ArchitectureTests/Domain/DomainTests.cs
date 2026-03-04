using BuildingBlocks.Abstractions;
using NetArchTest.Rules;
using Transactions.ArchitectureTests.Infrastructure;
using FluentAssertions;
using System.Reflection;
namespace Transactions.ArchitectureTests.Domain;

public class DomainTests:BaseTest
{
    [Fact]
    public void DomainEvent_Should_Be_Sealed_Records()
    {
        TestResult results = Types.InAssembly(DomainAssembly).That().ImplementInterface(typeof(IDomainEvent)).Should().BeSealed().GetResult();
        results.IsSuccessful.Should().BeTrue();
    }
    [Fact]
   public void DomainEvent_ShouldHave_DomainEventPostfix()
    {
        TestResult results = Types.InAssembly(DomainAssembly).That().ImplementInterface(typeof(IDomainEvent)).Should().HaveNameEndingWith("DomainEvent").GetResult();
        results.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void Entities_Shouldhave_PrivatePrameterlessConstructor()
    {
       IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly).That().Inherit(typeof(IEntity)).GetTypes();
        var failingTypes= new List<Type>();
        foreach (var entityType in entityTypes) {
            var constructors = entityType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length > 0)
            {
                failingTypes.Add(entityType);
            }
        }
        failingTypes.Should().BeEmpty("Entities should not have public constructors");
    }

}
