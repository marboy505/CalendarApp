import { createApp } from 'vue';
import App from './App.vue';
import vuetify from './plugins/vuetify';  // Mengimpor Vuetify
import { createRouter, createWebHistory } from 'vue-router';
import router from './router';  // Pengaturan router

createApp(App)
  .use(vuetify)  // Menggunakan Vuetify
  .use(router)
  .mount('#app');
