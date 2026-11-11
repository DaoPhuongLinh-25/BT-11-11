using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class BaoCaoDAL
    {
        private string connectionString = @"Data Source=.;Initial Catalog=SalesDB;Integrated Security=True";

        // Tính tổng doanh thu theo Tháng/Năm bằng ExecuteScalar
        public decimal TinhTongDoanhThu(int thang, int nam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ISNULL(SUM(TongTien), 0) 
                                FROM HoaDon 
                                WHERE MONTH(NgayLap) = @Thang AND YEAR(NgayLap) = @Nam";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);

                    object result = cmd.ExecuteScalar();
                    return Convert.ToDecimal(result);
                }
            }
        }

        // Lấy danh sách Top 5 sản phẩm bán chạy nhất bằng SqlDataReader
        public DataTable GetTop5SanPhambanChay()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MaSP", typeof(string));
            dt.Columns.Add("TenSP", typeof(string));
            dt.Columns.Add("TongSoLuongBan", typeof(int));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT TOP 5 sp.MaSP, sp.TenSP, SUM(ct.SoLuong) AS TongSoLuong
                                FROM ChiTietHoaDon ct
                                JOIN SanPham sp ON ct.MaSP = sp.MaSP
                                GROUP BY sp.MaSP, sp.TenSP
                                ORDER BY TongSoLuong DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dt.Rows.Add(
                                reader["MaSP"].ToString(),
                                reader["TenSP"].ToString(),
                                Convert.ToInt32(reader["TongSoLuong"])
                            );
                        }
                    }
                }
            }
            return dt;
        }
    }
}
