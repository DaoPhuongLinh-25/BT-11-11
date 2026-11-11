
using System;
using System.Collections.Generic;
using RetailSalesSystem.DAL;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.BUS
{
    public class HoaDonBUS
    {
        private HoaDonDAL dal = new HoaDonDAL();

        // Thanh toán hóa đơn
        public int ThanhToan(
            HoaDonDTO hd,
            List<ChiTietHoaDonDTO> danhSachChiTiet)
        {
            // Kiểm tra hóa đơn
            if (hd == null)
                throw new Exception("Thông tin hóa đơn không hợp lệ!");

            if (hd.MaNV <= 0)
                throw new Exception("Vui lòng chọn nhân viên!");

            if (hd.MaKH.HasValue && hd.MaKH.Value <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            if (danhSachChiTiet == null ||
                danhSachChiTiet.Count == 0)
            {
                throw new Exception(
                    "Hóa đơn phải có ít nhất một sản phẩm!");
            }

            // Kiểm tra từng sản phẩm
            decimal tongTien = 0;

            foreach (ChiTietHoaDonDTO ct in danhSachChiTiet)
            {
                if (ct == null)
                    throw new Exception(
                        "Thông tin sản phẩm không hợp lệ!");

                if (string.IsNullOrWhiteSpace(ct.MaSP))
                    throw new Exception(
                        "Mã sản phẩm không được để trống!");

                if (ct.SoLuong <= 0)
                    throw new Exception(
                        "Số lượng phải lớn hơn 0!");

                if (ct.DonGia <= 0)
                    throw new Exception(
                        "Đơn giá phải lớn hơn 0!");

                tongTien += ct.SoLuong * ct.DonGia;
            }

            // Tính lại tổng tiền để tránh sai lệch
            hd.TongTien = tongTien;
            hd.NgayLap = DateTime.Now;

            // Gọi DAL thực hiện thanh toán
            return dal.ThanhToan(hd, danhSachChiTiet);
        }
    }
}
