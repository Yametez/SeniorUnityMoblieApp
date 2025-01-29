from flask import Flask, jsonify
from flask_cors import CORS
from routes.users import users_bp
from routes.admin import admin_bp
from routes.exam import exam_bp
from routes.Training import training_bp
from routes.report import report_bp
import os

app = Flask(__name__)
CORS(app)

# Register blueprints
app.register_blueprint(users_bp, url_prefix='/api/users')
app.register_blueprint(admin_bp, url_prefix='/api/admin')
app.register_blueprint(exam_bp, url_prefix='/api/exam')
app.register_blueprint(training_bp, url_prefix='/api/training')
app.register_blueprint(report_bp, url_prefix='/api/report')

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
    # เปลี่ยนจาก port=3000 เป็น port=10000
    port = int(os.environ.get('PORT', 10000))
    app.run(host='0.0.0.0', port=3000)
