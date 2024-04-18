

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using ConsoleApp1;
using MongoDB.Bson;
using ConsoleApp1.Data;
using System.IO;
using System.Net;
using ConsoleApp1.Function;
using Quartz.Impl;
using Quartz;
using System.Reflection.Metadata;

public class Program
{
    public static ExamManager _examManager;
    public static void Main(string[] args)
    {
        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
        IScheduler scheduler = schedulerFactory.GetScheduler().Result;


        IJobDetail job = JobBuilder.Create<RunExamJob>()
            .WithIdentity("runExamJob", "group1")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("runExamTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(60)
                .RepeatForever())
            .Build();

        scheduler.ScheduleJob(job, trigger).Wait();

        scheduler.Start().Wait();

        Console.WriteLine("Ứng dụng đã được lên lịch trình. Nhấn Enter để thoát...");
        Console.ReadLine();

        scheduler.Shutdown().Wait();
    }
}

public class RunExamJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return Task.Run(() =>
        {
            Program._examManager = new ExamManager();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://khoahoc.vietjack.com/thi-online/trac-nghiem-tieng-anh-toeic-part-5-test/102867");
                Thread.Sleep(2000);
                FindExam findExam = new FindExam(Program._examManager);
                findExam.GoToExam(driver);
                Thread.Sleep(1000);
                findExam.GetExam();
            }
        });
    }
}