<template>
    <div class="calendar-container">
      <h2>Calendar</h2>
      <div v-for="day in calendarDays" :key="day.date" class="calendar-day">
        <div class="day-number">{{ day.date }}</div>
        <div v-if="day.events.length > 0" class="events">
          <div v-for="(event, index) in day.events" :key="index" class="event">
            <span class="event-icon">{{ event.icon }}</span>
            <span class="event-name">{{ event.eventName }}</span>
            <span class="event-notes">{{ event.notes }}</span>
          </div>
        </div>
        <div v-else class="no-events">
          <p>No events today.</p>
        </div>
        <button v-if="day.events.length === 0" @click="addEvent(day.date)">Add Event</button>
      </div>
    </div>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        calendarDays: [], // Data hari-hari kalender dengan event
      };
    },
    mounted() {
      // Mengambil data kalender dari backend
      this.fetchCalendarData();
    },
    methods: {
      // Fungsi untuk mengambil data dari backend
      fetchCalendarData() {
        axios.get('http://localhost:5152/calendar') // Ganti dengan URL backend Anda
          .then((response) => {
            const events = response.data;
            const calendar = [];
  
            // Mengelompokkan event berdasarkan tanggal
            for (let i = 1; i <= 31; i++) {
              const dayEvents = events.filter(event => new Date(event.EventDate).getDate() === i); // Event yang sesuai dengan tanggal
              calendar.push({
                date: i,
                events: dayEvents,
              });
            }
  
            // Menyimpan data kalender ke dalam calendarDays
            this.calendarDays = calendar;
          })
          .catch((error) => {
            console.error('Error fetching calendar data:', error);
          });
      },
      addEvent(day) {
        console.log(`Add event for day ${day}`);
        // Logic untuk menambah event di sini
        // Bisa ditambahkan dengan mengarahkan pengguna ke halaman form entry event
      },
    },
  };
  </script>
  
  <style scoped>
  .calendar-container {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
  }
  
  .calendar-day {
    width: 100px;
    border: 1px solid #ddd;
    padding: 10px;
    text-align: center;
  }
  
  .day-number {
    font-size: 1.5em;
  }
  
  .events {
    margin-top: 10px;
  }
  
  .event {
    background-color: #f1f1f1;
    padding: 5px;
    margin-bottom: 5px;
  }
  
  .event-icon {
    font-weight: bold;
  }
  
  .no-events {
    color: red;
  }
  
  button {
    margin-top: 10px;
    padding: 5px 10px;
    background-color: #4CAF50;
    color: white;
    border: none;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #45a049;
  }
  </style>
  