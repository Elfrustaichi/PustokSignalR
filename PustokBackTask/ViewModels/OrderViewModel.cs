namespace PustokBackTask.ViewModels
{
    public class OrderViewModel
    {
        public List<CheckoutItemViewModel> CheckoutItems { get; set; } = new List<CheckoutItemViewModel>();
        public decimal TotalPrice { get; set; }
        public OrderCreateViewModel Order { get; set; }
    }
}
