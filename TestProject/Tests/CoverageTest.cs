using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoalMineApi.Controllers;
using CoalMineApi.Entities;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using NetTopologySuite.Features;

namespace TestingProject
{
    public class CoverageTest : IClassFixture<EmissionsDbContextFixture>
    {
        private readonly EmissionsDBContext _context;
        private readonly CoveragesController _controller;

        public CoverageTest(EmissionsDbContextFixture fixture)
        {
            _context = fixture.Context;
            _controller = new CoveragesController(_context);
        }


        [Fact]
        public void GetCoveragesData_ReturnsCorrectData()
        {
            // Act
            var result = _controller.GetCoveragesData();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var coverages = Assert.IsType<List<CoveragesController.CoverageDTO>>(okResult.Value);
            Assert.Equal(2, coverages.Count);
        }

        [Fact]
        public void GetCoveragesLayer_ReturnsGeoJson()
        {
            // Act
            var result = _controller.GetCoveragesLayer();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var geoJson = Assert.IsType<string>(okResult.Value);
            Assert.Contains("\"type\":\"FeatureCollection\"", geoJson);
        }

        [Fact]
        public void PostCoverageData_ReturnsCreated()
        {
            // Arrange
            var coverage = new CoveragesController.CoveragePostBody
            {
                Name = "Test Coverage",
                Description = "Test Description",
                BufferSize = 10,
                Feature = "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[-104.05,48.99],[-97.22,48.98],[-96.58,45.94],[-104.03,45.94],[-104.05,48.99]]]},\"properties\":{}}"
            };

            // Act
            var result = _controller.PostCoverageData(coverage);

            // Assert
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
            var coverageResult = Assert.IsType<Coverage>(createdResult.Value);
            Assert.Equal(coverage.Name, coverageResult.Name);
            Assert.Equal(coverage.Description, coverageResult.Description);
            var coverageGeojson = new GeoJsonReader().Read<Feature>(coverage.Feature);
            Assert.Equal(coverageGeojson.Geometry.AsText(), coverageResult.Geometry.AsText()); // Compare WKT strings of geometries
        }

        [Fact]
        public void PostBulkCoverageData_ReturnsCreated()
        {
            // Arrange
            var coverage = new CoveragesController.CoverageBulkBody
            {
                Name = "Test MultiPolygon Coverage",
                Description = "Test Description",
                BufferSize = 10,
                FeatureCollection = "{\"type\":\"FeatureCollection\",\"features\":[{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[-104.05,48.99],[-97.22,48.98],[-96.58,45.94],[-104.03,45.94],[-104.05,48.99]]]},\"properties\":{}},{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[-100.05,40.99],[-90.22,40.98],[-90.58,35.94],[-100.03,35.94],[-100.05,40.99]]]},\"properties\":{}}]}"
            };

            // Act
            var result = _controller.PostCoverageBulkData(coverage);

            // Assert
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
            var coverageResult = Assert.IsType<Coverage>(createdResult.Value);
            Assert.Equal(coverage.Name, coverageResult.Name);
            Assert.Equal(coverage.Description, coverageResult.Description);
            Assert.IsType<MultiPolygon>(coverageResult.Geometry);
        }
    }
}