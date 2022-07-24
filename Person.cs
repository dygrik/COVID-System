using System;
using System.Collections.Generic;

namespace Assignment
{
	abstract class Person
	{
		public string Name { get; set; }

		public List<SafeEntry> SafeEntryList { get; set; } = new List<SafeEntry>();

		public List<TravelEntry> TravelEntryList { get; set; } = new List<TravelEntry>();



		public Person() { }

		public Person(string n)
		{
			Name = n;
			
		}

		public void AddTravelEntry(TravelEntry t)
		{
			TravelEntryList.Add(t);
		}

		public void AddSafeEntry(SafeEntry s)
		{
			SafeEntryList.Add(s);
		}

		public abstract double CalculateSHNCharges();
        

		public override string ToString()
		{
			return base.ToString();
		}
	}
}

