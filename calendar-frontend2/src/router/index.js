import { createRouter, createWebHistory } from 'vue-router';
import EventListPage from '../views/ EventListPage.vue';
import EventEntry from '../views/EventEntry.vue';
import Dashboard from '../views/Dashboard.vue';
import Calendar from '../views/Calendar.vue';
import WeeklyEventEntry from '../components/WeeklyEventEntry.vue';
import MonthlyEventEntry from '../components/MonthlyEventEntry.vue';
import YearlyEventEntry from '../components/YearlyEventEntry.vue';
import ErrorPage from '../views/ErrorPage.vue';
import EditPage from '../components/EditPopUp.vue';
import deletePage from '../components/DeletePopUp.vue'
import waWeb from '../views/waWeb.vue';


const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: Dashboard
  },
  {
    path: '/event-list',
    name: 'EventListPage',
    component: EventListPage
  },
  {
    path: '/event-entry',
    name: 'EventEntry',
    component: EventEntry
  },
  {
    path: '/calendar',
    name: 'Calendar',
    component: Calendar
  },
  {
    path: '/weekly-event-entry',
    name: 'WeeklyEventEntry',
    component: WeeklyEventEntry
  },
  {
    path: '/monthly-event-entry',
    name: 'MonthlyEventEntry',
    component: MonthlyEventEntry
  },
  {
    path: '/yearly-event-entry',
    name: 'YearlyEventEntry',
    component: YearlyEventEntry
  },
  {
    path: '/error',
    name: 'ErrorPage',
    component: ErrorPage
  },

  {
    path: '/edit',
    name: 'EditPage',
    component: EditPage
  }, {
    path: '/delete',
    name: 'deletePage',
    component: deletePage
  },
  {
    path: '/waMassage',
    name: 'waWeb',
    component: waWeb,
  },
];


const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

export default router;
