using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Util;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Enum.Material;
using ReaLTaiizor.Colors;
using System.IO;
using QuanLyCafe.BLL;
using QuanLyCafe.DTO;

namespace QuanLyCafe.GUI
{
    public partial class XuatHoaDonForm : MaterialForm
    {
        HoaDonBLL hoaDonBLL = new HoaDonBLL();
        TaiKhoanBLL taiKhoanBLL = new TaiKhoanBLL();
        VoucherBLL voucherBLL = new VoucherBLL();
        BanDatBLL banDatBLL = new BanDatBLL();
        BanBLL banBLL = new BanBLL();
        LichSuOrderBLL lichSuOrderBLL = new LichSuOrderBLL();
        HoaDon _hoaDonHienTai = null;
        BanDat _banDatHienTai = null;

        public XuatHoaDonForm()
        {
            try
            {
                InitializeComponent();

                _hoaDonHienTai = ControlForm.HoaDonHienTai;

                GetBanDat();
                if (_hoaDonHienTai != null)
                {

                    lblXuatHoaDon.Text = $"Xuất hóa đơn {_hoaDonHienTai.ID}";

                    ppdXemTruocHoaDon.Document = pdcHoaDon;
                    ppdXemTruocHoaDon.Show();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void GetBanDat()
        {
            if (_hoaDonHienTai != null)
            {
                _hoaDonHienTai = hoaDonBLL.LayThongTinHoaDon(_hoaDonHienTai.ID);
                _banDatHienTai = banDatBLL.LayThongTinBanDatByID(_hoaDonHienTai.IDBanDat);
            }
        }

        private void pdcHoaDon_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                // Kích thước trang in
                float pageWidth = e.PageSettings.PrintableArea.Width;
                float pageHeight = e.PageSettings.PrintableArea.Height;

                // Kích thước và vị trí ban đầu của nội dung
                float contentWidth = 500f; // Chiều rộng nội dung
                float contentHeight = 30f; // Chiều cao mỗi dòng
                float x = (pageWidth - contentWidth) / 2; // Tính x để căn giữa theo chiều ngang
                float y = 10f; // Vị trí y ban đầu

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                // Header
                RectangleF drawRect = new RectangleF(x, y, contentWidth, contentHeight);
                e.Graphics.DrawString($"{HeThong.TenCuaHang}", new Font("Arial", 20, FontStyle.Bold), Brushes.Black, drawRect, sf);

                y += contentHeight;
                drawRect = new RectangleF(x, y, contentWidth, contentHeight);
                e.Graphics.DrawString($"{HeThong.DiaChiCuaHang}", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, drawRect, sf);

                y += contentHeight;
                drawRect = new RectangleF(x, y, contentWidth, contentHeight);
                e.Graphics.DrawString($"Mã Hóa Đơn: {_hoaDonHienTai.ID}", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, drawRect, sf);

                y += contentHeight;
                drawRect = new RectangleF(x, y, contentWidth, contentHeight);
                e.Graphics.DrawString($"Ngày tạo hóa đơn: {_hoaDonHienTai.ThoiGianTao}", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, drawRect, sf);

                y += contentHeight;
                TaiKhoan getNhanVien = taiKhoanBLL.LayThongTinCaNhan(_hoaDonHienTai.NhanVienHoaDon);
                string fullNameNhanVien = $"{getNhanVien.FirstName} {getNhanVien.LastName}";
                drawRect = new RectangleF(x, y, contentWidth, contentHeight);
                e.Graphics.DrawString($"Người tạo hóa đơn: {fullNameNhanVien}", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, drawRect, sf);

                y += 40;

                // Khởi tạo và vẽ bảng chi tiết đơn hàng
                float[] columnWidths = { 50f, 100f, 200f, 150f, 100f, 100f }; // Chiều rộng từng cột
                string[] headers = { "STT", "Số lượng", "Tên sản phẩm", "Đơn giá", "Giảm giá", "Thành tiền" };

                // Vẽ tiêu đề bảng
                float headerHeight = contentHeight + 10f; // Tăng chiều cao tiêu đề để dễ đọc hơn
                float tableWidth = columnWidths.Sum();
                float headerX = x + (contentWidth - tableWidth) / 2; // X cho tiêu đề bảng để căn giữa

                for (int i = 0; i < headers.Length; i++)
                {
                    drawRect = new RectangleF(headerX, y, columnWidths[i], headerHeight);
                    e.Graphics.DrawString(headers[i], new Font("Arial", 12, FontStyle.Bold), Brushes.Black, drawRect, sf);
                    e.Graphics.DrawRectangle(Pens.Black, Rectangle.Round(drawRect)); // Vẽ kẻ ngang và kẻ dọc
                    headerX += columnWidths[i];
                }

                y += headerHeight;

                // Vẽ các dòng chi tiết đơn hàng
                DataTable dt = lichSuOrderBLL.LayThongTinChiTietLichSuOrder(_banDatHienTai.ID);
                int tongTien = 0;
                int stt = 0; // Biến đếm STT
                foreach (DataRow dr in dt.Rows)
                {
                    stt++;
                    float rowDataX = x + (contentWidth - tableWidth) / 2; // X cho dữ liệu hàng để căn giữa

                    string[] rowData = {
                stt.ToString(),
                dr["SOLUONG_LS"].ToString(),
                dr["TEN_SANPHAM_LS"].ToString(),
                string.Format("{0:#,##0}", double.Parse(dr["DONGIA_LS"].ToString())),
                string.Format("{0:#,##0}", double.Parse(dr["DONGIAGIAM_LS"].ToString())),
                string.Format("{0:#,##0}", double.Parse(dr["THANHTIEN_LS"].ToString()))
            };

                    for (int i = 0; i < rowData.Length; i++)
                    {
                        drawRect = new RectangleF(rowDataX, y, columnWidths[i], contentHeight);
                        e.Graphics.DrawString(rowData[i], new Font("Arial", 12, FontStyle.Regular), Brushes.Black, drawRect, sf);
                        e.Graphics.DrawRectangle(Pens.Black, Rectangle.Round(drawRect)); // Vẽ kẻ ngang và kẻ dọc
                        rowDataX += columnWidths[i];
                    }

                    y += contentHeight;
                    tongTien += (int)dr["THANHTIEN_LS"];
                }

                // Tóm tắt
                y += contentHeight;
                float middleX = pageWidth / 2; // Vị trí giữa theo chiều ngang của trang in
                e.Graphics.DrawString($"Tổng cộng: {string.Format("{0:#,##0}", tongTien)}", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new PointF(middleX, y), sf);

                y += contentHeight;
                string dateIn = $"Giờ vào: {_banDatHienTai.ThoiGianVaoBan}";
                e.Graphics.DrawString(
                    dateIn,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    new RectangleF(x, y, contentWidth, contentHeight),
                    sf
                );

                y += contentHeight;

                // Date Out
                string dateOut = $"Giờ ra: {_banDatHienTai.ThoiGianRaBan}";
                e.Graphics.DrawString(
                    dateOut,
                    new Font("Arial", 12, FontStyle.Regular),
                    Brushes.Black,
                    new RectangleF(x, y, contentWidth, contentHeight),
                    sf
                );

                y += 40;

                if (!string.IsNullOrEmpty(_hoaDonHienTai.VoucherHoaDon))
                {
                    Voucher getVoucher = voucherBLL.LayThongTinVoucher(_hoaDonHienTai.VoucherHoaDon);
                    float discountAmount = tongTien * getVoucher.GiamGia / 100;
                    e.Graphics.DrawString($"Voucher: {_hoaDonHienTai.VoucherHoaDon} - Giảm giá: {getVoucher.GiamGia}% - Số tiền giảm: {string.Format("{0:#,##0}", discountAmount)}", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new PointF(x, y));
                    tongTien -= (int)discountAmount;
                }

                y += contentHeight;
                e.Graphics.DrawString($"Khách trả: {string.Format("{0:#,##0}", _hoaDonHienTai.TienKhachTra)}", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new PointF(middleX, y), sf);

                y += contentHeight;
                e.Graphics.DrawString($"Tiền thừa: {string.Format("{0:#,##0}", _hoaDonHienTai.TienThua)}", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new PointF(middleX, y), sf);

                y += 40;
                e.Graphics.DrawString("Xin cảm ơn quý khách!", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new PointF(middleX, y), sf);

                // Đóng form sau khi in
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


    }
}

