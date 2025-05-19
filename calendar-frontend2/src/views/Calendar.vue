<template>
    <div>
      <h2 class="text-xl font-semibold mb-4">Calendar</h2>
      <!-- Menggunakan komponen CalendarView yang sudah diubah -->
      <calendar-view /> <!-- Komponen CalendarView yang berisi kalender menggunakan v-calendar -->
    </div>
  </template>
  
  <script>
  import { defineComponent } from 'vue';
  import CalendarView from '../components/CalendarView.vue'; // Mengimpor komponen CalendarView
  
  export default defineComponent({
    components: {
      CalendarView, // Menambahkan komponen CalendarView
    },
  });
  </script>
  