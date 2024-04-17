using APITool.Data;
using MongoDB.Bson;
using OpenQA.Selenium;

namespace APITool.Function
{
    public class FindExam
    {
        public int count = 0;
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
        public void GetPart5(ObjectId examId)
        {
            var examElements = updateDriver.FindElements(By.CssSelector(".leave-site"));
            foreach (var examElement in examElements)
            {
                var examName = examElement.FindElement(By.CssSelector(".item-course-test .number")).Text;
                if (_examManager.findExamById(examId).name == examName)
                {
                    IWebElement button4 = examElement.FindElement(By.CssSelector(".item-course-test .number"));
                    button4.Click();
                    Thread.Sleep(1000);
                    IWebElement button6 = updateDriver.FindElement(By.XPath("//*[@id=\"leave-test-modal\"]/div/div/div/a/button"));
                    button6.Click();
                    Thread.Sleep(1000);
                    break;
                }
            }
            var questionAnswerItems = updateDriver.FindElements(By.CssSelector(".quiz-answer-item"));
            foreach (var questionAnswerItem in questionAnswerItems)
            {
                count++;
                var question = questionAnswerItem.FindElement(By.CssSelector(".question-name")).Text;
                var answers = questionAnswerItem.FindElements(By.CssSelector(".anwser-item"));
                List<Answer> answersList = new List<Answer>();
                foreach (var answer in answers)
                {
                    var answerText = answer.FindElement(By.CssSelector("div")).Text.Trim();
                    var answerStatusElement = answer.FindElement(By.CssSelector(".result-anwser"));
                    var answerStatus = answerStatusElement.GetAttribute("value");
                    bool isCorrect = answerStatus == "Y";
                    var answerDB = new Answer
                    {
                        Id = ObjectId.GenerateNewId(),
                        text = answerText,
                        isTrue = isCorrect
                    };
                    answersList.Add(answerDB);
                }
                var questionDB = new Question
                {
                    Id = ObjectId.GenerateNewId(),
                    type = "Part 5",
                    questionText = question,
                    Answers = answersList
                };
                _examManager.AddQuestionToExam(questionDB, examId);
                IWebElement button5 = updateDriver.FindElement(By.XPath("//*[@id=\"next-question\"]"));
                button5.Click();
                if (count == 30)
                {
                    IWebElement button8 = updateDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[1]/div[1]/div/a[4]"));
                    button8.Click();
                    Thread.Sleep(1000);
                    IWebElement button6 = updateDriver.FindElement(By.XPath("//*[@id=\"leave-test-modal\"]/div/div/div/a/button"));
                    button6.Click();
                    Thread.Sleep(1000);
                    IWebElement button7 = updateDriver.FindElement(By.XPath("//*[@id=\"sidebar_sub_menu_16519\"]/li[7]/a"));
                    button7.Click();
                    Thread.Sleep(1000);
                    IWebElement button4 = updateDriver.FindElement(By.XPath("//*[@id=\"main-content\"]/div[2]/div/div/div[1]/div[1]/a"));
                    button4.Click();
                    Console.WriteLine("Success");
                    return;
                }
            }
        }
    }


}
