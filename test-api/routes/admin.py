from flask import Blueprint, request, jsonify
from config import get_db_connection

admin_bp = Blueprint('admin', __name__)

@admin_bp.route('/', methods=['GET'])
def get_all_admins():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM admin')
    admins = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(admins)

@admin_bp.route('/<int:admin_id>', methods=['GET'])
def get_admin(admin_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM admin WHERE id = %s', (admin_id,))
    admin = cursor.fetchone()
    cursor.close()
    connection.close()
    if admin:
        return jsonify(admin)
    return jsonify({'message': 'Admin not found'}), 404 