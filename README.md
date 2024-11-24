# MoblieApp
senior project

# Run Api Project
* Cd test-api
* npm install
* npm run dev

# แก้ปัญหาติดตั้ง pyhton ไม่ได้
*    pip install Flask Flask-Cors mysql-connector-python
*    pip show Flask
* * รัน python    
python app.py

# Prerequisites (สิ่งที่ต้องติดตั้งก่อน)
* Node.js
* Python 3.x
* MySQL Server

# Initial Database Setup
1. ติดตั้ง MySQL Server
2. สร้างฐานข้อมูลชื่อ 'user'
3. ตั้งค่าการเชื่อมต่อฐานข้อมูลในไฟล์:
   * test-api/config.py
   * test-api/config/database.js
   * test-api/.env

# ขั้นตอนการติดตั้งครั้งแรก

## 1. ติดตั้ง Node.js Dependencies
bash
cd test-api
npm install cors express mysql2 dotenv nodemon

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
npm run dev

## รัน Python Server
bash
python app.py

# API Endpoints ที่มีให้บริการ
* Users API: /api/users
* Admin API: /api/admin
* Exam API: /api/exam
* Quiz API: /api/quiz
* Report API: /api/report

# หมายเหตุสำคัญ
* ต้องแน่ใจว่า MySQL Server กำลังทำงานอยู่
* API Server จะทำงานที่ port 3000
* ตรวจสอบให้แน่ใจว่าได้ตั้งค่า Database credentials ถูกต้อง
* หากมีปัญหาการเชื่อมต่อ Database ให้ตรวจสอบการตั้งค่าใน config.py และ database.js