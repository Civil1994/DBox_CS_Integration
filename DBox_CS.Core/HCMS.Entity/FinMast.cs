using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class FinMast
    {
        [DataMember]
        public Int32 RecordNo { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public DateTime JoiningDate { get; set; }
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
        public String LocLib5A { get; set; }

        [DataMember]
        public String CategMast { get; set; }
        [DataMember]
        public String CategSec { get; set; }
        [DataMember]
        public String JobTitle { get; set; }
        [DataMember]
        public String SalProfile { get; set; }
        [DataMember]
        public String SalProfilePrev { get; set; }
        [DataMember]
        public String SalGrade { get; set; }
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
        public Byte ALType { get; set; }
        [DataMember]
        public Int16 NoOfDays { get; set; }
        [DataMember]
        public Int16 EWrkDays { get; set; }
        [DataMember]
        public Int16 ETicketEvery { get; set; }
        [DataMember]
        public Byte FTicketYN { get; set; }
        [DataMember]
        public Int16 FTicketEvery { get; set; }
        [DataMember]
        public Int16 NoOfFullTickets { get; set; }
        [DataMember]
        public Int16 NoOfChildTickets { get; set; }
        [DataMember]
        public Int16 NoOfInfantTickets { get; set; }
        [DataMember]
        public String RemarksE { get; set; }
        [DataMember]
        public String RemarksA { get; set; }
        [DataMember]
        public String RouteEmp { get; set; }
        [DataMember]
        public String RouteFam { get; set; }
        [DataMember]
        public String Attachment { get; set; }
        [DataMember]
        public Byte PymntType { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public Int16 FinFlag { get; set; }
        [DataMember]
        public Int16 ModChangeFlag { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public DateTime LastPaidDate { get; set; }
        [DataMember]
        public DateTime AttCloseDt { get; set; }
        [DataMember]
        public DateTime DerivedLPDT { get; set; }
        [DataMember]
        public Boolean FromClosed { get; set; }
        [DataMember]
        public String EmpIDChar { get; set; }
        [DataMember]
        public DateTime FirstFinEffdt { get; set; }
        [DataMember]
        public Decimal TotalInBasicCurr { get; set; }
        [DataMember]
        public Decimal TotalInOtherCurr { get; set; }
        [DataMember]
        public String OtherCurr { get; set; }
        [DataMember]
        public String ALCode { get; set; }
        [DataMember]  // Nitha Financial resubmit
        public Int32 SrNo { get; set; }

        #region "Ticket Screen Column"

        [DataMember]
        public string LocLib1E { get; set; }
        [DataMember]
        public string LocLib2E { get; set; }
        [DataMember]
        public string LocLib3E { get; set; }
        [DataMember]
        public string LocLib4E { get; set; }
        [DataMember]
        public string LocLib5E { get; set; } 
        [DataMember]
        public string EmployeeDestination { get; set; }
        [DataMember]
        public string EmployeeDestinationA { get; set; }
        [DataMember]
        public string FamilyDestination { get; set; }
        [DataMember]
        public string FamilyDestinationA { get; set; }
        [DataMember]
        public string SalaryProfile { get; set; }
        [DataMember]
        public Byte Sex { get; set; }

        [DataMember]
        public string EmpCode { get; set; }
        [DataMember]
        public string EmpNameE { get; set; }
        #endregion

    }

    public class FinMastViewAll
    { 
        [DataMember]
        public Int64 RowNo { get; set; }
        [DataMember]
        public Int32 RecordNo { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public string EncryptedEmpId { get; set; }
        [DataMember]
        public String EmpCode { get; set; }
        [DataMember]
        public String EmpNameE { get; set; }
        [DataMember]
        public String FamTicketYN { get; set; }
        [DataMember]
        public String EmpNameA { get; set; }
        [DataMember]
        public String ActiveStatusDesc { get; set; }
        [DataMember]
        public DateTime JoiningDate { get; set; }
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
        public String LocLib5A { get; set; }
        [DataMember]
        public String CatMast { get; set; }
        [DataMember]
        public String CatSec { get; set; }

        [DataMember]
        public String CatMastA { get; set; }
        [DataMember]
        public String CatSecA { get; set; }


        [DataMember]
        public String JobTitle { get; set; }
        [DataMember]
        public String SalProfile { get; set; }
        [DataMember]
        public String SalProfilePrev { get; set; }
        [DataMember]
        public String SalGrade { get; set; }


        [DataMember]
        public String SalGradeA { get; set; }

        [DataMember]
        public String BSalaryCurr { get; set; }
        [DataMember]
        public String BSalCurrDescE { get; set; }
        [DataMember]
        public String BSalCurrDescA { get; set; }
        [DataMember]
        public Decimal BSalaryAmt { get; set; }
        [DataMember]
        public String BSalAmtCurr { get; set; }
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
        public Byte ALType { get; set; }
        [DataMember]
        public Int16 NoOfDays { get; set; }
        [DataMember]
        public Int16 EWrkDays { get; set; }
        [DataMember]
        public Int16 ETicketEvery { get; set; }
        [DataMember]
        public Byte FTicketYN { get; set; }
        [DataMember]
        public Int16 FTicketEvery { get; set; }
        [DataMember]
        public Int16 NoOfFullTickets { get; set; }
        [DataMember]
        public Int16 NoOfChildTickets { get; set; }
        [DataMember]
        public Int16 NoOfInfantTickets { get; set; }
        [DataMember]
        public String RemarksE { get; set; }
        [DataMember]
        public String RemarksA { get; set; }
        [DataMember]
        public String RouteEmp { get; set; }
        [DataMember]
        public String RouteFam { get; set; }
        [DataMember]
        public String Attachment { get; set; }
        [DataMember]
        public Byte PymntType { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public Int16 FinFlag { get; set; }
        [DataMember]
        public Int16 ModChangeFlag { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public DateTime LastPaidDate { get; set; }
        [DataMember]
        public DateTime AttCloseDt { get; set; }
        [DataMember]
        public DateTime DerivedLPDT { get; set; }
        [DataMember]
        public Boolean FromClosed { get; set; }
        [DataMember]
        public String EmpIDChar { get; set; }
        [DataMember]
        public DateTime FirstFinEffdt { get; set; }
        [DataMember]
        public Decimal TotalInBasicCurr { get; set; }
        [DataMember]
        public Decimal TotalInOtherCurr { get; set; }
        [DataMember]
        public String OtherCurr { get; set; }
        [DataMember]
        public String ALCode { get; set; }
        [DataMember]
        public String ALCodeA { get; set; }

        [DataMember]
        public String EmpPhoto { get; set; }
        [DataMember]
        public Byte[] bEmpPhoto { get; set; }
        [DataMember]
        public String JobTitleA { get; set; }

          [DataMember]
        public String SalProfileA { get; set; }
        
    }

    //May 17 2021 - Added by Seetha for Taxation basic salary changes 
    [DataContract]
    [Serializable]
    public class FinTaxBasic
    {
        [DataMember]
        public Int32 RecordNo { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public String BSalaryTaxCurr { get; set; }
        [DataMember]
        public Decimal BSalaryTaxAmt { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
    }

    //17082021 - Added by Seetha for Finmast Percentage changes 
    [DataContract]
    [Serializable]
    public class FinMastPer
    {
        [DataMember]
        public Int32 RecordNo { get; set; }
        [DataMember]
        public Int32 EmpID { get; set; }
        [DataMember]
        public Decimal BSalaryAmt { get; set; }
        [DataMember]
        public Decimal HRAPerc { get; set; }
        [DataMember]
        public Decimal TranPerc { get; set; }
        [DataMember]
        public Decimal FoodPerc { get; set; }
        [DataMember]
        public Decimal AuxAll1Perc { get; set; }
        [DataMember]
        public Decimal AuxAll2Perc { get; set; }
        [DataMember]
        public Decimal AuxAll3Perc { get; set; }
        [DataMember]
        public Decimal AuxAll4Perc { get; set; }
        [DataMember]
        public Decimal AuxAll5Perc { get; set; }
        [DataMember]
        public Decimal AuxAll6Perc { get; set; }
        [DataMember]
        public Decimal AuxAll7Perc { get; set; }
        [DataMember]
        public Decimal AuxAll8Perc { get; set; }
        [DataMember]
        public DateTime LastModDateTime { get; set; }
        [DataMember]
        public Int32 ReqID { get; set; }
        [DataMember]
        public Byte Status { get; set; }
        [DataMember]
        public Int16 ActiveStatus { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}
