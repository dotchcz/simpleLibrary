using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Microsoft.Extensions.DependencyInjection;
using SimpleLibrary.BusinessLogic.Mapper;
using FluentAssertions;
using Xunit;

namespace SimpleLibrary.BusinessLogic.Tests;

public class SimpleLibraryProfileMapperTest(Fixture fixture): IClassFixture<Fixture>
{
    private readonly IServiceProvider _serviceProvider = fixture.ServiceProvider;
    
    [Fact]
    public void MappingConfigurationIsValid()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IMapper>();
        // Act
        var act = () => mapper.ConfigurationProvider.AssertConfigurationIsValid();
        // Assert
        act.Should().NotThrow();
    }
    
    [Fact]
    public void VerifyProfile()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.EnableEnumMappingValidation();

            cfg.AddProfile<SimpleLibraryProfileMapper>();
        });

        configuration.CompileMappings();

        // Act
        var act = () => configuration.AssertConfigurationIsValid();

        // Assert
        act.Should().NotThrow();
    }
}