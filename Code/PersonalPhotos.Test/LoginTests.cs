using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Equal("Login", viewResult?.ViewName,true);
        }

        [Fact]
        public async Task Login_GivenValidModel_ReditectToDisplay()
        {
            const string email = "test@abc.com";
            const string password = "Password1";

            var loginViewModel = Mock.Of<LoginViewModel>(l => l.Email == email && l.Password == password);

            // or 
            /*var model = new LoginViewModel
            {
                Email = "testuser@example.com",
                Password = "TestPassword123!",
                ReturnUrl = "/photos/display"
            };*/
            _logins.Setup(l => l.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(UserLoginResult.Success);
             
            var result = await _loginsController.Login(loginViewModel, CancellationToken.None); // invoke the Actual method 
            Assert.IsType<RedirectToActionResult>(result); // check if result is of type RedirectToActionResult
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.Equal("Display", redirectToActionResult?.ActionName, true); // verify name of action method to be redirected to
            Assert.Equal("Photos", redirectToActionResult?.ControllerName, true); // verify name of controller to be redirected to

        }
    }
}
