using DAL;
using System;
using IDAL;

namespace IBLL
{
    public class OrderService: IOrderService
    {
        public IOrderDAL OrderDal { get; set; }

        public int GetOrderDetial(string a)
        {
            //string s = Guid.NewGuid().ToString();
            //int j = int.Parse(s);
            return OrderDal.GetOrderID( a);
        }
    }
}
