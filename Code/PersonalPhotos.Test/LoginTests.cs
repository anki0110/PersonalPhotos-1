using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using PersonalPhotos.Controllers;

namespace PersonalPhotos.Test
{
    public class LoginTests
    {
        private readonly Mock<ILogins> _logins; // mock for Ilogin interface
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor; // mock for HttpContextAccessor
        private readonly Mock<LoginsController> _loginsController; // mock for LoginsController
        public LoginTests()
        {
            _logins = new Mock<ILogins>(); // initialize mock for Ilogin interface
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _loginsController = new Mock<LoginsController>(_logins.Object, _httpContextAccessor.Object); // initialize mock for LoginsController
        }

        //ILogins Logins = Mock.Of<ILogins>(); // get mock object for Ilogin interface directly. 
    }
}
