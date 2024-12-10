using GreenDefined.DTOs.Classfication;

namespace GreenDefined.Service.IServices
{
    public interface IClassficationService
    {
        Task<int> SavingImage(IFormFile file);
        Task<ClassficationImageResponseDTO> HandleClassfication(ClassficationImageDTO DTO);
    }
}
