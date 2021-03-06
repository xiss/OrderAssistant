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
    
    public partial class item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public item()
        {
            this.attachments = new HashSet<attachment>();
            this.balances = new HashSet<balance>();
            this.invoices = new HashSet<invoice>();
            this.prices = new HashSet<price>();
            this.pricesInternets = new HashSet<pricesInternet>();
            this.sales = new HashSet<sale>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string catNumber { get; set; }
        public string id1C { get; set; }
        public Nullable<int> idBrand { get; set; }
        public Nullable<int> idManufacturer { get; set; }
        public Nullable<decimal> netVolume { get; set; }
        public Nullable<decimal> grossVolume { get; set; }
        public Nullable<decimal> netWight { get; set; }
        public Nullable<decimal> grossWight { get; set; }
        public Nullable<int> minStock { get; set; }
        public Nullable<int> maxStock { get; set; }
        public string ABCgroup { get; set; }
        public Nullable<decimal> ABCratio { get; set; }
        public string note { get; set; }
        public string location { get; set; }
        public byte[] dateCreate { get; set; }
        public bool excludeOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<attachment> attachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<balance> balances { get; set; }
        public virtual brand brand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<invoice> invoices { get; set; }
        public virtual manufacturer manufacturer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<price> prices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pricesInternet> pricesInternets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale> sales { get; set; }
    }
}
