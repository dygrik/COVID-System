using System;
using System.Collections.Generic;

namespace Assignment
{
    class SafeEntry
    {

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; } = new DateTime(1000, 01, 01);

        public BusinessLocation Location { get; set; }


        public SafeEntry() { }

        public SafeEntry(DateTime i, BusinessLocation l)
        {
            CheckIn = i;
            Location = l;
        }

        public void PerformCheckOut()
        {
            CheckOut = DateTime.Now;
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }

}
