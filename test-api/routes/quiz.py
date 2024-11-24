from flask import Blueprint, request, jsonify
from config import get_db_connection

quiz_bp = Blueprint('quiz', __name__)

@quiz_bp.route('/', methods=['GET', 'POST'])
def quiz_routes():
    if request.method == 'POST':
        try:
            data = request.json
            connection = get_db_connection()
            cursor = connection.cursor()
            cursor.execute(
                'INSERT INTO quiz (quiz_name, question, time_limit, result_quiz) VALUES (%s, %s, %s, %s)',
                (data['quiz_name'], data['question'], data['time_limit'], data['result_quiz'])
            )
            connection.commit()
            new_id = cursor.lastrowid
            cursor.close()
            connection.close()
            return jsonify({'message': 'Quiz created successfully', 'quiz_id': new_id}), 201
        except Exception as e:
            return jsonify({'error': str(e)}), 400
    
    elif request.method == 'GET':
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM quiz')
        quizzes = cursor.fetchall()
        cursor.close()
        connection.close()
        return jsonify(quizzes)

@quiz_bp.route('/<int:quiz_id>', methods=['GET', 'PUT', 'DELETE'])
def quiz_detail(quiz_id):
    if request.method == 'GET':
        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        cursor.execute('SELECT * FROM quiz WHERE quiz_id = %s', (quiz_id,))
        quiz = cursor.fetchone()
        cursor.close()
        connection.close()
        if quiz:
            return jsonify(quiz)
        return jsonify({'message': 'Quiz not found'}), 404

    elif request.method == 'PUT':
        data = request.json
        connection = get_db_connection()
        cursor = connection.cursor()
        cursor.execute(
            'UPDATE quiz SET quiz_name = %s, question = %s, time_limit = %s, result_quiz = %s WHERE quiz_id = %s',
            (data['quiz_name'], data['question'], data['time_limit'], data['result_quiz'], quiz_id)
        )
        connection.commit()
        cursor.close()
        connection.close()
        return jsonify({'message': 'Quiz updated successfully'})

    elif request.method == 'DELETE':
        connection = get_db_connection()
        cursor = connection.cursor()
        cursor.execute('DELETE FROM quiz WHERE quiz_id = %s', (quiz_id,))
        connection.commit()
        cursor.close()
        connection.close()
        return jsonify({'message': 'Quiz deleted successfully'})