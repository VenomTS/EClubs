using AutoMapper;
using E_Clubs.Clubs;
using E_Clubs.Clubs.DTO;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Clubs.Services;
using E_Clubs.Users;
using E_Clubs.Users.Repositories;
using FluentAssertions;
using NSubstitute;
using OneOf.Types;
using System.Text;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class ClubServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IClubRepository _clubRepo;
        private readonly IClubStudentRepository _clubStudentRepo;
        private readonly IUserRepository _userRepo;
        private readonly ClubService _service;

        public ClubServiceTests()
        {
            _mapper = Substitute.For<IMapper>();
            _clubRepo = Substitute.For<IClubRepository>();
            _clubStudentRepo = Substitute.For<IClubStudentRepository>();
            _userRepo = Substitute.For<IUserRepository>();

            _service = new ClubService(_mapper, _clubRepo, _clubStudentRepo, _userRepo);
        }

        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };

        #region CreateClubAsync Tests

        [Fact]
        public async Task CreateClubAsync_ValidRequest_GeneratesCodeAndSaves()
        {
            var request = new CreateClubRequest();
            var clubModel = new Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };
            var expectedResponse = new GetClubResponse { Id = clubModel.Id };

            _mapper.Map<Club>(request).Returns(clubModel);
            _mapper.Map<GetClubResponse>(clubModel).Returns(expectedResponse);
            _clubRepo.CodeExists(Arg.Any<string>()).Returns(false);
            _clubRepo.CreateClubAsync(clubModel).Returns(clubModel);
            var result = await _service.CreateClubAsync(request);

            result.Should().BeEquivalentTo(expectedResponse);
            clubModel.IsActive.Should().BeTrue();
            clubModel.Code.Should().NotBeNullOrEmpty();
            clubModel.Code.Length.Should().Be(6);
        }

        #endregion

        #region GetStudentsByClubIdAsync Tests

        [Fact]
        public async Task GetStudentsByClubIdAsync_ClubDoesNotExist_ReturnsClubNotFound()
        {
            var clubId = Guid.NewGuid();
            _clubRepo.ClubExistsAsync(clubId).Returns(false);

            var result = await _service.GetStudentsByClubIdAsync(clubId);

            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task GetStudentsByClubIdAsync_ClubExists_ReturnsStudentList()
        {
            var clubId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _clubRepo.ClubExistsAsync(clubId).Returns(true);

            var clubStudents = new List<ClubStudent>
            {
                new()
                {
                    ClubId = clubId,
                    StudentId = studentId,
                    Student = CreateDummyUser(),
                    Club = new Club { Id = clubId, Professor = CreateDummyUser() }
                }
            };

            _clubStudentRepo.GetStudentsByClubIdAsync(clubId).Returns(clubStudents);
            var result = await _service.GetStudentsByClubIdAsync(clubId);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().HaveCount(1);
            result.AsT0.First().Id.Should().Be(studentId);
        }

        #endregion

        #region AddStudentToClubAsync Tests

        [Fact]
        public async Task AddStudentToClub_ClubNotFound_ReturnsClubNotFound()
        {
            var request = new JoinClubRequest { Code = "123456", StudentId = Guid.NewGuid() };
            _clubRepo.GetClubByCodeAsync(request.Code).Returns((Club)null!);

            var result = await _service.AddStudentToClubAsync(request);

            // Assert
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task AddStudentToClub_StudentNotFound_ReturnsStudentNotFound()
        {
            var request = new JoinClubRequest { Code = "123456", StudentId = Guid.NewGuid() };
            var club = new Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };

            _clubRepo.GetClubByCodeAsync(request.Code).Returns(club);
            _userRepo.UserExistsAsync(request.StudentId).Returns(false);

            var result = await _service.AddStudentToClubAsync(request);

            result.IsT2.Should().BeTrue();
        }

        [Fact]
        public async Task AddStudentToClub_ValidInputs_AddsStudentAndReturnsSuccess()
        {
            var request = new JoinClubRequest { Code = "123456", StudentId = Guid.NewGuid() };
            var club = new Club { Id = Guid.NewGuid(), Professor = CreateDummyUser() };

            _clubRepo.GetClubByCodeAsync(request.Code).Returns(club);
            _userRepo.UserExistsAsync(request.StudentId).Returns(true);
            _clubStudentRepo.IsStudentInClub(club.Id, request.StudentId).Returns(false);

            var result = await _service.AddStudentToClubAsync(request);

            result.IsT0.Should().BeTrue();
            await _clubStudentRepo.Received(1).AddStudentToClub(Arg.Is<ClubStudent>(cs => cs.ClubId == club.Id && cs.StudentId == request.StudentId));
        }

        #endregion

        #region DeleteStudentFromClub Tests

        [Fact]
        public async Task DeleteStudentFromClub_StudentNotInClub_ReturnsClubStudentNotFound()
        {
            var clubId = Guid.NewGuid();
            var request = new KickStudentRequest { StudentId = Guid.NewGuid() };

            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _userRepo.UserExistsAsync(request.StudentId).Returns(true);
            _clubStudentRepo.IsStudentInClub(clubId, request.StudentId).Returns(false);

            var result = await _service.DeleteStudentFromClub(clubId, request);


            result.IsT3.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteStudentFromClub_ValidInputs_RemovesStudentAndReturnsSuccess()
        {
            var clubId = Guid.NewGuid();
            var request = new KickStudentRequest { StudentId = Guid.NewGuid() };

            _clubRepo.ClubExistsAsync(clubId).Returns(true);
            _userRepo.UserExistsAsync(request.StudentId).Returns(true);
            _clubStudentRepo.IsStudentInClub(clubId, request.StudentId).Returns(true);

            var result = await _service.DeleteStudentFromClub(clubId, request);
            result.IsT0.Should().BeTrue();
            await _clubStudentRepo.Received(1).DeleteStudentFromClub(Arg.Is<ClubStudent>(cs => cs.ClubId == clubId && cs.StudentId == request.StudentId));
        }

        #endregion
    }
}