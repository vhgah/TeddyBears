using System;
using System.Linq;
using System.Web.Mvc;
using WebShopPet.Models;

namespace WebShopPet.Areas.Admin.Controllers
{
    public class HomesController : BaseController
    {
        private ShopPetDB db = new ShopPetDB();
        // GET: Admin/Home

        public ActionResult Index(string startDate, string endDate)
        {
            int? role = 0;
            if (Session["Role"] != null) role = int.Parse(Session["Role"].ToString());
            if (Session["ID"] == null & role != 1)
            {
                return Redirect("http://localhost:53553/Session/Create");
            }
            ViewBag.TongHang = db.PRODUCTS.Sum(x => x.AVAILABLE_QUANTITY);
            ViewBag.DaBan = db.PRODUCTS.Sum(x => x.QUANTITY_SOLD);
            ViewBag.TongDoanhSo  = db.ORDERS.Sum(x => x.TOTAL_AMOUNT);
            ViewBag.SoUser = db.USERS.Where(x=>x.ROLE == 0).Count();
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now; ;

            var list = new { };

            if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                DateTime date1 = Convert.ToDateTime(startDate);

                DateTime date2 = Convert.ToDateTime(endDate);
                var bookGrouped1 = db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= date1).Where(x => x.ORDER.DATE <= date2).GroupBy(x => x.PRODUCT_ID).Select(x => new Information
                {
                    ID = x.Key,
                    ORDER_DETAILS = x,
                    Amount = x.Sum(b => b.PRODUCT.PRICE * b.QUANTITY - b.PRODUCT.PRICE * (int)b.PRODUCT.DISCOUNT / 100 * b.QUANTITY),
                    Sell = x.Sum(b => b.QUANTITY),
                    Name = x.Select(b => b.PRODUCT.NAME),
                    Profit = x.Sum(b => b.PRODUCT.PRICE * b.QUANTITY - b.PRODUCT.PRICE * (int)b.PRODUCT.DISCOUNT / 100 * b.QUANTITY - b.PRODUCT.IMPORT_PRICE * b.QUANTITY)
                });

                ViewBag.TienThu = db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= date1).Where(x => x.ORDER.DATE <= date2).Sum(x => x.PRODUCT.PRICE * x.QUANTITY - x.PRODUCT.PRICE * x.QUANTITY * x.PRODUCT.DISCOUNT / 100);

                ViewBag.TienLai = db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= date1).Where(x => x.ORDER.DATE <= date2).Sum(x => x.PRODUCT.PRICE * x.QUANTITY - x.PRODUCT.PRICE * x.QUANTITY * x.PRODUCT.DISCOUNT / 100) - db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= date1).Where(x => x.ORDER.DATE <= date2).Sum(x => x.PRODUCT.IMPORT_PRICE * x.QUANTITY);

                return View(bookGrouped1.ToList());
            }

            var bookGrouped = db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= start).Where(x => x.ORDER.DATE <= end).GroupBy(x => x.PRODUCT_ID).Select(x => new Information {
                ID = x.Key,
                Name = x.Select(b => b.PRODUCT.NAME),
                Import_price = x.Select(b=>b.PRODUCT.IMPORT_PRICE),
                Price = x.Select(b => b.PRODUCT.PRICE),
                ORDER_DETAILS = x,
                Amount = x.Sum(b => b.PRODUCT.PRICE * b.QUANTITY - b.PRODUCT.PRICE * (int)b.PRODUCT.DISCOUNT/100 * b.QUANTITY),
                Sell = x.Sum(b=>b.QUANTITY),
                
                Profit = x.Sum(b => b.PRODUCT.PRICE * b.QUANTITY - b.PRODUCT.PRICE * (int)b.PRODUCT.DISCOUNT / 100 * b.QUANTITY - b.PRODUCT.IMPORT_PRICE * b.QUANTITY)
            });

            ViewBag.TienThu = db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= start).Where(x => x.ORDER.DATE <= end).Sum(x => x.PRODUCT.PRICE * x.QUANTITY  - x.PRODUCT.PRICE * x.QUANTITY*x.PRODUCT.DISCOUNT/100);

            ViewBag.TienLai = db.ORDER_DETAILS.Where(x => x.ORDER.DATE >= start).Where(x => x.ORDER.DATE <= end).Sum(x => x.PRODUCT.PRICE * x.QUANTITY - x.PRODUCT.PRICE * x.QUANTITY * x.PRODUCT.DISCOUNT / 100) - db.ORDER_DETAILS.Sum(x => x.PRODUCT.IMPORT_PRICE * x.QUANTITY);

            return View(bookGrouped.ToList());
        }

        //private void ExportExcel()
        //{
        //    ExcelPackage Ep = new ExcelPackage();
        //    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
        //    Sheet.Cells["D1"].Value = "Bảng thống kê bán hàng";
        //    Sheet.Cells["A2"].Value = "Mã hàng";
        //    Sheet.Cells["B2"].Value = "Tên hàng";
        //    Sheet.Cells["C2"].Value = "Giá nhập";
        //    Sheet.Cells["D2"].Value = "Giá bán";
        //    Sheet.Cells["E2"].Value = "Đã bán";
        //    Sheet.Cells["F2"].Value = "Tiền thu";
        //    Sheet.Cells["G2"].Value = "Lợi nhuận trên số lượng bán";

        //    int row = 3;
        //    foreach (var item in bookGrouped)
        //    {
        //        Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
        //        Sheet.Cells[string.Format("B{0}", row)].Value = item.Name;
        //        Sheet.Cells[string.Format("C{0}", row)].Value = item.Import_price;
        //        Sheet.Cells[string.Format("D{0}", row)].Value = item.Price;
        //        Sheet.Cells[string.Format("E{0}", row)].Value = item.Amount;
        //        Sheet.Cells[string.Format("F{0}", row)].Value = item.Sell;
        //        Sheet.Cells[string.Format("G{0}", row)].Value = item.Profit;
        //    }
        //    Sheet.Cells["A:AZ"].AutoFitColumns();
        //    Response.Clear();
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content - disposition", "attachment;  filename=" + "Report.xls");
        //    Response.BinaryWrite(Ep.GetAsByteArray());
        //    Response.End();
        //}
    }
}