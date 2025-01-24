from flask import Blueprint, request, jsonify
from config import get_db_connection

admin_bp = Blueprint('admin', __name__)

# Get all admins
@admin_bp.route('/', methods=['GET'])
def get_all_admins():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Users WHERE role = "admin"')
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
        required_fields = ['Name', 'Surname', 'Email', 'Password', 'Age', 'Gender']
        for field in required_fields:
            if field not in data:
                return jsonify({'error': f'Missing required field: {field}'}), 400

        # ตรวจสอบ Gender
        if data['Gender'] not in ['Male', 'Female']:
            return jsonify({'error': 'Gender must be either "Male" or "Female"'}), 400

        # หา ID ล่าสุดสำหรับ admin
        cursor.execute('SELECT MAX(User_ID) FROM Users WHERE role = "admin"')
        max_id = cursor.fetchone()[0]
        new_id = 1001 if max_id is None else max_id + 1

        cursor.execute('''
            INSERT INTO Users 
            (User_ID, Name, Surname, Email, Password, Age, Gender, role, auth_type) 
            VALUES (%s, %s, %s, %s, %s, %s, %s, 'admin', 'email')
        ''', (new_id, data['Name'], data['Surname'], data['Email'], 
              data['Password'], data['Age'], data['Gender']))
        
        connection.commit()
        cursor.close()
        connection.close()

        return jsonify({
            'message': 'Admin created successfully',
            'User_ID': new_id
        }), 201

    except Exception as e:
        return jsonify({'error': str(e)}), 500

# Update admin
@admin_bp.route('/<int:admin_id>', methods=['PUT'])
def update_admin(admin_id):
    try:
        data = request.json
        connection = get_db_connection()
        cursor = connection.cursor()

        update_fields = []
        update_values = []
        
        # ตรวจสอบว่ามีฟิลด์ไหนที่ต้องการอัพเดท
        if 'Name' in data:
            update_fields.append('Name = %s')
            update_values.append(data['Name'])
        if 'Surname' in data:
            update_fields.append('Surname = %s')
            update_values.append(data['Surname'])
        if 'Email' in data:
            update_fields.append('Email = %s')
            update_values.append(data['Email'])
        if 'Password' in data:
            update_fields.append('Password = %s')
            update_values.append(data['Password'])
        if 'Age' in data:
            update_fields.append('Age = %s')
            update_values.append(data['Age'])
        if 'Gender' in data:
            if data['Gender'] not in ['Male', 'Female']:
                return jsonify({'error': 'Gender must be either "Male" or "Female"'}), 400
            update_fields.append('Gender = %s')
            update_values.append(data['Gender'])

        if not update_fields:
            return jsonify({'message': 'No fields to update'}), 400

        # เพิ่ม admin_id เข้าไปใน values
        update_values.append(admin_id)

        # สร้างคำสั่ง SQL
        sql = f'''
            UPDATE Users 
            SET {', '.join(update_fields)}
            WHERE User_ID = %s AND role = 'admin'
        '''
        
        cursor.execute(sql, update_values)
        connection.commit()
        
        if cursor.rowcount == 0:
            return jsonify({'message': 'Admin not found'}), 404

        cursor.close()
        connection.close()

        return jsonify({'message': 'Admin updated successfully'})

    except Exception as e:
        return jsonify({'error': str(e)}), 500

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