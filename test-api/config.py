import mysql.connector
from dotenv import load_dotenv
import os

# โหลดค่าจากไฟล์ .env
load_dotenv()

def get_db_connection():
    connection = mysql.connector.connect(
        host=os.getenv('TIDB_HOST'),
        port=int(os.getenv('TIDB_PORT')),
        user=os.getenv('TIDB_USER'),
        password=os.getenv('TIDB_PASSWORD'),
        database=os.getenv('TIDB_DATABASE'),
        ssl_mode="VERIFY_IDENTITY",
        ssl_ca="/etc/ssl/certs/ca-certificates.crt"  # สำหรับ SSL verification
    )
    return connection 