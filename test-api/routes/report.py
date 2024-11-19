from flask import Blueprint, request, jsonify
from config import get_db_connection

report_bp = Blueprint('report', __name__)

@report_bp.route('/', methods=['GET'])
def get_all_reports():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM report')
    reports = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(reports)

@report_bp.route('/<int:report_id>', methods=['GET'])
def get_report(report_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM report WHERE id = %s', (report_id,))
    report = cursor.fetchone()
    cursor.close()
    connection.close()
    if report:
        return jsonify(report)
    return jsonify({'message': 'Report not found'}), 404 