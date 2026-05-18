using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBox_CS.Core.AppClass
{
    public class CSApprovalData
    {
        //26-06-2022: Robin Added class, copy of HCMS\Areas\EApproval\Old_App_Code\CSApprovalData.vb
        //Created for resolving financial bypass case 

        public int m_nViewNo;
        public int m_lReqNo;
        public int m_lEmpID;
        public string m_sEmpCode;
        public string m_sEmpName;
        public string m_sApp;
        public string m_sAppDate;
        public byte m_byNoOfAppr;
        public byte m_byStatus;
        public string m_sModuleTable;
        public string m_sDocAttach;
        public byte m_byAsGroup;
        public int m_nGroupNo;
        public string m_sCodeName;    // Nishad Added 25012017
        public string m_sWFCode;
        public int m_nSeqNo;

        public static string m_sDeputID;
        public static ArrayList m_saOrgID;

        public static string m_AltApproverID;
        public static string m_origApproverID;
    }
}