using System;
using System.Collections.Generic;

namespace Assignment
{
    class BusinessLocation
    {

        public string BusinessName { get; set; }

        public string BranchCode { get; set; }

        public int MaximumCapacity { get; set; }

        public int VisitorsNow { get; set; } = 0;

        public BusinessLocation() { }

        public BusinessLocation(string n, string b, int m)
        {
            BusinessName = n;
            BranchCode = b;
            MaximumCapacity = m;
        }

        public bool IsFull()
        {
            if (VisitorsNow==MaximumCapacity)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}


