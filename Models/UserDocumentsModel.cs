using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class UserDocumentsModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret SaveUserDocuments(UserDocUpload doc, JwtStatus jwtData)
        {
            try
            {
                CommonLogic CL = new CommonLogic();
                string UploadFileName = "";
                string extension = "";

                if (doc.document != null)
                {
                    extension = Path.GetExtension(doc.document.FileName.ToString());
                    if (!Directory.Exists(Path.GetFullPath("Uploads/UserDocuments/")))
                    {
                        Directory.CreateDirectory(Path.GetFullPath("Uploads/UserDocuments/"));
                    }

                    UploadFileName = Path.GetFileNameWithoutExtension(doc.document.FileName.ToString()) + "_document" + doc.userId;
                    string NewFileNameWithFullPath = Path.GetFullPath("Uploads/UserDocuments/" + UploadFileName + extension).Replace("~\\", "");

                    bool uploadstatus = CL.upload(doc.document, NewFileNameWithFullPath);

                    UserDocuments udoc = new UserDocuments
                    {
                        UserId = doc.userId,
                        UserType = doc.userType,
                        DocName = UploadFileName + extension,
                        DocType = doc.docType,
                        DocOriginalName = doc.document.FileName,
                        CreatedBy = jwtData.Id,
                        CreatedDate = DateTime.UtcNow
                    };
                    db.UserDocuments.Add(udoc);
                    db.SaveChanges();
                }

                return new Ret { status = true, message = SaveSuccessMessage(1, "Document "), data = doc.document };

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret GetAllDocuments(UserDocuments entity)
        {
            try
            {
                var documents = db.UserDocuments.Where(doc => doc.UserId == entity.UserId && doc.UserType == entity.UserType).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(documents, "Doctor Education"), data = documents };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }
}
