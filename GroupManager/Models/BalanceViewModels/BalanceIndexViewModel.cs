using System.Collections.Generic;

namespace GroupManager.Models.BalanceViewModels
{
    public class BalanceIndexViewModel
    {
        public List<BalanceEntryViewModel> Entries { get; set; }
        public double CurrentBalance { get; set; }
    }
}