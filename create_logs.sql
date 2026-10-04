IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemLogs](
        [LogID] [int] IDENTITY(1,1) PRIMARY KEY,
        [Username] [varchar](50) NULL,
        [Action] [nvarchar](max) NOT NULL,
        [CreatedAt] [datetime] DEFAULT GETDATE(),
        [IPAddress] [varchar](50) NULL
    );
END
