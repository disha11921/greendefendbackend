using GreenDefined.DTOs.files;
using GreenDefined.Service.IServices;

namespace GreenDefined.Service.Services
{
    public class UploadingImage : IUploadingImage
    {
        public async Task<responsing> SavingImage(IFormFile imageFile, string requestSchema, HostString hostString)
        {
            string _uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {

                    return new responsing() { IsSuccess = false, urlOrError = "Please provide a valid image file." };

                }

                // Ensure the uploads directory exists
                Directory.CreateDirectory(_uploadsDirectory);

                // Generate a unique file name
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                // Combine the uploads directory with the unique file name
                var filePath = Path.Combine(_uploadsDirectory, uniqueFileName);

                // Save the uploaded file to the uploads directory
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                // Construct the URL to access the uploaded file
                //  var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                var imageUrl = $"{requestSchema}://{hostString}/{"images"}/{uniqueFileName}";


                // Return the URL of the uploaded image
                return new responsing() { IsSuccess = true, urlOrError = imageUrl, name = uniqueFileName };

            }
            catch (Exception ex)
            {
                return new responsing() { IsSuccess = false, urlOrError = $"An error occurred: {ex.Message}" };


            }
        }
    }
}
