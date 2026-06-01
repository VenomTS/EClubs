using E_Clubs.OneOfTypes;
using E_Clubs.Reports;
using E_Clubs.Reports.DTO;
using E_Clubs.Reports.QueryObject;
using E_Clubs.Reports.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class ReportsControllerTests
    {
        private readonly IReportService _service;
        private readonly ReportsController _controller;

        public ReportsControllerTests()
        {
            _service = Substitute.For<IReportService>();

            _controller = new ReportsController(_service)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Path = "/api/reports/test-path" }
                    }
                }
            };
        }

        #region GetByClubId Tests

        [Fact]
        public async Task GetByClubId_ClubExists_ReturnsOkWithData()
        {
            var request = new GetReportsQueryObject { ClubId = Guid.NewGuid() };
            var expectedReports = new List<GetReportsResponse>
            {
                new() {}
            };

            _service.GetReportsByClubIdAsync(request).Returns(expectedReports);

            var result = await _controller.GetByClubId(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expectedReports);
        }

        [Fact]
        public async Task GetByClubId_ClubNotFound_ReturnsNotFoundWithProblemDetails()
        {
            var request = new GetReportsQueryObject { ClubId = Guid.NewGuid() };

            // Adjust the instantiated error type if your match expects something like new ClubNotFound()
            _service.GetReportsByClubIdAsync(request).Returns(new ClubNotFound());

            var result = await _controller.GetByClubId(request);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);

            problemDetails.Title.Should().Be("Not Found");
            problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
            problemDetails.Detail.Should().Contain(request.ClubId.ToString());
            problemDetails.Instance.Should().Be("/api/reports/test-path");
        }

        #endregion
    }
}