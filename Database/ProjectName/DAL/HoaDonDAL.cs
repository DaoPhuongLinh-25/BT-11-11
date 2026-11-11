
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.DAL
{
    public class HoaDonDAL
    {
        // Thanh toán và lưu hóa đơn bằng Transaction
        public int ThanhToan(
            HoaDonDTO hd,
            List<ChiTietHoaDonDTO> danhSachChiTiet)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Thêm hóa đơn
                        string sqlHoaDon = @"
                            INSERT INTO HoaDon
                                (NgayLap, MaNV, MaKH, TongTien)
                            OUTPUT INSERTED.MaHD
                            VALUES
                                (@NgayLap, @MaNV, @MaKH, @TongTien)";

                        int maHD;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlHoaDon, conn, transaction))
                        {
                            cmd.Parameters.Add("@NgayLap", SqlDbType.DateTime)
                                .Value = hd.NgayLap;

                            cmd.Parameters.Add("@MaNV", SqlDbType.Int)
                                .Value = hd.MaNV;

                            cmd.Parameters.Add("@MaKH", SqlDbType.Int)
                                .Value = (object)hd.MaKH ?? DBNull.Value;

                            cmd.Parameters.Add("@TongTien", SqlDbType.Decimal)
                                .Value = hd.TongTien;

                            cmd.Parameters["@TongTien"].Precision = 18;
                            cmd.Parameters["@TongTien"].Scale = 2;

                            maHD = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2. Thêm chi tiết hóa đơn và trừ tồn kho
                        foreach (ChiTietHoaDonDTO ct in danhSachChiTiet)
                        {
                            if (ct.SoLuong <= 0)
                                throw new Exception(
                                    "Số lượng sản phẩm phải lớn hơn 0!");

                            // Kiểm tra và trừ số lượng tồn kho
                            string sqlTonKho = @"
                                UPDATE SanPham
                                SET SoLuongTon = SoLuongTon - @SoLuong
                                WHERE MaSP = @MaSP
                                  AND SoLuongTon >= @SoLuong";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlTonKho, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@SoLuong", ct.SoLuong);

                                cmd.Parameters.AddWithValue(
                                    "@MaSP", ct.MaSP);

                                int ketQua = cmd.ExecuteNonQuery();

                                if (ketQua == 0)
                                {
                                    throw new Exception(
                                        "Sản phẩm " + ct.MaSP +
                                        " không tồn tại hoặc không đủ hàng trong kho!");
                                }
                            }

                            // Lưu chi tiết hóa đơn
                            string sqlChiTiet = @"
                                INSERT INTO ChiTietHoaDon
                                    (MaHD, MaSP, SoLuong, DonGia)
                                VALUES
                                    (@MaHD, @MaSP, @SoLuong, @DonGia)";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlChiTiet, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@MaHD", maHD);

                                cmd.Parameters.AddWithValue(
                                    "@MaSP", ct.MaSP);

                                cmd.Parameters.AddWithValue(
                                    "@SoLuong", ct.SoLuong);

                                cmd.Parameters.Add("@DonGia", SqlDbType.Decimal)
                                    .Value = ct.DonGia;

                                cmd.Parameters["@DonGia"].Precision = 18;
                                cmd.Parameters["@DonGia"].Scale = 2;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Hoàn tất giao dịch
                        transaction.Commit();

                        return maHD;
                    }
                    catch
                    {
                        // Có lỗi thì hủy toàn bộ giao dịch
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
