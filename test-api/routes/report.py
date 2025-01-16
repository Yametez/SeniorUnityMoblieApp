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

@report_bp.route('/', methods=['POST'])
def create_report():
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'INSERT INTO Report (User_ID, Activity_Type, Activity_ID, GameSession_ID, Start_Time, End_Time, Result) VALUES (%s, %s, %s, %s, %s, %s, %s)',
        (data['User_ID'], data['Activity_Type'], data['Activity_ID'], data['GameSession_ID'], 
         data['Start_Time'], data['End_Time'], data['Result'])
    )
    connection.commit()
    new_id = cursor.lastrowid
    cursor.close()
    connection.close()
    return jsonify({'message': 'Report created successfully', 'Report_ID': new_id}), 201 