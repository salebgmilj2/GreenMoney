using GM.Models.Public;

namespace GM.BusinessLogic.Interfaces
{
    public interface IUploadService
    {
        UploadModel GetUpload(int id);
    }
}
