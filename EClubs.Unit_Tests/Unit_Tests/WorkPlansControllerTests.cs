using AutoMapper;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Database;
using E_Clubs.Reports.Repositories;
using E_Clubs.Reports.Services;
using E_Clubs.Users;
using E_Clubs.Users.Repositories;
using E_Clubs.WorkPlans;
using E_Clubs.WorkPlans.DTO;
using E_Clubs.WorkPlans.Repositories;
using E_Clubs.WorkPlans.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class WorkPlansControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly WorkPlansService _service;
        private readonly WorkPlansController _controller;

        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };

        public WorkPlansControllerTests()
        {
            //Mock base:
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _mapper = Substitute.For<IMapper>();

            var workPlansRepo = new WorkPlansRepository(_context);
            var clubRepo = new ClubRepository(_context);
            var studentRepo = new ClubStudentRepository(_context);
            var attendanceRepo = new AttendanceRepository(_context);
            var reportRepo = new ReportRepository(_context);
            var userRepo = new UserRepository(_context);

            var reportService = new ReportService(
                _mapper,
                reportRepo,
                clubRepo,
                workPlansRepo,
                userRepo,
                attendanceRepo
            );

            _service = new WorkPlansService(
                _mapper,
                workPlansRepo,
                clubRepo,
                studentRepo,
                attendanceRepo,
                reportService
            );

            _controller = new WorkPlansController(_service)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Path = "/api/workplans-test-path" }
                    }
                }
            };
        }

        #region Get Tests

        [Fact]
        public async Task Get_ClubDoesNotExist_ReturnsNotFoundWithProblemDetails()
        {
            var nonExistentClubId = Guid.NewGuid();
            var result = await _controller.Get(nonExistentClubId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var problem = Assert.IsType<ProblemDetails>(notFoundResult.Value);
            problem.Detail.Should().Contain($"The club with ID {nonExistentClubId} was not found");
        }

        [Fact]
        public async Task Get_ClubExists_ReturnsOkWithWorkPlans()
        {
            var club = new E_Clubs.Clubs.Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };
            var workPlan = new WorkPlan { Id = Guid.NewGuid(), ClubId = club.Id, Club = club };

            _context.Clubs.Add(club);
            _context.WorkPlans.Add(workPlan);
            await _context.SaveChangesAsync();

            var expectedDto = new GetWorkPlanResponse
            {
                Id = workPlan.Id,
                Domain = "Test Domain",
                Indicator = "Test Indicator"
            };
            var expectedDtos = new List<GetWorkPlanResponse> { expectedDto };

            _mapper.Map<GetWorkPlanResponse>(Arg.Any<WorkPlan>()).Returns(expectedDto);
            _mapper.Map<IEnumerable<GetWorkPlanResponse>>(Arg.Any<IEnumerable<WorkPlan>>()).Returns(expectedDtos);
            var result = await _controller.Get(club.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualList = Assert.IsAssignableFrom<IEnumerable<GetWorkPlanResponse>>(okResult.Value);
            actualList.Should().NotContainNulls();
            actualList.First().Domain.Should().Be("Test Domain");
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ClubDoesNotExist_ReturnsNotFound()
        {
            var request = new CreateWorkPlanRequest();
            var result = await _controller.Create(Guid.NewGuid(), request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ClubExists_SavesToDbAndReturnsOk()
        {
            var club = new E_Clubs.Clubs.Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();

            var request = new CreateWorkPlanRequest {};
            var mappedWorkPlan = new WorkPlan { ClubId = club.Id, Club = club };
            var expectedDto = new GetWorkPlanResponse();

            _mapper.Map<WorkPlan>(request).Returns(mappedWorkPlan);
            _mapper.Map<GetWorkPlanResponse>(mappedWorkPlan).Returns(expectedDto);

            var result = await _controller.Create(club.Id, request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(expectedDto);
            var savedPlan = await _context.WorkPlans.AnyAsync(wp => wp.ClubId == club.Id);
            savedPlan.Should().BeTrue();
        }

        #endregion

        #region BatchCreate Tests

        [Fact]
        public async Task BatchCreate_ValidRequests_ExecutesSavesAndReturnsNoContent()
        {
            var club = new E_Clubs.Clubs.Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();

            var requests = new List<CreateWorkPlanRequest> { new(), new() };
            _mapper.Map<WorkPlan>(Arg.Any<CreateWorkPlanRequest>())
                   .Returns(x => new WorkPlan { Id = Guid.NewGuid(), ClubId = club.Id, Club = club });

            var result = await _controller.BatchCreate(club.Id, requests);

            Assert.IsType<NoContentResult>(result);

            var plansInDb = await _context.WorkPlans.CountAsync(wp => wp.ClubId == club.Id);
            plansInDb.Should().Be(2);
        }

        #endregion

        #region GetCurrentWorkPlan Tests

        [Fact]
        public async Task GetCurrentWorkPlan_ClubNotFound_ReturnsNotFound()
        {
            var result = await _controller.GetCurrentWorkPlan(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region GetDomains Tests

        [Fact]
        public async Task GetDomains_ClubDoesNotExist_ReturnsNotFound()
        {
            var result = await _controller.GetDomains(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}