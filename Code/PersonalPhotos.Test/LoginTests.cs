using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using PersonalPhotos.Controllers;
using PersonalPhotos.Models;

namespace PersonalPhotos.Test
{
    public class LoginTests
    {
        private readonly Mock<ILogins> _logins; // mock for Ilogin interface
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor; // mock for HttpContextAccessor
        private readonly LoginsController _loginsController; // mock for LoginsController
        public LoginTests()
        {
            _logins = new Mock<ILogins>(); // initialize mock for Ilogin interface
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _loginsController = new LoginsController(_logins.Object, _httpContextAccessor.Object); // initialize mock for LoginsController
        }

        //ILogins Logins = Mock.Of<ILogins>(); // get mock object for Ilogin interface directly. 

        [Fact]
        public async Task Login_GivenModelStateIsInvalid_ReturnsLoginsView()
        {
            _loginsController.ModelState.AddModelError("Test", "Test");
            var model = Mock.Of<LoginViewModel>();
            var result = await _loginsController.Login(model, CancellationToken.None);
        }
    }
}
