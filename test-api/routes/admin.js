const express = require('express');
const router = express.Router();
const pool = require('../../config/database');

// Get all admins
router.get('/', async (req, res) => {
  try {
    const [rows] = await pool.query('SELECT * FROM admin');
    res.json(rows);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Get admin by ID
router.get('/:id', async (req, res) => {
  try {
    const [rows] = await pool.query('SELECT * FROM admin WHERE id = ?', [req.params.id]);
    if (rows.length === 0) {
      return res.status(404).json({ message: 'Admin not found' });
    }
    res.json(rows[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

module.exports = router; 