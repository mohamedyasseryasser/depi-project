using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace depi__project.Models
{
    public class Medicine
    {
        [Key]
        public int medicineId { get; set; }

        public int StockQuantity { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SupplierName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
 
        public decimal UnitPrice { get; set; }

        public int RecordLevel { get; set; }

        public DateTime ExpiryDate { get; set; }
        //navigation proberity
        public ICollection<Prescriptionitems> Prescriptionitems { get; set; } = new List<Prescriptionitems>();
        public string CategoryName {  get; set; }
        public int cat_id { get; set; }
        [ForeignKey(nameof(cat_id))]
        public Category category {  get; set; }
        public string AddedBy {  get; set; }
        public string user_id { get; set; }
        [ForeignKey("user_id")]
        public Aplicationuser user { get; set; }
    }
}
