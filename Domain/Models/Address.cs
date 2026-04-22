using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;

namespace Domain.Models;

[Table("address")]
public partial class Address : Entity
{
    [Key]
    [Column("address_id")]
    public int AddressId { get; set; }

    [Column("country_code")]
    [StringLength(3)]
    public string CountryCode { get; set; } = null!;

    [Column("street")]
    [StringLength(100)]
    public string? Street { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [Column("state")]
    [StringLength(100)]
    public string State { get; set; } = null!;

    [ForeignKey("CountryCode")]
    public virtual Country Country { get; set; } = null!;
}
