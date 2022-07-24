using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment
{
    class TraceTogetherToken
    {
        public string SerialNo { get; set; }
        public string CollectionLocation { get; set; }
        public DateTime ExpiryDate { get; set; }
        public TraceTogetherToken() { }
        public TraceTogetherToken(string no, string cl, DateTime ed)
        {
            SerialNo = no;
            CollectionLocation = cl;
            ExpiryDate = ed;
        }

        public bool IsEligibleForReplacement()
        {
            if (ExpiryDate == null)
            {
                return true;
            }

            TimeSpan difference = ExpiryDate.Subtract(DateTime.Now);
            if (difference.Days<=31)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        public void ReplaceToken(string id, string l)
        {
            SerialNo = id;
            CollectionLocation = l;
            ExpiryDate = DateTime.Now.AddMonths(6);
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
