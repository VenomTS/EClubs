using E_Clubs.Messages;
using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Services;
using E_Clubs.Users.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class MessagesControllerTests
    {
        private readonly IMessageService _service;
        private readonly MessagesController _controller;

        public MessagesControllerTests()
        {
            _service = Substitute.For<IMessageService>();

            _controller = new MessagesController(_service)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Path = "/api/messages/test-path" }
                    }
                }
            };
        }

        #region GetMessageById Tests

        [Fact]
        public async Task GetMessageById_MessageExists_ReturnsOkWithData()
        {
            var messageId = Guid.NewGuid();

            var expectedData = new GetMessageResponse
            {
                Id = messageId,
                Sender = new GetUserResponse
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe"
                },
                Content = "Test message",
                SentAt = DateTime.UtcNow
            };

            _service.GetMessageByIdAsync(messageId).Returns(expectedData);

            var result = await _controller.GetMessageById(messageId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expectedData);
        }
        #endregion

        #region DeleteMessageById Tests

        [Fact]
        public async Task DeleteMessageById_ServiceReturnsSuccess_ReturnsNoContent()
        {
            var messageId = Guid.NewGuid();
            _service.DeleteMessageByIdAsync(messageId).Returns(new Success());

            var result = await _controller.DeleteMessageById(messageId);

            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region UpdateMessageById Tests

        [Fact]
        public async Task UpdateMessageById_ServiceReturnsSuccess_ReturnsNoContent()
        {
            var messageId = Guid.NewGuid();

            var request = new UpdateMessageRequest();

            _service.UpdateMessageByIdAsync(messageId, request).Returns(new Success());
            var result = await _controller.UpdateMessageById(messageId, request);
            Assert.IsType<NoContentResult>(result);
        }

        #endregion
    }
}