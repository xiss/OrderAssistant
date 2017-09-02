//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderAssistant
{
    using System;
    using System.Collections.Generic;
    
    public partial class supply
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public supply()
        {
            this.invoices = new HashSet<invoice>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string shipmentNumber { get; set; }
        public int idSupplier { get; set; }
        public System.DateTime datePlanPrePayment { get; set; }
        public Nullable<System.DateTime> dateRealPrePayment { get; set; }
        public System.DateTime datePlanConfPayment { get; set; }
        public Nullable<System.DateTime> dateRealConfPayment { get; set; }
        public System.DateTime datePlanEndPicking { get; set; }
        public Nullable<System.DateTime> dateRealEndPicking { get; set; }
        public System.DateTime datePlanLoadingToShip { get; set; }
        public Nullable<System.DateTime> dateRealLoadingToShip { get; set; }
        public System.DateTime datePlanPayment { get; set; }
        public Nullable<System.DateTime> dateRealPayment { get; set; }
        public System.DateTime datePlanArrival { get; set; }
        public Nullable<System.DateTime> dateRealArrival { get; set; }
        public System.DateTime datePlanArrivalStock { get; set; }
        public Nullable<System.DateTime> dateRealArrivalStock { get; set; }
        public Nullable<decimal> netWeight { get; set; }
        public Nullable<decimal> grossWeight { get; set; }
        public Nullable<decimal> grossVolume { get; set; }
        public Nullable<decimal> netVolume { get; set; }
        public bool isReceived { get; set; }
        public string note { get; set; }
        public bool isExistInvoice { get; set; }
        public bool isExistPackingList { get; set; }
        public Nullable<decimal> costOfDelivery { get; set; }
        public Nullable<decimal> costWORate { get; set; }
        public Nullable<decimal> sumInvoice { get; set; }
        public Nullable<decimal> exchengeRate { get; set; }
        public string invoice { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<invoice> invoices { get; set; }
        public virtual supplier supplier { get; set; }
    }
}
