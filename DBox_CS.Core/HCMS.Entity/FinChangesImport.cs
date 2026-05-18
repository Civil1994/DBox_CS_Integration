using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.HCMS.Entity
{
    [DataContract]
    [Serializable]
    public class FinChangesImport
    {
        public FinChangesImport()
        {
            ECode = "";
            EffDate = "";
            Basic = "";
            HRA = "";
            Trans = "";
            Food = "";
            Aux1 = "";
            Aux2 = "";
            Aux3 = "";
            Aux4 = "";
            Aux5 = "";
            Aux6 = "";
            Aux7 = "";
            Aux8 = "";
            JobTitle = "";
            Grade = "";
            CategMaster = "";
            CategSecondary = "";
            Location = "";
            ETicketEvery = "";
            ETicketRoute = "";
            EntitledToFamilyTicket = "";
            FTicketEvery = "";
            FTicketRoute = "";
            NoOfFullTickets = "";
            NoOfChildTickets = "";
            NoOfInFantTickets = "";
            AnnualLeaveType = "";
            HraBy = "";
            TransBy = "";
            FoodBy = "";
            Aux1By = "";
            Aux2By = "";
            Aux3By = "";
            Aux4By = "";
            Aux5By = "";
            Aux6By = "";
            Aux7By = "";
            Aux8By = "";
            SalProfile = "";
            Document = "";
            ActionReason = "";
        }
        [DataMember]
        public string ECode { get; set; }
        [DataMember]
        public string EffDate { get; set; }
        [DataMember]
        public string Basic { get; set; }
        [DataMember]
        public string HRA { get; set; }
        [DataMember]
        public string Trans { get; set; }
        [DataMember]
        public string Food { get; set; }
        [DataMember]
        public string Aux1 { get; set; }
        [DataMember]
        public string Aux2 { get; set; }
        [DataMember]
        public string Aux3 { get; set; }
        [DataMember]
        public string Aux4 { get; set; }
        [DataMember]
        public string Aux5 { get; set; }
        [DataMember]
        public string Aux6 { get; set; }
        [DataMember]
        public string Aux7 { get; set; }
        [DataMember]
        public string Aux8 { get; set; }
        [DataMember]
        public string JobTitle { get; set; }
        [DataMember]
        public string Grade { get; set; }
        [DataMember]
        public string CategMaster { get; set; }
        [DataMember]
        public string CategSecondary { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public string ETicketEvery { get; set; }
        [DataMember]
        public string ETicketRoute { get; set; }
        [DataMember]
        public string EntitledToFamilyTicket { get; set; }
        [DataMember]
        public string FTicketEvery { get; set; }
        [DataMember]
        public string FTicketRoute { get; set; }
        [DataMember]
        public string NoOfFullTickets { get; set; }
        [DataMember]
        public string NoOfChildTickets { get; set; }
        [DataMember]
        public string NoOfInFantTickets { get; set; }
        [DataMember]
        public string AnnualLeaveType { get; set; }
        [DataMember]
        public string HraBy { get; set; }
        [DataMember]
        public string TransBy { get; set; }
        [DataMember]
        public string FoodBy { get; set; }
        [DataMember]
        public string Aux1By { get; set; }
        [DataMember]
        public string Aux2By { get; set; }
        [DataMember]
        public string Aux3By { get; set; }
        [DataMember]
        public string Aux4By { get; set; }
        [DataMember]
        public string Aux5By { get; set; }
        [DataMember]
        public string Aux6By { get; set; }
        [DataMember]
        public string Aux7By { get; set; }
        [DataMember]
        public string Aux8By { get; set; }
        [DataMember]
        public string SalProfile { get; set; }
        [DataMember]
        public string Document { get; set; }
        [DataMember]
        public string ActionReason { get; set; }
    }
}
