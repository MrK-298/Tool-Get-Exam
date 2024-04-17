

using MongoDB.Bson;

namespace APITool.Data
{
    public class SubQuestion
    {
        public ObjectId Id { get; set; }
        public string text { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
