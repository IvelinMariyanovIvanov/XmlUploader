using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XmlUploader.Web.Dtos;
using XmlUploader.Web.Models;
using XmlUploader.Web.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace XmlUploader.Web.Controllers
{
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IFileApiService _fileApiService;

        public FilesController(IWebHostEnvironment hostEnvironment, IFileApiService fileApiService)
        {
            _hostEnvironment = hostEnvironment;
            _fileApiService = fileApiService;
        }

        public ActionResult UploadedFiles()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View(new UploadFileVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAsync(UploadFileVM form)
        {
            if(!ModelState.IsValid)
                return View(form);

            // check if files are valid xml files
            foreach (IFormFile file in form.MultipleFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);

                if (extension != ".xml")
                {
                    TempData["error"] = $"The {fileName} is not a xml file";
                    form.Errors.Add($"The {fileName} is not a xml file");

                    return View(form);
                }
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
                    TempData["error"] = $"{fileName} - {response.ErrorMessage}";
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

                        TempData["success"] = $"Successfully converted {fileName} file";
                    }
                    // file system error
                    catch (DirectoryNotFoundException)
                    {
                        TempData["error"] = $"Please enter a valid directory path";
                        form.Errors.Add($"Download directory {form.DownloadDirectory} is not valid");
                    }
                    catch (Exception)
                    {
                        TempData["error"] = "Can not convert file to json file";
                        form.Errors.Add($"Can not convert {fileName} to json file");
                    }
                }
            }
            return View(form);
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
