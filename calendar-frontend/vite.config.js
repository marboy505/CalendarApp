// vite.config.js
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@fullcalendar/vue3': '@fullcalendar/vue3/dist/fullcalendar-vue3.esm.js',
      // Polyfill untuk crypto
      crypto: 'crypto-browserify',
      stream: 'stream-browserify',
      buffer: 'buffer',
      path: 'path-browserify',
      os: 'os-browserify/browser',
      
    },
  },
  define: {
    'process.env': {},
  },
});
