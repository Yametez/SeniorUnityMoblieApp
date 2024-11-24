from flask import Blueprint, request, jsonify
from config import get_db_connection

exam_bp = Blueprint('exam', __name__)

# Get all exams
@exam_bp.route('/', methods=['GET'])
def get_all_exams():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM exam')
    exams = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(exams)

# Get exam by ID
@exam_bp.route('/<int:exam_id>', methods=['GET'])
def get_exam(exam_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM exam WHERE examid = %s', (exam_id,))
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
    cursor.execute(
        'INSERT INTO exam (examname, questionexam, resultexam) VALUES (%s, %s, %s)',
        (data['examname'], data['questionexam'], data['resultexam'])
    )
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Exam created successfully', 'examid': cursor.lastrowid}), 201

# Update exam
@exam_bp.route('/<int:exam_id>', methods=['PUT'])
def update_exam(exam_id):
    data = request.json
    connection = get_db_connection()
    cursor = connection.cursor()
    cursor.execute(
        'UPDATE exam SET examname = %s, questionexam = %s, resultexam = %s WHERE examid = %s',
        (data['examname'], data['questionexam'], data['resultexam'], exam_id)
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
    cursor.execute('DELETE FROM exam WHERE examid = %s', (exam_id,))
    connection.commit()
    cursor.close()
    connection.close()
    return jsonify({'message': 'Exam deleted successfully'})