using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XmlUploader.Web.Models
{
    public class UploadFileVM
    {
        [Required(ErrorMessage = "Please upload a file")]
        [DisplayName("Upload one or many files")]
        public IEnumerable<IFormFile> MultipleFiles { get; set; }

        [Required(ErrorMessage = "Please enter a Download Directory path")]
        [DisplayName("Download Directory Path")]
        public string DownloadDirectory { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}

