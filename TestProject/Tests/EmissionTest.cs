using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoalMineApi.Controllers;
using CoalMineApi.Entities;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace TestingProject
{
    public abstract class TestRegistration
    {
        protected readonly IConfiguration configuration;

        #region Seeding
        //…
        public TestRegistration(DbContextOptions<EmissionsDBContext> contextOptions)
        {
            ContextOptions = contextOptions;
        }

        protected DbContextOptions<EmissionsDBContext> ContextOptions { get; }
        #endregion
    }

    public class EmissionTest : TestRegistration
    {
        public EmissionTest() : base(new DbContextOptionsBuilder<EmissionsDBContext>()
            .UseNpgsql("Host=localhost;Database=emissions;Username=docker;Password=docker")
            .Options)
        {
        }

        [Fact]
        public void TestGetEmissions()
        {
            using (var context = new EmissionsDBContext(configuration))
            {
                // Arrange
                var controller = new EmissionsController(context);

                // Act
                var result = controller.GetEmissionsData();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewData.Model);
            }
        }
    }
}