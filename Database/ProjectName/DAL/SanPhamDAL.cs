    
using System.Data;
using Microsoft.Data.SqlClient;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.DAL
{
    public class SanPhamDAL
    {
        // 1. Lấy danh sách sản phẩm
        public DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = "SELECT * FROM SanPham";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // 2. Thêm sản phẩm
        public bool Insert(SanPhamDTO sp)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"INSERT INTO SanPham
                    (MaSP, TenSP, GiaBan, SoLuongTon)
                    VALUES (@MaSP, @TenSP, @GiaBan, @SoLuongTon)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 20).Value = sp.MaSP;
                    cmd.Parameters.Add("@TenSP", SqlDbType.NVarChar, 150).Value = sp.TenSP;

                    SqlParameter gia = cmd.Parameters.Add("@GiaBan", SqlDbType.Decimal);
                    gia.Precision = 18;
                    gia.Scale = 2;
                    gia.Value = sp.GiaBan;

                    cmd.Parameters.Add("@SoLuongTon", SqlDbType.Int).Value = sp.SoLuongTon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 3. Sửa sản phẩm
        public bool Update(SanPhamDTO sp)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"UPDATE SanPham
                    SET TenSP = @TenSP,
                        GiaBan = @GiaBan,
                        SoLuongTon = @SoLuongTon
                    WHERE MaSP = @MaSP";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 20).Value = sp.MaSP;
                    cmd.Parameters.Add("@TenSP", SqlDbType.NVarChar, 150).Value = sp.TenSP;

                    SqlParameter gia = cmd.Parameters.Add("@GiaBan", SqlDbType.Decimal);
                    gia.Precision = 18;
                    gia.Scale = 2;
                    gia.Value = sp.GiaBan;

                    cmd.Parameters.Add("@SoLuongTon", SqlDbType.Int).Value = sp.SoLuongTon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 4. Xóa sản phẩm
        public bool Delete(string maSP)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = "DELETE FROM SanPham WHERE MaSP = @MaSP";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 20).Value = maSP;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Tìm kiếm sản phẩm theo mã hoặc tên
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"SELECT * FROM SanPham
                    WHERE MaSP LIKE @TuKhoa
                    OR TenSP LIKE @TuKhoa";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 200).Value =
                        "%" + keyword + "%";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }
}
