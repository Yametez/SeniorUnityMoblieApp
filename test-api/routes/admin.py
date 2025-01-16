from flask import Blueprint, request, jsonify
from config import get_db_connection

admin_bp = Blueprint('admin', __name__)

# Get all admins
@admin_bp.route('/', methods=['GET'])
def get_all_admins():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Admin')
    admins = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(admins)

# Get admin by ID
@admin_bp.route('/<int:admin_id>', methods=['GET'])
def get_admin(admin_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Admin WHERE Admin_ID = %s', (admin_id,))
    admin = cursor.fetchone()
    cursor.close()
    connection.close()
    if admin:
        return jsonify(admin)
    return jsonify({'message': 'Admin not found'}), 404

# Create new admin
@admin_bp.route('/', methods=['POST'])
def create_admin():
    try:
        data = request.json
        connection = get_db_connection()
        cursor = connection.cursor()

        # ตรวจสอบว่ามีข้อมูลครบไหม
        required_fields = ['Name', 'Surname', 'Email', 'Password']
        for field in required_fields:
            if field not in data:
                return jsonify({'error': f'Missing required field: {field}'}), 400

        # หา ID ล่าสุดและบวกเพิ่ม 1
        cursor.execute('SELECT MAX(Admin_ID) FROM Admin')
        max_id = cursor.fetchone()[0]
        new_id = 1001 if max_id is None else max_id + 1  # เริ่มที่ 1001 ถ้าไม่มีข้อมูล

        # เพิ่มข้อมูลโดยระบุ ID
        cursor.execute(
            'INSERT INTO Admin (Admin_ID, Name, Surname, Email, Password) VALUES (%s, %s, %s, %s, %s)',
            (new_id, data['Name'], data['Surname'], data['Email'], data['Password'])
        )
        connection.commit()
        cursor.close()
        connection.close()

        return jsonify({'message': 'Admin created successfully', 'Admin_ID': new_id}), 201

    except Exception as e:
        return jsonify({'error': str(e)}), 500

# Update admin
@admin_bp.route('/<int:admin_id>', methods=['PUT'])
def update_admin(admin_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE Admin SET Name = %s, Surname = %s, Email = %s, Password = %s WHERE Admin_ID = %s',
        (data['Name'], data['Surname'], data['Email'], data['Password'], admin_id)
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
    cursor.execute('DELETE FROM Admin WHERE Admin_ID = %s', (admin_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Admin deleted successfully'})