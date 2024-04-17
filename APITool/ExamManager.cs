using APITool.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace APITool
{
    public class ExamManager
    {
        private readonly IMongoCollection<Exam> _examCollection;
        public ObjectId questionId;
        public ExamManager()
        {
            var client = new MongoClient("mongodb+srv://khoinguyen29082002:khoibia123@hoangkhoi.9ehzu5m.mongodb.net/");
            var database = client.GetDatabase("WebTiengAnh");
            _examCollection = database.GetCollection<Exam>("Exam");
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
    }
}
