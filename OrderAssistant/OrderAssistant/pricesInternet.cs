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
    
    public partial class pricesInternet
    {
        public int id { get; set; }
        public int idItem { get; set; }
        public int idCompetitor { get; set; }
        public decimal price { get; set; }
        public int position { get; set; }
        public byte[] dateRecord { get; set; }
        public decimal count { get; set; }
    
        public virtual competitorsInternet competitorsInternet { get; set; }
        public virtual item item { get; set; }
    }
}
