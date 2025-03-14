import mysql.connector
from dotenv import load_dotenv
import os

# โหลดค่าจากไฟล์ .env
load_dotenv()

def get_db_connection():
    connection = mysql.connector.connect(
        host='gateway01.ap-northeast-1.prod.aws.tidbcloud.com',  # TiDB host
        port=4000,  # TiDB port
        user=os.getenv('DB_USER'),
        password=os.getenv('DB_PASSWORD'),
        database=os.getenv('DB_DATABASE'),
        ssl_mode='VERIFY_IDENTITY',
        ssl_ca='/etc/ssl/certs/ca-certificates.crt'  # สำหรับ SSL connection
    )
    return connection 