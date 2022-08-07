//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSE3200_Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Content
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Content()
        {
            this.Shelves = new HashSet<Shelf>();
        }
    
        public int id { get; set; }
        public Nullable<int> creator_id { get; set; }
        public Nullable<System.DateTime> creation_datetime { get; set; }
        public Nullable<int> modifier_id { get; set; }
        public Nullable<System.DateTime> modification_datetime { get; set; }
        public string title { get; set; }
        public string details { get; set; }
        public string url { get; set; }
        public string alternative_url { get; set; }
        public string type { get; set; }
        public string privacy { get; set; }
        public Nullable<bool> status { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Shelf> Shelves { get; set; }
    }
}
