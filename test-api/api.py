from flask import Flask, request, jsonify
from flask_cors import CORS
import mysql.connector
from datetime import datetime, timedelta
import json
import os
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

app = Flask(__name__)
CORS(app)

# Database connection
def get_db_connection():
    connection = mysql.connector.connect(
        host=os.getenv('DB_HOST'),
        user=os.getenv('DB_USER'),
        password=os.getenv('DB_PASSWORD'),
        database=os.getenv('DB_DATABASE')
    )
    return connection

# Users routes
@app.route('/api/users/login', methods=['POST'])
def login():
    try:
        data = request.json
        if not data or 'Email' not in data or 'Password' not in data:
            return jsonify({'error': 'Email and Password are required'}), 400

        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)
        
        cursor.execute('''
            SELECT User_ID, Name, Email 
            FROM users 
            WHERE Email = %s AND Password = %s
        ''', (data['Email'], data['Password']))
        
        user = cursor.fetchone()
        cursor.close()
        connection.close()

        if user:
            return jsonify({
                'userId': user['User_ID'],
                'name': user['Name'],
                'email': user['Email']
            })
        else:
            return jsonify({'error': 'Invalid email or password'}), 401

    except Exception as e:
        return jsonify({'error': str(e)}), 500

# Training routes
@app.route('/api/training/', methods=['POST'])
def create_training():
    try:
        data = request.get_json()
        
        if not data or 'User_ID' not in data:
            return jsonify({'error': 'No data or User_ID not provided'}), 400

        connection = get_db_connection()
        cursor = connection.cursor()

        # Get latest Training_ID
        cursor.execute('SELECT MAX(Training_ID) FROM Training')
        last_training_id = cursor.fetchone()[0]
        new_training_id = 1 if last_training_id is None else last_training_id + 1

        # Get latest GameSession_ID for this user
        cursor.execute('''
            SELECT MAX(GameSession_ID) 
            FROM Training 
            WHERE User_ID = %s
        ''', (data['User_ID'],))
        
        last_session = cursor.fetchone()[0]
        new_session_id = 1 if last_session is None else last_session + 1

        # Insert new training record
        cursor.execute('''
            INSERT INTO Training (
                Training_ID, id, Training_name, Start_Time, End_Time, 
                Time_limit, Result_Training, User_ID, GameSession_ID
            ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (
            new_training_id,
            data.get('id', 302),
            data['Training_name'],
            data['Start_Time'],
            data['End_Time'],
            data['Time_limit'],
            data['Result_Training'],
            data['User_ID'],
            new_session_id
        ))
        
        connection.commit()
        cursor.close()
        connection.close()
        
        return jsonify({
            'message': 'Training created successfully',
            'Training_ID': new_training_id,
            'GameSession_ID': new_session_id
        }), 201
        
    except Exception as e:
        return jsonify({'error': str(e)}), 500

# Error handlers
@app.errorhandler(404)
def not_found(error):
    return jsonify({'error': 'Not found'}), 404

@app.errorhandler(405)
def method_not_allowed(error):
    return jsonify({'error': 'Method not allowed'}), 405

# Test route
@app.route('/')
def index():
    return jsonify({'message': 'Welcome to API'})

if __name__ == '__main__':
    app.run() 