from flask import Blueprint, request, jsonify
from config import get_db_connection
import json
from datetime import timedelta, datetime

training_bp = Blueprint('training', __name__)

# Get all trainings
@training_bp.route('/', methods=['GET'])
def get_all_trainings():
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Training')
        trainings = cursor.fetchall()
        
        serializable_trainings = []
        for training in trainings:
            result_training = json.loads(training['Result_Training']) if training['Result_Training'] else None
            serializable_training = {
                'Training_ID': str(training['Training_ID']),
                'User_ID': str(training['User_ID']),
                'id': str(training['id']),
                'Training_name': training['Training_name'],
                'Start_Time': training['Start_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(training['Start_Time'], datetime) else str(training['Start_Time']),
                'End_Time': training['End_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(training['End_Time'], datetime) else str(training['End_Time']),
                'Time_limit': str(training['Time_limit'].total_seconds()) if isinstance(training['Time_limit'], timedelta) else str(training['Time_limit']),
                'GameSession_ID': str(training['GameSession_ID']),
                'Result_Training': result_training
            }
            print("Debug - training data:", serializable_training)
            serializable_trainings.append(serializable_training)
                
        cursor.close()
        connection.close()
        return jsonify(serializable_trainings)
        
    except Exception as e:
        print("Error:", str(e))
        return jsonify({'error': str(e)}), 500

# Get training by ID
@training_bp.route('/<int:training_id>', methods=['GET'])
def get_training(training_id):
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Training WHERE Training_ID = %s', (training_id,))
        training = cursor.fetchone()
        cursor.close()
        connection.close()
        
        if training:
            # แปลง Result_Training จาก JSON string กลับเป็น dictionary
            if training['Result_Training']:
                training['Result_Training'] = json.loads(training['Result_Training'])
            # เพิ่ม id ในการส่งกลับ
            training['id'] = str(training['id'])
            return jsonify(training)
            
        return jsonify({'message': 'Training not found'}), 404
        
    except Exception as e:
        print("Error:", str(e))  # เพิ่ม debug log
        return jsonify({'error': str(e)}), 500

# Create new training
@training_bp.route('/', methods=['POST'])
def create_training():
    try:
        data = request.get_json()
        print(f"Received data: {data}")  # Debug log
        
        if not data or 'User_ID' not in data:
            return jsonify({'error': 'No data or User_ID not provided'}), 400

        # แปลง Result_Training เป็น dict
        result_training = json.loads(data['Result_Training'])
        print(f"Result training data: {result_training}")  # Debug log

        # ตรวจสอบว่ามีข้อมูลครบ
        if 'time' not in result_training or 'matches' not in result_training:
            return jsonify({'error': 'Missing time or matches in Result_Training'}), 400

        connection = get_db_connection()
        cursor = connection.cursor()

        # หา Training_ID ล่าสุด
        cursor.execute('SELECT MAX(Training_ID) FROM Training')
        last_training_id = cursor.fetchone()[0]
        new_training_id = 1 if last_training_id is None else last_training_id + 1

        # กำหนดค่า id จากข้อมูลที่ส่งมา
        game_id = data.get('id', 302)  # ถ้าไม่มี id ให้ใช้ค่า default เป็น 302

        # หา GameSession_ID ล่าสุดของ User นี้
        cursor.execute('''
            SELECT MAX(GameSession_ID) 
            FROM Training 
            WHERE User_ID = %s
        ''', (data['User_ID'],))
        
        last_session = cursor.fetchone()[0]
        new_session_id = 1 if last_session is None else last_session + 1

        # เช็คข้อมูลซ้ำ
        cursor.execute('''
            SELECT COUNT(*) FROM Training 
            WHERE Training_name = %s 
            AND Start_Time = %s 
            AND End_Time = %s 
            AND Time_limit = %s
            AND Result_Training = %s
            AND User_ID = %s
        ''', (
            data['Training_name'],
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            json.dumps(result_training),
            data['User_ID']
        ))
        
        if cursor.fetchone()[0] > 0:
            cursor.close()
            connection.close()
            return jsonify({'message': 'Duplicate training record, skipped'}), 200

        # บันทึกข้อมูลใหม่
        cursor.execute('''
            INSERT INTO Training (
                Training_ID, id, Training_name, Start_Time, End_Time, 
                Time_limit, Result_Training, User_ID, GameSession_ID
            ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (
            new_training_id,
            game_id,
            data['Training_name'],
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            json.dumps(result_training),  # บันทึกข้อมูลทั้งเวลาและจำนวนคู่
            data['User_ID'],
            new_session_id
        ))
        
        connection.commit()
        cursor.close()
        connection.close()
        
        return jsonify({
            'message': 'Training created successfully',
            'Training_ID': new_training_id,
            'GameSession_ID': new_session_id,
            'data': data
        }), 201
        
    except json.JSONDecodeError as je:
        print(f"JSON decode error: {je}")
        return jsonify({'error': f'Invalid JSON format: {je}'}), 400
    except Exception as e:
        print(f"Error: {str(e)}")
        return jsonify({'error': str(e)}), 500

# Update training
@training_bp.route('/<int:training_id>', methods=['PUT'])
def update_training(training_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE Training SET Training_name = %s, Start_Time = %s, End_Time = %s, Time_limit = %s, Result_Training = %s WHERE Training_ID = %s',
        (data['Training_name'], data['Start_Time'], data['End_Time'], data['Time_limit'], data['Result_Training'], training_id)
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Training updated successfully'})

# Delete training
@training_bp.route('/<int:training_id>', methods=['DELETE'])
def delete_training(training_id):
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute('DELETE FROM Training WHERE Training_ID = %s', (training_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Training deleted successfully'})

# Get training by Training_ID (เพิ่มใหม่)
@training_bp.route('/detail/<int:training_id>', methods=['GET'])
def get_training_detail(training_id):
    try:
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM Training WHERE Training_ID = %s', (training_id,))
        training = cursor.fetchone()
        cursor.close()
        connection.close()
        
        if training:
            try:
                # แยก try-except สำหรับการแปลง JSON
                result_training = None
                if training['Result_Training']:
                    if isinstance(training['Result_Training'], str):
                        result_training = json.loads(training['Result_Training'])
                    else:
                        result_training = training['Result_Training']
                
                # ตรวจสอบว่า result_training เป็น dict หรือไม่
                if result_training and isinstance(result_training, dict):
                    result_data = {
                        'time': result_training.get('time'),
                        'matches': result_training.get('matches')
                    }
                else:
                    result_data = None
                
                serializable_training = {
                    'Training_ID': str(training['Training_ID']),
                    'User_ID': str(training['User_ID']),
                    'id': str(training['id']),
                    'Training_name': training['Training_name'],
                    'Start_Time': training['Start_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(training['Start_Time'], datetime) else str(training['Start_Time']),
                    'End_Time': training['End_Time'].strftime('%Y-%m-%d %H:%M:%S') if isinstance(training['End_Time'], datetime) else str(training['End_Time']),
                    'Time_limit': str(training['Time_limit'].total_seconds()) if isinstance(training['Time_limit'], timedelta) else str(training['Time_limit']),
                    'GameSession_ID': str(training['GameSession_ID']),
                    'Result_Training': result_data
                }
                
                print(f"Debug - Returning training detail: {serializable_training}")
                return jsonify(serializable_training)
                
            except json.JSONDecodeError as je:
                print(f"JSON Decode Error for Training_ID {training_id}: {je}")
                return jsonify({'error': f'Invalid Result_Training format: {je}'}), 400
            
        return jsonify({'message': 'Training not found'}), 404
        
    except Exception as e:
        print(f"Error in get_training_detail: {str(e)}")
        return jsonify({'error': str(e)}), 500

# Get latest training by User_ID
@training_bp.route('/latest/<string:user_id>', methods=['GET'])
def get_latest_training(user_id):
    try:
        print(f"Fetching latest training for user: {user_id}")
        
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        
        query = '''
            SELECT * FROM Training 
            WHERE User_ID = %s 
            ORDER BY Start_Time DESC, Training_ID DESC
            LIMIT 1
        '''
        print(f"Executing query: {query}")
        
        cursor.execute(query, (user_id,))
        training = cursor.fetchone()
        
        print(f"Query result: {training}")
        
        cursor.close()
        connection.close()
        
        if training:
            serializable_training = {
                'Training_ID': str(training['Training_ID']),
                'User_ID': str(training['User_ID']),
                'id': str(training['id']),
                'Training_name': training['Training_name'],
                'Start_Time': training['Start_Time'].strftime('%Y-%m-%d %H:%M:%S'),
                'End_Time': training['End_Time'].strftime('%Y-%m-%d %H:%M:%S'),
                'Time_limit': str(training['Time_limit'].total_seconds()) if isinstance(training['Time_limit'], timedelta) else str(training['Time_limit']),
                'Result_Training': json.loads(training['Result_Training']) if training['Result_Training'] else None,
                'has_history': True
            }
        else:
            serializable_training = {
                'has_history': False,
                'message': 'No training history found for this user'
            }
        
        print(f"Returning data: {serializable_training}")
        return jsonify(serializable_training), 200  # ส่ง 200 ทั้งสองกรณี
        
    except Exception as e:
        print(f"Error in get_latest_training: {str(e)}")
        return jsonify({'error': str(e)}), 500
