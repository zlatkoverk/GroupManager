using System;
using System.ComponentModel.DataAnnotations;

namespace GroupManager.Models.BalanceViewModels
{
    public class BalanceEntryViewModel
    {
        public string Id { get; set; }
        public string User { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public double Value { get; set; }
        public DateTime Time { get; set; }
    }
}