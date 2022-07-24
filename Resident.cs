using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment
{
    class Resident:Person
    {
        public string Address { get; set; }
        public DateTime LastLeftCountry { get; set; }
        public TraceTogetherToken Token { get; set; }
        public Resident(string n, string a, DateTime llc) : base(n)
        {
            Address = a;
            LastLeftCountry = llc;

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
                return fee;
            }
            else if (targetT.LastCountryOfEmbarkation == "Macao SAR")
            {
                return fee + 20;
            }
            else
            {
                return fee + 20 + 1000;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
