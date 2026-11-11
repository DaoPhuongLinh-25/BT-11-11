
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.DAL
{
    public class KhachHangDAL
    {
        // 1. Lấy toàn bộ khách hàng
        public List<KhachHangDTO> GetAll()
        {
            List<KhachHangDTO> ds = new List<KhachHangDTO>();

            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM KhachHang";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        KhachHangDTO kh = new KhachHangDTO();

                        kh.MaKH = Convert.ToInt32(reader["MaKH"]);
                        kh.TenKH = reader["TenKH"].ToString();
                        kh.DienThoai = reader["DienThoai"].ToString();
                        kh.DiemTichLuy = Convert.ToInt32(reader["DiemTichLuy"]);

                        ds.Add(kh);
                    }
                }
            }

            return ds;
        }

        // 2. Thêm khách hàng
        public bool Insert(KhachHangDTO kh)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = @"INSERT INTO KhachHang
                               (TenKH, DienThoai)
                               VALUES (@TenKH, @DienThoai)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                    cmd.Parameters.AddWithValue(
                        "@DienThoai",
                        (object)kh.DienThoai ?? DBNull.Value
                    );

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 3. Sửa khách hàng
        public bool Update(KhachHangDTO kh)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE KhachHang
                               SET TenKH = @TenKH,
                                   DienThoai = @DienThoai
                               WHERE MaKH = @MaKH";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", kh.MaKH);
                    cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                    cmd.Parameters.AddWithValue(
                        "@DienThoai",
                        (object)kh.DienThoai ?? DBNull.Value
                    );

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 4. Xóa khách hàng
        public bool Delete(int maKH)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM KhachHang WHERE MaKH = @MaKH";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", maKH);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Tìm kiếm khách hàng theo tên hoặc điện thoại
        public List<KhachHangDTO> Search(string tuKhoa)
        {
            List<KhachHangDTO> ds = new List<KhachHangDTO>();

            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT * FROM KhachHang
                               WHERE TenKH LIKE @TuKhoa
                               OR DienThoai LIKE @TuKhoa";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            KhachHangDTO kh = new KhachHangDTO();

                            kh.MaKH = Convert.ToInt32(reader["MaKH"]);
                            kh.TenKH = reader["TenKH"].ToString();
                            kh.DienThoai = reader["DienThoai"].ToString();
                            kh.DiemTichLuy = Convert.ToInt32(reader["DiemTichLuy"]);

                            ds.Add(kh);
                        }
                    }
                }
            }

            return ds;
        }
    }
}
