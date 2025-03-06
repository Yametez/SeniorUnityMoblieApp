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
    cursor.execute('SELECT * FROM users WHERE User_ID = %s', (user_id,))
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

        # ตรวจสอบ Gender
        if data['Gender'] not in ['Male', 'Female']:
            return jsonify({'error': 'Gender must be either "Male" or "Female"'}), 400

        # กำหนด auth_type (เป็น 'email' อย่างเดียว)
        role = data.get('role', 'user')

        # หา ID ล่าสุดตาม role
        if role == 'admin':
            cursor.execute('SELECT MAX(User_ID) FROM users WHERE role = "admin"')
            max_id = cursor.fetchone()[0]
            new_id = 1001 if max_id is None else max_id + 1
        else:
            cursor.execute('SELECT MAX(User_ID) FROM users WHERE role = "user"')
            max_id = cursor.fetchone()[0]
            new_id = 101 if max_id is None else max_id + 1

        cursor.execute('''
            INSERT INTO users 
            (User_ID, Name, Surname, Email, Password, Age, Gender, role, auth_type) 
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, 'email')
        ''', (new_id, data['Name'], data['Surname'], data['Email'], 
              data['Password'], data['Age'], data['Gender'], role))
        
        connection.commit()
        cursor.close()
        connection.close()

        return jsonify({
            'message': f'{role.capitalize()} created successfully',
            'User_ID': new_id
        }), 201

    except Exception as e:
        return jsonify({'error': str(e)}), 500

@users_bp.route('/<int:user_id>', methods=['PUT'])
def update_user(user_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE users SET name = %s, surname = %s, password = %s, age = %s, gender = %s WHERE User_ID = %s',
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
    cursor.execute('DELETE FROM users WHERE User_ID = %s', (user_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'User deleted successfully'})