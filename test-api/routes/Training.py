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
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()

    # หา ID ล่าสุดและบวกเพิ่ม 1
    cursor.execute('SELECT MAX(Training_ID) FROM Training')
    max_id = cursor.fetchone()[0]
    new_id = 401 if max_id is None else max_id + 1  # เริ่มที่ 401 ถ้าไม่มีข้อมูล

    cursor.execute(
        'INSERT INTO Training (Training_ID, Training_name, Start_Time, End_Time, Time_limit, Result_Training) VALUES (%s, %s, %s, %s, %s, %s)',
        (new_id, data['Training_name'], data['Start_Time'], data['End_Time'], data['Time_limit'], data['Result_Training'])
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Training created successfully', 'Training_ID': new_id}), 201

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