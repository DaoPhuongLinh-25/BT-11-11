namespace DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public int MaNV { get; set; }
        public int? MaKH { get; set; } // Nullable nếu là khách lẻ
        public decimal TongTien { get; set; }
    }
}
