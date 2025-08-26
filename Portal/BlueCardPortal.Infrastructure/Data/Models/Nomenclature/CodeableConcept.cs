using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueCardPortal.Infrastructure.Data.Models.Nomenclature
{
    /// <summary>
    /// All system nomenclatures
    /// codeable_concepts
    /// </summary>
    [Comment("System nomenclatures")]
    [Table("codeable_concepts", Schema = "nomenclatures")]
    [Index("Type", "Code", "DateFrom", "DateTo", Name = "ix_codeable_concepts_type_code")]
    public class CodeableConcept
    {
        /// <summary>
        /// Systsem identifer of codeable concept
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("Systsem identifer of codeable concept")]
        public long Id { get; set; }

        /// <summary>
        /// Nomenclaure value identifier
        /// </summary>
        [StringLength(20)]
        [Required]
        [Comment("Nomenclaure value identifier")]
        public string Code { get; set; } = null!;

        /// <summary>
        /// Nomenclature value in BG
        /// </summary>
        [StringLength(255)]
        [Required]
        [Comment("Nomenclature value in BG")]
        public string Value { get; set; } = null!;

        /// <summary>
        /// Nomenclature value in EN
        /// </summary>
        [StringLength(255)]
        [Required]
        [Comment("Nomenclature value in EN")]
        public string ValueEn { get; set; } = null!;

        /// <summary>
        /// Start of the nomenclature validity
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        [Comment("Start of the nomenclature validity")]
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// End of the nomenclature validity, never expires if NULL
        /// </summary>
        [Column(TypeName = "date")]
        [Comment("End of the nomenclature validity, never expires if NULL")]
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Nomenclaure identifier
        /// </summary>
        [Required]
        [StringLength(50)]
        [Comment("Nomenclaure identifier")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// Parent item in hierarchical nomenclatures
        /// </summary>
        [Comment("FK for creating hierarchical nomenclatures")]
        public long? ParentId { get; set; }

        /// <summary>
        /// Parent item navigation property
        /// </summary>
        [ForeignKey(nameof(ParentId))]
        public CodeableConcept? Parent { get; set; }

        /// <summary>
        /// Record created by
        /// </summary>
        [Required]
        [Column(TypeName = "uuid")]
        [Comment("Record created by")]
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Record last updated by
        /// </summary>
        [Column(TypeName = "uuid")]
        [Comment("Record last updated by")]
        public Guid? LastUpdatedBy { get; set; }

        /// <summary>
        /// Record last updated on
        /// </summary>
        [Comment("Record last updated on")]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? LastUpdatedOn { get; set; }

        /// <summary>
        /// Additional columns navigation property
        /// </summary>
        public ICollection<AdditionalColumn> AdditionalColumns { get; set; } = new List<AdditionalColumn>();
    }
}
