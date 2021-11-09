using EventulaEntranceClient.Storage;
using System.Text.Json.Serialization;

namespace EventulaEntranceClient.Models
{
    public class Request
    {
        [JsonPropertyName("successful")]
        bool Successful { get; set; }

        [JsonPropertyName("reason")]
        string Reason { get; set; }
    }

    public class TicketRequest : Request
    {
        [JsonPropertyName("participant")]
        public Participant Participant { get; set; }
    }

    public class Participant : IStoreObject
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("ticket_id")]
        public int TicketId { get; set; }

        [JsonPropertyName("purchase_id")]
        public int PurchaseId { get; set; }

        [JsonPropertyName("qrcode")]
        public string QrCode { get; set; }

        [JsonPropertyName("staff")]
        public int Staff { get; set; }

        [JsonPropertyName("free")]
        public int Free { get; set; }

        //[JsonPropertyName("staff_free_assigned_by")]
        //public int StaffFreeAssignedBy { get; set; }

        [JsonPropertyName("signed_in")]
        public int SignedIn { get; set; }

        [JsonPropertyName("credit_applied")]
        public int CreditApplied { get; set; }

        //[JsonPropertyName("gift")]
        //public object Gift { get; set; }

        //[JsonPropertyName("gift_accepted")]
        //public object GiftAccepted { get; set; }

        //[JsonPropertyName("user_id")]
        //public object gift_accepted_url { get; set; }

        //[JsonPropertyName("user_id")]
        //public object gift_sendee { get; set; }

        [JsonPropertyName("transferred")]
        public int Transferred { get; set; }

        [JsonPropertyName("transferred_event_id")]
        public int TransferredEventId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("ticket")]
        public Ticket Ticket { get; set; }

        [JsonPropertyName("purchase")]
        public Purchase Purchase { get; set; }

        [JsonPropertyName("seat")]
        public Seat Seat { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("username_nice")]
        public string UsernameNice { get; set; }

        [JsonPropertyName("steamname")]
        public string Steamname { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phonenumber")]
        public string Phonenumber { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("steamid")]
        public string Steamid { get; set; }

        [JsonPropertyName("admin")]
        public int Admin { get; set; }

        [JsonPropertyName("credit_total")]
        public int CreditTotal { get; set; }

        [JsonPropertyName("seat")]
        public string LastLogin { get; set; }

        [JsonPropertyName("banned")]
        public int Banned { get; set; }

        [JsonPropertyName("email_verified_at")]
        public string EmailVerifiedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class Ticket
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("no_tickets_per_user")]
        public int NoTicketsPerUser { get; set; }

        [JsonPropertyName("price_credit")]
        public object PriceCredit { get; set; }

        [JsonPropertyName("seatable")]
        public int Seatable { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class Purchase
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("paypal_email")]
        public string PaypalEmail { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class Seat
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("seat")]
        public string SeatName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("event_seating_plan_id")]
        public int EventSeatingPlanId { get; set; }

        [JsonPropertyName("event_participant_id")]
        public int EventParticipantId { get; set; }

        [JsonPropertyName("gifted")]
        public int Gifted { get; set; }
    }


}
