using System;
using System.IO;
using System.Linq;
using GM.DataAccess.Entities;
using GM.DataAccess.Interfaces;
using GM.Models.Public;

namespace GM.DataAccess
{
    public class UploadsRepository : IUploadsRepository
    {
        public UploadModel GetUpload(int id)
        {
            using (var context = new greenMoneyEntities())
            {
                var upload = context.Uploads.Find(id);

                if (upload != null)
                {
                    UploadModel model = new UploadModel();
                    model.Contents = upload.Contents;
                    upload.ContentType = upload.ContentType;

                    return model;
                }
                return null;
            }
        }

        public UploadModel UploadFile(string providerUserKey, UploadModel model, bool isProfilePhoto = false)
        {
            using (var context = new greenMoneyEntities())
            {

                var upload = context.Uploads.Create();

                var user = context.Users1.Find(new Guid(providerUserKey));

                upload.Contents = model.Contents;
                upload.ContentType = model.ContentType;
                upload.FileName = model.FileName;
                upload.UploadedBy_Id = user.Id;


                context.Uploads.Add(upload);

                context.SaveChanges();

                if (isProfilePhoto)
                {
                    user.PhotoID = upload.Id;
                }

                context.SaveChanges();

                model.UploadId = upload.Id;

                return model;
            }
        }

        public string AddSupplierAddressDefault()
        {
            using (var context = new greenMoneyEntities())
            {

                var address = context.Addresses.Create();
                address.Id = 1;
                address.StreetNumber = "1";
                address.Instance_Id = 1;
                address.StreetName = "Supplier";
                address.Suburb = "Supplier";
                address.StreetType = "Supplier";
                address.UnitNumber = "Supplier";
                address.State = "000";
                address.Postcode = "0000";


                context.Addresses.Add(address);
                context.SaveChanges();

                return address.Id.ToString();
            }

        }
    }
}
