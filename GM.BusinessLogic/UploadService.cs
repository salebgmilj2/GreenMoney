using GM.BusinessLogic.Interfaces;
using GM.DataAccess;
using GM.Models.Public;

namespace GM.BusinessLogic
{
    public class UploadService : IUploadService
    {
        public UploadModel GetUpload(int id)
        {
            return new UploadsRepository().GetUpload(id);
        }

        public UploadModel UploadFile(string providerUserKey, UploadModel model, bool isProfilePhoto = false)
        {
            return new UploadsRepository().UploadFile(providerUserKey, model, isProfilePhoto);
        }

        public string AddSupplierAddressDefault()
        {
            return new UploadsRepository().AddSupplierAddressDefault();
        }
    }
}
