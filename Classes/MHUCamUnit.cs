using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAdvRoomFutureVacancy.Classes
{
    public class MHUCamUnit
    {
        public class Rootobject
        {
            public Payload[] Payload { get; set; }
            public int Hits { get; set; }
        }

        public class Payload
        {
            public string PK { get; set; }
            public string UN_BREF { get; set; }
            public string UN_UREF { get; set; }
            public string UN_NAME { get; set; }
            public string UN_OBS2 { get; set; }
            public string UN_CLASS { get; set; }
            public string UN_FAX { get; set; }
            public string UN_USER1 { get; set; }
            public int UN_NFA { get; set; }
            public string UN_VACT { get; set; }
            public string UN_VA { get; set; }
            public string UN_USER2 { get; set; }
            public string UN_ADD6 { get; set; }
            public string UN_ADD1 { get; set; }
            public string UN_ADD2 { get; set; }
            public string UN_ADD3 { get; set; }
            public string UN_OBS3 { get; set; }
            public string UN_ADD5 { get; set; }
            public string UN_ADD4 { get; set; }
            public string UN_POST { get; set; }
            public string S_UN_DESC { get; set; }
            public string UNX_BREF { get; set; }
            public string UNX_UREF { get; set; }
            public string UNX_OCC_FROM { get; set; }
            public string UNX_OCC_TO { get; set; }
            public string RLS_GROUP { get; set; }
            public string PR_SNAM { get; set; }
            public string PR_NAME { get; set; }
            public string PR_TYPE { get; set; }
            public string V_BREF { get; set; }
            public string V_UREF { get; set; }
            public string TL_LREF { get; set; }
            public string TL_TREF { get; set; }
            public string TL_LSTT { get; set; }
            public string TL_LEND { get; set; }
            public string TL_TYPE { get; set; }
            public string TL_LTYPE { get; set; }
            public int VACANCY_DAYS { get; set; }
            public string TT_TREF { get; set; }
            public string TT_NAME { get; set; }
            public string CT_TYPE { get; set; }
            public string CT_REF { get; set; }
            public string CT_NAME { get; set; }
            public string S_CT_DESC { get; set; }
            public string PROPERTY_REF { get; set; }
            public string PROPERTY_NAME { get; set; }
            public string VACANCY_FLAG { get; set; }
            public string VIEW_UNIT { get; set; }
            public string VIEW_BUILDING { get; set; }
            public string VIEW_UNITPICE { get; set; }
            public string VIEW_PROP { get; set; }
            public string UN_DESC { get; set; }
            public string CT_DESC { get; set; }
        }

        public class getUnitsVacancy
        {
            public string sessionID { get; set; }
            public string FORMAT { get; set; }
            public string GRIDID { get; set; }
            public string GRIDVIEW { get; set; }
            public int FROM { get; set; }
            public int HITS { get; set; }
            public string[] VISIBLE { get; set; }
            public List<PROFILE> PROFILE { get; set; }
            public string ORDERBY { get; set; }
        }

        public class PROFILE
        {
            public string VACANCY_FLAG { get; set; }
        }

        public class ErrorResult
        {
            public string ERROR { get; set; }
            public string STATUS { get; set; }
            public string Description { get; set; }
            public string AdditionalInfo { get; set; }
        }
    }
}
