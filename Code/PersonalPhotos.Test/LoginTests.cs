using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using PersonalPhotos.Controllers;
using PersonalPhotos.Models;
using System.Reflection;

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
        public async Task Create_GivenModelStateIsInvalid_ReturnsLoginsView()
        {
            _loginsController.ModelState.AddModelError("Test", "Test");
            var model = Mock.Of<LoginViewModel>();
            var result = await _loginsController.Create(model, CancellationToken.None);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Equal(model, viewResult.Model);
            Assert.False(_loginsController.ModelState.IsValid);
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

        [Fact]
        public async Task Create_GivenValidModel_ExistingUser()
        {
            const string email = "test@abc.com";
            const string password = "Password1";

            var loginViewModel = Mock.Of<LoginViewModel>(l => l.Email == email && l.Password == password);
            var user = Mock.Of<User>(l => l.Email == email && l.Password == password); // mock class object
           
            _logins.Setup(l => l.GetUser(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _loginsController.Create(loginViewModel, CancellationToken.None); // invoke the Actual method 
            Assert.IsType<ViewResult>(result); // check if result is of type RedirectToActionResult
            var viewResult = result as ViewResult;

            Assert.Equal(loginViewModel, viewResult.Model);
            Assert.True(_loginsController.ModelState.ContainsKey(""));
            Assert.Contains("already registered", _loginsController.ModelState[""].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Create_GivenValidModel_NewUser()
        {
            const string email = "test@abc.com";
            const string password = "Password1";

            var loginViewModel = Mock.Of<LoginViewModel>(l => l.Email == email && l.Password == password);
            var user = Mock.Of<User>(l => l.Email == email && l.Password == password); // mock class object

            _logins.Setup(l => l.GetUser(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);// return null for  getUser call
                                           // 
            // create mock for  await loginService.CreateUser(model.Email, model.Password, token);
            _logins.Setup(l => l.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _loginsController.Create(loginViewModel, CancellationToken.None);// invoke the Actual method 
            Assert.IsType<RedirectToActionResult>(result); // check if result is of type RedirectToActionResult
            var redirectToActionResult = result as RedirectToActionResult;
           
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Logins", redirectToActionResult.ControllerName);
        }
    }
}
