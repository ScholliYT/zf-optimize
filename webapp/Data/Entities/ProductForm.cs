namespace webapp.Data.Entities
{
    public class ProductForm
    {
        public int ProductId { get; set; }
        public int FormId { get; set; }
        public Product Product { get; set; }
        public Form Form { get; set; }
        public float Amount { get; set; }
    }
}