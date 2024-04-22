using MongoDB.Bson;

namespace APITool.Data.Table
{
    public class Answer
    {
        public ObjectId Id { get; set; }
        public string text { get; set; }
        public bool isTrue { get; set; }
    }
}
