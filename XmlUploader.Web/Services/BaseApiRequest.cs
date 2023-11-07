using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using XmlUploader.Web.Dtos;
using XmlUploader.Web.Services.Interfaces;
using static XmlUploader.Web.Constants.Constants;

namespace XmlUploader.Web.Services
{
    public class BaseApiRequest : IBaseApiRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        

        public BaseApiRequest(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto> SendRequestAsync(RequestDto requestDto)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("XmlConvertorApi");
                HttpRequestMessage httpRequest = new HttpRequestMessage();

                // set headers
                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    httpRequest.Headers.Add("Accept", "*/*");
                }
                else
                {
                    httpRequest.Headers.Add("Accept", "application/json");
                }


                httpRequest.RequestUri = new Uri(requestDto.ApiUrl);

                // set content for files
                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    MultipartFormDataContent content = new MultipartFormDataContent();

                    IFormFile xmlFile = (FormFile)requestDto.Data;

                    content.Add(new StreamContent(xmlFile.OpenReadStream()), "xmlFile", xmlFile.FileName);

                    // set content
                    httpRequest.Content = content;
                }
                // set content for dto object
                else
                {
                    if (requestDto.Data != null)
                    {
                        httpRequest.Content = new StringContent
                            (JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                    }
                }

                switch (requestDto.ApiType)
                {
                    case
                        ApiType.POST:
                        httpRequest.Method = HttpMethod.Post;
                        break;
                    case
                        ApiType.PUT:
                        httpRequest.Method = HttpMethod.Put;
                        break;
                    case
                        ApiType.DELETE:
                        httpRequest.Method = HttpMethod.Delete;
                        break;
                    default:
                        httpRequest.Method = HttpMethod.Get;
                        break;
                }

                // get response from api
                HttpResponseMessage apiResponse = await httpClient.SendAsync(httpRequest);

                switch (apiResponse.StatusCode)
                {
                    case
                        HttpStatusCode.NotFound:
                        return new ResponseDto() { IsSuccess = false, ErrorMessage = "Not Found" };
                    case
                        HttpStatusCode.Forbidden:
                        return new ResponseDto() { IsSuccess = false, ErrorMessage = "Access Denied" };
                    case
                        HttpStatusCode.Unauthorized:
                        return new ResponseDto() { IsSuccess = false, ErrorMessage = "Unauthorized" };
                    case
                        HttpStatusCode.BadRequest:
                        return new ResponseDto() { IsSuccess = false, ErrorMessage = "Bad Request" };
                    case
                        HttpStatusCode.UnsupportedMediaType:
                        return new ResponseDto() { IsSuccess = false, ErrorMessage = "Unsupported Media Type" };
                    default:
                        string apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                        ResponseDto apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiResponseContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseDto()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };

                return response;
            }
        }
    }
}
