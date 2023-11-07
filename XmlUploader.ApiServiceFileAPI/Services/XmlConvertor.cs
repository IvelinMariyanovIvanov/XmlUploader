using Newtonsoft.Json;
using System.Xml;
using XmlUploader.ApiServiceFileAPI.Dtos;

namespace XmlUploader.ApiServiceFileAPI.Services
{
    public class XmlConvertor : IXmlConvertor
    {
        public ResponseDto ConvertFromXmlToJson(IFormFile xmlFile, string fileName)
        {
            ResponseDto responseDto = new ResponseDto();

            // check for file length
            if (xmlFile == null || xmlFile.Length == 0)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = "Please upload a xml file";

                return responseDto;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = "The file name is null or emprty";

                return responseDto;
            }

            if (Path.GetExtension(xmlFile.FileName) != ".xml")
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = "The file is not a xml file";

                return responseDto;
            }

            using (Stream stream = xmlFile.OpenReadStream())
            {
                // convert to JSON
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);

                    string jsonFile = JsonConvert.SerializeXmlNode(xmlDoc);
                    // set converted json file
                    responseDto.Data = jsonFile;
                }
                // xml file is corrupt or not valid
                catch (XmlException xmlEx)
                {
                    responseDto.IsSuccess = false;
                    responseDto.ErrorMessage = "Invalid xml file. " + xmlEx.Message;
                }
                catch (Exception ex)
                {
                    responseDto.IsSuccess = false;
                    responseDto.ErrorMessage = ex.Message;
                }
            }

            return responseDto;
        }
    }
}
