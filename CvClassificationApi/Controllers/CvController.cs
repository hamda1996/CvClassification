using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClassLibraryCV;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvClassificationApi.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class CvController : ControllerBase
    {
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            List<TechnoDuree> listTechnoDuree = new List<TechnoDuree>();
            CvAnalysisResult cvAnalysisResult = null;
            List<CvQuality> listCvQuality = new List<CvQuality>();
            string error = "";
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        cvAnalysisResult = new CvService().TextFromWord(formFile.FileName, stream);

                        listTechnoDuree.AddRange(cvAnalysisResult.ListTechnoDuree);
                        listCvQuality.AddRange(cvAnalysisResult.ListCvQuality);
                        error = cvAnalysisResult.Error;
                    }
                }
            }
            cvAnalysisResult = new CvAnalysisResult(listCvQuality, listTechnoDuree, error);
            return new JsonResult(cvAnalysisResult);
        }
    }
}