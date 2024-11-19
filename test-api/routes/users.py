from flask import Blueprint, request, jsonify
from config import get_db_connection

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
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'INSERT INTO users (name, surname, password, age, gender) VALUES (%s, %s, %s, %s, %s)',
        (data['name'], data['surname'], data['password'], data['age'], data['gender'])
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'User created successfully', 'userId': cursor.lastrowid}), 201

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