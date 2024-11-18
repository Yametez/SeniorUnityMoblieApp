const express = require('express');
const router = express.Router();
const pool = require('../config/database');

// Get all users
router.get('/', async (req, res) => {
  try {
    const [rows] = await pool.query('SELECT * FROM users');
    res.json(rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Get user by ID
router.get('/:id', async (req, res) => {
  try {
    const [rows] = await pool.query('SELECT * FROM users WHERE userid = ?', [req.params.id]);
    if (rows.length === 0) {
      return res.status(404).json({ message: 'User not found' });
    }
    res.json(rows[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Create new user
router.post('/', async (req, res) => {
  try {
    const { name, surname, password, age, gender } = req.body;
    const [result] = await pool.query(
      'INSERT INTO users (name, surname, password, age, gender) VALUES (?, ?, ?, ?, ?)',
      [name, surname, password, age, gender]
    );
    res.status(201).json({
      message: 'User created successfully',
      userId: result.insertId
    });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Update user
router.put('/:id', async (req, res) => {
  try {
    const { name, surname, password, age, gender } = req.body;
    const [result] = await pool.query(
      'UPDATE users SET name = ?, surname = ?, password = ?, age = ?, gender = ? WHERE userid = ?',
      [name, surname, password, age, gender, req.params.id]
    );
    if (result.affectedRows === 0) {
      return res.status(404).json({ message: 'User not found' });
    }
    res.json({ message: 'User updated successfully' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Delete user
router.delete('/:id', async (req, res) => {
  try {
    const [result] = await pool.query('DELETE FROM users WHERE userid = ?', [req.params.id]);
    if (result.affectedRows === 0) {
      return res.status(404).json({ message: 'User not found' });
    }
    res.json({ message: 'User deleted successfully' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Login user
router.post('/login', async (req, res) => {
  try {
    const { name, password } = req.body;
    const [rows] = await pool.query(
      'SELECT * FROM users WHERE name = ? AND password = ?',
      [name, password]
    );
    if (rows.length === 0) {
      return res.status(401).json({ message: 'Invalid credentials' });
    }
    res.json({
      message: 'Login successful',
      user: {
        userid: rows[0].userid,
        name: rows[0].name,
        surname: rows[0].surname,
        age: rows[0].age,
        gender: rows[0].gender
      }
    });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

module.exports = router; 