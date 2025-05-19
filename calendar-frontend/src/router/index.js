import { createRouter, createWebHistory } from 'vue-router';
import TabelManagement from '../components/TabelManagement.vue';  // Ganti dengan komponen yang sesuai
import Calendar from '../components/Calendar.vue';
import EntryForm from '../components/EntryForm.vue';

const routes = [
  { path: '/iot-management', component: TabelManagement, name: 'iot-management' },
  { path: '/calendar', component: Calendar, name: 'calendar' },
  { path: '/entry-form', component: EntryForm, name: 'entry-form' },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
