using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ExpensesMgmtWeb.Models
{
    public class ChartData
    {
        [DataType(DataType.Date)]
        [Display(Name = "Date from")]
        [Required(ErrorMessage = "Must enter the date.")]
        public DateTime DateFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date to")]
        [Required(ErrorMessage = "Must enter the date.")]
        public DateTime DateTo { get; set; }

        public IQueryable Data { get; set; }

        public Dictionary<string, List<string>> InputDataErrors { get; set; }
    }
}