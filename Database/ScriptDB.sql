
CREATE DATABASE SalesDB;
GO

USE SalesDB;
GO


CREATE TABLE NhanVien
(
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    TenNV NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) UNIQUE,
    ChucVu NVARCHAR(50)
);
GO


CREATE TABLE KhachHang
(
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    DiemTichLuy INT DEFAULT 0
);
GO


CREATE TABLE SanPham
(
    MaSP VARCHAR(20) PRIMARY KEY,
    TenSP NVARCHAR(150) NOT NULL,
    GiaBan DECIMAL(18,2) CHECK (GiaBan > 0),
    SoLuongTon INT CHECK (SoLuongTon >= 0)
);
GO


CREATE TABLE HoaDon
(
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap DATETIME DEFAULT GETDATE(),

    MaNV INT NOT NULL,
    MaKH INT NULL,

    TongTien DECIMAL(18,2) NOT NULL,

    CONSTRAINT FK_HoaDon_NhanVien
        FOREIGN KEY (MaNV)
        REFERENCES NhanVien(MaNV),

    CONSTRAINT FK_HoaDon_KhachHang
        FOREIGN KEY (MaKH)
        REFERENCES KhachHang(MaKH)
);
GO


CREATE TABLE ChiTietHoaDon
(
    MaHD INT NOT NULL,
    MaSP VARCHAR(20) NOT NULL,

    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL,

    PRIMARY KEY (MaHD, MaSP),

    CONSTRAINT FK_CTHD_HoaDon
        FOREIGN KEY (MaHD)
        REFERENCES HoaDon(MaHD),

    CONSTRAINT FK_CTHD_SanPham
        FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP)
);
GO

INSERT INTO NhanVien(TenNV, SoDienThoai, ChucVu)
VALUES
(N'Nguyễn Văn An', '0901234567', N'Thu ngân'),
(N'Trần Thị Bình', '0901234568', N'Quản lý');

INSERT INTO KhachHang(TenKH, DienThoai, DiemTichLuy)
VALUES
(N'Phạm Minh Tuấn', '0912345678', 0),
(N'Lê Thị Hoa', '0912345679', 0);

INSERT INTO SanPham(MaSP, TenSP, GiaBan, SoLuongTon)
VALUES
('SP001', N'Nước suối', 5000, 100),
('SP002', N'Mì Hảo Hảo', 4500, 200),
('SP003', N'Sữa tươi', 12000, 50),
('SP004', N'Bánh Oreo', 15000, 80),
('SP005', N'Nước ngọt Coca', 10000, 120);
GO

SELECT * FROM SanPham;

GO
