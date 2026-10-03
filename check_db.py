import pyodbc

# Assuming standard connection string
conn_str = 'Driver={SQL Server};Server=localhost\\SQLEXPRESS;Database=StudentManagementSystem;Trusted_Connection=yes;'
try:
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()
    cursor.execute("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MonHoc'")
    cols = cursor.fetchall()
    for c in cols:
        print(f"{c[0]}: {c[1]}")
except Exception as e:
    print(e)
