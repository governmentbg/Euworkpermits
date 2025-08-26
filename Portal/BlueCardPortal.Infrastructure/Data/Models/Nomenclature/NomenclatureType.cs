using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueCardPortal.Infrastructure.Data.Models.Nomenclature
{
    /// <summary>
    /// Types of nomenclatures available to the system
    /// </summary>
    [Comment("Types of nomenclatures available to the system")]
    [Table("nomenclature_types", Schema = "nomenclatures")]
    public class NomenclatureType
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        [Comment("Primary key")]
        public int Id { get; set; }

        /// <summary>
        /// Nomenclature type
        /// </summary>
        [Required]
        [StringLength(50)]
        [Comment("Nomenclature type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Type Description
        /// </summary>
        [Required]
        [StringLength(200)]
        [Comment("Type name or description")]
        public string Description { get; set; } = null!;
    }
}
