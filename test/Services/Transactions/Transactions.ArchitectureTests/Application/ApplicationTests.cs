
using BuildingBlocks.CQRS;
using FluentAssertions;
using FluentValidation;
using NetArchTest.Rules;
using Transactions.ArchitectureTests.Infrastructure;

namespace Transactions.ArchitectureTests.Application;

public class ApplicationTests : BaseTest
{
    [Fact]
    public void CommandHandler_ShouldHave_NameEndWith_CommandHandler()
    {
        var results = Types.InAssembly(ApplicationAssembly).That()
            .ImplementInterface(typeof(ICommandHandler<>)).Should().HaveNameEndingWith("CommandHandler").GetResult();
        results.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void QueryHandler_ShouldHave_NameEndWith_QueryHandler()
    {
        var results = Types.InAssembly(ApplicationAssembly).That()
            .ImplementInterface(typeof(IQueryHandler<,>)).Should().HaveNameEndingWith("QueryHandler").GetResult();
        results.IsSuccessful.Should().BeTrue();
    }
    [Fact]
    public void Validator_ShouldHave_NameEndWith_CommandValidator()
    {
        var results = Types.InAssembly(ApplicationAssembly).That()
            .Inherit(typeof(AbstractValidator<>)).Should().HaveNameEndingWith("CommandValidator").GetResult();
        results.IsSuccessful.Should().BeTrue();
    }
}