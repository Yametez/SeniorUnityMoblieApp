from flask import Blueprint, request, jsonify
from config import get_db_connection
import json
from datetime import timedelta, datetime

exam_bp = Blueprint('exam', __name__)

# Get all exams
@exam_bp.route('/', methods=['GET'])
def get_all_exams():
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Exam')
        exams = cursor.fetchall()
        
        # แปลงข้อมูลให้เป็น JSON serializable
        serializable_exams = []
        for exam in exams:
            serializable_exam = {
                'Exam_ID': str(exam['Exam_ID']),
                'Exame_name': exam['Exame_name'],
                'Start_Time': exam['Start_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(exam['Start_Time'], datetime) else str(exam['Start_Time']),
                'End_Time': exam['End_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(exam['End_Time'], datetime) else str(exam['End_Time']),
                'Time_limit': str(exam['Time_limit'].total_seconds()) if isinstance(exam['Time_limit'], timedelta) else str(exam['Time_limit']),
                'Result_Exam': json.loads(exam['Result_Exam']) if exam['Result_Exam'] else None
            }
            serializable_exams.append(serializable_exam)
                
        cursor.close()
        connection.close()
        return jsonify(serializable_exams)
        
    except Exception as e:
        print("Error:", str(e))  # เพิ่ม debug log
        return jsonify({'error': str(e)}), 500

# Get exam by ID
@exam_bp.route('/<int:exam_id>', methods=['GET'])
def get_exam(exam_id):
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Exam WHERE Exam_ID = %s', (exam_id,))
        exam = cursor.fetchone()
        cursor.close()
        connection.close()
        
        if exam:
            # แปลง Result_Exam จาก JSON string กลับเป็น dictionary
            if exam['Result_Exam']:
                exam['Result_Exam'] = json.loads(exam['Result_Exam'])
            return jsonify(exam)
            
        return jsonify({'message': 'Exam not found'}), 404
        
    except Exception as e:
        print("Error:", str(e))  # เพิ่ม debug log
        return jsonify({'error': str(e)}), 500

# Create new exam
@exam_bp.route('/', methods=['POST'])
def create_exam():
    try:
        data = request.get_json()
        print("Received data:", data)
        
        if not data:
            return jsonify({'error': 'No data received'}), 400

        # ตรวจสอบข้อมูลที่จำเป็น
        required_fields = ['Exame_name', 'Start_Time', 'End_Time', 'Time_limit', 'Result_Exam']
        for field in required_fields:
            if field not in data:
                return jsonify({'error': f'Missing required field: {field}'}), 400

        connection = get_db_connection()
        cursor = connection.cursor()

        # เช็คว่ามีข้อมูลซ้ำหรือไม่
        cursor.execute('''
            SELECT COUNT(*) FROM Exam 
            WHERE Exame_name = %s 
            AND Start_Time = %s 
            AND End_Time = %s 
            AND Time_limit = %s
            AND Result_Exam = %s
        ''', (
            data['Exame_name'],
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            json.dumps(data['Result_Exam'])
        ))
        
        if cursor.fetchone()[0] > 0:
            cursor.close()
            connection.close()
            return jsonify({'message': 'Duplicate exam record, skipped'}), 200

        # ถ้าไม่ซ้ำ ดำเนินการบันทึก
        result_exam_json = json.dumps(data['Result_Exam'])
        cursor.execute('SELECT MAX(Exam_ID) FROM Exam')
        max_id = cursor.fetchone()[0]
        new_id = 301 if max_id is None else max_id + 1

        cursor.execute(
            'INSERT INTO Exam (Exam_ID, Exame_name, Start_Time, End_Time, Time_limit, Result_Exam) VALUES (%s, %s, %s, %s, %s, %s)',
            (new_id, data['Exame_name'], data['Start_Time'], data['End_Time'], data['Time_limit'], result_exam_json)
        )
        connection.commit()
        cursor.close()
        connection.close()
        
        return jsonify({
            'message': 'Exam created successfully',
            'Exam_ID': new_id,
            'data': data
        }), 201
        
    except Exception as e:
        print("Error:", str(e))
        return jsonify({'error': str(e)}), 500

# Update exam
@exam_bp.route('/<int:exam_id>', methods=['PUT'])
def update_exam(exam_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE Exam SET Exame_name = %s, Start_Time = %s, End_Time = %s, Time_limit = %s, Result_Exam = %s WHERE Exam_ID = %s',
        (data['Exame_name'], data['Start_Time'], data['End_Time'], data['Time_limit'], data['Result_Exam'], exam_id)
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Exam updated successfully'})

# Delete exam
@exam_bp.route('/<int:exam_id>', methods=['DELETE'])
def delete_exam(exam_id):
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute('DELETE FROM Exam WHERE Exam_ID = %s', (exam_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Exam deleted successfully'})