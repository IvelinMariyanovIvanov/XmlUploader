using XmlUploader.Web.Dtos;

namespace XmlUploader.Web.Services.Interfaces
{
    public interface IFileApiService
    {
        Task<ResponseDto> ConvertXmlToJson(IFormFile xmlFile, string fileName);
    }
}
