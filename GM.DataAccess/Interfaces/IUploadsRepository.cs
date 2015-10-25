using GM.Models.Public;

namespace GM.DataAccess.Interfaces
{
    public interface IUploadsRepository
    {
        UploadModel GetUpload(int id);
        UploadModel UploadFile(string providerUserKey, UploadModel model, bool isProfilePhoto = false);
    }
}
