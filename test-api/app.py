from flask import Flask, jsonify
from flask_cors import CORS
from routes.users import users_bp
from routes.admin import admin_bp
from routes.exam import exam_bp
from routes.training import training_bp
import os

app = Flask(__name__)
CORS(app)

# Register blueprints
app.register_blueprint(users_bp, url_prefix='/api/users')
app.register_blueprint(admin_bp, url_prefix='/api/admin')
app.register_blueprint(exam_bp, url_prefix='/api/exam')
app.register_blueprint(training_bp, url_prefix='/api/training')

@app.route('/')
def index():
    return jsonify({'status': 'ok', 'message': 'API is running'})

@app.route('/api')
def api_index():
    return jsonify({'status': 'ok', 'message': 'API endpoints are available'})

# Error handlers
@app.errorhandler(404)
def not_found(error):
    return jsonify({'error': 'Not found', 'status': 404}), 404

@app.errorhandler(500)
def server_error(error):
    return jsonify({'error': 'Internal server error', 'status': 500}), 500

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 10000))
    app.run(host='0.0.0.0', port=port)
