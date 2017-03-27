using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Workshop
    {
        public string Name;

        public string Session;

        public string Time;

        public string BookingRef;
        
        public Workshop(string Name, string Session, string Time, string BookingRef)
        {
            this.Name = Name;
            this.Session = Session;
            this.Time = Time;
            this.BookingRef = BookingRef;
        } 
    }

    class WorkshopTimes
    {
        public int ID; 

        public string type; 

        public string sessionTimes;

        public int numBooking;

        public int avabililty;

        public bool full; 

        public WorkshopTimes(int ID, string Type, string SessionTimes, int numBooking, int Availability, bool full)
        {
            this.ID = ID;
            this.type = Type;
            this.sessionTimes = SessionTimes;
            this.numBooking = numBooking;
            this.avabililty = Availability;
            this.full = full;
        }
    }
}
