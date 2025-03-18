# MoblieApp
senior project

# Run Api Project
* Cd test-api
* python app.py

# แก้ปัญหาติดตั้ง pyhton ไม่ได้
*    pip install Flask Flask-Cors mysql-connector-python
*    pip show Flask
* * รัน python    
python app.py

# Prerequisites (สิ่งที่ต้องติดตั้งก่อน)
* Python 3.x
* MySQL Server
* pip install python-dotenv gunicorn

# Initial Database Setup
1. ติดตั้ง MySQL Server
2. สร้างฐานข้อมูลชื่อ 'game_db'
3. ตั้งค่าการเชื่อมต่อฐานข้อมูลในไฟล์:
   * test-api/config.py
   * test-api/config/database.js
   * test-api/.env


## 2. ติดตั้ง Python Dependencies
bash
pip install Flask Flask-Cors mysql-connector-python

## 3. ตั้งค่าไฟล์ .env
สร้างไฟล์ .env ในโฟลเดอร์ test-api และใส่ข้อมูลดังนี้:
DB_HOST=localhost
DB_USER=root
DB_PASSWORD=your_password
DB_DATABASE=user
PORT=3306

# การรัน Project

## รัน Node.js Server
bash
cd test-api

## รัน Python Server
python app.py

# API Endpoints ที่มีให้บริการ
* users API: /api/users
* admin API: /api/admin
* exam API: /api/exam
* training API: /api/training

# หมายเหตุสำคัญ
* ต้องแน่ใจว่า MySQL Server กำลังทำงานอยู่
* API Server จะทำงานที่ port 3000
* ตรวจสอบให้แน่ใจว่าได้ตั้งค่า Database credentials ถูกต้อง
* หากมีปัญหาการเชื่อมต่อ Database ให้ตรวจสอบการตั้งค่าใน config.py และ database.js