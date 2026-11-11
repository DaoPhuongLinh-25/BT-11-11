
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.DAL
{
    public class NhanVienDAL
    {
        // Lấy toàn bộ danh sách nhân viên
        public DataTable GetAll()
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = "SELECT * FROM NhanVien ORDER BY MaNV DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // Thêm nhân viên
        public bool Insert(NhanVienDTO nv)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"INSERT INTO NhanVien
                               (TenNV, SoDienThoai, ChucVu)
                               VALUES (@TenNV, @SoDienThoai, @ChucVu)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TenNV", nv.TenNV);
                    cmd.Parameters.AddWithValue("@SoDienThoai",
                        (object)nv.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChucVu",
                        (object)nv.ChucVu ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Sửa nhân viên
        public bool Update(NhanVienDTO nv)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"UPDATE NhanVien
                               SET TenNV = @TenNV,
                                   SoDienThoai = @SoDienThoai,
                                   ChucVu = @ChucVu
                               WHERE MaNV = @MaNV";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
                    cmd.Parameters.AddWithValue("@TenNV", nv.TenNV);
                    cmd.Parameters.AddWithValue("@SoDienThoai",
                        (object)nv.SoDienThoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChucVu",
                        (object)nv.ChucVu ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xóa nhân viên
        public bool Delete(int maNV)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = "DELETE FROM NhanVien WHERE MaNV = @MaNV";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Tìm kiếm nhân viên
        public DataTable Search(string keyword)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"SELECT * FROM NhanVien
                               WHERE TenNV LIKE @Keyword
                                  OR SoDienThoai LIKE @Keyword
                                  OR ChucVu LIKE @Keyword
                               ORDER BY MaNV DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@Keyword", "%" + (keyword ?? "") + "%");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
