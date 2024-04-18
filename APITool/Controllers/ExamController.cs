using APITool.Function;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace APITool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ExamManager _examManager;

        public ExamController(ExamManager examManager)
        {
            _examManager = examManager;
        }
        [HttpPost("NewExam")]
        public IActionResult NewExam()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://khoahoc.vietjack.com/thi-online/trac-nghiem-tieng-anh-toeic-part-5-test/102867");
                Thread.Sleep(2000);
                FindExam findExam = new FindExam(_examManager);
                findExam.GoToExam(driver);
                Thread.Sleep(1000);
                findExam.GetExam();
            }
            return Ok("Exam has been retrieved.");
        }
    }
}
