using E_Clubs.Attendances;
using E_Clubs.Attendances.DTO;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class AttendancesControllerTests
    {
        private readonly IAttendanceService _service;
        private readonly AttendancesController _controller;

        public AttendancesControllerTests()
        {
            _service = Substitute.For<IAttendanceService>();
            _controller = new AttendancesController(_service)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Path = "/api/test-path" }
                    }
                }
            };
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ServiceReturnsAttendances_ReturnsOkWithData()
        {
            var clubId = Guid.NewGuid();
            var expectedData = new List<GetAttendanceResponse>();
            _service.GetAllAttendancesByClubIdAsync(clubId).Returns(expectedData);

            var result = await _controller.GetAll(clubId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetAll_ServiceReturnsClubNotFound_ReturnsNotFound()
        {
            var clubId = Guid.NewGuid();
            _service.GetAllAttendancesByClubIdAsync(clubId).Returns(new ClubNotFound());
            var result = await _controller.GetAll(clubId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.Value.Should().Be("Club not found");
        }

        #endregion

        #region Get (User) Tests

        [Fact]
        public async Task Get_ServiceReturnsData_ReturnsOk()
        {
            var clubId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var expectedData = new GetAttendanceResponse
            {
                Student = new GetUserResponse { Id = userId, FirstName = "Test", LastName = "User" },
                AttendanceHistory = new List<AttendanceHistoryResponse>()
            };

            _service.GetUserAttendanceByClubIdAsync(clubId, userId).Returns(expectedData);
            var result = await _controller.Get(clubId, userId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // Note: ActionResult<T> requires .Result
            okResult.Value.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task Get_ServiceReturnsClubNotFound_ReturnsNotFound()
        {
            var clubId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _service.GetUserAttendanceByClubIdAsync(clubId, userId).Returns(new ClubNotFound());

            var result = await _controller.Get(clubId, userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            notFoundResult.Value.Should().Be("Club not found");
        }

        [Fact]
        public async Task Get_ServiceReturnsUserNotFound_ReturnsNotFound()
        {
            var clubId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _service.GetUserAttendanceByClubIdAsync(clubId, userId).Returns(new UserNotFound());

            var result = await _controller.Get(clubId, userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            notFoundResult.Value.Should().Be("User not found");
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_ServiceReturnsSuccess_ReturnsNoContent()
        {
            var clubId = Guid.NewGuid();
            var request = new RegisterAttendanceRequest { StudentId = Guid.NewGuid() };
            _service.RegisterAttendanceAsync(clubId, request).Returns(new Success());

            var result = await _controller.Register(clubId, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Register_ServiceReturnsClubNotFound_ReturnsProblemDetails()
        {
            var clubId = Guid.NewGuid();
            var request = new RegisterAttendanceRequest { StudentId = Guid.NewGuid() };
            _service.RegisterAttendanceAsync(clubId, request).Returns(new ClubNotFound());

            var result = await _controller.Register(clubId, request);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);

            problemDetails.Title.Should().Be("Not Found");
            problemDetails.Detail.Should().Contain(clubId.ToString());
            problemDetails.Instance.Should().Be("/api/test-path"); // Matches our fake HttpContext!
        }

        [Fact]
        public async Task Register_ServiceReturnsUserNotFound_ReturnsProblemDetails()
        {
            var clubId = Guid.NewGuid();
            var request = new RegisterAttendanceRequest { StudentId = Guid.NewGuid() };
            _service.RegisterAttendanceAsync(clubId, request).Returns(new UserNotFound());

            var result = await _controller.Register(clubId, request);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);

            problemDetails.Title.Should().Be("Not Found");
            problemDetails.Detail.Should().Contain(request.StudentId.ToString());
        }

        #endregion
    }
}