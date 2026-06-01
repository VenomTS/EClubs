using AutoMapper;
using E_Clubs.Attendances;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Database;
using E_Clubs.Enums;
using E_Clubs.Reports;
using E_Clubs.Reports.DTO;
using E_Clubs.Reports.QueryObject;
using E_Clubs.Reports.Repositories;
using E_Clubs.Reports.Services;
using E_Clubs.Users;
using E_Clubs.Users.Repositories;
using E_Clubs.WorkPlans;
using E_Clubs.WorkPlans.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class ReportServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private readonly ReportRepository _reportRepo;
        private readonly ClubRepository _clubRepo;
        private readonly WorkPlansRepository _workPlansRepo;
        private readonly UserRepository _userRepo;
        private readonly AttendanceRepository _attendanceRepo;

        private readonly ReportService _service;

        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };

        public ReportServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mapper = Substitute.For<IMapper>();
            _reportRepo = new ReportRepository(_context);
            _clubRepo = new ClubRepository(_context);
            _workPlansRepo = new WorkPlansRepository(_context);
            _userRepo = new UserRepository(_context);
            _attendanceRepo = new AttendanceRepository(_context);
            _service = new ReportService(
                _mapper,
                _reportRepo,
                _clubRepo,
                _workPlansRepo,
                _userRepo,
                _attendanceRepo);
        }

        #region GetReportsByClubIdAsync Tests

        [Fact]
        public async Task GetReportsByClubIdAsync_ClubDoesNotExist_ReturnsClubNotFound()
        {
            var request = new GetReportsQueryObject { ClubId = Guid.NewGuid() };
            var result = await _service.GetReportsByClubIdAsync(request);
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task GetReportsByClubIdAsync_ClubExists_ReturnsMappedReports()
        {
            var clubId = Guid.NewGuid();
            var club = new E_Clubs.Clubs.Club { Id = clubId, Professor = CreateDummyUser() };

            var report = new Report { Id = Guid.NewGuid(), ClubId = clubId };

            _context.Clubs.Add(club);
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            var request = new GetReportsQueryObject { ClubId = clubId };

            var expectedDtoList = new List<GetReportsResponse>
            {
                new GetReportsResponse {}
            };

            _mapper.Map<IEnumerable<GetReportsResponse>>(Arg.Any<IEnumerable<Report>>())
                   .Returns(expectedDtoList);

            var result = await _service.GetReportsByClubIdAsync(request);
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().BeEquivalentTo(expectedDtoList);
        }

        #endregion

        #region CreateReportAsync Tests

        [Fact]
        public async Task CreateReportAsync_WorkPlanDoesNotExist_DoesNotCreateReport()
        {
            var workPlanId = Guid.NewGuid();
            await _service.CreateReportAsync(workPlanId);
            var reportsInDb = await _context.Reports.ToListAsync();
            reportsInDb.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateReportAsync_WorkPlanHasNoRealizationDate_DoesNotCreateReport()
        {
            var club = new E_Clubs.Clubs.Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };
            var workPlan = new WorkPlan
            {
                Id = Guid.NewGuid(),
                ClubId = club.Id,
                RealizationDate = null,
                Club = club
            };

            _context.Clubs.Add(club);
            _context.WorkPlans.Add(workPlan);
            await _context.SaveChangesAsync();

            await _service.CreateReportAsync(workPlan.Id);
            var reportsInDb = await _context.Reports.ToListAsync();
            reportsInDb.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateReportAsync_ValidData_CalculatesAttendanceAndSavesReport()
        {
            var realizationDate = new DateOnly(2026, 5, 15);

            var professor = new User { Id = Guid.NewGuid(), FirstName = "Prof", LastName = "X" };
            var student1 = new User { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith" };
            var student2 = new User { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Jones" };

            _context.Users.AddRange(professor, student1, student2);

            var club = new E_Clubs.Clubs.Club { Id = Guid.NewGuid(), ProfessorId = professor.Id, Professor = professor };
            var workPlan = new WorkPlan { Club = club, Id = Guid.NewGuid(), ClubId = club.Id, RealizationDate = realizationDate };

            _context.Clubs.Add(club);
            _context.WorkPlans.Add(workPlan);
            var attendance1 = new Attendance
            {
                Id = Guid.NewGuid(),
                ClubId = club.Id,
                Date = realizationDate,
                Status = AttendanceStatus.Present,
                StudentId = student1.Id,
                Student = student1,
                Club = club
            };
            var attendance2 = new Attendance
            {
                Id = Guid.NewGuid(),
                ClubId = club.Id,
                Date = realizationDate,
                Status = AttendanceStatus.Absent,
                StudentId = student2.Id,
                Student = student2,
                Club = club
            };

            _context.Attendances.AddRange(attendance1, attendance2);
            await _context.SaveChangesAsync();

            await _service.CreateReportAsync(workPlan.Id);

            var savedReport = await _context.Reports.FirstOrDefaultAsync(r => r.WorkPlanId == workPlan.Id);

            savedReport.Should().NotBeNull();
            savedReport!.ClubId.Should().Be(club.Id);
            savedReport.ProfessorId.Should().Be(professor.Id);
            savedReport.Date.Should().Be(realizationDate);

            savedReport.PresentCount.Should().Be(1);
            savedReport.AbsentCount.Should().Be(1);
            savedReport.Present.Should().Be("Alice Smith");
            savedReport.Absent.Should().Be("Bob Jones");
        }

        #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}