using DataExtractionFromImageIntellixio.Model;
using Tesseract;

namespace DataExtractionFromImageIntellixio.Utility
{
    public class ExtractionHelper
    {
        public static string ExtractTextFromImage(byte[] imageBytes)
        {
            try
            {
                // Save image temporarily
                string tempImagePath = Path.GetTempFileName();
                File.WriteAllBytes(tempImagePath, imageBytes);

                // Define the Tesseract OCR engine
                string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                using (var img = Pix.LoadFromFile(tempImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        string extractedText = page.GetText();
                        return extractedText.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting text: " + ex.Message);
                return null;
            }
        }
        public static Info ParseMessyText(string raw)
        {
            string cleaned = raw.Replace('“', '"').Replace('”', '"');

            var lines = cleaned.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();

            var person = new Info();

            foreach (var line in lines)
            {
                if (line.StartsWith("\"name\""))
                    person.Name = GetValueFromLine(line);
                else if (line.StartsWith("\"organization\""))
                    person.Organization = GetValueFromLine(line);
                else if (line.Any(char.IsDigit) && person.Address == null)
                    person.Address = GetValueFromLine(line);
                else if (line.Any(char.IsDigit) && person.Address != null)
                    person.Mobile = GetValueFromLine(line);
            }

            return person;
        }

        public static string GetValueFromLine(string line)
        {
            var parts = line.Split(new[] { ':' }, 2);
            if (parts.Length != 2) return null;
            var value = parts[1].Trim();
            if (value.StartsWith("\"")) value = value.Substring(1);
            if (value.EndsWith("\"")) value = value.Substring(0, value.Length - 1);
            value = value.TrimEnd('\\', ',', '"');

            return value.Trim();
        }
    }
}
