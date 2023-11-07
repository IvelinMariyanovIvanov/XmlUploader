
using static XmlUploader.Web.Constants.Constants;

namespace XmlUploader.Web.Dtos
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;

        public string ApiUrl { get; set; }

        public object Data { get; set; }

        public ContentType ContentType { get; set; }
    }
}
