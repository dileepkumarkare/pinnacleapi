using Microsoft.AspNetCore.Http.HttpResults;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class SampleReceiveModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret Save(SampleReceiveEntity entity, JwtStatus jwtData)
        {
            try
            {
                bool isReceived = false;
                foreach (var item in entity.SampleReceivedTests)
                {
                    if (!db.SampleReceive.Any(sr => sr.SCTID == entity.SCTID && sr.SCId == entity.SCId))
                    {
                        var _sampleReceive = new SampleReceiveEntity
                        {

                            SCId = entity.SCId,
                            SCTID = item.SCTID,
                            IsReceived = item.IsReceived,
                            CreatedBy = jwtData.Id,
                            CreatedDate = DateTime.UtcNow
                        };
                        db.SampleReceive.Add(_sampleReceive);
                        db.SaveChanges();
                        isReceived = true;
                    }

                }
                if (isReceived)
                {
                    return new Ret { status = true, message = "Samples received successfully" };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to receive samples" };
                }

            }
            catch (Exception ex)
            {
                Log.Information("Sample Receive Model => Save exception at " + DateTime.UtcNow.ToString() + " error message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
    }
}
