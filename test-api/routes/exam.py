from flask import Blueprint, request, jsonify
from config import get_db_connection
import json
from datetime import timedelta, datetime
import mysql.connector

exam_bp = Blueprint('exam', __name__)

# Get all exams
@exam_bp.route('/', methods=['GET'])
def get_all_exams():
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Exam')
        exams = cursor.fetchall()
        
        serializable_exams = []
        for exam in exams:
            # ตรวจสอบและแปลง Result_Exam เป็น dict ก่อน
            try:
                result_exam = json.loads(exam['Result_Exam']) if exam['Result_Exam'] else {}
            except:
                result_exam = {}
                
            serializable_exam = {
                'Exam_ID': str(exam['Exam_ID']),
                'User_ID': str(exam['User_ID']),
                'id': str(exam['id']),
                'Exame_name': exam['Exame_name'],
                'Start_Time': exam['Start_Time'].strftime('%Y-%m-%d %H:%M:%S'),
                'End_Time': exam['End_Time'].strftime('%Y-%m-%d %H:%M:%S'),
                'Time_limit': str(exam['Time_limit']),
                'Result_Exam': result_exam
            }
            serializable_exams.append(serializable_exam)
                
        cursor.close()
        connection.close()
        
        # ส่งคืนเป็น array ตรงๆ เหมือน training.py
        return jsonify(serializable_exams)
        
    except mysql.connector.Error as err:
        print(f"Database error: {err}")
        return jsonify({
            'error': 'Database connection failed',
            'details': str(err)
        }), 500
    except Exception as e:
        print(f"General error: {e}")
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
            # เพิ่ม id ในการส่งกลับ
            exam['id'] = str(exam['id'])
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
        
        if not data or 'User_ID' not in data:
            return jsonify({'error': 'No data or User_ID not provided'}), 400

        connection = get_db_connection()
        cursor = connection.cursor()

        # หา Exam_ID ล่าสุด
        cursor.execute('SELECT MAX(Exam_ID) FROM Exam')
        last_exam_id = cursor.fetchone()[0]
        new_exam_id = 1 if last_exam_id is None else last_exam_id + 1

        # กำหนดค่า id จากข้อมูลที่ส่งมา
        game_id = data.get('id', 302)  # ถ้าไม่มี id ให้ใช้ค่า default เป็น 302

        # หา GameSession_ID ล่าสุดของ User นี้
        cursor.execute('''
            SELECT MAX(GameSession_ID) 
            FROM Exam 
            WHERE User_ID = %s
        ''', (data['User_ID'],))
        
        last_session = cursor.fetchone()[0]
        new_session_id = 1 if last_session is None else last_session + 1

        # เช็คข้อมูลซ้ำ
        cursor.execute('''
            SELECT COUNT(*) FROM Exam 
            WHERE Exame_name = %s 
            AND Start_Time = %s 
            AND End_Time = %s 
            AND Time_limit = %s
            AND Result_Exam = %s
            AND User_ID = %s
        ''', (
            data['Exame_name'],
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            json.dumps(data['Result_Exam']),
            data['User_ID']
        ))
        
        if cursor.fetchone()[0] > 0:
            cursor.close()
            connection.close()
            return jsonify({'message': 'Duplicate exam record, skipped'}), 200

        # บันทึกข้อมูลใหม่
        cursor.execute('''
            INSERT INTO Exam (
                Exam_ID,
                id, 
                Exame_name, 
                Start_Time, 
                End_Time, 
                Time_limit, 
                Result_Exam, 
                User_ID, 
                GameSession_ID
            ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (
            new_exam_id,
            game_id,  # ใช้ค่า id ที่ส่งมา
            data['Exame_name'],
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            json.dumps(data['Result_Exam']),
            data['User_ID'],
            new_session_id
        ))
        
        connection.commit()
        cursor.close()
        connection.close()
        
        return jsonify({
            'message': 'Exam created successfully',
            'Exam_ID': new_exam_id,
            'GameSession_ID': new_session_id,
            'data': data
        }), 201
        
    except Exception as e:
        print("Error:", str(e))  # เพิ่ม debug log
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

# Get exam by Exam_ID (เพิ่มใหม่)
@exam_bp.route('/detail/<int:exam_id>', methods=['GET'])
def get_exam_detail(exam_id):
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Exam WHERE Exam_ID = %s', (exam_id,))
        exam = cursor.fetchone()
        cursor.close()
        connection.close()
        
        if exam:
            # แปลงข้อมูลให้เป็น JSON serializable
            serializable_exam = {
                'Exam_ID': str(exam['Exam_ID']),
                'User_ID': str(exam['User_ID']),
                'id': str(exam['id']),  # รหัสเกม (301 = Coin Game)
                'Exame_name': exam['Exame_name'],
                'Start_Time': exam['Start_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(exam['Start_Time'], datetime) else str(exam['Start_Time']),
                'End_Time': exam['End_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(exam['End_Time'], datetime) else str(exam['End_Time']),
                'Time_limit': str(exam['Time_limit'].total_seconds()) if isinstance(exam['Time_limit'], timedelta) else str(exam['Time_limit']),
                'GameSession_ID': str(exam['GameSession_ID']),
                'Result_Exam': json.loads(exam['Result_Exam']) if exam['Result_Exam'] else None
            }
            return jsonify(serializable_exam)
            
        return jsonify({'message': 'Exam not found'}), 404
        
    except Exception as e:
        print("Error:", str(e))
        return jsonify({'error': str(e)}), 500

# Get latest exam by User_ID
@exam_bp.route('/latest/<string:user_id>', methods=['GET'])
def get_latest_exam(user_id):
    try:
        print(f"Fetching latest exam for user: {user_id}")
        
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        
        query = '''
            SELECT * FROM Exam 
            WHERE User_ID = %s 
            ORDER BY Start_Time DESC, Exam_ID DESC
            LIMIT 1
        '''
        print(f"Executing query: {query}")
        
        cursor.execute(query, (user_id,))
        exam = cursor.fetchone()
        
        print(f"Query result: {exam}")
        
        cursor.close()
        connection.close()
        
        if exam:
            serializable_exam = {
                'Exam_ID': str(exam['Exam_ID']),
                'User_ID': str(exam['User_ID']),
                'id': str(exam['id']),
                'Exame_name': exam['Exame_name'],
                'Start_Time': exam['Start_Time'].strftime('%Y-%m-%d %H:%M:%S'),
                'End_Time': exam['End_Time'].strftime('%Y-%m-%d %H:%M:%S'),
                'Time_limit': str(exam['Time_limit'].total_seconds()) if isinstance(exam['Time_limit'], timedelta) else str(exam['Time_limit']),
                'Result_Exam': json.loads(exam['Result_Exam']) if exam['Result_Exam'] else None,
                'has_history': True
            }
        else:
            serializable_exam = {
                'has_history': False,
                'message': 'No exam history found for this user'
            }
        
        print(f"Returning data: {serializable_exam}")
        return jsonify(serializable_exam), 200  # ส่ง 200 ทั้งสองกรณี
        
    except Exception as e:
        print(f"Error in get_latest_exam: {str(e)}")
        return jsonify({'error': str(e)}), 500