//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderAssistantUI
{
    using System;
    using System.Collections.Generic;
    
    public partial class invoice
    {
        public int id { get; set; }
        public decimal rowNumber { get; set; }
        public int idItem { get; set; }
        public int idSupply { get; set; }
        public int price { get; set; }
        public int count { get; set; }
        public string note { get; set; }
    
        public virtual item item { get; set; }
        public virtual supply supply { get; set; }
    }
}
