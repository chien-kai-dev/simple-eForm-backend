using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace axiosTest.Models
{
    [Table("FormInfo")]
    public class FormInfo
    {
        [Key]
        public int FormInfoId { get; set; }
        
        [Required]
        [StringLength(30)]
        public string FormNum { get; set; }
        
        [Required]
    	[DataType(DataType.Date)]
    	public DateTime CreatedDate { get; set; }
    	
    	[Required]
    	[DataType(DataType.Time)]
    	public TimeSpan CreatedTime { get; set; }
    	
    	public int Creator { get; set; }
    }
}
