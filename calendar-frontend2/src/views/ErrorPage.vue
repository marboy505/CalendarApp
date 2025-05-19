<template>
    <v-app>
      <v-container>
        <v-calendar
          v-model="selectedDate"
          :events="events"
          is-expanded
          :event-content="eventContent"
          @click:date="onDateClick"
        />
      </v-container>
    </v-app>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  
  // Menyimpan tanggal yang dipilih
  const selectedDate = ref(null)
  
  // Data event yang akan ditampilkan di kalender
  const events = ref([
    { name: 'Event 1', start: '2025-05-10', end: '2025-05-10' },
    { name: 'Event 2', start: '2025-05-12', end: '2025-05-12' }
  ])
  
  // Menangani klik pada tanggal
  const onDateClick = (date) => {
    console.log('Tanggal yang dipilih: ', date)
    // Logika untuk menambahkan event pada tanggal tertentu
    addEventToDate(date)
  }
  
  // Fungsi untuk menambahkan event pada tanggal tertentu
  const addEventToDate = (date) => {
    events.value.push({
      name: `Event pada ${date}`,
      start: date,
      end: date
    })
    console.log('Event ditambahkan pada:', date)
  }
  
  // Menampilkan konten event pada kalender
  const eventContent = (event) => {
    return `
      <div>
        <strong>${event.name}</strong>
      </div>
    `
  }
  
  // Fungsi untuk menyesuaikan waktu Indonesia (WIB)
  const dateInIndonesia = (date) => {
    return new Date(date).toLocaleString('id-ID', { timeZone: 'Asia/Jakarta' })
  }
  </script>
  
  <style>
  .v-calendar {
    background-color: #f5f5f5;
  }
  
  .v-calendar .v-btn--active {
    background-color: #6200ea;
  }
  
  .v-calendar .v-btn--today {
    background-color: #00bcd4;
    color: white;
  }
  
  .v-calendar .v-btn {
    padding: 10px;
  }
  
  .v-calendar .v-btn:hover {
    background-color: #6200ea;
    color: white;
  }
  </style>
  