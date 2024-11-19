from flask import Flask
from flask_cors import CORS
from routes.users import users_bp
from routes.admin import admin_bp
from routes.exam import exam_bp
from routes.quiz import quiz_bp
from routes.report import report_bp

app = Flask(__name__)
CORS(app)  # เปิดใช้งาน CORS

# ลงทะเบียน blueprint
app.register_blueprint(users_bp, url_prefix='/api/users')
app.register_blueprint(admin_bp, url_prefix='/api/admin')
app.register_blueprint(exam_bp, url_prefix='/api/exam')
app.register_blueprint(quiz_bp, url_prefix='/api/quiz')
app.register_blueprint(report_bp, url_prefix='/api/report')

@app.route('/test', methods=['GET'])
def test():
    return {'message': 'API is working!'}

if __name__ == '__main__':
    app.run(port=3000)
