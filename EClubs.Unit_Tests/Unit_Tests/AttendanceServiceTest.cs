using AutoMapper;
using E_Clubs.Attendances;
using E_Clubs.Attendances.DTO;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Attendances.Services;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Users.Repositories;
using Xunit;
using FluentAssertions;
using NSubstitute;
using E_Clubs.Enums;
using E_Clubs.OneOfTypes;
using E_Clubs.Users.DTO;
using E_Clubs.WorkPlans.Repositories;
using OneOf.Types;
using E_Clubs.Users;
using E_Clubs.Clubs;

namespace EClubs.Unit_Tests
{
    public class AttendanceServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly AttendanceService _service;
        private readonly IClubRepository _clubRepo;
        private readonly IUserRepository _userRepo;

        public AttendanceServiceTests()
        {
            //Mock:
            _mapper = Substitute.For<IMapper>();
            _attendanceRepo = Substitute.For<IAttendanceRepository>();
            _clubRepo = Substitute.For<IClubRepository>();
            _userRepo = Substitute.For<IUserRepository>();

            //Insert fakes
            _service = new AttendanceService(_mapper, _attendanceRepo, _clubRepo, _userRepo);
        }

        #region GetAllAttendancesByClubIdAsync Tests

        [Fact]
        public async Task GetAllAttendancesByClubIdAsync_ClubDoesNotExist_ReturnsClubNotFound()
        {
            var clubId = Guid.NewGuid();
            _clubRepo.ClubExistsAsync(clubId).Returns(false);

            var result = await _service.GetAllAttendancesByClubIdAsync(clubId);

            result.IsT1.Should().BeTrue(); //T1 corresponds to ClubNotFound
        }

        [Fact]
        public async Task GetAllAttendancesByClubIdAsync_ClubExists_ReturnsMappedAttendanceList()
        {
            var clubId = Guid.NewGuid();
            var student = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var mockAttendances = new List<Attendance>
            {
                new()
                {
                    ClubId = clubId, Student = student, Date = DateOnly.FromDateTime(DateTime.Now), Status = AttendanceStatus.Present,
                    Club = new Club{Professor = new User {Id= Guid.NewGuid(), FirstName = "TestName", LastName = "TestSurname"} }
                }
            };
            var expectedResponse = new List<GetAttendanceResponse> {
                new()
            {
                Student = new GetUserResponse
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "User"
                },
                AttendanceHistory = new List<AttendanceHistoryResponse>()
            }
            };

            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _attendanceRepo.GetAttendancesByClubId(clubId).Returns(mockAttendances);

            _mapper.Map<List<GetAttendanceResponse>>(Arg.Any<object>()).Returns(expectedResponse);
            var result = await _service.GetAllAttendancesByClubIdAsync(clubId);
            result.IsT0.Should().BeTrue(); //T0 corresponds to List<GetAttendanceResponse>
            result.AsT0.Should().HaveCount(1);
        }

        #endregion

        #region GetUserAttendanceByClubIdAsync Tests

        [Fact]
        public async Task GetUserAttendanceByClubIdAsync_ClubDoesNotExist_ReturnsClubNotFound()
        {
            var clubId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _clubRepo.ClubExistsAsync(clubId).Returns(false);
            var result = await _service.GetUserAttendanceByClubIdAsync(clubId, userId);
            result.IsT1.Should().BeTrue(); //T1 corresponds to ClubNotFound
        }

        [Fact]
        public async Task GetUserAttendanceByClubIdAsync_UserDoesNotExist_ReturnsUserNotFound()
        {
            var clubId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _userRepo.GetUserByIdAsync(userId).Returns((User)null);
            var result = await _service.GetUserAttendanceByClubIdAsync(clubId, userId);
            result.IsT2.Should().BeTrue(); //T2 corresponds to UserNotFound
        }

        [Fact]
        public async Task GetUserAttendanceByClubIdAsync_ValidInputs_ReturnsAttendanceDto()
        {
            var clubId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var mockUser = new User { Id = userId, FirstName = "Jane", LastName = "Doe" };
            var mockAttendances = new List<Attendance>
            {
                new()
                {
                    ClubId = clubId,
                    StudentId = userId,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Status = AttendanceStatus.Absent,
                    Student = new User { Id = userId, FirstName = "Jane", LastName = "Doe" },
                    Club = new Club
                    {
                        Id = clubId,
                        Professor = new User { Id = Guid.NewGuid(), FirstName = "Prof", LastName = "Test" }
                    }
                }
            };

            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _userRepo.GetUserByIdAsync(userId).Returns(mockUser);
            _attendanceRepo.GetUserAttendancesByClubId(clubId, userId).Returns(mockAttendances);
            var result = await _service.GetUserAttendanceByClubIdAsync(clubId, userId);
            result.IsT0.Should().BeTrue(); //T0 corresponds to GetAttendanceResponse
            result.AsT0.Student.Id.Should().Be(userId);
            result.AsT0.AttendanceHistory.Should().HaveCount(1);
        }

        #endregion

        #region RegisterAttendanceAsync Tests

        [Fact]
        public async Task RegisterAttendanceAsync_ClubDoesNotExist_ReturnsClubNotFound()
        {
            var clubId = Guid.NewGuid();
            var request = new RegisterAttendanceRequest { StudentId = Guid.NewGuid() };
            _clubRepo.ClubExistsAsync(clubId).Returns(false);
            var result = await _service.RegisterAttendanceAsync(clubId, request);
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterAttendanceAsync_UserDoesNotExist_ReturnsUserNotFound()
        {
            var clubId = Guid.NewGuid();
            var request = new RegisterAttendanceRequest { StudentId = Guid.NewGuid() };
            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _userRepo.UserExistsAsync(request.StudentId).Returns(false);
            var result = await _service.RegisterAttendanceAsync(clubId, request);
            result.IsT2.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterAttendanceAsync_ValidRequest_SavesToRepoAndReturnsSuccess()
        {
            var clubId = Guid.NewGuid();
            var request = new RegisterAttendanceRequest { StudentId = Guid.NewGuid() };
            var mockAttendance = new Attendance {
                ClubId = clubId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Status = AttendanceStatus.Absent,
                Student = new User {FirstName = "Jane", LastName = "Doe" },
                Club = new Club
                {
                    Id = clubId,
                    Professor = new User { Id = Guid.NewGuid(), FirstName = "Prof", LastName = "Test" }
                }
            };

            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _userRepo.UserExistsAsync(request.StudentId).Returns(true);
            _mapper.Map<Attendance>(request).Returns(mockAttendance);
            var result = await _service.RegisterAttendanceAsync(clubId, request);
            result.IsT0.Should().BeTrue(); //T0 corresponds to Success
            await _attendanceRepo.Received(1).RegisterAttendance(Arg.Is<Attendance>(a => a.ClubId == clubId));
        }

        #endregion
    }
}