using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayOperatorTool.Model
{
    public enum PanelStatus
    {
        InReview,
        Reviewed,
        UploadingToDb,
        UploadedToDb,
        UploadingToDbFailed
    }
}
