using System;

namespace WashMachine.Web.Models
{
    public class DashboardDataFilterModel
    {
        
    }

    public class OrderByYearFilterModel : DashboardDataFilterModel
    {
        public int Year { get; set; } = DateTime.Now.Year;
    }
}
