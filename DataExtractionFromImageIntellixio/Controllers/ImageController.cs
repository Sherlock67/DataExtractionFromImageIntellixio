using DataExtractionFromImageIntellixio.Model;
using DataExtractionFromImageIntellixio.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;


[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    [HttpPost("UploadImage")]
    public IActionResult ExtractJsonFromImage([FromBody] ImageModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.ImageBase64))
        {
            return BadRequest("No image data provided.");
        }
        try
        {
            byte[] imageBytes = Convert.FromBase64String(model.ImageBase64.Split(',')[1]);

            string extractedText =ExtractionHelper.ExtractTextFromImage(imageBytes);
            var person = ExtractionHelper.ParseMessyText(extractedText);

            return Ok(new ApiResponse
            {
                Success = true,
                Data = person,
                Message = "Successfully extracted JSON from image"
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing image: {ex.Message}");
        }
    }
    
}
