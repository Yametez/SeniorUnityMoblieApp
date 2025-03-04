from flask import Blueprint, request, jsonify
from config import get_db_connection

training_bp = Blueprint('training', __name__)

# Get all trainings
@training_bp.route('/', methods=['GET'])
def get_all_trainings():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Training')
    trainings = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(trainings)

# Get training by ID
@training_bp.route('/<int:training_id>', methods=['GET'])
def get_training(training_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Training WHERE Training_ID = %s', (training_id,))
    training = cursor.fetchone()
    cursor.close()
    connection.close()
    if training:
        return jsonify(training)
    return jsonify({'message': 'Training not found'}), 404

# Create new training
@training_bp.route('/', methods=['POST'])
def create_training():
    try:
        data = request.json
        connection = get_db_connection()
        cursor = connection.cursor()

        # หา Training_ID ล่าสุด
        cursor.execute('SELECT MAX(Training_ID) FROM Training')
        last_id = cursor.fetchone()[0]
        new_id = 1 if last_id is None else last_id + 1

        # หา GameSession_ID ล่าสุดของ User นี้
        cursor.execute('''
            SELECT MAX(GameSession_ID) 
            FROM Training 
            WHERE User_ID = %s
        ''', (data['User_ID'],))
        
        last_session = cursor.fetchone()[0]
        new_session_id = 1 if last_session is None else last_session + 1

        cursor.execute('''
            INSERT INTO Training (
                Training_ID, 
                User_ID,
                id,
                Training_name,
                Start_Time,
                End_Time,
                Time_limit,
                Result_Training,
                GameSession_ID
            ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (
            new_id,
            data['User_ID'],
            302,  # ID สำหรับเกมจับคู่ไพ่
            "Card Matching Game",
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            data['Result_Training'],
            new_session_id
        ))
        
        connection.commit()
        return jsonify({
            'message': 'Training created successfully',
            'Training_ID': new_id,
            'GameSession_ID': new_session_id
        }), 201
        
    except Exception as e:
        print("Error:", str(e))
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