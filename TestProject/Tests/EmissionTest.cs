using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoalMineApi.Controllers;
using CoalMineApi.Entities;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TestingProject
{
    public class EmissionTest : IClassFixture<EmissionsDbContextFixture>
    {
        private readonly EmissionsDBContext _context;
        private readonly EmissionsController _controller;

        public EmissionTest(EmissionsDbContextFixture fixture)
        {
            _context = fixture.Context;
            _controller = new EmissionsController(_context);
        }

        [Fact]
        public void GetEmissionsData_ReturnsCorrectData()
        {
            // Act
            var result = _controller.GetEmissionsData();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var emissions = Assert.IsType<List<EmissionsController.EmissionDto>>(okResult.Value);
            Assert.Equal(2, emissions.Count);
        }

        [Fact]
        public void GetEmissionsLayer_ReturnsGeoJson()
        {
            // Act
            var result = _controller.GetEmissionsLayer();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var geoJson = Assert.IsType<string>(okResult.Value);
            Assert.Contains("\"type\":\"FeatureCollection\"", geoJson);
        }
    }
}