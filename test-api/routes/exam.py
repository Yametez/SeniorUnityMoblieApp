from flask import Blueprint, request, jsonify
from config import get_db_connection

exam_bp = Blueprint('exam', __name__)

@exam_bp.route('/', methods=['GET'])
def get_all_exams():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM exam')
    exams = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(exams)

@exam_bp.route('/<int:exam_id>', methods=['GET'])
def get_exam(exam_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM exam WHERE id = %s', (exam_id,))
    exam = cursor.fetchone()
    cursor.close()
    connection.close()
    if exam:
        return jsonify(exam)
    return jsonify({'message': 'Exam not found'}), 404 