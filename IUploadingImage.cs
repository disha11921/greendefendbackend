using GreenDefined.DTOs.files;

namespace GreenDefined.Service.IServices
{
    public interface IUploadingImage
    {
        Task<responsing> SavingImage(IFormFile imageFile, string requestSchema, HostString hostString);
    }
}
