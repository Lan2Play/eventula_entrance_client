using System.Collections.Generic;

namespace eventula_entrance_client.Models 
{ 
    public class User 
    { 
        public int UserId { get; set; } 
        public string Name { get; set; } 
        public string Seat { get; set; } 
        public int Counter { get; set; } 
        public bool PaymentCompleted { get; set; } = false;
        public bool CovtestCompleted { get; set; } = false;
        public bool SignedIn { get; set; } = false;
        public bool New { get; set; } = true;
    } 

}