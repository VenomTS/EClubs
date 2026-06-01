using AutoMapper;
using E_Clubs.Clubs;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Messages;
using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Repositories;
using E_Clubs.Messages.Services;
using E_Clubs.Users;
using E_Clubs.Users.DTO;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class MessageServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepo;
        private readonly IClubRepository _clubRepo;
        private readonly MessageService _service;

        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };

        public MessageServiceTests()
        {
            _mapper = Substitute.For<IMapper>();
            _messageRepo = Substitute.For<IMessageRepository>();
            _clubRepo = Substitute.For<IClubRepository>();

            _service = new MessageService(_mapper, _messageRepo, _clubRepo);
        }

        #region GetMessageByIdAsync Tests

        [Fact]
        public async Task GetMessageByIdAsync_MessageNotFound_ReturnsMessageNotFound()
        {
            var messageId = Guid.NewGuid();
            _messageRepo.GetMessageByIdAsync(messageId).Returns((E_Clubs.Messages.Message)null!);
            var result = await _service.GetMessageByIdAsync(messageId);
            result.IsT1.Should().BeTrue(); // T1 is MessageNotFound
        }

        [Fact]
        public async Task GetMessageByIdAsync_MessageExists_ReturnsMessageDto()
        {
            var messageId = Guid.NewGuid();

            var message = new E_Clubs.Messages.Message
            {
                Id = messageId,
                Content = "Test",
                Sender = new User(),
                Club = new Club {Professor = CreateDummyUser() }
            };
            var expectedDto = new GetMessageResponse
            {
                Id = messageId,
                Sender = new GetUserResponse()
            };

            _messageRepo.GetMessageByIdAsync(messageId).Returns(message);
            _mapper.Map<GetMessageResponse>(message).Returns(expectedDto);
            var result = await _service.GetMessageByIdAsync(messageId);

            result.IsT0.Should().BeTrue(); // T0 is the data
            result.AsT0.Should().BeEquivalentTo(expectedDto);
        }

        #endregion

        #region GetAllMessagesByClubIdAsync Tests

        [Fact]
        public async Task GetAllMessagesByClubIdAsync_ClubNotFound_ReturnsClubNotFound()
        {
            var clubId = Guid.NewGuid();
            _clubRepo.GetClubByIdAsync(clubId).Returns((Club)null!);
            var result = await _service.GetAllMessagesByClubIdAsync(clubId);
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllMessagesByClubIdAsync_ClubExists_ReturnsListOfMessages()
        {
            var clubId = Guid.NewGuid();
            var club = new Club { Id = clubId, Professor = new User() };
            var messages = new List<E_Clubs.Messages.Message> { new() { Sender = CreateDummyUser(), Club = new Club { Professor=CreateDummyUser()}, Id = Guid.NewGuid() } };
            var expectedDtos = new List<GetMessageResponse> { new() {Id = Guid.NewGuid(), Sender = new GetUserResponse()
} };

            _clubRepo.GetClubByIdAsync(clubId).Returns(club);
            _messageRepo.GetMessagesByClubIdAsync(clubId).Returns(messages);
            _mapper.Map<List<GetMessageResponse>>(messages).Returns(expectedDtos);
            var result = await _service.GetAllMessagesByClubIdAsync(clubId);
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().BeEquivalentTo(expectedDtos);
        }

        #endregion

        #region CreateMessageAsync Tests

        [Fact]
        public async Task CreateMessageAsync_ClubNotFound_ReturnsClubNotFound()
        {
            var clubId = Guid.NewGuid();
            var request = new CreateMessageRequest();
            _clubRepo.GetClubByIdAsync(clubId).Returns((Club)null!);
            var result = await _service.CreateMessageAsync(clubId, request);
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task CreateMessageAsync_ValidRequest_CreatesAndReturnsMessage()
        {
            var clubId = Guid.NewGuid();
            var request = new CreateMessageRequest { Content = "Hello!" };
            var club = new Club { Id = clubId, Professor = new User() };

            var initialMessageModel = new E_Clubs.Messages.Message
            {
                Content = "Test",
                Sender = new User(),
                Club = new Club { Professor = CreateDummyUser() }
            };
            var savedMessageModel = new E_Clubs.Messages.Message
            {
                Content = "Test",
                Sender = new User(),
                Club = new Club { Professor = CreateDummyUser() }
            };

            var expectedDto = new GetMessageResponse
            {
                Id = Guid.NewGuid(),
                Sender = new GetUserResponse()
            };

            _clubRepo.GetClubByIdAsync(clubId).Returns(club);
            _mapper.Map<E_Clubs.Messages.Message>(request).Returns(initialMessageModel);
            _messageRepo.CreateMessageAsync(initialMessageModel).Returns(savedMessageModel);
            _messageRepo.GetMessageByIdAsync(savedMessageModel.Id).Returns(savedMessageModel);

            _mapper.Map<GetMessageResponse>(savedMessageModel).Returns(expectedDto);
            var result = await _service.CreateMessageAsync(clubId, request);
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().BeEquivalentTo(expectedDto);
            initialMessageModel.ClubId.Should().Be(clubId);
        }

        #endregion

        #region DeleteMessageByIdAsync Tests

        [Fact]
        public async Task DeleteMessageByIdAsync_MessageNotFound_ReturnsMessageNotFound()
        {
            var messageId = Guid.NewGuid();
            _messageRepo.GetMessageByIdAsync(messageId).Returns((E_Clubs.Messages.Message)null!);
            var result = await _service.DeleteMessageByIdAsync(messageId);
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteMessageByIdAsync_MessageExists_DeletesAndReturnsSuccess()
        {
            var messageId = Guid.NewGuid();
            var message = new E_Clubs.Messages.Message
            {
                Content = "Test",
                Sender = new User(),
                Club = new Club { Professor = CreateDummyUser() }
            }; ;
            _messageRepo.GetMessageByIdAsync(messageId).Returns(message);
            var result = await _service.DeleteMessageByIdAsync(messageId);
            result.IsT0.Should().BeTrue();
            await _messageRepo.Received(1).DeleteMessageAsync(message);
        }

        #endregion

        #region UpdateMessageByIdAsync Tests

        [Fact]
        public async Task UpdateMessageByIdAsync_UpdateFails_ReturnsMessageNotFound()
        {
            var messageId = Guid.NewGuid();
            var request = new UpdateMessageRequest();
            var mappedMessage = new E_Clubs.Messages.Message
            {
                Content = "Test",
                Sender = new User(),
                Club = new Club { Professor = CreateDummyUser() }
            }; ;

            _mapper.Map<E_Clubs.Messages.Message>(request).Returns(mappedMessage);
            _messageRepo.UpdateMessageAsync(messageId, mappedMessage).Returns(false);
            var result = await _service.UpdateMessageByIdAsync(messageId, request);
            result.IsT1.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateMessageByIdAsync_UpdateSucceeds_ReturnsSuccess()
        {
            var messageId = Guid.NewGuid();
            var request = new UpdateMessageRequest();
            var mappedMessage = new E_Clubs.Messages.Message
            {
                Content = "Test",
                Sender = new User(),
                Club = new Club { Professor = CreateDummyUser() }
            }; ;

            _mapper.Map<E_Clubs.Messages.Message>(request).Returns(mappedMessage);
            _messageRepo.UpdateMessageAsync(messageId, mappedMessage).Returns(true);
            var result = await _service.UpdateMessageByIdAsync(messageId, request);

            result.IsT0.Should().BeTrue();
        }

        #endregion
    }
}