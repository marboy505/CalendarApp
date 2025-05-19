<template>
    <v-app>
      <v-container class="calendar-container" fluid>
        <v-calendar
          v-model="selectedDate"
          :events="events"
          :attributes="customAttributes"
          is-expanded
          @input="onCalendarChange"
          @click:date="onDateClick"
        >
          <!-- Scoped Slot untuk Custom Konten Hari -->
          <template v-slot:day-content="{ day, attrs }">
            <div 
              v-bind="attrs"
              :class="{
                'day-content': true,
                'sunday': day.weekday === 0,  // Warna merah untuk Minggu
                'has-event': isEventDay(day.date)  // Jika ada event, beri warna khusus
              }"
            >
              <span>{{ formatDate(day.date) }}</span>
              <!-- Menambahkan Ikon jika ada Event -->
              <v-icon v-if="isEventDay(day.date)">mdi-calendar-check</v-icon>
            </div>
          </template>
        </v-calendar>
      </v-container>
    </v-app>
  </template>
<script setup>
import { ref, watch } from 'vue'

// Menyimpan tanggal yang dipilih
const selectedDate = ref(null)

// Variabel untuk menyimpan event yang ditampilkan di kalender
const events = ref([
  { name: 'Event 1', start: '2025-05-10', end: '2025-05-10', icon: 'mdi-calendar' },
  { name: 'Event 2', start: '2025-05-12', end: '2025-05-12', icon: 'mdi-alert-circle' }
])

// Menyimpan atribut kustom untuk kalender
const customAttributes = ref([])

// Variabel untuk menyimpan data kalender dari v-calendar
const calendarData = ref([])

// Fungsi untuk menangani perubahan pada kalender
const onCalendarChange = (dates) => {
  console.log('Data kalender yang diperbarui: ', dates)
  calendarData.value = dates // Menyimpan data tanggal yang dipilih
  setCustomAttributes() // Menyesuaikan atribut berdasarkan data kalender
}

// Menyesuaikan atribut berdasarkan event
const setCustomAttributes = () => {
  customAttributes.value = []

  // Loop melalui data event dan tentukan atribut untuk tanggal tertentu
  events.value.forEach(event => {
    customAttributes.value.push({
      key: event.start, // Key yang unik untuk setiap tanggal
      dates: [event.start], // Tanggal yang terkena pengaruh atribut
      style: { backgroundColor: '#ffeb3b', color: '#000' }, // Gaya untuk tanggal tersebut
      content: `Event: ${event.name}`, // Konten kustom yang muncul saat hover
    })
  })
}

// Fungsi untuk menangani klik pada tanggal
const onDateClick = (date) => {
  console.log('Tanggal yang dipilih: ', date)
  addEventToDate(date)
}

// Fungsi untuk menambahkan event pada tanggal tertentu
const addEventToDate = (date) => {
  events.value.push({
    name: `Event pada ${date}`,
    start: date,
    end: date
  })
  setCustomAttributes() // Memperbarui atribut setelah menambahkan event
  console.log('Event ditambahkan pada:', date)
}

// Fungsi untuk memformat tanggal
const formatDate = (date) => {
  const formattedDate = new Date(date)
  return formattedDate.toLocaleDateString('id-ID', { day: 'numeric' }) // Hanya menampilkan hari (tanggal)
}

// Fungsi untuk memeriksa apakah tanggal memiliki event
const isEventDay = (date) => {
  return events.value.some(event => event.start === date || event.end === date)
}

// Panggil setCustomAttributes saat komponen pertama kali dimuat
setCustomAttributes()
</script>
<style scoped>
.v-calendar {
  background-color: #333; /* Latar belakang hitam */
  color: #fff; /* Warna teks putih */
  width: 100%;
  height: 700px; /* Memperbesar ukuran kalender */
  border-radius: 8px; /* Menambahkan sudut membulat pada kalender */
}

.calendar-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.day-content {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 10px;
  font-size: 28px; /* Ukuran font besar */
  color: #e0e0e0; /* Warna teks abu-abu muda untuk angka */
  text-align: center;
}

.sunday {
  color: red; /* Mengubah warna teks menjadi merah untuk hari Minggu */
}

.has-event {
  color: #ffeb3b; /* Mengubah warna teks jika ada event */
}

.v-btn {
  font-size: 18px; /* Menambah ukuran teks tombol */
  padding: 20px;
  border-radius: 8px; /* Menambahkan sudut membulat pada tombol */
  transition: all 0.3s ease;
}

.v-btn:hover {
  background-color: #6200ea; /* Warna latar belakang saat tombol hover */
  color: white;
}

.v-icon {
  font-size: 22px; /* Ukuran ikon */
  color: #ffeb3b; /* Warna ikon kuning untuk hari Minggu */
  margin-top: 5px;
}

.sunday-icon {
  position: absolute;
  top: 0;
  right: 0;
  font-size: 18px;
  color: #ff7043; /* Warna oranye untuk ikon Minggu */
}

.v-btn--today {
  background-color: #00bcd4;
  color: white;
}

.v-btn.event-day {
  background-color: #ffeb3b; /* Warna latar belakang untuk event */
  color: black;
}

/* Styling untuk Minggu */
.v-btn.weekend {
  background-color: #ff7043; /* Latar belakang untuk hari Minggu */
  color: white;
}

/* Warna untuk teks hari */
.v-btn span {
  font-weight: bold;
  font-size: 18px;
}
</style>
  