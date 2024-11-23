from flask import Blueprint, request, jsonify
from config import get_db_connection

admin_bp = Blueprint('admin', __name__)

# Get all admins
@admin_bp.route('/', methods=['GET'])
def get_all_admins():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM admin')
    admins = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(admins)

# Get admin by ID
@admin_bp.route('/<int:admin_id>', methods=['GET'])
def get_admin(admin_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM admin WHERE adminid = %s', (admin_id,))
    admin = cursor.fetchone()
    cursor.close()
    connection.close()
    if admin:
        return jsonify(admin)
    return jsonify({'message': 'Admin not found'}), 404

# Create new admin
@admin_bp.route('/', methods=['POST'])
def create_admin():
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'INSERT INTO admin (name, surname, adminname, password) VALUES (%s, %s, %s, %s)',
        (data['name'], data['surname'], data['adminname'], data['password'])
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Admin created successfully', 'adminid': cursor.lastrowid}), 201

# Update admin
@admin_bp.route('/<int:admin_id>', methods=['PUT'])
def update_admin(admin_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE admin SET name = %s, surname = %s, adminname = %s, password = %s WHERE adminid = %s',
        (data['name'], data['surname'], data['adminname'], data['password'], admin_id)
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Admin updated successfully'})

# Delete admin
@admin_bp.route('/<int:admin_id>', methods=['DELETE'])
def delete_admin(admin_id):
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute('DELETE FROM admin WHERE adminid = %s', (admin_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Admin deleted successfully'})