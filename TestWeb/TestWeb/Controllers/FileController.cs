using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TestWeb.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> FileUpload([FromForm]requestForm form)
        {
            int result = -1;
            IList<IFormFile>? files = form.files;
            string uploadDir = "C:\\Users\\Symation\\Desktop\\Server\\";
            try
            {
                // 업로드 폴더 경로 존재 확인
                DirectoryInfo di = new DirectoryInfo(uploadDir);
                // 폴더가 없을 경우 신규 작성
                if (di.Exists == false) di.Create();

                if (files != null)
                {
                    // 선택한 파일 개수만큼 반복
                    foreach (var formFile in files)
                    {
                        if (formFile.Length > 0)
                        {
                            var fileFullPath = uploadDir + formFile.FileName;

                            // 파일명이 이미 존재하는 경우 파일명 변경
                            int filecnt = 1;
                            String newFilename = string.Empty;
                            while (new FileInfo(fileFullPath).Exists)
                            {
                                var idx = formFile.FileName.LastIndexOf('.');
                                var tmp = formFile.FileName[..idx];
                                newFilename = tmp + String.Format("({0})", filecnt++) + formFile.FileName.Substring(idx);
                                fileFullPath = uploadDir + newFilename;
                            }

                            // 파일 업로드
                            using (var stream = new FileStream(fileFullPath, FileMode.CreateNew))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                }
                result = 0;
            }
            catch (Exception)
            {
                throw;
            }
            return Json(new { result });
        }

        public FileResult FileDownload()
        {
            string filepath = "C:\\Users\\Symation\\Desktop\\Server\\";        // 파일 경로
            string filename = "123.txt";                 // 파일명
            string path = filepath + filename;          // 파일 경로 / 파일명
                                                        // 파일을 바이트 형식으로 읽음
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            // 파일 다운로드 처리
            return File(bytes, "application/octet-stream", "이름체크하기.txt");
        }
    }

    public class requestForm
    {
        public string? title { get; set; }
        public List<IFormFile>? files { get; set; }
    }
}
