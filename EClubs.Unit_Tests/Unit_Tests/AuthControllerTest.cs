using E_Clubs.Auth;
using E_Clubs.Auth.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace EClubs.Unit_Tests
{
    public class AuthControllerTests
    {
        private readonly IUserService _userService;
        private readonly AuthController _controller;
        private readonly DefaultHttpContext _httpContext;

        public AuthControllerTests()
        {
            _userService = Substitute.For<IUserService>();

            //TODO: Issues with fake path:
            _httpContext = new DefaultHttpContext();
            _httpContext.Request.Path = "/api/auth/test";

            _controller = new AuthController(_userService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };
        }

        #region GetMe Tests

        [Fact]
        public async Task GetMe_NoCookieProvided_ReturnsUnauthorized()
        {
            var result = await _controller.GetMe();
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetMe_ValidToken_ReturnsOkWithUser()
        {
            var fakeToken = "valid-jwt-token";
            _httpContext.Request.Headers["Cookie"] = $"TOKEN={fakeToken}";

            var expectedUser = new GetMeResponse { Id = Guid.NewGuid() };
            _userService.GetMeAsync(fakeToken).Returns(expectedUser);

            var result = await _controller.GetMe();
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(expectedUser);
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_ValidRequest_AppendsCookieAndReturnsNoContent()
        {
            var request = new RegisterUserRequest { FirstName = "John", LastName = "Doe", Mail = "test@gmail.com", Password = "Password123!" };
            var fakeReturnedToken = "new-jwt-token";

            _userService.RegisterUserAsync(request).Returns(fakeReturnedToken);
            var result = await _controller.Register(request);
            Assert.IsType<NoContentResult>(result);
            var setCookieHeader = _httpContext.Response.Headers["Set-Cookie"].ToString();
            setCookieHeader.Should().Contain("TOKEN=new-jwt-token");
            setCookieHeader.Should().Contain("httponly");
        }
        #endregion

        #region Login Tests

        #endregion
    }
}