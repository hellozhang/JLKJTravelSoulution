using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ECardPass.Project.Domain.Entities;
using ECardPass.Project.IBLL.Order;
using ECardPass.Project.Infrastructure;
using ECardPass.Project.WebApi.WebLibrary;
using ECardPass.Project.DTO.CCBPay;
using System.Configuration;
using ECardPass.Project.IBLL.Marketing;
using ECardPass.Project.DTO.Enums;
using ECardPass.Project.Domain.Entities.Specialty;
using ECardPass.Project.IBLL.Specialty;
using ECardPass.Project.IBLL.BatchGiftCard;

namespace ECardPass.Project.WebApi.Controllers
{
    public class CCBPayController : ApiBaseController
    {
        private IT_OrderService _it_OrderService;
        private IT_PromotionsService PromotionsService;
        private IT_SpecialOrderService _it_SpecialOrderService;
        private IT_GiftCardChargeBillService giftCardChargeBillService;
        private IT_GiftCardService giftCardService;
        public CCBPayController()
        {
            this._it_OrderService = DIContainer.Resolve<IT_OrderService>();
            base.AddDisposableObject(this._it_OrderService);

            this.PromotionsService = DIContainer.Resolve<IT_PromotionsService>();
            base.AddDisposableObject(this.PromotionsService);

            this._it_SpecialOrderService = DIContainer.Resolve<IT_SpecialOrderService>();
            base.AddDisposableObject(this._it_SpecialOrderService);

            this.giftCardChargeBillService = DIContainer.Resolve<IT_GiftCardChargeBillService>();
            base.AddDisposableObject(this.giftCardChargeBillService);

            this.giftCardService = DIContainer.Resolve<IT_GiftCardService>();
            base.AddDisposableObject(this.giftCardService);
        }
        /// <summary>
        /// 建行一卡通支付
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> EcardPassPay(Guid id, string issinscode, string channel)
        {
            return await Task.Run(() =>
          {
              ResultMsg _resultMsg = new ResultMsg();
              long orderNo = 0; decimal totalPayPrice = 0;
              var partmeter = GetEcardPassPayPartmeter(id, channel);
              orderNo = partmeter.Item1; totalPayPrice = partmeter.Item2;
              if (orderNo <= 0)
              {
                  _resultMsg.Info = "订单编号有误,请再次操作";
                  _resultMsg.IsSuccess = false;
                  return _resultMsg.ToJson().ResponseMessage();
              }
              CCBPayParam ccb = new CCBPayParam();
              ccb.ORDERID = orderNo + "-" + DateTime.Now.ToString("mmssfff");
              ccb.POSID = ConfigurationManager.AppSettings["ECardPassPOSID"];
              decimal _minTotalPayPrice = (0.01).ToDecimal();
              if (totalPayPrice < _minTotalPayPrice)
              {
                  totalPayPrice = _minTotalPayPrice;
              }
              ccb.PAYMENT = totalPayPrice.ToDecimal(2);
              ccb.ISSINSCODE = issinscode;
              ccb.THIRDAPPINFO = "comccbpay" + ccb.MERCHANTID + "alipay";
              if (!issinscode.ToUpper().Equals("CCB"))
              {
                  ccb.GATEWAY = "UnionPay";
              }
              else
              {
                  ccb.GATEWAY = "";
              }
              string PublicKey = ConfigurationManager.AppSettings["ECardPassPublicKey"];
              ccb.PUB = PublicKey.Substring(PublicKey.Length - 30, 30);
              #region 字符拼接
              string _firstparam = "MERCHANTID=" + ccb.MERCHANTID.Trim() +
                                   "&POSID=" + ccb.POSID.Trim() +
                                   "&BRANCHID=" + ccb.BRANCHID.Trim() +
                                   "&ORDERID=" + ccb.ORDERID.Trim() +
                                   "&PAYMENT=" + ccb.PAYMENT.ToString().Trim() +
                                   "&CURCODE=" + ccb.CURCODE +
                                   "&TXCODE=" + ccb.TXCODE.Trim() +
                                   "&REMARK1=&REMARK2=&TYPE=1";

              string _secondparam = "&PUB=" + ccb.PUB;

              string _threeparam = "&GATEWAY=" + ccb.GATEWAY +
                                    "&CLIENTIP=" + ccb.CLIENTIP +
                                    "&REGINFO=" + ccb.REGINFO +
                                    "&PROINFO=" + ccb.PROINFO +
                                    "&REFERER=" + ccb.REFERER +
                                    "&THIRDAPPINFO=" + ccb.THIRDAPPINFO +
                                    "&ISSINSCODE=" + ccb.ISSINSCODE;
              #endregion
              //要加密的串
              ccb.MAC = Md5.md5(_firstparam.Trim() + _secondparam.Trim() + _threeparam.Trim(), 32).ToLower().Trim();
              string strURl = ccb.Url + "?" + _firstparam.Trim() + _threeparam.Trim() + "&MAC=" + ccb.MAC.Trim();
              _resultMsg.Data = strURl;

              return _resultMsg.ToJson().ResponseMessage();
          });
        }

        /// <summary>
        /// 建行卡通支付
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Pay(Guid id, string issinscode, string channel)
        {
            return await Task.Run(() =>
            {
                ResultMsg _resultMsg = new ResultMsg();
                long orderNo = 0; decimal totalPayPrice = 0;
                var partmeter = GetPayPartmeter(id, channel);
                orderNo = partmeter.Item1; totalPayPrice = partmeter.Item2;
                if (orderNo <= 0)
                {
                    _resultMsg.Info = "订单编号有误,请再次操作";
                    _resultMsg.IsSuccess = false;
                    return _resultMsg.ToJson().ResponseMessage();
                }
                CCBPayParam ccb = new CCBPayParam();
                ccb.ORDERID = orderNo + "-" + DateTime.Now.ToString("mmssfff");
                ccb.POSID = ConfigurationManager.AppSettings["POSID"];
                decimal _minTotalPayPrice = (0.01).ToDecimal();
                if (totalPayPrice < _minTotalPayPrice)
                {
                    totalPayPrice = _minTotalPayPrice;
                }
                ccb.PAYMENT = totalPayPrice.ToDecimal(2);
                ccb.ISSINSCODE = issinscode;
                if (!issinscode.ToUpper().Equals("CCB"))
                {
                    ccb.GATEWAY = "UnionPay";
                }
                else
                {
                    ccb.GATEWAY = "";
                }

                string PublicKey = ConfigurationManager.AppSettings["PublicKey"];
                ccb.PUB = PublicKey.Substring(PublicKey.Length - 30, 30);
                ccb.THIRDAPPINFO = "comccbpay" + ccb.MERCHANTID + "alipay";
                #region 字符拼接
                string _firstparam = "MERCHANTID=" + ccb.MERCHANTID.Trim() +
                                        "&POSID=" + ccb.POSID.Trim() +
                                        "&BRANCHID=" + ccb.BRANCHID.Trim() +
                                        "&ORDERID=" + ccb.ORDERID.Trim() +
                                        "&PAYMENT=" + ccb.PAYMENT.ToString().Trim() +
                                        "&CURCODE=" + ccb.CURCODE +
                                        "&TXCODE=" + ccb.TXCODE.Trim() +
                                        "&REMARK1=&REMARK2=&TYPE=1";

                string _secondparam = "&PUB=" + ccb.PUB;

                string _threeparam = "&GATEWAY=" + ccb.GATEWAY +
                                      "&CLIENTIP=" + ccb.CLIENTIP +
                                      "&REGINFO=" + ccb.REGINFO +
                                      "&PROINFO=" + ccb.PROINFO +
                                      "&REFERER=" + ccb.REFERER +
                                      "&THIRDAPPINFO=" + ccb.THIRDAPPINFO +
                                      "&ISSINSCODE=" + ccb.ISSINSCODE;
                #endregion
                //要加密的串
                ccb.MAC = Md5.md5(_firstparam.Trim() + _secondparam.Trim() + _threeparam.Trim(), 32).ToLower().Trim();
                string strURl = ccb.Url + "?" + _firstparam.Trim() + _threeparam.Trim() + "&MAC=" + ccb.MAC.Trim();
                _resultMsg.Data = strURl;

                return _resultMsg.ToJson().ResponseMessage();
            });
        }


        /// <summary>
        /// 建行卡通支付
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> PayBankEconomic(Guid carid, string issinscode, string fee)
        {
            return await Task.Run(() =>
            {
                ResultMsg _resultMsg = new ResultMsg();
                string orderNo = string.Empty; decimal totalPayPrice = 0;
                var partmeter = GetPayPartmeterByCard(carid, fee.ToDecimal());
                orderNo = partmeter.Item1; totalPayPrice = partmeter.Item2;
               
                CCBPayParam ccb = new CCBPayParam();
                ccb.ORDERID = orderNo;
                ccb.POSID = ConfigurationManager.AppSettings["POSID"];
                decimal _minTotalPayPrice = (0.01).ToDecimal();
                if (totalPayPrice < _minTotalPayPrice)
                {
                    totalPayPrice = _minTotalPayPrice;
                }
                ccb.PAYMENT = totalPayPrice.ToDecimal(2);
                ccb.ISSINSCODE = issinscode;
                if (!issinscode.ToUpper().Equals("CCB"))
                {
                    ccb.GATEWAY = "UnionPay";
                }
                else
                {
                    ccb.GATEWAY = "";
                }

                string PublicKey = ConfigurationManager.AppSettings["PublicKey"];
                ccb.PUB = PublicKey.Substring(PublicKey.Length - 30, 30);
                ccb.THIRDAPPINFO = "comccbpay" + ccb.MERCHANTID + "alipay";
                #region 字符拼接
                string _firstparam = "MERCHANTID=" + ccb.MERCHANTID.Trim() +
                                        "&POSID=" + ccb.POSID.Trim() +
                                        "&BRANCHID=" + ccb.BRANCHID.Trim() +
                                        "&ORDERID=" + ccb.ORDERID.Trim() +
                                        "&PAYMENT=" + ccb.PAYMENT.ToString().Trim() +
                                        "&CURCODE=" + ccb.CURCODE +
                                        "&TXCODE=" + ccb.TXCODE.Trim() +
                                        "&REMARK1=&REMARK2=&TYPE=1";

                string _secondparam = "&PUB=" + ccb.PUB;

                string _threeparam = "&GATEWAY=" + ccb.GATEWAY +
                                      "&CLIENTIP=" + ccb.CLIENTIP +
                                      "&REGINFO=" + ccb.REGINFO +
                                      "&PROINFO=" + ccb.PROINFO +
                                      "&REFERER=" + ccb.REFERER +
                                      "&THIRDAPPINFO=" + ccb.THIRDAPPINFO +
                                      "&ISSINSCODE=" + ccb.ISSINSCODE;
                #endregion
                //要加密的串
                ccb.MAC = Md5.md5(_firstparam.Trim() + _secondparam.Trim() + _threeparam.Trim(), 32).ToLower().Trim();
                string strURl = ccb.Url + "?" + _firstparam.Trim() + _threeparam.Trim() + "&MAC=" + ccb.MAC.Trim();
                _resultMsg.Data = strURl;

                return _resultMsg.ToJson().ResponseMessage();
            });
        }


        /// <summary>
        /// 建行一卡通支付
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> EcardPassPayEconomic(Guid carid, string issinscode, string fee)
        {
            return await Task.Run(() =>
            {
                ResultMsg _resultMsg = new ResultMsg();
                string orderNo = string.Empty; decimal totalPayPrice = 0;
                var partmeter = GetPayPartmeterByCard(carid, fee.ToDecimal());
                orderNo = partmeter.Item1; totalPayPrice = partmeter.Item2;
              
                CCBPayParam ccb = new CCBPayParam();
                ccb.ORDERID = orderNo;
                ccb.POSID = ConfigurationManager.AppSettings["ECardPassPOSID"];
                decimal _minTotalPayPrice = (0.01).ToDecimal();
                if (totalPayPrice < _minTotalPayPrice)
                {
                    totalPayPrice = _minTotalPayPrice;
                }
                ccb.PAYMENT = totalPayPrice.ToDecimal(2);
                ccb.ISSINSCODE = issinscode;
                ccb.THIRDAPPINFO = "comccbpay" + ccb.MERCHANTID + "alipay";
                if (!issinscode.ToUpper().Equals("CCB"))
                {
                    ccb.GATEWAY = "UnionPay";
                }
                else
                {
                    ccb.GATEWAY = "";
                }
                string PublicKey = ConfigurationManager.AppSettings["ECardPassPublicKey"];
                ccb.PUB = PublicKey.Substring(PublicKey.Length - 30, 30);
                #region 字符拼接
                string _firstparam = "MERCHANTID=" + ccb.MERCHANTID.Trim() +
                                     "&POSID=" + ccb.POSID.Trim() +
                                     "&BRANCHID=" + ccb.BRANCHID.Trim() +
                                     "&ORDERID=" + ccb.ORDERID.Trim() +
                                     "&PAYMENT=" + ccb.PAYMENT.ToString().Trim() +
                                     "&CURCODE=" + ccb.CURCODE +
                                     "&TXCODE=" + ccb.TXCODE.Trim() +
                                     "&REMARK1=&REMARK2=&TYPE=1";

                string _secondparam = "&PUB=" + ccb.PUB;

                string _threeparam = "&GATEWAY=" + ccb.GATEWAY +
                                      "&CLIENTIP=" + ccb.CLIENTIP +
                                      "&REGINFO=" + ccb.REGINFO +
                                      "&PROINFO=" + ccb.PROINFO +
                                      "&REFERER=" + ccb.REFERER +
                                      "&THIRDAPPINFO=" + ccb.THIRDAPPINFO +
                                      "&ISSINSCODE=" + ccb.ISSINSCODE;
                #endregion
                //要加密的串
                ccb.MAC = Md5.md5(_firstparam.Trim() + _secondparam.Trim() + _threeparam.Trim(), 32).ToLower().Trim();
                string strURl = ccb.Url + "?" + _firstparam.Trim() + _threeparam.Trim() + "&MAC=" + ccb.MAC.Trim();
                _resultMsg.Data = strURl;

                return _resultMsg.ToJson().ResponseMessage();
            });
        }

        private Tuple<long, decimal> GetPayPartmeter(Guid id, string channel)
        {
            decimal totalPayPrice = 0;
            long orderNo = 0;
            if (!string.IsNullOrWhiteSpace(channel) && channel.ToLower().Equals("Specialty".ToLower()))
            {
                T_SpecialOrder order = _it_SpecialOrderService.FindEntityById(id);
                if (order != null)
                {
                    totalPayPrice = (order.TotalAmount);
                    orderNo = order.OrderNo;
                }
            }
            else
            {
                T_Order _order = _it_OrderService.FindEntityById(id);
                if (_order != null)
                {
                    if (!_order.PromotionsId.IsEmpty())
                    {
                        T_Promotions _promotions = PromotionsService.FindEntityById(_order.PromotionsId);
                        if (_promotions != null && _promotions.PayCondition == (int)E_Pay.ECardPass)
                        {
                            _order.TotalPayPrice = _order.TotalSalePrice; //如果是一卡通专享则其他支付不享受
                        }
                    }
                    totalPayPrice = _order.TotalPayPrice;
                    orderNo = _order.OrderNo;
                }
            }

            return Tuple.Create<long, decimal>(orderNo, totalPayPrice);
        }


        private Tuple<string, decimal> GetPayPartmeterByCard(Guid carid, decimal fee)
        {
            decimal totalPayPrice = 0;
            string orderNo = string.Empty;
            totalPayPrice = (fee);
            orderNo = RandomHelper.CreateNumCode(3)+DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var giftcard = giftCardService.FindEntityByIdToDB(carid);
            if (giftcard != null)
            {
                giftCardChargeBillService.AddEntity(new T_GiftCardChargeBill()
                {
                    ConsumeFee = 0,
                    GiftCardID = carid,
                    ConsumeType = (int)E_ConsumeType.Recharge,
                    Id = Guid.NewGuid(),
                    Out_trade_no = orderNo,
                    CreatorAccount = giftcard.CreatorAccount,
                    CreatorTime = DateTime.Now,
                    CustomerId = giftcard.BatchId,
                    CustomerName = giftcard.CustomerName,
                    CreatorUserId = giftcard.CreatorUserId,
                    LastModifyTime = DateTime.Now
                });

            }
            return Tuple.Create<string, decimal>(orderNo, totalPayPrice);
        }

        private Tuple<long, decimal> GetEcardPassPayPartmeter(Guid id, string channel)
        {
            decimal totalPayPrice = 0;
            long orderNo = 0;
            if (!string.IsNullOrWhiteSpace(channel) && channel.ToLower().Equals("Specialty".ToLower()))
            {
                T_SpecialOrder order = _it_SpecialOrderService.FindEntityById(id);
                if (order != null)
                {
                    totalPayPrice = (order.TotalAmount);
                    orderNo = order.OrderNo;
                }
            }
            else
            {
                T_Order _order = _it_OrderService.FindEntityById(id);
                if (_order != null)
                {
                    totalPayPrice = (_order.TotalPayPrice);
                    orderNo = _order.OrderNo;
                }
            }

            return Tuple.Create<long, decimal>(orderNo, totalPayPrice);
        }
    }
}
