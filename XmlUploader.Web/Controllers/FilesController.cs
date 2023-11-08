using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XmlUploader.Web.Dtos;
using XmlUploader.Web.Models;
using XmlUploader.Web.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace XmlUploader.Web.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileApiService _fileApiService;

        public FilesController(IFileApiService fileApiService)
        {
            _fileApiService = fileApiService;
        }

        public ActionResult Upload()
        {
            return View(new UploadFileVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAsync(UploadFileVM form)
        {
            if (!ModelState.IsValid)
                return View(form);

            List<string> notificationErrors = new List<string>();
            List<string> notificationSuccess = new List<string>();

            //check if files are valid xml files
            bool areXmlExtensions = CheckForXmlExtension(form, notificationErrors);

            if (areXmlExtensions == false)
            {
                TempData["error"] = notificationErrors;
                return View(form);
            }

            Dictionary<string, Task<ResponseDto>> apiTasks = CreateConcurrencyApiTasks(form);

            // run all tasks concurrency:
            await Task.WhenAll(apiTasks.Values);

            foreach (KeyValuePair<string, Task<ResponseDto>> task in apiTasks)
            {
                var response = await task.Value;
                var fileName = task.Key;

                // check for server error
                if (response == null || response.IsSuccess == false)
                {
                    notificationErrors.Add($"{fileName} - {response.ErrorMessage}");
                    form.Errors.Add($"{fileName} - {response.ErrorMessage}");
                }
                else
                {
                    try
                    {
                        string jsonName = fileName + ".json";
                        string serializedFile = JsonConvert.SerializeObject(response.Data);
                        var filePath = Path.Combine(form.DownloadDirectory, jsonName);

                        await SaveJsonFileToUserDirectory(filePath, serializedFile);

                        notificationSuccess.Add($"Successfully converted {fileName} file");
                    }
                    // file system error
                    catch (DirectoryNotFoundException)
                    {
                        notificationErrors.Add($"Download directory {form.DownloadDirectory} is not valid");
                        form.Errors.Add($"Download directory {form.DownloadDirectory} is not valid");
                    }
                    catch (Exception)
                    {
                        notificationErrors.Add("Can not convert file to json file");
                        form.Errors.Add($"Can not convert {fileName} to json file");
                    }
                }
            }

            if (notificationErrors.Count > 0)
                TempData["error"] = notificationErrors;

            if(notificationSuccess.Count > 0)
                TempData["success"] = notificationSuccess;

            return View(form);
        }

        [NonAction]
        private bool CheckForXmlExtension(UploadFileVM form, List<string> notificationErros)
        {
            bool areXmlExtensions = true;

            foreach (IFormFile file in form.MultipleFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);

                if (extension != ".xml")
                {
                    notificationErros.Add($"The {fileName} is not a xml file");
                    form.Errors.Add($"The {fileName} is not a xml file");

                    areXmlExtensions = false;
                }
            }

            return areXmlExtensions;
        }

        [NonAction]
        private Dictionary<string, Task<ResponseDto>> CreateConcurrencyApiTasks(UploadFileVM form)
        {
            var apiTasks = new Dictionary<string, Task<ResponseDto>>();

            foreach (IFormFile xmlFile in form.MultipleFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(xmlFile.FileName);

                async Task<ResponseDto> func()
                {
                    ResponseDto response = await _fileApiService.ConvertXmlToJson(xmlFile, fileName);

                    return response;
                }

                apiTasks.Add(fileName, func());
            }

            return apiTasks;
        }

        [NonAction]
        private async Task SaveJsonFileToUserDirectory(string filePath, string serializedFile)
        {
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                await streamWriter.WriteAsync(serializedFile);
            }
        }
    }
}
