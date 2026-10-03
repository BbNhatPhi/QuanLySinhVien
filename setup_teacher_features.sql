IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DiemDanh' and xtype='U')
BEGIN
    CREATE TABLE DiemDanh (
        DiemDanhID INT IDENTITY(1,1) PRIMARY KEY,
        MaLopHP VARCHAR(50) NOT NULL,
        MaSV VARCHAR(50) NOT NULL,
        NgayHoc DATE NOT NULL,
        TrangThai NVARCHAR(50) NOT NULL,
        GhiChu NVARCHAR(255) NULL
    );
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ThongBaoLop' and xtype='U')
BEGIN
    CREATE TABLE ThongBaoLop (
        ThongBaoID INT IDENTITY(1,1) PRIMARY KEY,
        MaLopHP VARCHAR(50) NOT NULL,
        TieuDe NVARCHAR(200) NOT NULL,
        NoiDung NVARCHAR(MAX) NOT NULL,
        NgayDang DATETIME DEFAULT GETDATE()
    );
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaiLieuLop' and xtype='U')
BEGIN
    CREATE TABLE TaiLieuLop (
        TaiLieuID INT IDENTITY(1,1) PRIMARY KEY,
        MaLopHP VARCHAR(50) NOT NULL,
        TenTaiLieu NVARCHAR(200) NOT NULL,
        DuongDan NVARCHAR(500) NOT NULL,
        NgayTaiLen DATETIME DEFAULT GETDATE()
    );
END
