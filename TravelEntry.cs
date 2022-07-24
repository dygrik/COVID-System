using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment
{
    class TravelEntry
    {
        public string LastCountryOfEmbarkation { get; set; }

        public string EntryMode { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime ShnEndDate { get; set; }

        public SHNFacility ShnStay { get; set; }

        public bool IsPaid { get; set; }

        public TravelEntry() { }

        public TravelEntry(string l, string m, DateTime ed)
        {
            LastCountryOfEmbarkation = l;
            EntryMode = m;
            EntryDate = ed;
        }

        public DateTime CalculateSHNDuration()
        {
            DateTime end = EntryDate;
            if (LastCountryOfEmbarkation == "New Zealand" || LastCountryOfEmbarkation == "Vietnam")
            {
                return end;
            }
            else if (LastCountryOfEmbarkation == "Macao SAR")
            {
                end = EntryDate.AddDays(7);
                return end;
            }
            else
            {
                end = EntryDate.AddDays(14);
                return end;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
