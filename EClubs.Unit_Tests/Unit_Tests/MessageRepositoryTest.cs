using E_Clubs.Database;
using E_Clubs.Messages;
using E_Clubs.Messages.Repositories;
using E_Clubs.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace E_Clubs.Unit_Tests
{
    public class MessageRepositoryTests
    {
        //Helper method to create a fresh, empty In-Memory database for EVERY test
        //Using a new Guid for the database name ensures tests don't share data
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static User CreateDummyUser() => new User { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" };

        #region GetMessageByIdAsync Tests

        [Fact]
        public async Task GetMessageByIdAsync_MessageExists_ReturnsMessageWithSender()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);

            var sender = new User { Id = Guid.NewGuid(), FirstName = "John" };
            var messageId = Guid.NewGuid();
            var message = new Messages.Message { Id = messageId, Content = "Test", Sender = sender, Club = new Clubs.Club { Professor = CreateDummyUser()} };

            context.Users.Add(sender);
            context.Messages.Add(message);
            await context.SaveChangesAsync();

            var result = await repository.GetMessageByIdAsync(messageId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(messageId);
            result.Sender.Should().NotBeNull();
            result.Sender.FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetMessageByIdAsync_MessageDoesNotExist_ReturnsNull()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);

            var result = await repository.GetMessageByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        #endregion

        #region CreateMessageAsync Tests

        [Fact]
        public async Task CreateMessageAsync_AddsMessageAndLoadsSender()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);
            var senderId = Guid.NewGuid();
            var sender = new User { Id = senderId };
            context.Users.Add(sender);
            await context.SaveChangesAsync();

            var dummyClubId = Guid.NewGuid();
            var newMessage = new Message
            {
                Id = Guid.NewGuid(),
                Content = "Hello",
                SenderId = senderId,
                ClubId = dummyClubId,
                Club = new Clubs.Club { Id = dummyClubId, Professor = CreateDummyUser() },
                Sender = sender,
            };

            var result = await repository.CreateMessageAsync(newMessage);

            result.Should().NotBeNull();
            result.Id.Should().Be(newMessage.Id);

            var dbMessage = await context.Messages.FindAsync(newMessage.Id);
            dbMessage.Should().NotBeNull();

            result.Sender.Should().NotBeNull();
            result.Sender.Id.Should().Be(senderId);
        }

        #endregion

        #region GetMessagesByClubIdAsync Tests

        [Fact]
        public async Task GetMessagesByClubIdAsync_ReturnsFilteredList()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);

            var clubId = Guid.NewGuid();
            var otherClubId = Guid.NewGuid();
            var sender = new User { Id = Guid.NewGuid() };

            context.Users.Add(sender);

            var targetClub = new Clubs.Club { Id = clubId, Professor = CreateDummyUser() };
            var externalClub = new Clubs.Club { Id = otherClubId, Professor = CreateDummyUser() };

            context.Messages.AddRange(
                new Message
                {
                    Id = Guid.NewGuid(),
                    ClubId = clubId,
                    Sender = sender,
                    Club = targetClub //Reusing reference
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    ClubId = clubId,
                    Sender = sender,
                    Club = targetClub //Reusing reference
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    ClubId = otherClubId,
                    Sender = sender,
                    Club = externalClub //Reusing reference
                }
            );
            await context.SaveChangesAsync();
            var result = await repository.GetMessagesByClubIdAsync(clubId);
            result.Should().HaveCount(2);
            result.All(m => m.ClubId == clubId).Should().BeTrue();
            result.All(m => m.Sender != null).Should().BeTrue();
        }

        #endregion

        #region DeleteMessageAsync Tests

        [Fact]
        public async Task DeleteMessageAsync_RemovesMessageFromDatabase()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);

            var message = new Message { Id = Guid.NewGuid(), Content = "Delete Me", Club = new Clubs.Club { Professor = CreateDummyUser() }, Sender = CreateDummyUser() };
            context.Messages.Add(message);
            await context.SaveChangesAsync();

            await repository.DeleteMessageAsync(message);

            var dbMessage = await context.Messages.FindAsync(message.Id);
            dbMessage.Should().BeNull();
        }

        #endregion

        #region UpdateMessageAsync Tests

        [Fact]
        public async Task UpdateMessageAsync_MessageNotFound_ReturnsFalse()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);

            var newMessageData = new Message { Content = "New Content", Sender = CreateDummyUser(), Club = new Clubs.Club { Professor = CreateDummyUser() } };

            var result = await repository.UpdateMessageAsync(Guid.NewGuid(), newMessageData);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateMessageAsync_MessageExists_UpdatesContentAndReturnsTrue()
        {
            await using var context = GetInMemoryDbContext();
            var repository = new MessageRepository(context);

            var messageId = Guid.NewGuid();
            var originalMessage = new Message { Id = messageId, Content = "Old Content", Sender = CreateDummyUser(), Club = new Clubs.Club { Professor = CreateDummyUser() } };

            context.Messages.Add(originalMessage);
            await context.SaveChangesAsync();

            var updateData = new Message { Content = "Brand New Content", Club = new Clubs.Club { Professor = CreateDummyUser() }, Sender = CreateDummyUser() };

            var result = await repository.UpdateMessageAsync(messageId, updateData);

            result.Should().BeTrue();

            var dbMessage = await context.Messages.FindAsync(messageId);
            dbMessage!.Content.Should().Be("Brand New Content");
        }

        #endregion
    }
}