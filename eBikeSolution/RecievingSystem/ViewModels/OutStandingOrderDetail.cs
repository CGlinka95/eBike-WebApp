namespace RecievingSystem.ViewModels
{
    public class OutStandingOrderDetail
    {
        // COmmented it ou properties will be used in command
        public int PurcahseOrderID { get; set; }
        public int PurchaseOrderDetailID { get; set; }
        public int RecieveOrderDetailID { get; set; }
        //public int RecieveOrderID { get; set; }
        //public int EmployeeID { get; set; }
        public int PartID { get; set; }
        public string PartDescription { get; set; }
        public int OustandingQtyOriginal { get; set; }
        public int OutStandingQty { get; set; }
        //public int RecievedQty { get; set; }
        //public int ReturnedQty { get; set; }

    }
}
