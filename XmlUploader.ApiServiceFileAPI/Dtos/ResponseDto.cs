namespace XmlUploader.ApiServiceFileAPI.Dtos
{
    public class ResponseDto
    {
        public object Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
