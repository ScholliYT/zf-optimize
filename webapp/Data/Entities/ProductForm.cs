namespace webapp.Data.Entities
{
    public class ProductForm
    {
        public int ProductId { get; set; }
        public int FormId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Form Form { get; set; }
        public float Amount { get; set; }
    }
}