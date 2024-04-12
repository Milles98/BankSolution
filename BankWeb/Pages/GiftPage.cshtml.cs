using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Newtonsoft.Json;
using DataLibrary.Services.Interfaces;

namespace BankWeb.Pages
{
    [Authorize(Roles = "Cashier")]
    public class GiftPageModel(ICustomerService customerService) : PageModel
    {
        private readonly ICustomerService _customerService = customerService;

        public string GiftsJson { get; set; }

        public void OnGet()
        {
            List<string> gifts = new List<string>
            {
                "Customer won a free hotel weekend!",
                "Customer won a free dinner for two!",
                "Customer won a free movie ticket!",
                "Customer won a free spa treatment!",
                "Customer won a 50% discount on their next purchase!",
                "Customer won a free coffee!",
                "Customer won a free ice cream!",
                "Customer won a free cake!",
                "Customer won a free pizza!",
                "Customer won a free burger!",
                "Customer won a free salad!",
                "Customer won a free sandwich!",
                "Customer won a free hot dog!",
                "Customer won a free soda!",
                "Customer won a free beer!",
                "Customer won a free wine!",
                "Customer won a free cocktail!",
                "Customer won a free juice!",
                "Customer won a free smoothie!",
                "Customer won a free milkshake!",
                "Customer won a free donut!",
                "Customer won a free muffin!",
                "Customer won a free croissant!",
                "Customer won a free bagel!",
                "Customer won a free pretzel!",
                "Customer won a free popcorn!",
                "Customer won a free candy!",
                "Customer won a free chocolate!",
                "Customer won a free cookie!",
            };

            GiftsJson = JsonConvert.SerializeObject(gifts);
        }
    }
}
