import axios from 'axios';

const instance = axios.create({
  baseURL: 'http://localhost:5152/',  // Ganti dengan URL backend Anda jika berbeda
  timeout: 5000,  // Waktu timeout 5 detik
  headers: {
    'Content-Type': 'application/json',
  }
});

export default instance;
