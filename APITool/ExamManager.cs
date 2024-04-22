using APITool.Data.Config;
using APITool.Data.Table;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace APITool
{
    public class ExamManager
    {
        private readonly IMongoCollection<Exam> _examCollection;
        public ObjectId questionId;
        private readonly MongoDBSettings _mongoSettings;
        public ExamManager(IOptions<MongoDBSettings> mongoSettings)
        {
            _mongoSettings = mongoSettings.Value;
            var client = new MongoClient(_mongoSettings.client);
            var database = client.GetDatabase(_mongoSettings.database);
            _examCollection = database.GetCollection<Exam>(_mongoSettings.collection);
        }
        public void AddExam(Exam exam)
        {
            _examCollection.InsertOne(exam);
        }
        public bool IsExamExists(string examName)
        {
            var filter = Builders<Exam>.Filter.Eq(x => x.name, examName);
            var exam = _examCollection.Find(filter).FirstOrDefault();
            return exam != null;
        }
        public Exam findExamById(ObjectId examId)
        {
            var filter = Builders<Exam>.Filter.Eq("_id", examId);
            var exam = _examCollection.Find(filter).FirstOrDefault();
            return exam;
        }
        public void AddQuestionToExam(Question question, ObjectId examId)
        {
            var filter = Builders<Exam>.Filter.Eq("_id", examId);
            var update = Builders<Exam>.Update.Push("Questions", question);
            _examCollection.UpdateOne(filter, update);
            questionId = question.Id;
        }
        public void AddSubquestionToQuestion(SubQuestion subquestion, ObjectId examId)
        {
            var filter = Builders<Exam>.Filter.And(
                Builders<Exam>.Filter.Eq("_id", examId),
                Builders<Exam>.Filter.ElemMatch(x => x.Questions, q => q.Id == questionId)
            );
            var update = Builders<Exam>.Update.Push("Questions.$.SubQuestions", subquestion);
            _examCollection.UpdateOne(filter, update);
        }
        public void AddAnswersToQuestion(List<Answer> answers, ObjectId examId)
        {
            var filter = Builders<Exam>.Filter.And(
                Builders<Exam>.Filter.Eq("_id", examId),
                Builders<Exam>.Filter.ElemMatch(x => x.Questions, q => q.Id == questionId)
            );

            var update = Builders<Exam>.Update.PushEach("Questions.$.Answers", answers);

            _examCollection.UpdateOne(filter, update);
        }
    }
}
