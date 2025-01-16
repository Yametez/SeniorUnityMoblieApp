const mysql = require('mysql2');

const pool = mysql.createPool({
  host: 'localhost',
  user: 'root',
  password: '2545',
  database: 'game_db'
}).promise();

module.exports = pool; 