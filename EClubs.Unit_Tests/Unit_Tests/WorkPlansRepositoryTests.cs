using E_Clubs.Clubs;
using E_Clubs.Database;
using E_Clubs.Users;
using E_Clubs.WorkPlans;
using E_Clubs.WorkPlans.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static System.Reflection.Metadata.BlobBuilder;

namespace EClubs.Unit_Tests
{
    public class WorkPlansRepositoryTests
    {
        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        #region GetWorkPlansByClubIdAsync Tests

        [Fact]
        public async Task GetWorkPlansByClubIdAsync_WorkPlansExist_ReturnsFilteredAndOrderedList()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var targetClubId = Guid.NewGuid();
            var wrongClubId = Guid.NewGuid();

            var targetClub = new E_Clubs.Clubs.Club { Id = targetClubId, Professor = CreateDummyUser() };
            var wrongClub = new E_Clubs.Clubs.Club { Id = wrongClubId, Professor = CreateDummyUser() };
            context.Clubs.Add(targetClub);
            context.Clubs.Add(wrongClub);

            var plan2 = new WorkPlan
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ClubId = targetClubId,
                Club = targetClub,
                RealizationDate = null,
                Domain = "Domain B",
                Indicator = "Indicator B",
                Unit = "Unit B",
                LearningOutcome = "Outcome B"
            };
            var plan1 = new WorkPlan
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ClubId = targetClubId,
                Club = targetClub,
                RealizationDate = null,
                Domain = "Domain A",
                Indicator = "Indicator A",
                Unit = "Unit A",
                LearningOutcome = "Outcome A"
            };
            var planFromOtherClub = new WorkPlan
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                ClubId = wrongClubId,
                Club = wrongClub,
                RealizationDate = null,
                Domain = "Domain O",
                Indicator = "Indicator O",
                Unit = "Unit O",
                LearningOutcome = "Outcome O"
            };

            context.WorkPlans.AddRange(plan2, plan1, planFromOtherClub);
            await context.SaveChangesAsync();
            var result = await repository.GetWorkPlansByClubIdAsync(targetClubId);
            result.Should().HaveCount(2);
            result.All(wp => wp.ClubId == targetClubId).Should().BeTrue();
            result[0].Id.Should().Be(plan1.Id);
            result[1].Id.Should().Be(plan2.Id);
        }

        #endregion

        #region CreateWorkPlanAsync Tests

        [Fact]
        public async Task CreateWorkPlanAsync_ValidWorkPlan_SavesToDatabase()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var workPlan = new WorkPlan
            {
                Id = Guid.NewGuid(),
                ClubId = Guid.NewGuid(),
                Domain = "Data Architecture",
                Club = new Club { Professor = CreateDummyUser()}
            };

            var result = await repository.CreateWorkPlanAsync(workPlan);

            result.Should().NotBeNull();
            result.Id.Should().Be(workPlan.Id);

            var dbPlan = await context.WorkPlans.FindAsync(workPlan.Id);
            dbPlan.Should().NotBeNull();
            dbPlan!.Domain.Should().Be("Data Architecture");
        }

        #endregion

        #region RealizeWorkPlanAsync Tests

        [Fact]
        public async Task RealizeWorkPlanAsync_PlanExists_UpdatesRealizationDate()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var workPlanId = Guid.NewGuid();
            var workPlan = new WorkPlan { Id = workPlanId, RealizationDate = null, Club = new Club { Professor = CreateDummyUser() } };

            context.WorkPlans.Add(workPlan);
            await context.SaveChangesAsync();

            var realizationDate = new DateOnly(2026, 6, 1);

            await repository.RealizeWorkPlanAsync(workPlanId, realizationDate);

            var dbPlan = await context.WorkPlans.FindAsync(workPlanId);
            dbPlan.Should().NotBeNull();
            dbPlan!.RealizationDate.Should().Be(realizationDate);
        }

        [Fact]
        public async Task RealizeWorkPlanAsync_PlanDoesNotExist_ReturnsGracefullyWithoutCrashing()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);
            var nonExistentId = Guid.NewGuid();

            Func<Task> act = async () => await repository.RealizeWorkPlanAsync(nonExistentId, new DateOnly(2026, 6, 1));
            await act.Should().NotThrowAsync();
        }

        #endregion

        #region GetCurrentWorkPlanByClubIdAsync Tests

        [Fact]
        public async Task GetCurrentWorkPlanByClubIdAsync_MultiplePlans_ReturnsFirstUnrealizedPlanById()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var clubId = Guid.NewGuid();
            var club = new E_Clubs.Clubs.Club { Id = clubId, Professor = CreateDummyUser() };
            context.Clubs.Add(club);
            var alreadyRealizedPlan = new WorkPlan
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ClubId = clubId,
                Club = club,
                RealizationDate = new DateOnly(2026, 5, 20),
                Domain = "Domain 1",
                Indicator = "Indicator 1",
                Unit = "Unit 1",
                LearningOutcome = "Outcome 1"
            };
            var firstCurrentPlan = new WorkPlan
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ClubId = clubId,
                Club = club,
                RealizationDate = null,
                Domain = "Domain 2",
                Indicator = "Indicator 2",
                Unit = "Unit 2",
                LearningOutcome = "Outcome 2"
            };
            var secondCurrentPlan = new WorkPlan
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                ClubId = clubId,
                Club = club,
                RealizationDate = null,
                Domain = "Domain 3",
                Indicator = "Indicator 3",
                Unit = "Unit 3",
                LearningOutcome = "Outcome 3"
            };

            context.WorkPlans.AddRange(alreadyRealizedPlan, secondCurrentPlan, firstCurrentPlan);
            await context.SaveChangesAsync();
            var result = await repository.GetCurrentWorkPlanByClubIdAsync(clubId);
            result.Should().NotBeNull();
            result!.Id.Should().Be(firstCurrentPlan.Id);
        }

        [Fact]
        public async Task GetCurrentWorkPlanByClubIdAsync_AllPlansRealized_ReturnsNull()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var clubId = Guid.NewGuid();
            var realizedPlan = new WorkPlan { Id = Guid.NewGuid(), ClubId = clubId, RealizationDate = new DateOnly(2026, 5, 20), Club = new Club { Professor = CreateDummyUser() } };

            context.WorkPlans.Add(realizedPlan);
            await context.SaveChangesAsync();
            var result = await repository.GetCurrentWorkPlanByClubIdAsync(clubId);

            result.Should().BeNull();
        }

        #endregion

        #region WorkPlanExistsAsync Tests

        [Fact]
        public async Task WorkPlanExistsAsync_PlanExists_ReturnsTrue()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var targetId = Guid.NewGuid();
            context.WorkPlans.Add(new WorkPlan { Id = targetId, Club = new Club { Professor = CreateDummyUser() } });
            await context.SaveChangesAsync();
            var result = await repository.WorkPlanExistsAsync(targetId);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task WorkPlanExistsAsync_PlanDoesNotExist_ReturnsFalse()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);
            var result = await repository.WorkPlanExistsAsync(Guid.NewGuid());
            result.Should().BeFalse();
        }

        #endregion

        #region GetWorkPlanByIdAsync Tests

        [Fact]
        public async Task GetWorkPlanByIdAsync_PlanExists_ReturnsPlan()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);

            var targetId = Guid.NewGuid();
            var workPlan = new WorkPlan { Id = targetId, Domain = "Software Engineering", Club = new Club { Professor = CreateDummyUser() } };
            context.WorkPlans.Add(workPlan);
            await context.SaveChangesAsync();
            var result = await repository.GetWorkPlanByIdAsync(targetId);
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetId);
            result.Domain.Should().Be("Software Engineering");
        }

        [Fact]
        public async Task GetWorkPlanByIdAsync_PlanDoesNotExist_ReturnsNull()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new WorkPlansRepository(context);
            var result = await repository.GetWorkPlanByIdAsync(Guid.NewGuid());
            result.Should().BeNull();
        }

        #endregion
    }
}