namespace Orders.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public Status Status { get; set; }
        public bool Active { get; set; }
    }
}
