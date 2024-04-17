using APITool.Data;
using MongoDB.Bson;
using OpenQA.Selenium;

namespace APITool.Function
{
    public class FindExam
    {
        private readonly ExamManager _examManager;
        public IWebDriver updateDriver;
        public FindExam(ExamManager examManager)
        {
            _examManager = examManager;
        }
        public void GoToExam(IWebDriver driver)
        {
            IWebElement button1 = driver.FindElement(By.XPath("//*[@id=\"main-content\"]/div[2]/div/div/div[1]/div[1]/a"));
            button1.Click();
            Thread.Sleep(1000);
            IWebElement button2 = driver.FindElement(By.XPath("//*[@id=\"exam-modal_notice\"]/div/div/div[2]/div[2]/a"));
            button2.Click();
            Thread.Sleep(1000);
            var emailInput = driver.FindElement(By.XPath("//*[@id=\"login-box\"]/div/div[2]/div/div[1]/form/div/div[1]/input"));
            emailInput.SendKeys("binbb1324@gmail.com");
            var passwordInput = driver.FindElement(By.XPath("//*[@id=\"login-box\"]/div/div[2]/div/div[1]/form/div/div[2]/input"));
            passwordInput.SendKeys("khoibia123");
            Thread.Sleep(2000);
            IWebElement button3 = driver.FindElement(By.XPath("//*[@id=\"login-box\"]/div/div[2]/div/div[1]/form/div/div[3]/button"));
            button3.Click();
            Thread.Sleep(3000);
            IWebElement button4 = driver.FindElement(By.XPath("//*[@id=\"main-content\"]/div[2]/div/div/div[1]/div[1]/a"));
            button4.Click();
            updateDriver = driver;
        }
        public void GetExam()
        {
            var examElements = updateDriver.FindElements(By.CssSelector(".leave-site"));
            var newID = ObjectId.GenerateNewId();
            foreach (var examElement in examElements)
            {
                var examName = examElement.FindElement(By.CssSelector(".item-course-test .number")).Text;
                if (!_examManager.IsExamExists(examName))
                {
                    var examDB = new Exam
                    {
                        Id = newID,
                        name = examName,
                        isDelete = false,
                        Questions = new List<Question>()
                    };
                    _examManager.AddExam(examDB);
                    break;
                }
            }
        }
    }

}
