using Microsoft.AspNetCore.Mvc;
using XmlUploader.ApiServiceFileAPI.Dtos;
using XmlUploader.ApiServiceFileAPI.Services;

namespace XmlUploader.ApiServiceFileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private IXmlConvertor _xmlConvertor;

        public FilesController(IXmlConvertor xmlConvertor)
        {
            _xmlConvertor = xmlConvertor;
        }

        /// <summary>
        /// Convert Xml ToJson.
        /// </summary>
        /// <param name="xmlFile">xml file</param>
        /// <param name="fileName">file name with extension</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{fileName}")]
        public ResponseDto ConvertXmlToJson(IFormFile xmlFile, string fileName)
        {
            // convert to json
            ResponseDto response = _xmlConvertor.ConvertFromXmlToJson(xmlFile, fileName);

            return response;
        }

    }
}
