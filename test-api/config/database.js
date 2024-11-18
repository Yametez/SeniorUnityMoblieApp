const mysql = require('mysql2');

const pool = mysql.createPool({
  host: 'localhost',
  user: 'root',
  password: '2545',
  database: 'user'
}).promise();

module.exports = pool; 