using E_Clubs.Database;
using E_Clubs.Reports;
using E_Clubs.Reports.Repositories;
using E_Clubs.Users;
using E_Clubs.WorkPlans;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class ReportRepositoryTests
    {
        // Helper to generate a completely isolated database for every test run
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        #region GetReportsByClubIdAsync Tests

        [Fact]
        public async Task GetReportsByClubIdAsync_ReportsExist_ReturnsFilteredListWithIncludes()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var repository = new ReportRepository(context);

            var targetClubId = Guid.NewGuid();
            var wrongClubId = Guid.NewGuid();

            // Define parent entities to avoid the EF core tracking trap
            var professor = new User { Id = Guid.NewGuid(), FirstName = "Charles" };
            var club = new E_Clubs.Clubs.Club { Id = targetClubId, Professor = professor };
            var workPlan = new WorkPlan { Id = Guid.NewGuid(), ClubId = targetClubId, Club = club };

            context.Users.Add(professor);
            context.Clubs.Add(club);
            context.WorkPlans.Add(workPlan);

            // Add reports attached to our target club and an irrelevant club
            context.Reports.AddRange(
                new Report
                {
                    Id = Guid.NewGuid(),
                    ClubId = targetClubId,
                    Club = club,
                    WorkPlan = workPlan,
                    Professor = professor
                },
                new Report
                {
                    Id = Guid.NewGuid(),
                    ClubId = targetClubId,
                    Club = club,
                    WorkPlan = workPlan,
                    Professor = professor
                },
                new Report
                {
                    Id = Guid.NewGuid(),
                    ClubId = wrongClubId // Should be ignored by the query filter
                }
            );
            await context.SaveChangesAsync();

            // Act
            var result = (await repository.GetReportsByClubIdAsync(targetClubId)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result.All(r => r.ClubId == targetClubId).Should().BeTrue();

            // Verify all navigation properties were loaded successfully via the .Include() clauses
            result.All(r => r.Club!.Id == targetClubId).Should().BeTrue();
            result.All(r => r.WorkPlan != null).Should().BeTrue();
            result.All(r => r.Professor!.FirstName == "Charles").Should().BeTrue();
        }

        [Fact]
        public async Task GetReportsByClubIdAsync_NoMatchingReports_ReturnsEmptyList()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var repository = new ReportRepository(context);

            // Act
            var result = await repository.GetReportsByClubIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region CreateReportAsync Tests

        [Fact]
        public async Task CreateReportAsync_SavesReportToDatabase()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var repository = new ReportRepository(context);

            var reportId = Guid.NewGuid();
            var report = new Report
            {
                Id = reportId,
                PresentCount = 10,
                AbsentCount = 2,
                Present = "Alice",
                Absent = "Bob"
            };

            // Act
            var result = await repository.CreateReportAsync(report);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(reportId);

            // Double check it's actually written inside the database context state
            var dbReport = await context.Reports.FindAsync(reportId);
            dbReport.Should().NotBeNull();
            dbReport!.PresentCount.Should().Be(10);
            dbReport.Absent.Should().Be("Bob");
        }

        #endregion
    }
}