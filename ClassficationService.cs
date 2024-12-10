using GreenDefined.DTOs.Classfication;
using GreenDefined.Models;
using GreenDefined.Repository.IRepo;
using GreenDefined.Service.IServices;

namespace GreenDefined.Service.Services
{
    public class ClassficationService : IClassficationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadingImage _uploadingImage;
        private readonly IClassficationRepository _classfication;
        public ClassficationService(IHttpContextAccessor httpContextAccessor, IUploadingImage uploadingImage
            , IClassficationRepository classfication)
        {

            _httpContextAccessor = httpContextAccessor;
            _uploadingImage = uploadingImage;
            _classfication = classfication;

        }


        public async Task<int> SavingImage(IFormFile file)
        {
            if (file == null) return 0;
            var schema = _httpContextAccessor.HttpContext.Request.Scheme;
            var host = _httpContextAccessor.HttpContext.Request.Host;
            var response = await _uploadingImage.SavingImage(file, schema, host);
            if (response.IsSuccess)
            {
                //Saving in DB
                var saveRecord = new ClassificationImage()
                {
                    imageTitle = response.name,
                    imageUrl = response.urlOrError
                };
                await _classfication.AddAsync(saveRecord);
                return saveRecord.Id;
            }

            return 0;
        }

        public async Task<ClassficationImageResponseDTO> HandleClassfication(ClassficationImageDTO DTO)
        {
            //Get image by id
            var imagePath = _classfication.GetTableNoTracking().Where(x => x.Id == DTO.ImageID).FirstOrDefault().imageUrl;
            //return value
            //Then !
            ClassficationImageResponseDTO response = new()
            {
                imagePath = imagePath,
                value = "none"
            };
            return response;
        }
    }
}
