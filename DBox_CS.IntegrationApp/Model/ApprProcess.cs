using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SF_CS.IntegrationApp.Model
{
    [DataContract]
    [Serializable]
    public class ApprProcess
    {
        [DataMember]
        public Byte Priority { get; set; }
        [DataMember]
        public Int16 ViewNo { get; set; }
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public DateTime RequestDate { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String Isl { get; set; }
        [DataMember]
        public String App { get; set; }
        [DataMember]
        public String AppDate { get; set; }
        [DataMember]
        public Byte NoOfAppr { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public String Remarks { get; set; }
        [DataMember]
        public String DocAttach { get; set; }
        [DataMember]
        public Boolean OnHold { get; set; }
        [DataMember]
        public Int32 HoldUserNo { get; set; }
        [DataMember]
        public Boolean Deleted { get; set; }
        [DataMember]
        public Boolean Returned { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 LockedByUser { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public String NextApprAuth { get; set; }
        [DataMember]
        public Byte AsGroup { get; set; }
        [DataMember]
        public Int32 GroupNo { get; set; }
        [DataMember]
        public Byte Selected { get; set; }
        [DataMember]
        public Byte Bypassed { get; set; }
        [DataMember]
        public Int32 ReturnedUserNo { get; set; }
        [DataMember]
        public String Isla { get; set; }
        [DataMember]
        public String WFCode { get; set; }

    }

    [DataContract]
    [Serializable]
    public class ApprovalAuthorityDetails
    {
        [DataMember]
        public String Icon { get; set; }
        [DataMember]
        public String ApprID { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String ApprDate { get; set; }
        [DataMember]
        public String IconHand { get; set; }
        [DataMember]
        public String Status { get; set; }
        [DataMember]
        public String Change { get; set; }
        [DataMember]
        public int ApprovalLevel { get; set; }
        [DataMember]
        public String DeliveredDate { get; set; }
        [DataMember]
        public String SeenDate { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ApprProcessDetail
    {
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String UserNameE { get; set; }
        [DataMember]
        public String UserNameA { get; set; }
        [DataMember]
        public ApprProcess oApprProcess { get; set; }
        [DataMember]
        public Object oObject { get; set; }
        [DataMember]
        public List<ApprovalAuthorityDetails> oApprovalAuthorityDetails { get; set; }

    }

    [DataContract]
    [Serializable]
    public class ApprProcessLocAddDetails
    {
        [DataMember]
        public String IslLoc { get; set; }
        [DataMember]
        public String IslaLoc { get; set; }
        [DataMember]
        public String Isl { get; set; }
        [DataMember]
        public String Isla { get; set; }
        [DataMember]
        public String App { get; set; }
        [DataMember]
        public String AppDate { get; set; }
        [DataMember]
        public Byte NoOfAppr { get; set; }
        [DataMember]
        public String NextApprAuth { get; set; }
        public String AppLoc { get; set; }
        [DataMember]
        public String AppDateLoc { get; set; }
        [DataMember]
        public Byte NoOfApprLoc { get; set; }
        [DataMember]
        public String NextApprAuthLoc { get; set; }


    }

    [DataContract]
    [Serializable]
    public class ApprProcessMsgInfoDetails
    {
        [DataMember]
        public Int16 ViewNo { get; set; }
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public DateTime RequestDate { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String App { get; set; }
        [DataMember]
        public String AppDate { get; set; }
        [DataMember]
        public Byte NoOfAppr { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public string m_sDeliveredDate { get; set; }
        [DataMember]
        public string m_sSeenDate { get; set; }
        [DataMember]
        public List<ApprovalMsgDateDetails> oApprovalMsgDateDetails { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ApprovalMsgDateDetails
    {
        [DataMember]
        public String ApprID { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String ApprDate { get; set; }
        [DataMember]
        public String Status { get; set; }
        [DataMember]
        public int ApprovalLevel { get; set; }
        [DataMember]
        public String DeliveredDate { get; set; }
        [DataMember]
        public String SeenDate { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ApprRepHierarchyAppDetails
    {
        [DataMember]
        public Int16 ViewNo { get; set; }
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public DateTime RequestDate { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String App { get; set; }
        [DataMember]
        public String AppDate { get; set; }
        [DataMember]
        public Byte NoOfAppr { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public Int32 GroupNo { get; set; }
    }
}
