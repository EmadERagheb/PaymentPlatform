

using Transactions.ArchitectureTests.Infrastructure;

namespace Transactions.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Application_Or_Infrastructure()
    {
        var domainTypes = DomainAssembly.GetTypes();
        var applicationTypes = ApplicationAssembly.GetTypes();
        var infrastructureTypes = InfrastructureAssembly.GetTypes();
        foreach (var domainType in domainTypes)
        {
            foreach (var applicationType in applicationTypes)
            {
                Assert.False(domainType.IsAssignableFrom(applicationType), $"{domainType.FullName} should not depend on {applicationType.FullName}");
            }
            foreach (var infrastructureType in infrastructureTypes)
            {
                Assert.False(domainType.IsAssignableFrom(infrastructureType), $"{domainType.FullName} should not depend on {infrastructureType.FullName}");
            }
        }
    }
    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        var applicationTypes = ApplicationAssembly.GetTypes();
        var infrastructureTypes = InfrastructureAssembly.GetTypes();
        var infrastructureNamespace = "Transactions.Infrastructure";

        foreach (var applicationType in applicationTypes)
        {
            // Check if the application type has any direct dependencies on infrastructure types
            // by examining base types, implemented interfaces, fields, properties, method parameters, etc.
            
            // Check base type
            if (applicationType.BaseType != null && applicationType.BaseType.Namespace?.StartsWith(infrastructureNamespace) == true)
            {
                Assert.Fail($"{applicationType.FullName} should not inherit from {applicationType.BaseType.FullName} in Infrastructure layer");
            }

            // Check fields
            foreach (var field in applicationType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static))
            {
                if (field.FieldType.Namespace?.StartsWith(infrastructureNamespace) == true)
                {
                    Assert.Fail($"{applicationType.FullName} should not have field of type {field.FieldType.FullName} from Infrastructure layer");
                }
            }

            // Check properties
            foreach (var property in applicationType.GetProperties())
            {
                if (property.PropertyType.Namespace?.StartsWith(infrastructureNamespace) == true)
                {
                    Assert.Fail($"{applicationType.FullName} should not have property of type {property.PropertyType.FullName} from Infrastructure layer");
                }
            }

            // Check constructor parameters
            foreach (var constructor in applicationType.GetConstructors())
            {
                foreach (var parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType.Namespace?.StartsWith(infrastructureNamespace) == true)
                    {
                        Assert.Fail($"{applicationType.FullName} constructor should not depend on {parameter.ParameterType.FullName} from Infrastructure layer");
                    }
                }
            }

            // Check method parameters and return types
            foreach (var method in applicationType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly))
            {
                if (method.ReturnType.Namespace?.StartsWith(infrastructureNamespace) == true)
                {
                    Assert.Fail($"{applicationType.FullName}.{method.Name} should not return type {method.ReturnType.FullName} from Infrastructure layer");
                }
                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.ParameterType.Namespace?.StartsWith(infrastructureNamespace) == true)
                    {
                        Assert.Fail($"{applicationType.FullName}.{method.Name} should not have parameter of type {parameter.ParameterType.FullName} from Infrastructure layer");
                    }
                }
            }
        }
    }
    [Fact]
    public void Api_Should_Not_Depend_On_Domain_Or_Infrastructure()
    {
        var apiTypes = ApiAssembly.GetTypes();
        var domainTypes = DomainAssembly.GetTypes();
        var infrastructureTypes = InfrastructureAssembly.GetTypes();
        foreach (var apiType in apiTypes)
        {
            foreach (var domainType in domainTypes)
            {
                Assert.False(apiType.IsAssignableFrom(domainType), $"{apiType.FullName} should not depend on {domainType.FullName}");
            }
            foreach (var infrastructureType in infrastructureTypes)
            {
                Assert.False(apiType.IsAssignableFrom(infrastructureType), $"{apiType.FullName} should not depend on {infrastructureType.FullName}");
            }
        }

    }
}
