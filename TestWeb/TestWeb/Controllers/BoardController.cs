using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestWeb.Data;
using TestWeb.Models;

namespace TestWeb.Controllers
{
    public class BoardController : Controller
    {
        private readonly MyContext _context;

        public BoardController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            var board = from u in _context.BoardViews select u;
            board = board.OrderByDescending(x => x.BoardNo);
            int nCount = board.Count();  // 게시글 총 개수
            int nPageSize = 10;  // 한 글에 보여지는 글 개수
            int nSkip = nPageSize * (page - 1);
            ViewBag.Count = nCount;
            ViewBag.Page = page;
            ViewData["COUNT"] = nCount;
            ViewData["PAGE"] = page;
            if (nSkip >= nCount)  // 초과
            {
                ViewData["PAGE"] = 1;
                return View(board.Take(nPageSize));
            }
            return View(board.Skip(nSkip).Take(nPageSize));

            //var board = from u in _context.BoardViews select u;
            //return View(board);
        }

        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(BoardSend model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Board board = new Board();
        //        try
        //        {
        //            board.BoardWritter = HttpContext.Session.GetString("User");
        //        }
        //        catch (Exception)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        board.BoardTitle = model.BoardTitle;
        //        board.BoardContent = model.BoardContent;
        //        board.BoardViewCount = 0;
        //        try
        //        {
        //            _context.Boards.Add(board);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Index", "Board");
        //        }
        //        catch (Exception ex)
        //        {
        //            return NotFound();
        //        }
        //    }
        //    return new EmptyResult();
        //}

        public async Task<string> AddBoard(BoardSend boardsend)
        {
            Board board = new Board();
            if (boardsend.BoardTitle == null || boardsend.BoardTitle.Length < 1)
            {
                return "제목을 입력해주세요";
            }
            else if (boardsend.BoardContent == null || boardsend.BoardContent.Length < 1)
            {
                return "내용을 입력해주세요";
            }
            try
            {
                board.BoardWritter = HttpContext.Session.GetString("User");
            }
            catch (Exception)
            {
                return "세션 만료";
            }
            string uploadDir = "C:\\Users\\Symation\\Desktop\\Server\\";
            List<string> lstFileNames = new List<string>();
            List<string> lstFileIds = new List<string>();

            if (boardsend.Files != null)
            {
                try
                {
                    // 업로드 폴더 경로 존재 확인
                    DirectoryInfo di = new DirectoryInfo(uploadDir);
                    // 폴더가 없을 경우 신규 작성
                    if (di.Exists == false) di.Create();


                    // 선택한 파일 개수만큼 반복
                    foreach (var formFile in boardsend.Files)
                    {
                        if (formFile.Length > 0)
                        {
                            string guid = Guid.NewGuid().ToString().Replace("-", "");
                            var fileFullPath = uploadDir + guid;

                            // 파일 저장
                            using (var stream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            lstFileNames.Add(formFile.FileName);
                            lstFileIds.Add(guid);
                        }
                    }
                }
                catch (Exception)
                {
                    return "게시글 등록에 실패했습니다";
                }
            }
            if (lstFileNames.Count() > 0)
            {
                board.FileNames = string.Join('\n', lstFileNames);
                board.FileIds = string.Join('\n', lstFileIds);
            }
            board.BoardTitle = boardsend.BoardTitle;
            board.BoardContent = boardsend.BoardContent;
            board.BoardViewCount = 0;
            board.Deleted = 0;
            board.CreateDate = DateTime.Now;
            board.EditDate = DateTime.Now;
            try
            {
                _context.Boards.Add(board);
                _context.SaveChanges();
                return "Sucess";
            }
            catch (Exception)
            {
                return "게시글 등록에 실패했습니다";
            }
        }

        public string EditBoard(string BoardNo, string BoardTitle, string BoardContent)
        {
            int no = -1;
            if (int.TryParse(BoardNo, out no))
            {
                return "Error";
            };
            if (no < 1)
            {
                return "Error";
            }
            Board board = new Board();
            if (BoardTitle.Length < 1)
            {
                return "제목을 입력해 주십시오";
            }
            else if (BoardContent.Length < 1)
            {
                return "내용을 입력해 주십시오";
            }
            else
            {
                try
                {
                    board.BoardWritter = HttpContext.Session.GetString("User");
                }
                catch (Exception)
                {
                    return "세션만료";
                }
                board.BoardNo = no;
                board.BoardTitle = BoardTitle;
                board.BoardContent = BoardContent;
                board.BoardViewCount = 0;
                board.Deleted = 0;
                try
                {
                    _context.Boards.Update(board);
                    _context.SaveChanges();
                    return "Sucess";
                }
                catch (Exception)
                {
                    return "게시글 편집 실패!";
                }
            }
        }

        public string[] GetFilePath(int BoardNo, int idx)
        {
            string[] result = new string[] { string.Empty, string.Empty };
            var board = from u in _context.Boards select u;
            Board? x = board.FirstOrDefault(x => x.BoardNo == BoardNo);
            if (x != null && x.FileIds != null && x.FileNames != null)
            {
                string serverPath = "C:\\Users\\Symation\\Desktop\\Server\\";
                string[] fileIds = x.FileIds.Split('\n');
                string[] fileNames = x.FileNames.Split('\n');
                if (idx >= 0 && idx  < fileIds.Length && fileIds.Length == fileNames.Length)
                {
                    string filePath = serverPath + fileIds[idx];
                    string fileName = fileNames[idx];
                    result[0] = filePath;
                    result[1] = fileName;
                }
            }
            return result;
        }

        public async Task<IActionResult> FileDownload([FromBody] FileDownloadRequest request)
        {
            int BoardNo = request.BoardNo;
            int idx = request.idx;
            int startPosition = request.startPosition;
            int endPosition = request.endPosition;

            string[] pathNname = GetFilePath(BoardNo, idx);
            string filePath = pathNname[0];
            string fileName = pathNname[1];

            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(fileName))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 8192, useAsync: true);

                var response = HttpContext.Response;
                response.ContentType = "application/octet-stream";
                response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                await fileStream.CopyToAsync(response.Body, 1024*1024); // 80KB chunk size
                await response.Body.FlushAsync();

                return new EmptyResult();
            }

            return NotFound();
        }

        //public async Task<IActionResult> FileDownloadAsync([FromBody] FileDownloadRequest request)
        //{
        //    int BoardNo = request.BoardNo;
        //    int idx = request.idx;
        //    int startPosition = request.startPosition;
        //    int endPosition = request.endPosition;

        //    string[] pathNname = GetFilePath(BoardNo, idx);
        //    string fpath = pathNname[0];
        //    string fname = pathNname[1];
        //    if (!string.IsNullOrEmpty(fpath) && !string.IsNullOrEmpty(fname))
        //    {
        //        using (FileStream fs = new FileStream(fpath, FileMode.Open, FileAccess.Read))
        //        {
        //            long totalSize = fs.Length;
        //            long chunkSize = endPosition - startPosition + 1;

        //            // 전송할 청크의 크기 설정
        //            long bufferSize = Math.Min(chunkSize, 1024 * 1024); // 1MB 단위로 전송

        //            byte[] buffer = new byte[bufferSize];
        //            fs.Position = startPosition;

        //            Response.Headers.Add("Content-Disposition", $"attachment; filename={fname}"); // 다운로드될 파일의 이름 설정
        //            Response.Headers.Add("Content-Length", chunkSize.ToString());

        //            while (chunkSize > 0)
        //            {
        //                int bytesRead = fs.Read(buffer, 0, (int)Math.Min(bufferSize, chunkSize));

        //                if (bytesRead == 0)
        //                {
        //                    break;
        //                }

        //                await Response.Body.WriteAsync(buffer, 0, bytesRead);

        //                chunkSize -= bytesRead;
        //            }

        //            return new EmptyResult();
        //        }
        //    }

        //    return NotFound();
        //}

        public IActionResult GetFileSize(int BoardNo, int idx)
        {
            string[] pathNname = GetFilePath(BoardNo, idx);
            string fpath = pathNname[0];
            if (System.IO.File.Exists(fpath))
            {
                FileInfo fileInfo = new FileInfo(fpath);
                long fileSize = fileInfo.Length;

                return Ok(fileSize.ToString());
            }

            return NotFound();
        }
        public string Register_temp(string NAME, string USER_ID, string USER_PWD, string USER_PWD_CHECK, string EMAIL)
        {
            Register_Input user = new Register_Input();

            if (NAME.Length < 1)
            {
                return "이름을 입력해 주십시오";
            }
            else if (USER_ID.Length < 1)
            {
                return "아이디를 입력해 주십시오";
            }
            else if (USER_PWD.Length < 1)
            {
                return "비밀번호를 입력해 주십시오";
            }
            else if (USER_PWD != USER_PWD_CHECK)
            {
                return "작성하신 비밀번호와 확인문구가 일치하지 않습니다.";
            }
            else if (EMAIL == null)
            {
                return "이메일을 입력해 주십시오";
            }
            else if (!Regex.IsMatch(EMAIL, @"^[a-zA-Z0-9+-_.]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                return "이메일형식에 맞게 입력해주십시오";
            }
            else
            {
                user.NAME = NAME;
                user.USER_ID = USER_ID;
                user.USER_PWD = USER_PWD;
                user.EMAIL_ADRESS = EMAIL;
                try
                {
                    _context.Registers.Add(user);
                    //_context.Registers.AddRange([user.USER_ID, user.USER_PWD, user.NAME, user.EMAIL_ADRESS]);
                    _context.SaveChanges();
                    return "Success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "Fail";
                }
            }
        }

        

        [HttpGet]
        public IActionResult Details(int id, int page)
        {
            //Board_view? model = _context.BoardViews.Find(id);
            //if (model != null)
            //{
            //    try
            //    {
            //        var board = from u in _context.Boards select u;
            //        if (board != null)
            //        {
            //            Board? boards = board.FirstOrDefault(x => x.BoardNo == id);
            //            if (boards != null)
            //            {
            //                boards.BoardViewCount += 1;
            //                _context.Boards.Update(boards);
            //                _context.SaveChanges();
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    return View(model);
            //}
            //return NotFound();
            Board? model = _context.Boards.Include(x=>x.Comments).FirstOrDefault(x => x.BoardNo == id);
            if (model != null)
            {
                try
                {
                    var board = from u in _context.Boards select u;
                    if (board != null)
                    {
                        Board? boards = board.FirstOrDefault(x => x.BoardNo == id);
                        if (boards != null)
                        {
                            boards.BoardViewCount += 1;
                            _context.Boards.Update(boards);
                            _context.SaveChanges();
                            ViewData["Page"] = page;
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return NotFound();
        }

        public IActionResult CreateComment(int BoardNo, string content)
        {
            // postID = 게시글 번호
            // Id = 댓글 작성자
            string writter = string.Empty;
            try
            {
                writter = HttpContext.Session.GetString("User");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(writter))
                return RedirectToAction("Index", "Home");

            var post = _context.Boards.Include(x => x.Comments).FirstOrDefault(x => x.BoardNo == BoardNo);
            if (post == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                BoardNo = BoardNo,
                Content = content,
                Writter = writter,
                Class = 0,
                GroupNum = post.Comments.Count(x => x.Class == 0),
                CreateDate = DateTime.Now,
            };

            //_context.Comments.Add(comment);
            post.Comments.Add(comment);  // Error
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = BoardNo });
        }

        public IActionResult CreateCommentReply(int BoardNo, int GroupNum, string content)
        {
            // postID = 게시글 번호
            // Id = 댓글 작성자
            string writter = string.Empty;
            try
            {
                writter = HttpContext.Session.GetString("User");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(writter))
                return RedirectToAction("Index", "Home");

            var post = _context.Boards.Find(BoardNo);
            if (post == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                BoardNo = BoardNo,
                Content = content,
                Writter = writter,
                Class = 1,
                GroupNum = GroupNum,
                CreateDate = DateTime.Now,
            };

            //_context.Comments.Add(comment);
            post.Comments.Add(comment);  // Error
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = BoardNo });
        }

        public IActionResult Edit(int id)
        {
            Board? model = _context.Boards.Find(id);
            if (model != null)
            {
                if (HttpContext.Session.GetString("User") != model.BoardWritter.Trim())
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(model);
            }
            return NotFound();
        }

        public string Delete(int BoardNo)
        {
            Board? model = _context.Boards.Find(BoardNo);
            if (model != null)
            {
                if (HttpContext.Session.GetString("User") != model.BoardWritter.Trim())
                {
                    return "세션 만료";
                }
                else
                {
                    model.Deleted = 1;
                    _context.Update(model);
                    _context.SaveChanges();
                    return "Sucess";
                }
            }
            return "Fail to delete";
        }
    }
    public class FileDownloadRequest
    {
        public int BoardNo { get; set; }

        public int idx { get; set; }

        public int startPosition { get; set; }

        public int endPosition { get; set; }
    }
}
