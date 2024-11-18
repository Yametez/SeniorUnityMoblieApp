const express = require('express');
const router = express.Router();
const pool = require('../../config/database');

// Get all exams
router.get('/', async (req, res) => {
  try {
    const [rows] = await pool.query('SELECT * FROM exam');
    res.json(rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Get exam by ID
router.get('/:id', async (req, res) => {
  try {
    const [rows] = await pool.query('SELECT * FROM exam WHERE id = ?', [req.params.id]);
    if (rows.length === 0) {
      return res.status(404).json({ message: 'Exam not found' });
    }
    res.json(rows[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

module.exports = router; 