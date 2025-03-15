from flask import Flask, jsonify
from flask_cors import CORS
import os
import sys

# เพิ่มบรรทัดนี้เพื่อให้หา routes ได้
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

# import blueprints
from routes.users import users_bp
from routes.admin import admin_bp
from routes.exam import exam_bp
from routes.training import training_bp

app = Flask(__name__)
CORS(app)

# Register blueprints
app.register_blueprint(users_bp, url_prefix='/api/users')
app.register_blueprint(admin_bp, url_prefix='/api/admin')
app.register_blueprint(exam_bp, url_prefix='/api/exam')
app.register_blueprint(training_bp, url_prefix='/api/training')

# เพิ่มการจัดการข้อผิดพลาด
@app.errorhandler(404)
def not_found(error):
    return jsonify({'error': 'Not found'}), 404

@app.errorhandler(405)
def method_not_allowed(error):
    return jsonify({'error': 'Method not allowed'}), 405

@app.route('/')
def index():
    return jsonify({'message': 'Welcome to API'})

@app.route('/test', methods=['GET'])
def test():
    return jsonify({'message': 'API is working!'})

if __name__ == '__main__':
    app.run()
    
