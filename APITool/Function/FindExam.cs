using APITool.Data.Config;
using APITool.Data.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using OpenQA.Selenium;
using System.IO;
using System.Net;
using static MongoDB.Driver.WriteConcern;
using static System.Net.Mime.MediaTypeNames;


namespace APITool.Function
{
    public class FindExam
    {
        public int count = 0;
        List<Answer> answersList = new List<Answer>();
        private readonly ExamManager _examManager;
        private readonly EmailSettings _emailSettings;
        public IWebDriver updateDriver;
        public FindExam(ExamManager examManager, IOptions<EmailSettings> emailSettings)
        {
            _examManager = examManager;
            _emailSettings = emailSettings.Value;
        }
        public void LoginVietJack(IWebDriver driver)
        {
            string email = _emailSettings.email;
            string password = _emailSettings.password;
            var emailInput = driver.FindElement(By.XPath("//*[@id=\"login-box\"]/div/div[2]/div/div[1]/form/div/div[1]/input"));
            emailInput.SendKeys(email);
            var passwordInput = driver.FindElement(By.XPath("//*[@id=\"login-box\"]/div/div[2]/div/div[1]/form/div/div[2]/input"));
            passwordInput.SendKeys(password);
        }
        public void AddExamToMongoDB(ObjectId newID, string examName)
        {
            var examDB = new Exam
            {
                Id = newID,
                name = examName,
                isDelete = false,
                Questions = new List<Question>()
            };
            _examManager.AddExam(examDB);
        }
        public void AddListAnswerToQuestion(IList<IWebElement> answers)
        {
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
        }
        public void AddSubQuestionToQuestion(List<SubQuestion> subQuestions,string questionText)
        {
            var subquestionDB = new SubQuestion
            {
                Id = ObjectId.GenerateNewId(),
                text = questionText,
                Answers = answersList
            };
            subQuestions.Add(subquestionDB);
            answersList = new List<Answer>();
        }
        public void AddQuestionPart5ToExam(string question,ObjectId examId)
        {
            var questionDB = new Question
            {
                Id = ObjectId.GenerateNewId(),
                type = "Part 5",
                questionText = question,
                Answers = answersList
            };
            _examManager.AddQuestionToExam(questionDB, examId);
            answersList = new List<Answer>();
            IWebElement button5 = updateDriver.FindElement(By.XPath("//*[@id=\"next-question\"]"));
            button5.Click();
        }
        public void AddQuestionPart7ToExam(string question,string path,List<SubQuestion> subQuestions,ObjectId examId)
        {
            var questionDB = new Question
            {
                Id = ObjectId.GenerateNewId(),
                type = "Part 7",
                questionText = question,
                imageUrl = "../Images/" + path + ".png",
                SubQuestions = subQuestions
            };
            _examManager.AddQuestionToExam(questionDB, examId);
            IWebElement button5 = updateDriver.FindElement(By.XPath("//*[@id=\"next-question\"]"));
            button5.Click();
        }
        public void AddSubQuestionToQuestion(int newCount, string question,ObjectId examId)
        {
            
            var subquestionDB = new SubQuestion
            {
                Id = ObjectId.GenerateNewId(),
                text = $"{newCount}. {question}",
                Answers = answersList
            };
            _examManager.AddSubquestionToQuestion(subquestionDB, examId);
            IWebElement button5 = updateDriver.FindElement(By.XPath("//*[@id=\"next-question\"]"));
            button5.Click();
            answersList = new List<Answer>();
        }
        public void MoveToPart7()
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
        public void MoveToOtherExam(IWebElement examElement)
        {
            IWebElement button4 = examElement.FindElement(By.CssSelector(".item-course-test .number"));
            button4.Click();
            Thread.Sleep(1000);
            IWebElement button6 = updateDriver.FindElement(By.XPath("//*[@id=\"leave-test-modal\"]/div/div/div/a/button"));
            button6.Click();
            Thread.Sleep(1000);
        }
        public void GoToExam(IWebDriver driver)
        {
            IWebElement button1 = driver.FindElement(By.XPath("//*[@id=\"main-content\"]/div[2]/div/div/div[1]/div[1]/a"));
            button1.Click();
            Thread.Sleep(1000);
            IWebElement button2 = driver.FindElement(By.XPath("//*[@id=\"exam-modal_notice\"]/div/div/div[2]/div[2]/a"));
            button2.Click();
            Thread.Sleep(1000);
            LoginVietJack(driver);
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
                    AddExamToMongoDB(newID, examName);
                    break;
                }
            }
            GetPart5(newID);
            GetPart7(newID);
        }
        public void GetPart5(ObjectId examId)
        {
            var examElements = updateDriver.FindElements(By.CssSelector(".leave-site"));
            foreach (var examElement in examElements)
            {
                var examName = examElement.FindElement(By.CssSelector(".item-course-test .number")).Text;
                if (_examManager.findExamById(examId).name == examName)
                {
                    MoveToOtherExam(examElement);
                    break;
                }
            }
            var questionAnswerItems = updateDriver.FindElements(By.CssSelector(".quiz-answer-item"));
            foreach (var questionAnswerItem in questionAnswerItems)
            {
                count++;
                var question = questionAnswerItem.FindElement(By.CssSelector(".question-name")).Text;
                var answers = questionAnswerItem.FindElements(By.CssSelector(".anwser-item"));
                AddListAnswerToQuestion(answers);
                AddQuestionPart5ToExam(question,examId);              
                if (count == 30)
                {
                    MoveToPart7();
                }
            }
        }
        public void GetPart7(ObjectId examId)
        {
            var examElements = updateDriver.FindElements(By.CssSelector(".leave-site"));
            foreach (var examElement in examElements)
            {
                var examName = examElement.FindElement(By.CssSelector(".item-course-test .number")).Text;
                if (_examManager.findExamById(examId).name == examName)
                {
                    MoveToOtherExam(examElement);
                    break;
                }
            }
            int newCount = 0;
            if (count == 84)
            {
                Console.WriteLine("Success");
                return;
            }
            else
            {
                var questionAnswerItems = updateDriver.FindElements(By.CssSelector(".quiz-answer-item"));
                foreach (var questionAnswerItem in questionAnswerItems)
                {
                    IReadOnlyCollection<IWebElement> imageElements = questionAnswerItem.FindElements(By.CssSelector(".question-name img"));
                    if (imageElements.Count > 0)
                    {
                        List<SubQuestion> subQuestions = new List<SubQuestion>();
                        count++;
                        newCount++;
                        var question = questionAnswerItem.FindElement(By.CssSelector(".question-name h4")).Text;
                        var imageDiv = questionAnswerItem.FindElement(By.CssSelector(".question-name img"));
                        string xpath = "//*[@id=\"test-course\"]/div[" + newCount + "]/div[2]/div/div/div";
                        var exam = _examManager.findExamById(examId);
                        string path = "Part7_exam" + exam.name + "_cau" + newCount;
                        string imageUrl = imageDiv.GetAttribute("src");
                        string localImagePath = @"C:\xampp\htdocs\webtienganh\images\" + path + ".png";
                        WebClient client = new WebClient();
                        client.DownloadFile(imageUrl, localImagePath);
                        var questionSubElement = questionAnswerItem.FindElement(By.XPath(xpath)).Text;
                        var answers = questionAnswerItem.FindElements(By.CssSelector(".anwser-item"));
                        AddListAnswerToQuestion(answers);
                        AddSubQuestionToQuestion(subQuestions, questionSubElement);
                        AddQuestionPart7ToExam(question,path,subQuestions,examId);
                    }
                    else
                    {
                        count++;
                        newCount++;
                        var question = questionAnswerItem.FindElement(By.CssSelector(".question-name")).Text;
                        var answers = questionAnswerItem.FindElements(By.CssSelector(".anwser-item"));
                        AddListAnswerToQuestion(answers);
                        AddSubQuestionToQuestion(newCount,question,examId);
                    }
                }
            }
        }
    }
}