using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class EOSTran
    {
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public DateTime ReqDate { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String LocLib1 { get; set; }
        [DataMember]
        public String LocLib2 { get; set; }
        [DataMember]
        public String LocLib3 { get; set; }
        [DataMember]
        public String LocLib4 { get; set; }
        [DataMember]
        public String LocLib5 { get; set; }
        [DataMember]
        public String SalProfile { get; set; }
        [DataMember]
        public String JobTitle { get; set; }
        [DataMember]
        public DateTime LastDayInService { get; set; }
        [DataMember]
        public Byte EndOfServiceType { get; set; }
        [DataMember]
        public DateTime ResignationDate { get; set; }
        [DataMember]
        public DateTime TerminationDate { get; set; }
        [DataMember]
        public String EOSRemarks { get; set; }
        [DataMember]
        public String Officialtreatment { get; set; }
        [DataMember]
        public DateTime LOCancDate { get; set; }
        [DataMember]
        public Byte LOBan { get; set; }
        [DataMember]
        public Int16 LOMonths { get; set; }
        [DataMember]
        public DateTime ICancDate { get; set; }
        [DataMember]
        public Byte IBan { get; set; }
        [DataMember]
        public Int16 IMonths { get; set; }
        [DataMember]
        public DateTime LeavingDate { get; set; }
        [DataMember]
        public DateTime SCReducingDate { get; set; }
        [DataMember]
        public String SCRegNo { get; set; }
        [DataMember]
        public DateTime LastActLeavingDate { get; set; }
        [DataMember]
        public DateTime EstRejoiningDate { get; set; }
        [DataMember]
        public Int16 DiffTillReqDate { get; set; }
        [DataMember]
        public DateTime LONotifyDate { get; set; }
        [DataMember]
        public String LONotifySrNo { get; set; }
        [DataMember]
        public Decimal BankAmt { get; set; }
        [DataMember]
        public String BankDetails { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public String EosTypeChar { get; set; }
        [DataMember]
        public String EOSReason { get; set; }
        [DataMember]
        public string EOSOffTreat { get; set; }
        [DataMember]
        public Boolean NoticeWrk { get; set; }
        [DataMember]
        public DateTime NoticeWrkDate { get; set; }
        [DataMember]
        public Byte SettleEnt { get; set; }

    }

    [DataContract]
    [Serializable]
    public class EOSTranViewAll
    {
        [DataMember]
        public long RowNo { get; set; }
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public Int32 NReqNo { get; set; }
        [DataMember]
        public DateTime ReqDate { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String LocLib1 { get; set; }
        [DataMember]
        public String LocLib2 { get; set; }
        [DataMember]
        public String LocLib3 { get; set; }
        [DataMember]
        public String LocLib4 { get; set; }
        [DataMember]
        public String LocLib5 { get; set; }
        [DataMember]
        public String Loc1DescE { get; set; }
        [DataMember]
        public String Loc2DescE { get; set; }
        [DataMember]
        public String Loc3DescE { get; set; }
        [DataMember]
        public String Loc4DescE { get; set; }
        [DataMember]
        public String Loc5DescE { get; set; }
        [DataMember]
        public String Loc1DescA { get; set; }
        [DataMember]
        public String Loc2DescA { get; set; }
        [DataMember]
        public String Loc3DescA { get; set; }
        [DataMember]
        public String Loc4DescA { get; set; }
        [DataMember]
        public String Loc5DescA { get; set; }
        [DataMember]
        public String SalProfile { get; set; }
        [DataMember]
        public String JobTitle { get; set; }
        [DataMember]
        public DateTime LastDayInService { get; set; }
        [DataMember]
        public Byte EndOfServiceType { get; set; }
        [DataMember]
        public DateTime ResignationDate { get; set; }
        [DataMember]
        public DateTime TerminationDate { get; set; }
        [DataMember]
        public String EOSRemarks { get; set; }
        [DataMember]
        public String Officialtreatment { get; set; }
        [DataMember]
        public DateTime LOCancDate { get; set; }
        [DataMember]
        public Byte LOBan { get; set; }
        [DataMember]
        public Int16 LOMonths { get; set; }
        [DataMember]
        public DateTime ICancDate { get; set; }
        [DataMember]
        public Byte IBan { get; set; }
        [DataMember]
        public Int16 IMonths { get; set; }
        [DataMember]
        public DateTime LeavingDate { get; set; }
        [DataMember]
        public DateTime SCReducingDate { get; set; }
        [DataMember]
        public String SCRegNo { get; set; }
        [DataMember]
        public DateTime LastActLeavingDate { get; set; }
        [DataMember]
        public DateTime EstRejoiningDate { get; set; }
        [DataMember]
        public Int16 DiffTillReqDate { get; set; }
        [DataMember]
        public DateTime LONotifyDate { get; set; }
        [DataMember]
        public String LONotifySrNo { get; set; }
        [DataMember]
        public Decimal BankAmt { get; set; }
        [DataMember]
        public String BankDetails { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public String ActiveStatusDesc { get; set; }
        [DataMember]
        public Int16 Status { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public String EosTypeChar { get; set; }
        [DataMember]
        public String EOSReason { get; set; }
        [DataMember]
        public string EOSOffTreat { get; set; }
        [DataMember]
        public String OffTreatE { get; set; }
        [DataMember]
        public String OffTreatA { get; set; }
        [DataMember]
        public String TypeDescE { get; set; }
        [DataMember]
        public String TypeDescA { get; set; }
        [DataMember]
        public String ReasonDescE { get; set; }
        [DataMember]
        public String ReasonDescA { get; set; }

        [DataMember]
        public String SalDescE { get; set; }
        [DataMember]
        public String SalDescA { get; set; }

        [DataMember]
        public String JobDescE { get; set; }
        [DataMember]
        public String JobDescA { get; set; }
        [DataMember]
        public String IsCalcDone { get; set; }
        [DataMember]
        public String IsFreezed { get; set; }
        [DataMember]
        public String EmpPhoto { get; set; }
        [DataMember]
        public Byte[] bEmpPhoto { get; set; }
        [DataMember]
        public DateTime JoiningDate { get; set; }
    }
}
