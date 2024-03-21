using System;
using System.Collections.Generic;

namespace BankWeb.Data;

public partial class Account
{
    public int AccountId { get; set; }

    public string Frequency { get; set; } = null!;

    public DateOnly Created { get; set; }

    public decimal Balance { get; set; }

    public virtual ICollection<Disposition> Dispositions { get; set; } = new List<Disposition>();

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<PermenentOrder> PermenentOrders { get; set; } = new List<PermenentOrder>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
