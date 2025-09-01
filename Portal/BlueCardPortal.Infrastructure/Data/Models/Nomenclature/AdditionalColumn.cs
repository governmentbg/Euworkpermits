using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueCardPortal.Infrastructure.Data.Models.Nomenclature
{
    /// <summary>
    /// Additional information for Nomenclatures
    /// additional_columns
    /// </summary>
    [Table("additional_columns", Schema = "nomenclatures")]
    [Comment("Additiona values for codeable concepts")]
    public class AdditionalColumn
    {
        /// <summary>
        /// System identifier of additional column
        /// </summary>
        [Key]
        [Comment("System identifier of additional column")]
        public long Id { get; set; }

        /// <summary>
        /// Identificator of Codeable concept to which 
        /// this column mus be attached
        /// </summary>
        [Required]
        [Comment("FK Codeable concept identificator")]
        public long NomenclatureId { get; set; }

        /// <summary>
        /// Column name
        /// </summary>
        [Required]
        [StringLength(50)]
        [Comment("Column name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Column value in BG
        /// </summary>
        [Required]
        [StringLength(1024)]
        [Comment("Column value in BG")]
        public string ValueBg { get; set; } = null!;

        /// <summary>
        /// Column value in EN
        /// </summary>
        [Required]
        [StringLength(1024)]
        [Comment("Column value in EN")]
        public string ValueEn { get; set; } = null!;

        /// <summary>
        /// Codeable concept navigation property
        /// </summary>
        [ForeignKey(nameof(NomenclatureId))]
        public CodeableConcept Nomenclature { get; set; } = null!;
    }
}
