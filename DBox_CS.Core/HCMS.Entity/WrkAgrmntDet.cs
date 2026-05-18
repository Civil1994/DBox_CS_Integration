using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class WrkAgrmntDet
    {
        [DataMember]
        public Int32 ReqNo { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public string EncryptedEmpId { get; set; }
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
        public String WrkAgreeNo { get; set; }
        [DataMember]
        public Byte WrkAgreeType { get; set; }
        [DataMember]
        public DateTime WrkAgrStartDt { get; set; }
        [DataMember]
        public DateTime WrkAgrExpDt { get; set; }
        [DataMember]
        public String Profession { get; set; }
        [DataMember]
        public String BSalaryCurr { get; set; }
        [DataMember]
        public Decimal BSalaryAmt { get; set; }
        [DataMember]
        public String HRABy { get; set; }
        [DataMember]
        public String HRACurr { get; set; }
        [DataMember]
        public Decimal HRAAmt { get; set; }
        [DataMember]
        public Byte TranBy { get; set; }
        [DataMember]
        public String TranCurr { get; set; }
        [DataMember]
        public Decimal TranAmt { get; set; }
        [DataMember]
        public Byte FoodBy { get; set; }
        [DataMember]
        public String FoodCurr { get; set; }
        [DataMember]
        public Decimal FoodAmt { get; set; }
        [DataMember]
        public Byte AuxAll1By { get; set; }
        [DataMember]
        public String AuxAll1Curr { get; set; }
        [DataMember]
        public Decimal AuxAll1Amt { get; set; }
        [DataMember]
        public Byte AuxAll2By { get; set; }
        [DataMember]
        public String AuxAll2Curr { get; set; }
        [DataMember]
        public Decimal AuxAll2Amt { get; set; }
        [DataMember]
        public Byte AuxAll3By { get; set; }
        [DataMember]
        public String AuxAll3Curr { get; set; }
        [DataMember]
        public Decimal AuxAll3Amt { get; set; }
        [DataMember]
        public Byte AuxAll4By { get; set; }
        [DataMember]
        public String AuxAll4Curr { get; set; }
        [DataMember]
        public Decimal AuxAll4Amt { get; set; }
        [DataMember]
        public Byte AuxAll5By { get; set; }
        [DataMember]
        public String AuxAll5Curr { get; set; }
        [DataMember]
        public Decimal AuxAll5Amt { get; set; }
        [DataMember]
        public Byte AuxAll6By { get; set; }
        [DataMember]
        public String AuxAll6Curr { get; set; }
        [DataMember]
        public Decimal AuxAll6Amt { get; set; }
        [DataMember]
        public Byte AuxAll7By { get; set; }
        [DataMember]
        public String AuxAll7Curr { get; set; }
        [DataMember]
        public Decimal AuxAll7Amt { get; set; }
        [DataMember]
        public Byte AuxAll8By { get; set; }
        [DataMember]
        public String AuxAll8Curr { get; set; }
        [DataMember]
        public Decimal AuxAll8Amt { get; set; }
        [DataMember]
        public String OtherConditionE { get; set; }
        [DataMember]
        public String OtherConditionA { get; set; }
        [DataMember]
        public string NoticeByEmp { get; set; }
        [DataMember]
        public string NoticeByComp { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public Byte RiseMonths { get; set; }
        [DataMember]
        public String RiseMonthsStr { get; set; }
        [DataMember]
        public Int16 ProbationPeriod { get; set; }
        [DataMember]
        public String ProbationPeriodStr { get; set; }
        [DataMember]
        public Int16 NoticeDayByEmp { get; set; }
        [DataMember]
        public Int16 NoticeDayByComp { get; set; }
        [DataMember]
        public String NoticeIn { get; set; }
        [DataMember]
        public Boolean ConfirmedYN { get; set; }
        [DataMember]
        public String DurationIn { get; set; }
        [DataMember]
        public Double ContDuration { get; set; }

        //For View All
        [DataMember]
        public Int16 PPNoticeByEmp { get; set; }
        [DataMember]
        public Int16 PPNoticeByCmp { get; set; }

        //For View All
        [DataMember]
        public Int64 RowNo { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String WrkAgreeTypeDesc { get; set; }
        [DataMember]
        public String NoticeInDesc { get; set; }
        [DataMember]
        public String ActiveStatusDesc { get; set; }
        [DataMember]
        public Byte EmployeeStatus { get; set; }

        public String EmpPhoto { get; set; }
        [DataMember]
        public Byte[] bEmpPhoto { get; set; }

        [DataMember]
        public String DurationInDesc { get; set; }
    }
}