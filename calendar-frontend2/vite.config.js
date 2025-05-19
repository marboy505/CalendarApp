import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vuetify from 'vite-plugin-vuetify'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [vue(), vuetify(), tailwindcss(),],
  resolve: {
    alias: {
      // Hapus alias untuk FullCalendar karena tidak diperlukan lagi
      // Menggunakan default import seperti biasa
      'process.env': process.env,  // Memastikan proses berjalan sesuai di browser
    },
  },
  define: {
    // Menyediakan objek 'process.env' untuk menghindari error
    'process.env': {},
  },
})
