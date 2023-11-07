using XmlUploader.Web.Dtos;
using XmlUploader.Web.Services.Interfaces;
using static XmlUploader.Web.Constants.Constants;

namespace XmlUploader.Web.Services
{
    public class FileApiService : BaseApiRequest, IFileApiService
    {
        public FileApiService(IHttpClientFactory httpClientFactory) 
            : base(httpClientFactory)
        {

        }

        public async Task<ResponseDto> ConvertXmlToJson(IFormFile xmlFile, string fileName)
        {
            RequestDto request = new RequestDto()
            {
                ApiType = ApiType.POST,
                ApiUrl = FileApiUrl + "/api/Files/" + fileName,
                ContentType = ContentType.MultipartFormData,
                Data = xmlFile
            };

            ResponseDto response = await SendRequestAsync(request);

            return response;
        }
    }
}
