using Microsoft.VisualBasic;

namespace DataExtractionFromImageIntellixio.Model
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public Info Data { get; set; }
        public string Message { get; set; }
    }
}
