using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment
{
    class Visitor:Person
    {
        public string PassportNo { get; set; }
        public string Nationality { get; set; }
        public Visitor(string n, string no, string nat) : base(n)
        {
            PassportNo = no;
            Nationality = nat;
        }

        public override double CalculateSHNCharges()
        {
            TravelEntry targetT = new TravelEntry();
            double fee = 200;
            foreach (TravelEntry t in TravelEntryList)
            {
                if (t.IsPaid == false && DateTime.Now > t.ShnEndDate)
                {
                    targetT = t;
                    break;
                }
            }
            if (targetT.LastCountryOfEmbarkation == "Vietnam" || targetT.LastCountryOfEmbarkation == "New Zealand")
            {
                return fee + 80;
            }
            else if (targetT.LastCountryOfEmbarkation == "Macao SAR")
            {
                return fee + 80;
            }
            else
            {
                return fee + 2000;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
