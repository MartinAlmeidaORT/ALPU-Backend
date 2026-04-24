using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Common.Inputs;

namespace Domain.Models;

[Table("address")]
public partial class Address : Entity
{
    public Address() { }

    public Address(Country country, string state, string city, string? street)
    {
        CountryCode = country.CountryCode;
        Country = country;
        State = state;
        City = city;
        Street = street;
    }

    public void Update(Country? country, UpdateAddressInput? input)
    {
        CountryCode = country?.CountryCode ?? CountryCode;
        Country = country ?? Country;
        State = input?.State ?? State;
        City = input?.City ?? City;
        Street = input?.Street ?? Street;
    }

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
