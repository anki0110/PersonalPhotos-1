using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PersonalPhotos.Controllers;
using PersonalPhotos.Models;

namespace PersonalPhotos.Test
{
    public class PhotosTests
    {
        private readonly Mock<IFileStorage> _filestorage;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<IKeyGenerator> _keyGenerator;
        private readonly Mock<IPhotoMetaData> _photoMetaData;
        private readonly PhotosController _photosController;

        public PhotosTests()
        {
            _filestorage = new Mock<IFileStorage>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _keyGenerator = new Mock<IKeyGenerator>();
            _photoMetaData = new Mock<IPhotoMetaData>();
            _photosController = new PhotosController(_keyGenerator.Object,_httpContextAccessor.Object, _photoMetaData.Object,_filestorage.Object);
        }

        [Fact]
        public async Task Upload_GivenValidFilePath_RedirectToDisplayAction()
        {
            var viewModel = new Mock<PhotoUploadViewModel>();
            var fromFile = new Mock<IFormFile>();
            // viewModel.SetupGet(x => x.File).Returns(fromFile.Object);
            viewModel.Object.File = fromFile.Object;

            //mocking this : _httpContextAccessor.HttpContext.Session.GetString("User");
            var session = new Mock<ISession>();
            session.Setup(x => x.Set("User", It.IsAny<byte[]>()));
            
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Session).Returns(session.Object);
            _httpContextAccessor.SetupGet( x => x.HttpContext).Returns(context.Object);

            var result = await _photosController.Upload(viewModel.Object) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Display",result.ActionName, true);
        }
    }
}
