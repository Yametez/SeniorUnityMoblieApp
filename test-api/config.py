import mysql.connector
from dotenv import load_dotenv
import os

# โหลดค่าจากไฟล์ .env
load_dotenv()

def get_db_connection():
    try:
        connection = mysql.connector.connect(
            host=os.getenv('TIDB_HOST'),
            port=int(os.getenv('TIDB_PORT', 4000)),  # เพิ่ม port
            user=os.getenv('TIDB_USER'),
            password=os.getenv('TIDB_PASSWORD'),
            database=os.getenv('TIDB_DATABASE')
        )
        return connection
    except Exception as e:
        print(f"Database connection error: {str(e)}")
        raise e 