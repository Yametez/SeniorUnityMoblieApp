import mysql.connector

def get_db_connection():
    connection = mysql.connector.connect(
        host='localhost',
        user='root',
        password='2545',  # เปลี่ยนเป็น password ของคุณ
        database='user'  # เปลี่ยนเป็นชื่อฐานข้อมูลที่คุณสร้างไว้
    )
    return connection 