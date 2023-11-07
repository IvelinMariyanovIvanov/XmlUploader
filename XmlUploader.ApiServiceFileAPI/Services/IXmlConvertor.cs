using XmlUploader.ApiServiceFileAPI.Dtos;

namespace XmlUploader.ApiServiceFileAPI.Services
{
    public interface IXmlConvertor
    {
        public ResponseDto ConvertFromXmlToJson(IFormFile xml, string fileName);
    }
}
