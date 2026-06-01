using E_Clubs.Attendances;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Database;
using E_Clubs.Users;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace EClubs.Unit_Tests
{
    public class AttendanceRepositoryTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly AttendanceRepository _repository;
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        public AttendanceRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _repository = new AttendanceRepository(_dbContext);
        }

        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAttendancesByClubId_ShouldReturnOnlyAttendancesForThatClub()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new AttendanceRepository(context);

            var targetClubId = Guid.NewGuid();
            var targetClub = new E_Clubs.Clubs.Club { Id = targetClubId, Professor = CreateDummyUser() };

            var student = new User { Id = Guid.NewGuid() };
            context.Users.Add(student);

            context.Attendances.AddRange(
                new Attendance { Id = Guid.NewGuid(), ClubId = targetClubId, Club = targetClub, StudentId = student.Id , Student = CreateDummyUser()},
                new Attendance { Id = Guid.NewGuid(), ClubId = targetClubId, Club = targetClub, StudentId = student.Id , Student = CreateDummyUser() }
            );
            await context.SaveChangesAsync();

            var result = await repository.GetAttendancesByClubId(targetClubId);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task RegisterAttendance_ShouldAddAttendanceToDatabase()
        {
            var newAttendance = new Attendance
            {
                Id = Guid.NewGuid(),
                ClubId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Now),
                Student = CreateDummyUser(),
                Club = new E_Clubs.Clubs.Club { Professor= CreateDummyUser() }
            };

            await _repository.RegisterAttendance(newAttendance);
            var savedAttendance = await _dbContext.Attendances.FindAsync(newAttendance.Id);
            Assert.NotNull(savedAttendance);
            Assert.Equal(newAttendance.ClubId, savedAttendance.ClubId);
        }

        [Fact]
        public async Task GetAttendancesByClubIdByDate_ShouldReturnMatchingClubAndDate()
        {
            await using var context = GetInMemoryDbContext();
            var targetClubId = Guid.NewGuid();
            var targetDate = new DateOnly(2026, 5, 31);
            var wrongDate = new DateOnly(2026, 6, 1);
            var targetClub = new E_Clubs.Clubs.Club { Id = targetClubId, Professor = CreateDummyUser() };
            var repository = new AttendanceRepository(context);

            var expectedStudent = new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" };

            var attendances = new List<Attendance>
            {
                new() { Id = Guid.NewGuid(), Date = targetDate, Student = expectedStudent, Club = targetClub  }, // Match
                new() { Id = Guid.NewGuid(), Date = wrongDate, Student = CreateDummyUser(),Club = targetClub  }, // Wrong date
                new() { Id = Guid.NewGuid(), Date = targetDate, Student = CreateDummyUser(), Club = new E_Clubs.Clubs.Club{ Professor = CreateDummyUser() }  }  // Wrong club
            };

            await _dbContext.Attendances.AddRangeAsync(attendances);
            await _dbContext.SaveChangesAsync();
            var result = await _repository.GetAttendancesByClubIdByDate(targetClubId, targetDate);
            Assert.Single(result);
            Assert.Equal(targetClubId, result[0].ClubId);
            Assert.Equal(targetDate, result[0].Date);
            Assert.NotNull(result[0].Student);
        }
    }
}