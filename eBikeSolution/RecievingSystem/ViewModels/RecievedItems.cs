namespace RecievingSystem.ViewModels
{
    public class RecievedItems
    {
        public int PurchaseOrderDetailID { get; set; }
        public int QuantityRecieved { get; set; }
        public int QuantityReturned { get; set; }
        public string ReturnReason { get; set; }
        // These properties are used for inutitive Error handling
        public int QuantityOutStanding { get; set; }
        public string PartDescription { get; set; }
        public int PartID { get; set; }
    }
}
