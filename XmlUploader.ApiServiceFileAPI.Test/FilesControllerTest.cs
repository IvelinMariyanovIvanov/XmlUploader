
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using XmlUploader.ApiServiceFileAPI.Services;
using FilesController = XmlUploader.ApiServiceFileAPI.Controllers.FilesController;

namespace XmlUploader.ApiServiceFileAPI.Test
{
    public class FilesControllerTest
    {
        private readonly IXmlConvertor _xmlConvertorService;
        private readonly FilesController _filesController;

        public FilesControllerTest()
        {
            _xmlConvertorService = new XmlConvertor();
            _filesController = new FilesController(_xmlConvertorService);
        }

        /// <summary>
        /// With a null xml file should return false
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(null)]
        public void ConvertXmlToJson_With_Null_File(IFormFile formFile)
        {
            // Arrange
            string fileName = "valid.xml";

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile, fileName);

            // Assert
            Assert.True(response.IsSuccess == false);
        }

        /// <summary>
        /// With fileLength = 0 should return false
        /// </summary>
        /// <param name="xmlFile"></param>
        [Theory]
        [InlineData(0)]
        public void ConvertXmlToJson_With_Empty_File(int fileLength)
        {
            // Arrange
            string emptyFile = string.Empty;
            string validFileNameAndExtenstion = "valid.xml";

            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(emptyFile, fileLength, validFileNameAndExtenstion, formFile);

            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, validFileNameAndExtenstion);

            // Assert
            Assert.True(response.IsSuccess == false);
        }

        /// <summary>
        /// Wih a valid xml file should return true
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>")]
        public void ConvertXmlToJson_Convert_Valid_XmlContentAsync(string validXmlContent)
        {
            // Arrange
            int fileLength = 111;
            string validFileNameAndExtenstion = "valid.xml";

            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(validXmlContent, fileLength, validFileNameAndExtenstion, formFile);


            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, validFileNameAndExtenstion);

            // Assert
            Assert.True(response.IsSuccess);
        }

        /// <summary>
        /// With a valid xml file and valid file name extension should return true
        /// </summary>
        /// <param name="validXmlContent"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", "valid.xml")]
        public void ConvertXmlToJson_Convert_Valid_XmlContent_With_Valid_FileName_Extension(string validXmlContent, string fileName)
        {
            // Arrange
            int fileLength = 111;
            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(validXmlContent, fileLength, fileName, formFile);

            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, fileName);

            // Assert
            Assert.True(response.IsSuccess);
        }

        /// <summary>
        /// With a valid xml file and invalid file name extension should return false
        /// </summary>
        /// <param name="validXmlContent"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", "invalid.pdf")]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", "invalid.doc")]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", "invalid.jpg")]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", "invalid.text")]
        public void ConvertXmlToJson_Convert_Valid_XmlContent_With_Invalid_FileName_Extension(string validXmlContent, string fileName)
        {
            // Arrange
            int fileLength = 111;
            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(validXmlContent, fileLength, fileName, formFile);

            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, fileName);

            // Assert
            Assert.True(response.IsSuccess == false);
        }

        /// <summary>
        /// Wih a valid xml file an empty file name should return false
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", "")]
        public void ConvertXmlToJson_Convert_Valid_XmlContent_With_Null_FileName(string validXmlContent, string fileName)
        {
            // Arrange   
            int fileLength = 111;

            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(validXmlContent, fileLength, fileName, formFile);

            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, fileName);

            // Assert
            Assert.True(response.IsSuccess == false);
        }

        /// <summary>
        /// Wih a valid xml file an null file name should return false
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n</company>", null)]
        public void ConvertXmlToJson_Convert_Valid_XmlContent_With_Empty_FileName(string validXmlContent, string fileName)
        {
            // Arrange
            int fileLength = 111;

            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(validXmlContent, fileLength, fileName, formFile);

            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, fileName);

            // Assert
            Assert.True(response.IsSuccess == false);
        }

        /// <summary>
        /// Wih a invalid xml file should return false
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<company>\r\n  <employees>\r\n    <employee>\r\n      <id>1</id>\r\n      <name>John Doe</name>\r\n      <position>Software Engineer</position>\r\n      <department>Engineering</department>\r\n    </employee>\r\n  </employees>\r\n<company>")]
        public void ConvertXmlToJson_Convert_Not_Valid_XmlContent(string invalidXmlContent)
        {
            // Arrange
            int fileLength = 111;
            string validFileNameAndExtenstion = "valid.xml";

            Mock<IFormFile> formFile = new Mock<IFormFile>();

            // Set up the form file with a stream containing the expected file contents
            Mock<IFormCollection> forms = MockXmlFile(invalidXmlContent, fileLength, validFileNameAndExtenstion, formFile);

            // Set up the context
            _filesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Set up the forms
            _filesController.Request.Form = forms.Object;

            // Act - Invoke the method
            var response = _filesController.ConvertXmlToJson(formFile.Object, validFileNameAndExtenstion);

            // Assert
            Assert.True(response.IsSuccess == false);
        }

        private static Mock<IFormCollection> MockXmlFile(string xmlContent, int fileLength, string fileNameAndExtension, Mock<IFormFile> formFile)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(xmlContent);
            writer.Flush();
            ms.Position = 0;

            formFile.Setup(x => x.OpenReadStream()).Returns(ms);
            formFile.SetupGet(x => x.Length).Returns(fileLength);
            formFile.SetupGet(x => x.FileName).Returns(fileNameAndExtension);

            // Set up the form collection with the mocked form
            Mock<IFormCollection> forms = new Mock<IFormCollection>();
            forms.Setup(f => f.Files[It.IsAny<int>()]).Returns(formFile.Object);

            return forms;
        }

    }
}
