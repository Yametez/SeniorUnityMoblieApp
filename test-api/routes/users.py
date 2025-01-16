from flask import Blueprint, request, jsonify
from config import get_db_connection
import mysql.connector
from mysql.connector import Error

users_bp = Blueprint('users', __name__)

@users_bp.route('/', methods=['GET'])
def get_all_users():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM users')
    users = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(users)

@users_bp.route('/<int:user_id>', methods=['GET'])
def get_user(user_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM users WHERE userid = %s', (user_id,))
    user = cursor.fetchone()
    cursor.close()
    connection.close()
    if user:
        return jsonify(user)
    return jsonify({'message': 'User not found'}), 404

@users_bp.route('/', methods=['POST'])
def create_user():
    try:
        data = request.json
        connection = get_db_connection()
        cursor = connection.cursor()

        # ตรวจสอบว่ามีข้อมูลครบไหม
        required_fields = ['Name', 'Surname', 'Email', 'Password', 'Age', 'Gender']
        for field in required_fields:
            if field not in data:
                return jsonify({'error': f'Missing required field: {field}'}), 400

        # ตรวจสอบว่า Gender เป็นค่าที่ถูกต้อง
        if data['Gender'] not in ['Male', 'Female']:
            return jsonify({'error': 'Gender must be either "Male" or "Female"'}), 400

        # หา ID ล่าสุดและบวกเพิ่ม 1
        cursor.execute('SELECT MAX(User_ID) FROM users')
        max_id = cursor.fetchone()[0]
        new_id = 101 if max_id is None else max_id + 1  # เริ่มที่ 101 ถ้าไม่มีข้อมูล

        # เพิ่มข้อมูลโดยระบุ ID
        cursor.execute(
            'INSERT INTO users (User_ID, Name, Surname, Email, Password, Age, Gender) VALUES (%s, %s, %s, %s, %s, %s, %s)',
            (new_id, data['Name'], data['Surname'], data['Email'], data['Password'], data['Age'], data['Gender'])
        )
        connection.commit()
        cursor.close()
        connection.close()

        return jsonify({'message': 'User created successfully', 'User_ID': new_id}), 201

    except Exception as e:
        return jsonify({'error': str(e)}), 500

@users_bp.route('/<int:user_id>', methods=['PUT'])
def update_user(user_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE users SET name = %s, surname = %s, password = %s, age = %s, gender = %s WHERE userid = %s',
        (data['name'], data['surname'], data['password'], data['age'], data['gender'], user_id)
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'User updated successfully'})

@users_bp.route('/<int:user_id>', methods=['DELETE'])
def delete_user(user_id):
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute('DELETE FROM users WHERE userid = %s', (user_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'User deleted successfully'})