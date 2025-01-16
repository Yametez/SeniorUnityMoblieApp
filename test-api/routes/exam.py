from flask import Blueprint, request, jsonify
from config import get_db_connection

exam_bp = Blueprint('exam', __name__)

# Get all exams
@exam_bp.route('/', methods=['GET'])
def get_all_exams():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Exam')
    exams = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(exams)

# Get exam by ID
@exam_bp.route('/<int:exam_id>', methods=['GET'])
def get_exam(exam_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM Exam WHERE Exam_ID = %s', (exam_id,))
    exam = cursor.fetchone()
    cursor.close()
    connection.close()
    if exam:
        return jsonify(exam)
    return jsonify({'message': 'Exam not found'}), 404

# Create new exam
@exam_bp.route('/', methods=['POST'])
def create_exam():
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()

    # หา ID ล่าสุดและบวกเพิ่ม 1
    cursor.execute('SELECT MAX(Exam_ID) FROM Exam')
    max_id = cursor.fetchone()[0]
    new_id = 301 if max_id is None else max_id + 1  # เริ่มที่ 301 ถ้าไม่มีข้อมูล

    cursor.execute(
        'INSERT INTO Exam (Exam_ID, Exame_name, Start_Time, End_Time, Time_limit, Result_Exam) VALUES (%s, %s, %s, %s, %s, %s)',
        (new_id, data['Exame_name'], data['Start_Time'], data['End_Time'], data['Time_limit'], data['Result_Exam'])
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Exam created successfully', 'Exam_ID': new_id}), 201

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