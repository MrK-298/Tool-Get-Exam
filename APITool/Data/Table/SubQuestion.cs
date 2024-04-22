using MongoDB.Bson;

namespace APITool.Data.Table
{
    public class SubQuestion
    {
        public ObjectId Id { get; set; }
        public string text { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
