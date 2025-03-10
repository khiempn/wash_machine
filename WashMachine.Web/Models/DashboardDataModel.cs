using WashMachine.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Web.Models
{
    public class DashboardDataModel
    {
        public OrderByYearDataModel OrderByYear { get; set; } = new OrderByYearDataModel();
    }

    public class OrderByYearDataModel
    {
        public OrderByYearFilterModel Filter { get; set; } = new OrderByYearFilterModel();
        public List<OrderModel> OrderModels { get; set; } = new List<OrderModel>();

        public List<int> OrderGroupMonths
        {
            get
            {
                var report = OrderModels.GroupBy(g => g.InsertTime.Value.Month).Select(s => new
                {
                    Month = s.Key,
                    Total = s.Count()
                });

                Dictionary<int, int> months = new Dictionary<int, int>()
                {
                    { 1, 0},
                    { 2, 0},
                    { 3, 0},
                    { 4, 0},
                    { 5, 0},
                    { 6, 0},
                    { 7, 0},
                    { 8, 0},
                    { 9, 0},
                    { 10, 0},
                    { 11, 0},
                    { 12, 0}
                };

                foreach (var item in report)
                {
                    months[item.Month] = item.Total;
                }

                return months.Select(s=> s.Value).ToList();
            }
        }
    }
}
