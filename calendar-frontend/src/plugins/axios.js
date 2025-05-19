import axios from 'axios';

// Konfigurasi axios untuk endpoint backend
const instance = axios.create({
  baseURL: 'http://localhost:5152',  // Ganti dengan URL backend Anda
  headers: {
    'Content-Type': 'application/json',
  },
});

export default instance;
