//================================================
// Student Number : S10203107, S10206196
// Student Name   : Chua Che Khai, Dylan Sim Jing Ren
// Module Group   : P06
//================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace Assignment
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welome to your friendly neighbourhood COVID Protector!");
            Console.WriteLine("");
            //Initiating facilities, person and business lists
            List<SHNFacility> facilityList = InitFacility();
            List<Person> personList = InitPersonList(facilityList);
            List<BusinessLocation> locationList = InitBusinessList();

            //driver code
            while (true)
            {
                int choice = 0;
                DisplayMenu();
                Console.Write("Enter your choice: ");
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                }

                catch (System.FormatException)
                {
                    Console.WriteLine("Error! Not a number.");
                    continue;
                }

                catch(System.OverflowException)
                {
                    Console.WriteLine("That's a huge number. Are you trying to crash the program?");
                    continue;
                }

                if (choice == 1)
                {
                    facilityList = InitFacility();
                    personList = InitPersonList(facilityList);
                    locationList = InitBusinessList();
                    Console.WriteLine();
                }

                else if (choice == 2)
                {
                    facilityList = InitFacility();
                    personList = InitPersonList(facilityList);
                    locationList = InitBusinessList();
                    Console.WriteLine();
                }

                else if (choice == 3)
                {
                    ListVisitors(personList);
                }

                else if (choice == 4)
                {
                    DisplayPersonDetails(personList);
                }

                else if (choice == 5)
                {
                    AssignToken(personList);
                }

                else if (choice == 6)
                {
                    DisplayBusinessLocations(locationList);
                }

                else if (choice == 7)
                {
                    EditLocationCapacity(locationList);
                }

                else if (choice == 8)
                {
                    SafeEntryCheckIn(personList, locationList);
                }

                else if (choice == 9)
                {
                    SafeEntryCheckOut(personList, locationList);
                }

                else if (choice == 10)
                {
                    DisplayFacility(facilityList);
                }

                else if (choice == 11)
                {
                    CreateVisitor(personList);
                    Console.WriteLine();
                }

                else if (choice == 12)
                {
                    CreateTravelEntry(personList, facilityList);
                }

                else if (choice == 13)
                {
                    CalculateCharges(personList);
                }

                else if (choice == 14)
                {
                    ContactTracing(personList, locationList);
                }

                else if (choice == 15)
                {
                    StatusReport(personList);
                }
                else if (choice==16)
                {
                    AdvancedContactTracing(personList, locationList);
                }

                else if (choice == 0)
                {
                    Console.WriteLine("Exiting...");
                    break;
                }

            }



        }

        //Menu
        static void DisplayMenu()
        {
            Console.WriteLine("==============Menu==============");
            Console.WriteLine("[1] Load Person and Business Location Data");
            Console.WriteLine("[2] Load SHN Facility Data");
            Console.WriteLine("[3] List All Visitors");
            Console.WriteLine("[4] List Person Details");
            Console.WriteLine();
            Console.WriteLine("=====SafeEntry/TraceTogether=====");
            Console.WriteLine("[5] Assign/Replace TraceTogether Token");
            Console.WriteLine("[6] List All Business Locations");
            Console.WriteLine("[7] Edit Business Location Capacity");
            Console.WriteLine("[8] SafeEntry Check-In");
            Console.WriteLine("[9] SafeEntry Check-Out");
            Console.WriteLine();
            Console.WriteLine("===========TravelEntry===========");
            Console.WriteLine("[10] List All SHN Facilities");
            Console.WriteLine("[11] Create Visitor");
            Console.WriteLine("[12] Create TravelEntry Record");
            Console.WriteLine("[13] Calculate SHN Charges");
            Console.WriteLine();
            Console.WriteLine("============Advanced============");
            Console.WriteLine("[14] Contract Tracing Reporting (ADVANCED)");
            Console.WriteLine("[15] SHN Status Reporting (ADVANCED)");
            Console.WriteLine("[16] Advanced Contact Tracing (ADDITIONAL FEATURE)");
            Console.WriteLine("[0] Exit");
            Console.WriteLine();
        }

        static List<Person> InitPersonList(List<SHNFacility> fList)
        {
            string[] all = File.ReadAllLines("csv/Person.csv");
            List<Person> personList = new List<Person>();
            for (int i = 1; i < all.Length; i++)
            {
                string[] line = all[i].Split(",");

                //depending on what type of person the current line is, there are different blank values. This checks each type to ensure no errors pop up.
                if (line[0] == "resident")
                {
                    DateTime lastLeftCountry = Convert.ToDateTime(line[3]);
                    Resident tempResident = new Resident(line[1], line[2], lastLeftCountry);

                    //checks if resident has token. If not, ignores line[6] to line[8]
                    if (line[6] != "")
                    {
                        DateTime tokenExpiry = Convert.ToDateTime(line[8]);
                        TraceTogetherToken tempToken = new TraceTogetherToken(line[6], line[7], tokenExpiry);
                        tempResident.Token = tempToken;
                    }

                    //same as token part. 
                    if (line[9] != "")
                    {
                        TravelEntry tempTravelEntry = InitTravelEntry(fList, line);
                        tempResident.AddTravelEntry(tempTravelEntry);
                    }
                    personList.Add(tempResident);


                }

                else if (line[0] == "visitor")
                {
                    Visitor tempVisitor = new Visitor(line[1], line[4], line[5]);
                    if (line[9] != "")
                    {
                        TravelEntry tempTravelEntry = InitTravelEntry(fList, line);
                        tempVisitor.AddTravelEntry(tempTravelEntry);
                    }
                    personList.Add(tempVisitor);
                }

            }

            return personList;
        }

        static TravelEntry InitTravelEntry(List<SHNFacility> fList, string[] line)
        {
            //Retrieves data from the csv file and converting to the appropriate formats
            
            DateTime travelEntryDate = Convert.ToDateTime(line[11]);
            DateTime travelEndDate = Convert.ToDateTime(line[12]);
            TravelEntry tempSafeEntry = new TravelEntry(line[9], line[10], travelEntryDate);
            tempSafeEntry.ShnEndDate = travelEndDate;
            tempSafeEntry.IsPaid = Convert.ToBoolean(line[13]);
            SHNFacility stayingAt = SearchFacility(fList, line[14]);
            if (line[14] != "")
            {
                stayingAt.FacilityVacancy -= 1;
            }
            tempSafeEntry.ShnStay = stayingAt;

            return tempSafeEntry;
        }

        static SHNFacility SearchFacility(List<SHNFacility> fList, string fName)
        {
            //Searches for facilities based on data from the csv file
            foreach (SHNFacility f in fList)
            {
                if (f.FacilityName == fName)
                {
                    return f;
                }
            }
            return null;
        }

        static List<BusinessLocation> InitBusinessList()
        {
            string[] all = File.ReadAllLines("csv/BusinessLocation.csv");
            List<BusinessLocation> locationList = new List<BusinessLocation>();
            for (int i = 1; i < all.Length; i++)
            {
                string[] line = all[i].Split(",");
                BusinessLocation tempLocation = new BusinessLocation(line[0], line[1], Convert.ToInt32(line[2]));
                locationList.Add(tempLocation);
            }
            return locationList;

        }

        static void ListVisitors(List<Person> pList)
        {
            Console.WriteLine("{0,-15}{1,-20}{2,-1}", "Name", "Passport Number", "Nationality");
            for (int i = 0; i < pList.Count; i++)
            {
                //checks if current person is a visitor.
                if (pList[i].ToString() == "Assignment.Visitor")
                {
                    Visitor v = (Visitor)pList[i];
                    Console.WriteLine("{0,-15}{1,-20}{2,-1}", v.Name, v.PassportNo, v.Nationality);
                }

                else
                {
                    continue;
                }
            }

            Console.WriteLine();
        }

        static void ListResidents(List<Person> pList)
        {
            Console.WriteLine("{0,-15}{1,-1}", "Name", "Token Expiry Date");
            for (int i = 0; i < pList.Count; i++)
            {
                if (pList[i].ToString() == "Assignment.Resident")
                {
                    Resident r = (Resident)pList[i];
                    //checks if resident has token. If not, prints "No Token" instead of a blank space.
                    if (r.Token == null)
                    {
                        Console.WriteLine("{0,-15}{1,-1}", r.Name, "No Token");
                    }

                    else
                    {
                        Console.WriteLine("{0,-15}{1,-1}", r.Name, r.Token.ExpiryDate.ToString("dd/MM/yyyy"));
                    }

                    
                }

                else
                {
                    continue;
                }
            }

            Console.WriteLine();
        }



        static List<SHNFacility> InitFacility()
        {
            List<SHNFacility> facilityList = new List<SHNFacility>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://covidmonitoringapiprg2.azurewebsites.net");
                Task<HttpResponseMessage> responseTask = client.GetAsync("/facility");
                responseTask.Wait();
                HttpResponseMessage result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Task<string> readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    string data = readTask.Result;
                    facilityList = JsonConvert.DeserializeObject<List<SHNFacility>>(data);
                    //
                    foreach (SHNFacility f in facilityList)
                    {
                        f.FacilityVacancy = f.FacilityCapacity;
                    }
                }

            }
            Console.WriteLine("SHN Facilities have been initiated successfully!");
            return facilityList;
        }

        static void DisplayFacility(List<SHNFacility> facilityList)
        {
            Console.WriteLine("{0,-21} {1,-13} {2,-8} {3,-33} {4,-33} {5,-34}", "Facility Name", "Capacity", "Vacancy", "Distance From Air Checkpoint", "Distance From Sea Checkpoint", "Distance From Land Checkpoint");
            foreach (SHNFacility f in facilityList)
            {
                Console.WriteLine("{0,-21} {1,-13} {2,-8} {3,-33} {4,-33} {5,-34}", f.FacilityName, f.FacilityCapacity, f.FacilityVacancy, f.DistFromAirCheckpoint, f.DistFromSeaCheckpoint, f.DistFromLandCheckpoint);
            }
        }

        static void DisplayPersonDetails(List<Person> pList)
        {
            while (true)
            {
                DisplayNames(pList);
                Console.Write("Enter name of person: ");
                string targetname = Console.ReadLine();
                for (int i = 0; i < pList.Count; i++)
                {
                    
                    if (pList[i].Name == targetname)
                    {
                        //since person can be either visitor or resident, this checks for type to print different information. 
                        if (pList[i].ToString() == "Assignment.Visitor")
                        {
                            Visitor v = (Visitor)pList[i];
                            Console.WriteLine("\nVisitor Details: ");
                            Console.WriteLine("{0,-15}{1,-20}{2,-1}", "Name", "Passport Number", "Nationality");
                            Console.WriteLine("{0,-15}{1,-20}{2,-1}", v.Name, v.PassportNo, v.Nationality);
                            List<TravelEntry> tList = v.TravelEntryList;
                            List<SafeEntry> sList = v.SafeEntryList;

                            //only triggers if travel entry list is not empty. 
                            if (tList.Count > 0)
                            {
                                Console.WriteLine("\nTravelEntry Details: ");
                                Console.WriteLine("{0,-30}{1,-15}{2,-20}{3,-20}{4,-1}", "Last Country of Embarkation", "Entry Mode", "Entry Date", "SHN End Date", "Is Paid");
                                foreach (TravelEntry t in tList)
                                {
                                    Console.WriteLine("{0,-30}{1,-15}{2,-20}{3,-20}{4,-1}", t.LastCountryOfEmbarkation, t.EntryMode, t.EntryDate.ToString("dd/MM/yyyy"), t.ShnEndDate.ToString("dd/MM/yyyy"), t.IsPaid);
                                }
                            }

                            //only triggers if safe entry list is not empty. 
                            if (sList.Count>0)
                            {
                                Console.WriteLine("\nSafeEntry Details: ");
                                Console.WriteLine("{0,-30}{1,-30}{2,-20}{3,-1}", "Check In", "Check Out", "Business Name", "Branch Code");
                                
                                foreach (SafeEntry s in sList)
                                {
                                    //default date of checkout is 1/1/1000, which is basically a null value. Prints "not checked out" instead if it is a 'null' value.
                                    if (s.CheckOut.Year==1000)
                                    {
                                     
                                        Console.WriteLine("{0,-30}{1,-30}{2,-20}{3,-1}", s.CheckIn, "Not Checked Out", s.Location.BusinessName, s.Location.BranchCode);
                                    }

                                    else
                                    {
                                        Console.WriteLine("{0,-30}{1,-30}{2,-20}{3,-1}", s.CheckIn, s.CheckOut, s.Location.BusinessName, s.Location.BranchCode);
                                    }
                                    
                                }

                            }
                            Console.WriteLine();
                            return;


                        }

                        //this section is very similar to visitor, just different initial attributes to print.
                        else if (pList[i].ToString() == "Assignment.Resident")
                        {
                            Resident r = (Resident)pList[i];
                            Console.WriteLine("\nResident Details: ");
                            Console.WriteLine("{0,-20}{1,-30}{2,-1}", "Name", "Address", "Last Left Country");
                            Console.WriteLine("{0,-20}{1,-30}{2,-1}", r.Name, r.Address, r.LastLeftCountry.ToString("dd/MM/yyyy"));
                            List<TravelEntry> tList = r.TravelEntryList;
                            List<SafeEntry> sList = r.SafeEntryList;
                            TraceTogetherToken token = r.Token;

                            if (tList.Count > 0)
                            {
                                Console.WriteLine("\nTravelEntry Details: ");
                                Console.WriteLine("{0,-30}{1,-15}{2,-20}{3,-20}{4,-1}", "Last Country of Embarkation", "Entry Mode", "Entry Date", "SHN End Date", "Is Paid");
                                foreach (TravelEntry t in tList)
                                {
                                    Console.WriteLine("{0,-30}{1,-15}{2,-20}{3,-20}{4,-1}", t.LastCountryOfEmbarkation, t.EntryMode, t.EntryDate.ToString("dd/MM/yyyy"), t.ShnEndDate.ToString("dd/MM/yyyy"), t.IsPaid);
                                }
                            }

                            if (sList.Count > 0)
                            {
                                Console.WriteLine("\nSafeEntry Details: ");
                                Console.WriteLine("{0,-30}{1,-30}{2,-20}{3,-1}", "Check In", "Check Out", "Business Name", "Branch Code");
                                foreach (SafeEntry s in sList)
                                {
                                    if (s.CheckOut.Year == 1000)
                                    {

                                        Console.WriteLine("{0,-30}{1,-30}{2,-20}{3,-1}", s.CheckIn, "Not Checked Out", s.Location.BusinessName, s.Location.BranchCode);
                                    }

                                    else
                                    {
                                        Console.WriteLine("{0,-30}{1,-30}{2,-20}{3,-1}", s.CheckIn, s.CheckOut, s.Location.BusinessName, s.Location.BranchCode);
                                    }
                                   
                                }

                            }

                            //only triggers if resident has token. 
                            if (token != null)
                            {
                                Console.WriteLine("\nTrace Together Token Details: ");
                                Console.WriteLine("{0,-20}{1,-35}{2,-1}", "Serial Number", "Collection Location", "Expiry Date");
                                Console.WriteLine("{0,-20}{1,-35}{2,-1}", token.SerialNo, token.CollectionLocation, token.ExpiryDate.ToString("dd/MM/yyyy"));

                            }

                            Console.WriteLine();
                            return;
                        }

                        break;
                    }
                }
                //this line is only reachable if the person is not found, as when it is found the loop breaks.
                Console.WriteLine("Name not found. Please try again.");
            }
        }

        static void DisplayBusinessLocations(List<BusinessLocation> bList)
        {
            Console.WriteLine("{0,-25}{1,-15}{2,-20}{3,-1}", "Business Name", "Branch Code", "Maximum Capacity", "Visitors Now");

            foreach (BusinessLocation b in bList)
            {
                Console.WriteLine("{0,-25}{1,-15}{2,-20}{3,-1}", b.BusinessName, b.BranchCode, b.MaximumCapacity, b.VisitorsNow);
            }

            Console.WriteLine();
        }

        static void EditLocationCapacity(List<BusinessLocation> bList)
        {
            int newCapacity=0;
            DisplayBusinessLocations(bList);
            BusinessLocation targetLocation = FindLocation(bList);
            

            while(true)
            {
                Console.Write("Enter location's new capacity: ");
                //basic exception handling to ensure user does not enter string, and the rare cases of overflow. 
                try
                {
                    newCapacity = Convert.ToInt32(Console.ReadLine());
                }

                catch (System.FormatException)
                {
                    Console.WriteLine("Not an integer!");
                    continue;
                }

                catch (System.OverflowException)
                {
                    Console.WriteLine("That's a big number. Don't crash the program please :(");
                    continue;
                }

                if (newCapacity<0)
                {
                    Console.WriteLine("Enter a positive number!");
                    continue;
                }

                else if (newCapacity<targetLocation.VisitorsNow)
                {
                    Console.WriteLine("Cannot change capacity to be lower than current number of visitors!");
                    continue;
                }

                else
                {
                    break;
                }
                

            }
            
            targetLocation.MaximumCapacity = newCapacity;
            Console.WriteLine("Capacity of {0} has been changed to {1}\n", targetLocation.BusinessName, targetLocation.MaximumCapacity);
            

        }

        static void AssignToken(List<Person> pList)
        {
            
            List<Resident> rList = new List<Resident>();
            Resident targetResident=null;

            //initiates a list of residents to ensure that non-residents do not get the token.
            foreach (Person p in pList)
            {
                if (p.ToString()=="Assignment.Resident")
                {
                    Resident r = (Resident)p;
                    rList.Add(r);
                }
            }
            while(true)
            {
                ListResidents(pList);
                Person targetPerson = FindPerson(pList);
                if (targetPerson == null)
                {
                    return;
                }

                //since I'm checking the personlist, there is a small chance that the user enters a visitor's name even though it is not displayed.
                if (targetPerson.ToString()=="Assignment.Visitor")
                {
                    Console.WriteLine("Not a resident. How did you remember a visitor's name anyway?");
                    continue;
                }

                //breaks the loop once the person is found. 
                else if(targetPerson.ToString()=="Assignment.Resident")
                {
                    targetResident = (Resident)targetPerson;
                    break;
                }
            }

            //there will be two scenarios here: the resident has no token, or the resident has one. 
            //this first if statement accounts for the resident not having a token, meaning that they will be allowed to get one.
            if (targetResident.Token==null)
            {
                Console.Write("Enter new token serial number: ");
                string serial = Console.ReadLine();
                Console.Write("Enter collection location: ");
                string location = Console.ReadLine();
                TraceTogetherToken newtoken = new TraceTogetherToken(serial, location, DateTime.Now.AddMonths(6));
                targetResident.Token = newtoken;
                Console.WriteLine("Token added!\n");

            }

            //if the resident already has a token, it calls the class method and checks if it can be replaced. 
            else if (targetResident.Token.IsEligibleForReplacement()==false)
            {
                Console.WriteLine("{0}'s token is not eligible for replacement.", targetResident.Name);
            }

            //triggers if the token is eligible for replacement.
            else
            {
                Console.Write("Enter new token serial number: ");
                string serial = Console.ReadLine();
                Console.Write("Enter collection location: ");
                string location = Console.ReadLine();
                targetResident.Token.ReplaceToken(serial, location);
                Console.WriteLine("Token updated!\n");
            }
            
            
        }

        static void SafeEntryCheckIn(List<Person> pList, List<BusinessLocation> bList)
        {
            
            while (true)
            {
                //used to check whether the person has an active safe entry record in that exact location. 
                bool check = true;
                DisplayNames(pList);
                Person targetPerson = FindPerson(pList);
                if (targetPerson == null)
                {
                    return;
                }
                DisplayBusinessLocations(bList);
                BusinessLocation targetLocation = FindLocation(bList);
                //loops if business location is full.
                if (targetLocation.IsFull() == true)
                {
                    Console.WriteLine("Chosen location is at maximum capacity.");
                    continue;

                }

                foreach (SafeEntry s in targetPerson.SafeEntryList)
                {
                    if (s.Location.BranchCode==targetLocation.BranchCode && s.CheckOut.Year==1000)
                    {
                        Console.WriteLine("{0} has not checked out from that location yet.", targetPerson.Name);
                        check = false;
                        break;
                    }
                }

                //since the break at line 615 only breaks from the foreach loop, this short if statement sends the program back to the start of the while loop. 
                if(check==false)
                {
                    continue;
                }

                
                //creating the safe entry record, increases visitors of the location by one. 
                SafeEntry newSE = new SafeEntry(DateTime.Now, targetLocation);
                targetLocation.VisitorsNow += 1;
                targetPerson.AddSafeEntry(newSE);
                Console.WriteLine("Safe Entry check in successful!\n");
                break;
            }
            
        }

        static Person FindPerson(List<Person> pList)
        {
            //this method returns a person object, with exception handling as well.
            //many of these methods were implemented to streamline the code as they were used very often. 
            while (true)
            {
                Console.Write("Enter name of person (or 0 to exit): ");
                string targetname = Console.ReadLine();
                if (targetname == "0")
                {
                    Console.WriteLine();
                    return null;
                }
                for (int i = 0; i < pList.Count; i++)
                {
                    if (pList[i].Name == targetname)
                    {
                        Person p = pList[i];
                        return p;
                    }
                }

                Console.WriteLine("Name not found!");

            }
        }

        static BusinessLocation FindLocation(List<BusinessLocation> bList)
        {
            
            while (true)
            {
                Console.Write("Enter Location Name or Branch Code: ");
                string target = Console.ReadLine();
                foreach (BusinessLocation b in bList)
                {
                    if (b.BusinessName == target || b.BranchCode == target)
                    {
                        return b;
                    }
                }

                Console.WriteLine("Business Location not found.");
            }
        }

        static void DisplayNames(List<Person> pList)
        {
            //simple display names method to increase usability of program.
            Console.WriteLine("Name");
            foreach (Person p in pList)
            {
                Console.WriteLine(p.Name);
            }
            Console.WriteLine();
        }

        static void SafeEntryCheckOut(List<Person> pList, List<BusinessLocation> bList)
        {
            //lots of exceptions needed to be caught here. 

            //initializing variables to eliminate errors. 
            List<SafeEntry> validSafeEntry = new List<SafeEntry>();

            int choice=0;
            SafeEntry tempSafeEntry = new SafeEntry();
            DisplayNames(pList);
            Person targetPerson = FindPerson(pList);
            if (targetPerson == null)
            {
                return;
            }
            List<SafeEntry> activeSafeEntry = new List<SafeEntry>();

            //this for loop adds all the safe entry records that have no check out yet to a list. 
            foreach (SafeEntry s in targetPerson.SafeEntryList)
            {
                if (s.CheckOut.Year==1000)
                {
                    activeSafeEntry.Add(s);
                }
            }

            int count = 1;
            //with the list initialized, check if it is empty. If yes, return from this method. 
            if (activeSafeEntry.Count==0)
            {
                Console.WriteLine("{0} has no active Safe Entry Records.", targetPerson.Name);
                return;
            }

            
            Console.WriteLine("\nSafeEntry Details: ");
            Console.WriteLine("{0,-10}{1,-30}{2,-30}{3,-20}{4,-1}", "Number", "Check In", "Check Out", "Business Name", "Branch Code");
            foreach (SafeEntry s in targetPerson.SafeEntryList)
            {
                //lists all active safe entry records and removes the resolved ones.   
                if (s.CheckOut.Year == 1000)
                {
                    Console.WriteLine("{0,-10}{1,-30}{2,-30}{3,-20}{4,-1}", count, s.CheckIn, "Not Checked Out", s.Location.BusinessName, s.Location.BranchCode);
                    validSafeEntry.Add(s);
                    count += 1;
                }

            }
            

            while (true)
            {
                //standard exception handling.
                Console.Write("Enter number of desired check-out: ");
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                }

                catch (System.FormatException)
                {
                    Console.WriteLine("Not a number!");
                    continue;
                }

                catch (System.OverflowException)
                {
                    Console.WriteLine("That's a big number, my dude. ");
                    continue;
                }

                //checks if the user entered a number that was within the list.
                try
                {
                    tempSafeEntry = validSafeEntry[choice - 1];
                }

                catch
                {
                    Console.WriteLine("Number not in list.");
                    continue;
                }

                
                break;

                
            }

            //finally, the program goes through the person's safe entry list and locates the exact one selected by the user.
            //once it is found, it performs checkout and exits the while loop. 
            foreach (SafeEntry s in targetPerson.SafeEntryList)
            {
                if (tempSafeEntry == s)
                {
                    s.PerformCheckOut();
                    Console.WriteLine("Safe Entry check out successful!\n");
                    break;
                }
            }

            //reduces the business location visitor count by one.
            foreach (BusinessLocation b in bList)
            {
                if (tempSafeEntry.Location.BranchCode==b.BranchCode)
                {
                    b.VisitorsNow -= 1;
                }
            }

        }
    
        static void CreateVisitor(List<Person> pList)
        {
            Console.Write("Enter name: ");
            string tempname = Console.ReadLine();
            Console.Write("Enter passport number: ");
            string tempno = Console.ReadLine();
            Console.Write("Enter nationality: ");
            string tempnat = Console.ReadLine();
            Visitor tempv = new Visitor(tempname, tempno, tempnat);
            Console.WriteLine("Visitor has been created!");
            pList.Add(tempv);
        }

        static void CreateTravelEntry(List<Person> pList, List<SHNFacility> fList)
        {
            DisplayNames(pList);
            string tempEntry = "";
            Person targetPerson = FindPerson(pList);
            if (targetPerson == null)
            {
                return;
            }
            Console.Write("Enter last country of embarkation: ");
            string tempCountry = Console.ReadLine();
            while (true)
            {
                Console.WriteLine("Entry modes: Air, Sea, Land");
                Console.Write("Enter entry mode: ");
                tempEntry = Console.ReadLine();
                if (tempEntry == "Air" || tempEntry == "Sea" || tempEntry == "Land")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("That is not a valid option! Please choose one from the list!");
                    continue;
                }
            }
            DateTime currentDate = DateTime.Now;
            TravelEntry newEntry = new TravelEntry(tempCountry, tempEntry, currentDate);
            Console.WriteLine("TravelEntry has been created!");
            newEntry.ShnEndDate = newEntry.CalculateSHNDuration();
            Console.WriteLine();
            if (newEntry.ShnEndDate == newEntry.EntryDate)
            {
                targetPerson.AddTravelEntry(newEntry);
            }
            else
            {
                DisplayFacility(fList);
                while (true)
                {
                    SHNFacility facility = FindFacility(fList);
                    if (facility.IsAvailable() == false)
                    {
                        Console.WriteLine("This facility has no vacancy. Choose another facility.");
                        continue;
                    }
                    else
                    {
                        newEntry.ShnStay = facility;
                        facility.FacilityVacancy = facility.FacilityVacancy - 1;
                        Console.WriteLine("Facility has been chosen.");
                        break;
                    }
                }
                newEntry.IsPaid = false;
                targetPerson.AddTravelEntry(newEntry);
            }
        }

        static SHNFacility FindFacility(List<SHNFacility> fList)
        {
            while (true)
            {
                Console.Write("Enter facility name: ");
                string target = Console.ReadLine();
                foreach (SHNFacility f in fList)
                {
                    if (f.FacilityName == target)
                    {
                        return f;
                    }
                }

                Console.WriteLine("SHN Facility not found.");
            }
        }

        static void CalculateCharges(List<Person> pList)
        {
            double charge = 0;
            List<TravelEntry> tList = new List<TravelEntry>();
            DisplayNames(pList);
            Person targetPerson;
            while (true)
            {
                targetPerson = FindPerson(pList);
                if (targetPerson == null)
                {
                    return;
                }
                foreach (TravelEntry t in targetPerson.TravelEntryList)
                {
                    if (t.IsPaid == false)
                    {
                        tList.Add(t);
                    }
                }
                if (tList.Count == 0)
                {
                    Console.WriteLine("This person does not have any TravelEntry! Choose a different person to pay for!");
                    continue;
                }
                else
                {
                    break;
                }

            }
            TravelEntry targetT = FindTravelEntry(targetPerson);
            if (targetPerson.ToString() == "Assignment.Visitor" && targetT.ShnStay != null)
            {
                charge += targetPerson.CalculateSHNCharges() + targetT.ShnStay.CalculateTravelCost(targetT.EntryMode, targetT.EntryDate);
            }
            else
            {
                charge += targetPerson.CalculateSHNCharges();
            }
            charge = charge * 1.07;
            targetT.IsPaid = true;
            Console.WriteLine("Your bill tallies up to $" + Math.Round(charge, 2));
            Console.WriteLine("[Imagine that there is a processing screen here]");
            Console.WriteLine();
            Console.WriteLine("              |------------|");
            Console.WriteLine("              |            |");
            Console.WriteLine("              |    BILL    |");
            Console.WriteLine("              |            |");
            Console.WriteLine("              |------------|");
            Console.WriteLine();
            Console.WriteLine("Your payment has been processed. Thank you for your payment!");
            Console.WriteLine();
        }

        static TravelEntry FindTravelEntry(Person target)
        {
            int count = 0;
            int choice = 0;
            TravelEntry targetT = new TravelEntry();
            List<TravelEntry> tList = target.TravelEntryList;
            Console.WriteLine("{0,-4}{1,-24}{2,-18}{3,-14}{4,-24}{5,-16}{6,-1}", "No.", "Entry Date", "From", "Entry Mode", "End Date", "SHN Facility", "Paid");
            foreach (TravelEntry t in tList)
            {
                if (t.IsPaid == false && DateTime.Now > t.ShnEndDate)
                {
                    count += 1;
                    Console.WriteLine("{0,-4}{1,-24}{2,-18}{3,-14}{4,-24}{5,-16}{6,-1}", count, t.EntryDate, t.LastCountryOfEmbarkation, t.EntryMode, t.ShnEndDate, t.ShnStay.FacilityName, t.IsPaid);
                    continue;
                }
                else
                {
                    continue;
                }
                
            }
            Console.WriteLine();
            while (true)
            {
                try
                {
                    Console.Write("Enter number of Travel Entry to pay: ");
                    choice = Convert.ToInt32(Console.ReadLine()) - 1;
                    targetT = tList[choice];
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Invalid number entered! Please enter a valid number from the list!");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                break;
            }
            return targetT;
        }


        
        

        static void ContactTracing(List<Person> pList, List<BusinessLocation> bList)
        {
            //initializes variables to resolve errors.
            DateTime targetDateTime = new DateTime();
            List<SafeEntry> sList = new List<SafeEntry>();
            List<Person> personList = new List<Person>();
            string data;

            //lots of exception handling here as well.
            while(true)
            {
                Console.Write("Enter date (dd/MM/yyyy): ");
                string dateString = Console.ReadLine();
                string[] splitDate = dateString.Split("/");
                //if user enters a random string (sadhoaidho) or a date with too many or too few slashes (10/02/2021/2021), the prorgam catches it and loops. 
                if (splitDate.Count() < 3 || splitDate.Count()>3)
                {
                    Console.WriteLine("Incorrect format. Make sure you follow the format specified exactly.");
                    continue;
                }

                //this line is used to check if the user entered a valid date. For example, 31/2/2021 is not an actual date, and there's very few ways to catch that error other than trying to create the date.
                try
                {
                    //also the date creating constructor is in a weird order, it's (year, month, day)
                    DateTime test = new DateTime(Convert.ToInt32(splitDate[2]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[0]));
                }

                catch
                {
                    Console.WriteLine("Please enter a valid date.");
                    continue;
                }

                

                Console.Write("Enter time (e.g. 23:45): ");
                string timeString = Console.ReadLine();
                string[] splitTime = timeString.Split(":");

                //again, this checks if the user enters a nonsensical string (asiodhadh) or a time with too many colons (13:34:23), since this program does not take seconds into account.
                if (splitTime.Count() < 2 || splitTime.Count()>2)
                {
                    Console.WriteLine("Incorrect format. Make sure you follow the format specified exactly.");
                    continue;
                }
                //checks if the hour is above 23 or minute above 59 (which should not be possible)
                else if (Convert.ToInt32(splitTime[0]) > 23 || Convert.ToInt32(splitTime[1]) > 59)
                {
                    Console.WriteLine("Invalid time.");
                    continue;
                }

                //after the initial rounds of exception handling, this line will not have any errors as all the constructors have been properly validated.
                targetDateTime = new DateTime(Convert.ToInt32(splitDate[2]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitTime[0]), Convert.ToInt32(splitTime[1]), 0);

                DisplayBusinessLocations(bList);
                BusinessLocation targetLocation = FindLocation(bList);

                //given the targetDateTime, the program checks every person's safe entry list to find which check ins fall within the target date time. 
                foreach (Person p in pList)
                {
                    foreach (SafeEntry s in p.SafeEntryList)
                    {

                        //there are two scenarios here: the person has not checked out yet and the person has already checked out. 
                        //this two if statements account for both scenarios and add the person and safe entry to the lists. 
                        if (targetLocation.BranchCode==s.Location.BranchCode && s.CheckOut.Year==1000 && targetDateTime>=s.CheckIn)
                        {
                            personList.Add(p);
                            sList.Add(s);
                            break;
                        }

                        else if(targetLocation.BranchCode == s.Location.BranchCode && targetDateTime>=s.CheckIn && targetDateTime <= s.CheckOut)
                        {
                            personList.Add(p);
                            sList.Add(s);
                            break;
                            
                        }
                    }
                }

                //if there's no one in the location at the specified time, it prints this statement only and ends the method. 
                if (personList.Count()==0)
                {
                    Console.WriteLine("Nobody was present at that location during that time.\n");
                }

                else
                {
                    //creates the csv file and adds the heading.
                    string heading = "Name,Check In Time,Check Out Time,Business Name,Branch Code\n";
                    File.WriteAllText("csv/contactTracing.csv", heading);
                    Console.WriteLine("{0,-30}{1,-30}{2,-30}", "Name", "Check In Time", "Check Out Time");

                    foreach (Person p in personList)
                    {
                        foreach(SafeEntry s in p.SafeEntryList)
                        {
                            //due to there being the possibility of the person not checking out, this makes sure that "Not checked out" is appended to the file instead of the placeholder date.
                            if (s.CheckOut.Year==1000)
                            {
                                data = p.Name + ',' + s.CheckIn+",Not checked out,"+s.Location.BusinessName+','+s.Location.BranchCode;
                                Console.WriteLine("{0,-30}{1,-30}{2,-20}", p.Name, s.CheckIn, "Not checked out");
                                using (StreamWriter sw = new StreamWriter("csv/contactTracing.csv", true))
                                {
                                    sw.WriteLine(data);
                                }


                            }

                            //standard printing and appending of data 
                            else
                            {
                                data = p.Name + ',' + s.CheckIn +','+ s.CheckOut +','+ s.Location.BusinessName +','+ s.Location.BranchCode;
                                Console.WriteLine("{0,-30}{1,-30}{2,-30}", p.Name, s.CheckIn , s.CheckOut);
                                using (StreamWriter sw = new StreamWriter("csv/contactTracing.csv", true))
                                {
                                    sw.WriteLine(data);
                                }
                            }
                        }

                    }

                }
                Console.WriteLine();
                break;

            }

             
        }

        static void AdvancedContactTracing(List<Person> pList, List<BusinessLocation> bList)
        {
            Console.WriteLine("This feature will list all people directly or indirectly in contact with a confirmed Covid-19 case.");
            Console.WriteLine("Direct contact: Was in the same location as specified person at the same time.");
            Console.WriteLine("Indirect contact: Was in the same location as the specified person, but not at the same time.\n");
            DisplayNames(pList);
            Person targetPerson = FindPerson(pList);
            if (targetPerson == null)
            {
                return;
            }

            Console.WriteLine("{0} visited the following locations:", targetPerson.Name);

            //since specified person could have visited more than one location, this first for loop checks all the safe entries the person has.
            foreach (SafeEntry s in targetPerson.SafeEntryList)
            {
                //resets the lists and variables each time the first for loop runs. 
                List<Person> highRiskList = new List<Person>();
                List<Person> lowRiskList = new List<Person>();
                BusinessLocation targetLocation = s.Location;
                DateTime targetCheckIn = s.CheckIn;
                DateTime targetCheckOut = s.CheckOut;

                //second for loop now checks each person in person list.
                foreach (Person p in pList)
                {
                    //last for loop checks each safe entry for each person and compares it to the target person's location and time.
                    foreach (SafeEntry se in p.SafeEntryList)
                    {
                        //if the infected person checks in before the current person, but checks out after the current person checks in, their time of visit
                        //overlaps and has to be accounted for in this if statement. 
                        if (targetLocation.BranchCode == se.Location.BranchCode && p.Name != targetPerson.Name && targetCheckOut >= se.CheckIn)
                        {
                            highRiskList.Add(p);
                            break;
                        }

                        //if any point infected person's visit falls in between the current person's visit, this if statement checks for the scenario where that happens. 
                        else if (targetLocation.BranchCode == s.Location.BranchCode && p.Name != targetPerson.Name && targetCheckIn >= se.CheckIn && targetCheckIn <= se.CheckOut)
                        {
                            highRiskList.Add(p);
                            break;

                        }

                        //if the first 2 criteria are not met, it checks whether the infected visited the same location in the same day as the current person.
                        //(since Covid can survive on surfaces)
                        //while transmission is unlikely, they should be accounted for, but distinguised from the other list. 
                        else if (targetLocation.BranchCode == s.Location.BranchCode && p.Name != targetPerson.Name && targetCheckIn.Date == se.CheckIn.Date)
                        {
                            lowRiskList.Add(p);
                        }
                    }
                }

                //prints all the information out from the different lists. 
                Console.WriteLine("At {0}: \n", targetLocation.BusinessName);
                Console.WriteLine("The following individuals were directly in contact with {0}: ", targetPerson.Name);
                foreach (Person p in highRiskList)
                {
                    Console.WriteLine(p.Name);
                }
                Console.WriteLine();
                Console.WriteLine("The following individuals visited the same location as {0}, but not with him in person: ", targetPerson.Name);
                foreach (Person p in lowRiskList)
                {
                    Console.WriteLine(p.Name);
                }
                Console.WriteLine();

            }


        }

        static void StatusReport(List<Person> pList)
        {
            DateTime date = new DateTime();
            string data = "";

            while (true)
            {
                Console.Write("Enter date (dd/MM/yyyy): ");
                string dateString = Console.ReadLine();
                string[] splitDate = dateString.Split("/");
                if (splitDate.Count() < 3 || splitDate.Count() > 3)
                {
                    Console.WriteLine("Incorrect format. Make sure you follow the format specified exactly.");
                    continue;
                }

                try
                {
                    date = new DateTime(Convert.ToInt32(splitDate[2]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[0]));
                }

                catch
                {
                    Console.WriteLine("Please enter a valid date.");
                    continue;
                }

                string heading = "Name,SHN End Date, SHN Facility\n";
                File.WriteAllText("csv/SHNStatusReporting.csv", heading);
                Console.WriteLine("File created successfully!");
                foreach (Person p in pList)
                {
                    foreach (TravelEntry t in p.TravelEntryList)
                    {
                        if (t.ShnEndDate > date && t.EntryDate < date)
                        {
                            data = p.Name + "," + t.ShnEndDate + "," + t.ShnStay.FacilityName;
                            using (StreamWriter sw = new StreamWriter("csv/SHNStatusReporting.csv", true))
                            {
                                sw.WriteLine(data);
                            }
                            Console.WriteLine("1 line of data added!");
                        }
                    }
                }
                Console.WriteLine("All data added successfully!");
                Console.WriteLine();
                break;
            }
        }
    }
}


