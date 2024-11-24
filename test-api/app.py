from flask import Flask, jsonify
from flask_cors import CORS
from routes.users import users_bp
from routes.admin import admin_bp
from routes.exam import exam_bp
from routes.quiz import quiz_bp
from routes.report import report_bp

app = Flask(__name__)
CORS(app, resources={r"/api/*": {"origins": "*"}})  # กำหนด CORS ให้ชัดเจนขึ้น

# ลงทะเบียน blueprint
app.register_blueprint(users_bp, url_prefix='/api/users')
app.register_blueprint(admin_bp, url_prefix='/api/admin')
app.register_blueprint(exam_bp, url_prefix='/api/exam')
app.register_blueprint(quiz_bp, url_prefix='/api/quiz')
app.register_blueprint(report_bp, url_prefix='/api/report')

# เพิ่มการจัดการข้อผิดพลาด
@app.errorhandler(404)
def not_found(error):
    return jsonify({'error': 'Not found'}), 404

@app.errorhandler(405)
def method_not_allowed(error):
    return jsonify({'error': 'Method not allowed'}), 405

@app.route('/test', methods=['GET'])
def test():
    return jsonify({'message': 'API is working!'})

if __name__ == '__main__':
    app.run(debug=True, port=3000)  # เพิ่ม debug mode
