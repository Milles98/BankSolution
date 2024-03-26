using System;
using System.Collections.Generic;

namespace BankWeb.Data;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Gender { get; set; } = null!;

    public string Givenname { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Streetaddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string? NationalId { get; set; }

    public string? Telephonecountrycode { get; set; }

    public string? Telephonenumber { get; set; }

    public string? Emailaddress { get; set; }

    public virtual ICollection<Disposition> Dispositions { get; set; } = new List<Disposition>();
}
