using System;
using System.Collections.Generic;

namespace BankWeb.Data;

public partial class PermenentOrder
{
    public int OrderId { get; set; }

    public int AccountId { get; set; }

    public string BankTo { get; set; } = null!;

    public string AccountTo { get; set; } = null!;

    public decimal? Amount { get; set; }

    public string Symbol { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
