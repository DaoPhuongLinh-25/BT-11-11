using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class BanHangDAL
    {
        private string connectionString = @"Data Source=.;Initial Catalog=SalesDB;Integrated Security=True";

        // 1. Tìm kiếm sản phẩm chống SQL Injection
        public DataTable TimKiemSanPham(string keyword)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM SanPham WHERE TenSP LIKE @keyword OR MaSP LIKE @keyword";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // 2. Thực thi Lập hóa đơn và trừ tồn kho bằng SqlTransaction
        public bool ThanhToanHoaDon(HoaDonDTO hoaDon, List<ChiTietHoaDonDTO> listCTHD, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // Bước 1: Thêm mới bản ghi HoaDon
                    string sqlHD = @"INSERT INTO HoaDon (NgayLap, MaNV, MaKH, TongTien) 
                                    VALUES (@NgayLap, @MaNV, @MaKH, @TongTien);
                                    SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdHD = new SqlCommand(sqlHD, conn, trans);
                    cmdHD.Parameters.AddWithValue("@NgayLap", hoaDon.NgayLap);
                    cmdHD.Parameters.AddWithValue("@MaNV", hoaDon.MaNV);
                    cmdHD.Parameters.AddWithValue("@MaKH", (object)hoaDon.MaKH ?? DBNull.Value);
                    cmdHD.Parameters.AddWithValue("@TongTien", hoaDon.TongTien);

                    // Lấy MaHD tự tăng vừa chèn
                    int maHDVuaTao = Convert.ToInt32(cmdHD.ExecuteScalar());

                    // Bước 2 & 3: Lặp qua từng chi tiết hóa đơn, kiểm tra tồn kho và trừ số lượng
                    foreach (var item in listCTHD)
                    {
                        // Kiểm tra SoLuongTon hiện tại
                        string sqlCheckTon = "SELECT SoLuongTon FROM SanPham WHERE MaSP = @MaSP";
                        SqlCommand cmdCheckTon = new SqlCommand(sqlCheckTon, conn, trans);
                        cmdCheckTon.Parameters.AddWithValue("@MaSP", item.MaSP);

                        object resultTon = cmdCheckTon.ExecuteScalar();
                        if (resultTon == null)
                        {
                            throw new Exception($"Sản phẩm mã '{item.MaSP}' không tồn tại!");
                        }

                        int tonHienTai = Convert.ToInt32(resultTon);
                        if (tonHienTai < item.SoLuong)
                        {
                            // Nếu tồn kho ít hơn bán -> Throw Exception để Rollback
                            throw new Exception($"Sản phẩm mã '{item.MaSP}' không đủ tồn kho! (Tồn: {tonHienTai}, Cần: {item.SoLuong})");
                        }

                        // Chèn ChiTietHoaDon
                        string sqlCTHD = @"INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) 
                                          VALUES (@MaHD, @MaSP, @SoLuong, @DonGia)";
                        SqlCommand cmdCTHD = new SqlCommand(sqlCTHD, conn, trans);
                        cmdCTHD.Parameters.AddWithValue("@MaHD", maHDVuaTao);
                        cmdCTHD.Parameters.AddWithValue("@MaSP", item.MaSP);
                        cmdCTHD.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdCTHD.Parameters.AddWithValue("@DonGia", item.DonGia);
                        cmdCTHD.ExecuteNonQuery();

                        // Trừ SoLuongTon tương ứng
                        string sqlTruTon = @"UPDATE SanPham 
                                            SET SoLuongTon = SoLuongTon - @SoLuong 
                                            WHERE MaSP = @MaSP";
                        SqlCommand cmdTruTon = new SqlCommand(sqlTruTon, conn, trans);
                        cmdTruTon.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdTruTon.Parameters.AddWithValue("@MaSP", item.MaSP);
                        cmdTruTon.ExecuteNonQuery();
                    }

                    // Bước 4: Cộng điểm tích lũy cho Khách hàng (nếu không phải khách lẻ)
                    if (hoaDon.MaKH.HasValue)
                    {
                        int diemCong = (int)(hoaDon.TongTien / 100000);
                        if (diemCong > 0)
                        {
                            string sqlCongDiem = @"UPDATE KhachHang 
                                                  SET DiemTichLuy = DiemTichLuy + @DiemCong 
                                                  WHERE MaKH = @MaKH";
                            SqlCommand cmdCongDiem = new SqlCommand(sqlCongDiem, conn, trans);
                            cmdCongDiem.Parameters.AddWithValue("@DiemCong", diemCong);
                            cmdCongDiem.Parameters.AddWithValue("@MaKH", hoaDon.MaKH.Value);
                            cmdCongDiem.ExecuteNonQuery();
                        }
                    }

                    // Bước 5: Gọi Commit khi toàn bộ thao tác thành công
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Hoàn tác dữ liệu khi có bất kỳ lỗi nào xảy ra
                    trans.Rollback();
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }
    }
}
