from flask import Blueprint, request, jsonify
from config import get_db_connection

quiz_bp = Blueprint('quiz', __name__)

@quiz_bp.route('/', methods=['GET'])
def get_all_quizzes():
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM quiz')
    quizzes = cursor.fetchall()
    cursor.close()
    connection.close()
    return jsonify(quizzes)

@quiz_bp.route('/<int:quiz_id>', methods=['GET'])
def get_quiz(quiz_id):
    connection = get_db_connection()
    cursor = connection.cursor(dictionary=True)
    cursor.execute('SELECT * FROM quiz WHERE id = %s', (quiz_id,))
    quiz = cursor.fetchone()
    cursor.close()
    connection.close()
    if quiz:
        return jsonify(quiz)
    return jsonify({'message': 'Quiz not found'}), 404 