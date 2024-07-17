using QuanLyCafe.DAL;
using QuanLyCafe.DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCafe.BLL
{
    public class LichSuOrderBLL
    {
        LichSuOrderDAL dal = new LichSuOrderDAL();

        public LichSuOrder LayThongTinLichSuOrder(int idDatBan, int idHoaDon, string idSanPham)
        {
            try
            {
                return dal.LayThongTinLichSuOrder(idDatBan, idHoaDon, idSanPham);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public DataTable LayThongTinTatCaLichSuOrder(int idHoaDon)
        {
            try
            {
                return dal.LayThongTinTatCaLichSuOrder(idHoaDon);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public DataTable TimLichSuOrderBySanPham(string getDateBatDau, string getDateKetThuc, string idSanPham)
        {
            try
            {
                return dal.TimLichSuOrderBySanPham(getDateBatDau, getDateKetThuc, idSanPham);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public DataTable LayThongTinChiTietLichSuOrder(int idBanDat)
        {
            try
            {
                return dal.LayThongTinChiTietLichSuOrder(idBanDat);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public bool CapNhatThongTinOrder(int soLuong, int donGia, int donGiaGiam, int thanhTien, int idHoaDon, int idDatBan, string idSanPham)
        {
            try
            {
                return dal.CapNhatThongTinOrder(soLuong, donGia, donGiaGiam, thanhTien, idHoaDon, idDatBan, idSanPham);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public int ThemOrder(LichSuOrder lichSuOrder)
        {
            try
            {
                return dal.ThemOrder(lichSuOrder);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public bool XoaOrder(int idHoaDon, int idDatBan, string idSanPham)
        {
            try
            {
                return dal.XoaOrder(idHoaDon, idDatBan, idSanPham);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        private int ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            SqlCommand command = Database.CreateCommand(query);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command.ExecuteNonQuery();
        }
    }

}
