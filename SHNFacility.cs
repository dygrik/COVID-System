using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment
{
    class SHNFacility
    {
        public string FacilityName { get; set; }

        public int FacilityCapacity { get; set; }

        public int FacilityVacancy { get; set; } 

        public double DistFromAirCheckpoint { get; set; }

        public double DistFromSeaCheckpoint { get; set; }

        public double DistFromLandCheckpoint { get; set; }

        public SHNFacility() { }

        public SHNFacility(string n, int c, double a, double s, double l)
        {
            FacilityName = n;
            FacilityCapacity = c;
            DistFromAirCheckpoint = a;
            DistFromSeaCheckpoint = s;
            DistFromLandCheckpoint = l;
        }

        public double CalculateTravelCost(string entryMode, DateTime entryDate)
        {
            double total = 50;
            if (entryMode == "Air")
            {
                total += DistFromAirCheckpoint * 0.22;
            }
            else if (entryMode == "Sea")
            {
                total += DistFromSeaCheckpoint * 0.22;
            }
            else
            {
                total += DistFromLandCheckpoint * 0.22;
            }
            if (entryDate.Hour >= 6 && entryDate.Hour < 9)
            {
                total = total * 1.25;
            }
            else if (entryDate.Hour >= 18 && entryDate.Hour < 0)
            {
                total = total * 1.25;
            }
            else if (entryDate.Hour >= 0 && entryDate.Hour < 6)
            {
                total = total * 1.5;
            }
            return total;
        }

        public bool IsAvailable()
        {
            if (FacilityVacancy > 0)
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
