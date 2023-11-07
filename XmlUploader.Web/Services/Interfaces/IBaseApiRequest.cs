using XmlUploader.Web.Dtos;

namespace XmlUploader.Web.Services.Interfaces
{
    public interface IBaseApiRequest
    {
        Task<ResponseDto> SendRequestAsync(RequestDto requestDto);
    }
}
